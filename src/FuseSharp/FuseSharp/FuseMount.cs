using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using FuseSharp.Native;

namespace FuseSharp
{
    /// <inheritdoc cref="IFuseMount"/>
    internal sealed class FuseMount : IFuseMount
    {
        private readonly FuseFileSystemBase _fileSystem;
        private readonly MountOptions _options;
        private readonly TaskCompletionSource<bool> _loopExitTcs;
        private GCHandle _fileSystemHandle;
        private unsafe FuseOperations* _operations;
        private unsafe byte** _argv;
        private int _argc;
        private unsafe byte* _mountPointPtr;
        private unsafe void* _channel;
        private unsafe void* _fuse;

        /// <inheritdoc/>
        public string MountPoint { get; }

        /// <inheritdoc/>
        public unsafe bool IsMounted => _fuse is not null && !_loopExitTcs.Task.IsCompleted;

        public FuseMount(string mountPoint, FuseFileSystemBase fileSystem, MountOptions options)
        {
            MountPoint = mountPoint;
            _fileSystem = fileSystem;
            _options = options;
            _loopExitTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Establishes the mount and starts the request loop on a dedicated background thread.
        /// </summary>
        /// <exception cref="FuseException">The mount could not be established.</exception>
        public unsafe void MountFileSystem()
        {
            // Build the FUSE argument vector: { program name, [ -o options ] }
            var arguments = new List<string>() { "FuseSharp" };
            if (!string.IsNullOrEmpty(_options.Options))
            {
                arguments.Add("-o");
                arguments.Add(_options.Options);
            }

            _argc = arguments.Count;
            _argv = (byte**)NativeMemory.AllocZeroed((nuint)(_argc + 1), (nuint)sizeof(byte*));
            for (var i = 0; i < _argc; i++)
                _argv[i] = Utf8StringMarshaller.ConvertToUnmanaged(arguments[i]);

            _mountPointPtr = Utf8StringMarshaller.ConvertToUnmanaged(MountPoint);

            var args = new LibFuse.FuseArgs()
            {
                argc = _argc,
                argv = _argv,
                allocated = 0
            };

            _channel = LibFuse.fuse_mount(_mountPointPtr, &args);
            if (_channel is null)
            {
                FreeNativeResources();
                throw new FuseException($"fuse_mount failed for '{MountPoint}'. Check that macFUSE is installed and the mount point is available.");
            }

            _operations = (FuseOperations*)NativeMemory.AllocZeroed((nuint)sizeof(FuseOperations));
            PopulateOperations(_operations, _fileSystem);

            _fileSystemHandle = GCHandle.Alloc(_fileSystem);
            _fuse = LibFuse.fuse_new(_channel, &args, _operations, (nuint)sizeof(FuseOperations), (void*)GCHandle.ToIntPtr(_fileSystemHandle));
            if (_fuse is null)
            {
                LibFuse.fuse_unmount(_mountPointPtr, _channel);
                FreeNativeResources();
                throw new FuseException("fuse_new failed. The mount options may be invalid.");
            }

            var loopThread = new Thread(FuseLoop)
            {
                IsBackground = true,
                Name = $"FuseSharp loop ({MountPoint})"
            };
            loopThread.Start();

            WaitForMountCompletion();
        }

        /// <summary>
        /// Blocks until the kernel-side mount is fully established.
        /// </summary>
        /// <remarks>
        /// On macFUSE the mount(2) syscall is completed asynchronously by the mount helper after the
        /// file system answers the kernel's init handshake. Returning before that point would let
        /// callers observe (and write into) the raw mount point directory.
        /// </remarks>
        private unsafe void WaitForMountCompletion()
        {
            var parent = Directory.GetParent(MountPoint)?.FullName;
            var deadline = Environment.TickCount64 + 10_000L;

            while (Environment.TickCount64 < deadline)
            {
                if (_loopExitTcs.Task.IsCompleted)
                    throw new FuseException($"The FUSE request loop exited before the mount at '{MountPoint}' was established.");

                if (IsMountEstablished(parent))
                    return;

                Thread.Sleep(20);
            }

            _ = TryKernelUnmount(force: true);
            throw new FuseException($"Timed out waiting for the mount at '{MountPoint}' to be established.");
        }

        private unsafe bool IsMountEstablished(string? parent)
        {
            StatVfs mountPointStat = default;
            var mountPointPtr = Utf8StringMarshaller.ConvertToUnmanaged(MountPoint);
            try
            {
                if (LibC.statvfs(mountPointPtr, &mountPointStat) == -1)
                    return false;
            }
            finally
            {
                Utf8StringMarshaller.Free(mountPointPtr);
            }

            if (parent is null)
                return true;

            StatVfs parentStat = default;
            var parentPtr = Utf8StringMarshaller.ConvertToUnmanaged(parent);
            try
            {
                if (LibC.statvfs(parentPtr, &parentStat) == -1)
                    return true;
            }
            finally
            {
                Utf8StringMarshaller.Free(parentPtr);
            }

            // A completed mount places a distinct file system id on the mount point
            return mountPointStat.f_fsid != parentStat.f_fsid;
        }

        /// <inheritdoc/>
        public async Task<bool> UnmountAsync(bool force = false)
        {
            if (!HasMounted())
                return false;

            if (_loopExitTcs.Task.IsCompleted)
                return true;

            // Waking the request loop requires the kernel-side unmount. The loop thread
            // then observes the closed session and performs the remaining cleanup
            if (!TryKernelUnmount(force) && !_loopExitTcs.Task.IsCompleted)
            {
                // The volume may have already been ejected externally (e.g. through Finder)
                return false;
            }

            var completed = await Task.WhenAny(_loopExitTcs.Task, Task.Delay(TimeSpan.FromSeconds(30)));
            return completed == _loopExitTcs.Task;
        }

        private unsafe bool HasMounted()
        {
            return _fuse is not null;
        }

        private unsafe bool TryKernelUnmount(bool force)
        {
            var mountPointPtr = Utf8StringMarshaller.ConvertToUnmanaged(MountPoint);
            try
            {
                return LibC.unmount(mountPointPtr, force ? LibC.MNT_FORCE : 0) != -1;
            }
            finally
            {
                Utf8StringMarshaller.Free(mountPointPtr);
            }
        }

        private unsafe void FuseLoop()
        {
            try
            {
                _ = _fileSystem.SupportsMultiThreading
                    ? LibFuse.fuse_loop_mt(_fuse)
                    : LibFuse.fuse_loop(_fuse);
            }
            finally
            {
                LibFuse.fuse_unmount(_mountPointPtr, _channel);
                LibFuse.fuse_destroy(_fuse);
                FreeNativeResources();

                if (_fileSystemHandle.IsAllocated)
                    _fileSystemHandle.Free();

                _loopExitTcs.TrySetResult(true);
            }
        }

        private unsafe void FreeNativeResources()
        {
            if (_argv is not null)
            {
                for (var i = 0; i < _argc; i++)
                    Utf8StringMarshaller.Free(_argv[i]);

                NativeMemory.Free(_argv);
                _argv = null;
            }

            if (_mountPointPtr is not null)
            {
                Utf8StringMarshaller.Free(_mountPointPtr);
                _mountPointPtr = null;
            }

            if (_operations is not null)
            {
                NativeMemory.Free(_operations);
                _operations = null;
            }
        }

        private static unsafe void PopulateOperations(FuseOperations* operations, FuseFileSystemBase fileSystem)
        {
            // Only register callbacks the file system actually implements so that FUSE
            // reports unimplemented operations to the kernel instead of invoking managed stubs
            var openImplemented = Implements(nameof(FuseFileSystemBase.Open));

            if (Implements(nameof(FuseFileSystemBase.GetAttr)))
            {
                operations->getattr = &FuseCallbacks.GetAttr;
                operations->fgetattr = &FuseCallbacks.FGetAttr;
            }

            if (Implements(nameof(FuseFileSystemBase.ReadLink)))
                operations->readlink = &FuseCallbacks.ReadLink;

            if (Implements(nameof(FuseFileSystemBase.MkDir)))
                operations->mkdir = &FuseCallbacks.MkDir;

            if (Implements(nameof(FuseFileSystemBase.Unlink)))
                operations->unlink = &FuseCallbacks.Unlink;

            if (Implements(nameof(FuseFileSystemBase.RmDir)))
                operations->rmdir = &FuseCallbacks.RmDir;

            if (Implements(nameof(FuseFileSystemBase.SymLink)))
                operations->symlink = &FuseCallbacks.SymLink;

            if (Implements(nameof(FuseFileSystemBase.Rename)))
            {
                operations->rename = &FuseCallbacks.Rename;
                operations->renamex = &FuseCallbacks.RenameX;
            }

            if (Implements(nameof(FuseFileSystemBase.Link)))
                operations->link = &FuseCallbacks.Link;

            if (Implements(nameof(FuseFileSystemBase.ChMod)))
                operations->chmod = &FuseCallbacks.ChMod;

            if (Implements(nameof(FuseFileSystemBase.Chown)))
                operations->chown = &FuseCallbacks.Chown;

            if (Implements(nameof(FuseFileSystemBase.Truncate)))
            {
                operations->truncate = &FuseCallbacks.Truncate;
                operations->ftruncate = &FuseCallbacks.FTruncate;
            }

            if (openImplemented)
            {
                operations->open = &FuseCallbacks.Open;
                operations->release = &FuseCallbacks.Release;
            }

            if (Implements(nameof(FuseFileSystemBase.Read)))
                operations->read = &FuseCallbacks.Read;

            if (Implements(nameof(FuseFileSystemBase.Write)))
                operations->write = &FuseCallbacks.Write;

            if (Implements(nameof(FuseFileSystemBase.StatFS)))
                operations->statfs = &FuseCallbacks.StatFS;

            if (Implements(nameof(FuseFileSystemBase.Flush)))
                operations->flush = &FuseCallbacks.Flush;

            if (Implements(nameof(FuseFileSystemBase.FSync)))
                operations->fsync = &FuseCallbacks.FSync;

            if (Implements(nameof(FuseFileSystemBase.SetXAttr)))
                operations->setxattr = &FuseCallbacks.SetXAttr;

            if (Implements(nameof(FuseFileSystemBase.GetXAttr)))
                operations->getxattr = &FuseCallbacks.GetXAttr;

            if (Implements(nameof(FuseFileSystemBase.ListXAttr)))
                operations->listxattr = &FuseCallbacks.ListXAttr;

            if (Implements(nameof(FuseFileSystemBase.RemoveXAttr)))
                operations->removexattr = &FuseCallbacks.RemoveXAttr;

            if (Implements(nameof(FuseFileSystemBase.OpenDir)))
                operations->opendir = &FuseCallbacks.OpenDir;

            if (Implements(nameof(FuseFileSystemBase.ReadDir)))
                operations->readdir = &FuseCallbacks.ReadDir;

            if (Implements(nameof(FuseFileSystemBase.ReleaseDir)))
                operations->releasedir = &FuseCallbacks.ReleaseDir;

            if (Implements(nameof(FuseFileSystemBase.FSyncDir)))
                operations->fsyncdir = &FuseCallbacks.FSyncDir;

            if (Implements(nameof(FuseFileSystemBase.Access)))
                operations->access = &FuseCallbacks.Access;

            if (Implements(nameof(FuseFileSystemBase.Create)))
                operations->create = &FuseCallbacks.Create;

            if (Implements(nameof(FuseFileSystemBase.UpdateTimestamps)))
                operations->utimens = &FuseCallbacks.UpdateTimestamps;

            if (Implements(nameof(FuseFileSystemBase.FAllocate)))
                operations->fallocate = &FuseCallbacks.FAllocate;

            // Init is always registered - it preserves the private_data pointer
            // which carries the GCHandle used to dispatch callbacks
            operations->init = &FuseCallbacks.Init;

            bool Implements(string methodName)
            {
                return fileSystem.GetType().GetMethod(methodName)?.DeclaringType != typeof(FuseFileSystemBase);
            }
        }
    }
}
