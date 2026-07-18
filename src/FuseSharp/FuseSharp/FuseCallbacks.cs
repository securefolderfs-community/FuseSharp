using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FuseSharp.Native;

namespace FuseSharp
{
    /// <summary>
    /// Static trampolines invoked by the native FUSE library. Each callback resolves the target
    /// <see cref="FuseFileSystemBase"/> from the request context and forwards the invocation.
    /// </summary>
    /// <remarks>
    /// Exceptions must never propagate to native code; any unhandled managed exception is reported as EIO.
    /// </remarks>
    internal static unsafe class FuseCallbacks
    {
        private static FuseFileSystemBase Target
        {
            get
            {
                var context = LibFuse.fuse_get_context();
                return (FuseFileSystemBase)GCHandle.FromIntPtr((IntPtr)context->private_data).Target!;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<byte> PathOf(byte* path)
        {
            return MemoryMarshal.CreateReadOnlySpanFromNullTerminated(path);
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int GetAttr(byte* path, Stat* stat)
        {
            try
            {
                *stat = default;
                return Target.GetAttr(PathOf(path), ref *stat, default);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int FGetAttr(byte* path, Stat* stat, FuseFileInfo* fi)
        {
            try
            {
                *stat = default;
                return Target.GetAttr(PathOf(path), ref *stat, new FuseFileInfoRef(fi));
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int ReadLink(byte* path, byte* buffer, nuint bufferSize)
        {
            try
            {
                return Target.ReadLink(PathOf(path), new Span<byte>(buffer, (int)Math.Min(bufferSize, int.MaxValue)));
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int MkDir(byte* path, ushort mode)
        {
            try
            {
                return Target.MkDir(PathOf(path), mode);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Unlink(byte* path)
        {
            try
            {
                return Target.Unlink(PathOf(path));
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int RmDir(byte* path)
        {
            try
            {
                return Target.RmDir(PathOf(path));
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int SymLink(byte* target, byte* linkPath)
        {
            try
            {
                return Target.SymLink(PathOf(target), PathOf(linkPath));
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Rename(byte* path, byte* newPath)
        {
            try
            {
                return Target.Rename(PathOf(path), PathOf(newPath), 0u);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int RenameX(byte* path, byte* newPath, uint flags)
        {
            try
            {
                return Target.Rename(PathOf(path), PathOf(newPath), flags);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Link(byte* path, byte* newPath)
        {
            try
            {
                return Target.Link(PathOf(path), PathOf(newPath));
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int ChMod(byte* path, ushort mode)
        {
            try
            {
                return Target.ChMod(PathOf(path), mode, default);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Chown(byte* path, uint uid, uint gid)
        {
            try
            {
                return Target.Chown(PathOf(path), uid, gid, default);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Truncate(byte* path, long length)
        {
            try
            {
                return Target.Truncate(PathOf(path), length, default);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int FTruncate(byte* path, long length, FuseFileInfo* fi)
        {
            try
            {
                return Target.Truncate(PathOf(path), length, new FuseFileInfoRef(fi));
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Open(byte* path, FuseFileInfo* fi)
        {
            try
            {
                return Target.Open(PathOf(path), ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Read(byte* path, byte* buffer, nuint size, long offset, FuseFileInfo* fi)
        {
            try
            {
                var span = new Span<byte>(buffer, (int)Math.Min(size, int.MaxValue));
                return Target.Read(PathOf(path), (ulong)offset, span, ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Write(byte* path, byte* buffer, nuint size, long offset, FuseFileInfo* fi)
        {
            try
            {
                var span = new ReadOnlySpan<byte>(buffer, (int)Math.Min(size, int.MaxValue));
                return Target.Write(PathOf(path), (ulong)offset, span, ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int StatFS(byte* path, StatVfs* statfs)
        {
            try
            {
                *statfs = default;
                return Target.StatFS(PathOf(path), ref *statfs);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Flush(byte* path, FuseFileInfo* fi)
        {
            try
            {
                return Target.Flush(PathOf(path), ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Release(byte* path, FuseFileInfo* fi)
        {
            try
            {
                Target.Release(PathOf(path), ref *fi);
                return 0;
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int FSync(byte* path, int onlyData, FuseFileInfo* fi)
        {
            try
            {
                return Target.FSync(PathOf(path), onlyData != 0, ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int SetXAttr(byte* path, byte* name, byte* value, nuint size, int flags, uint position)
        {
            try
            {
                var valueSpan = value is null
                    ? ReadOnlySpan<byte>.Empty
                    : new ReadOnlySpan<byte>(value, (int)Math.Min(size, int.MaxValue));

                return Target.SetXAttr(PathOf(path), PathOf(name), valueSpan, flags, position);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int GetXAttr(byte* path, byte* name, byte* value, nuint size, uint position)
        {
            try
            {
                var valueSpan = value is null
                    ? Span<byte>.Empty
                    : new Span<byte>(value, (int)Math.Min(size, int.MaxValue));

                return Target.GetXAttr(PathOf(path), PathOf(name), valueSpan, position);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int ListXAttr(byte* path, byte* list, nuint size)
        {
            try
            {
                var listSpan = list is null
                    ? Span<byte>.Empty
                    : new Span<byte>(list, (int)Math.Min(size, int.MaxValue));

                return Target.ListXAttr(PathOf(path), listSpan);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int RemoveXAttr(byte* path, byte* name)
        {
            try
            {
                return Target.RemoveXAttr(PathOf(path), PathOf(name));
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int OpenDir(byte* path, FuseFileInfo* fi)
        {
            try
            {
                return Target.OpenDir(PathOf(path), ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int ReadDir(byte* path, void* buffer, void* filler, long offset, FuseFileInfo* fi)
        {
            try
            {
                var content = new DirectoryContent(buffer, filler);
                return Target.ReadDir(PathOf(path), (ulong)offset, content, ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int ReleaseDir(byte* path, FuseFileInfo* fi)
        {
            try
            {
                return Target.ReleaseDir(PathOf(path), ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int FSyncDir(byte* path, int onlyData, FuseFileInfo* fi)
        {
            try
            {
                return Target.FSyncDir(PathOf(path), onlyData != 0, ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Access(byte* path, int mode)
        {
            try
            {
                return Target.Access(PathOf(path), mode);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int Create(byte* path, ushort mode, FuseFileInfo* fi)
        {
            try
            {
                return Target.Create(PathOf(path), mode, ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int UpdateTimestamps(byte* path, TimeSpec* times)
        {
            try
            {
                // A null timespec array means "set both timestamps to now"
                var now = new TimeSpec() { tv_nsec = TimeSpec.UTIME_NOW };
                ref var atime = ref (times is null ? ref now : ref times[0]);
                ref var mtime = ref (times is null ? ref now : ref times[1]);

                return Target.UpdateTimestamps(PathOf(path), ref atime, ref mtime, default);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static int FAllocate(byte* path, int mode, long offset, long length, FuseFileInfo* fi)
        {
            try
            {
                return Target.FAllocate(PathOf(path), mode, (ulong)offset, length, ref *fi);
            }
            catch
            {
                return -LibC.EIO;
            }
        }

        [UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ])]
        internal static void* Init(FuseConnInfo* conn)
        {
            var context = LibFuse.fuse_get_context();
            try
            {
                ((FuseFileSystemBase)GCHandle.FromIntPtr((IntPtr)context->private_data).Target!).Init(ref *conn);
            }
            catch
            {
                // The connection is usable even if managed initialization fails
            }

            // The returned pointer becomes the new private_data - it must be preserved
            // as it carries the GCHandle used to resolve the managed file system
            return context->private_data;
        }
    }
}
