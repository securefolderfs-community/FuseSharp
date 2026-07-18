using System.Runtime.InteropServices.Marshalling;
using FuseSharp.Native;

namespace FuseSharp
{
    /// <summary>
    /// The entry point for mounting macFUSE file systems.
    /// </summary>
    public static class Fuse
    {
        private const string MACFUSE_BUNDLE_PATH = "/Library/Filesystems/macfuse.fs";

        /// <summary>
        /// Determines whether macFUSE is installed and its user-space library can be loaded.
        /// </summary>
        public static bool CheckDependencies()
        {
            return OperatingSystem.IsMacOS()
                   && Directory.Exists(MACFUSE_BUNDLE_PATH)
                   && File.Exists(LibFuse.LibraryPath);
        }

        /// <summary>
        /// Mounts <paramref name="fileSystem"/> at <paramref name="mountPoint"/> and starts serving requests on a background thread.
        /// </summary>
        /// <exception cref="FuseException">The mount could not be established.</exception>
        public static IFuseMount Mount(string mountPoint, FuseFileSystemBase fileSystem, MountOptions? options = null)
        {
            var mount = new FuseMount(mountPoint, fileSystem, options ?? new MountOptions());
            mount.MountFileSystem();

            return mount;
        }

        /// <summary>
        /// Forcibly unmounts the file system mounted at <paramref name="mountPoint"/>, if any.
        /// </summary>
        public static unsafe void LazyUnmount(string mountPoint)
        {
            var mountPointPtr = Utf8StringMarshaller.ConvertToUnmanaged(mountPoint);
            try
            {
                _ = LibC.unmount(mountPointPtr, LibC.MNT_FORCE);
            }
            finally
            {
                Utf8StringMarshaller.Free(mountPointPtr);
            }
        }
    }

    /// <summary>
    /// Represents a mounted FUSE file system.
    /// </summary>
    public interface IFuseMount
    {
        /// <summary>
        /// Gets the path where the file system is mounted.
        /// </summary>
        string MountPoint { get; }

        /// <summary>
        /// Gets whether the file system is still mounted and serving requests.
        /// </summary>
        bool IsMounted { get; }

        /// <summary>
        /// Unmounts the file system and waits for the request loop to finish.
        /// </summary>
        /// <returns>Whether the file system was successfully unmounted.</returns>
        Task<bool> UnmountAsync(bool force = false);
    }

    /// <summary>
    /// Additional options applied when mounting a file system.
    /// </summary>
    public sealed class MountOptions
    {
        /// <summary>
        /// Gets or sets the comma-separated list of raw mount options (see the macFUSE mount options wiki).
        /// </summary>
        public string? Options { get; set; }
    }

    /// <summary>
    /// The exception thrown when a FUSE mount cannot be established.
    /// </summary>
    public sealed class FuseException : Exception
    {
        public FuseException(string message)
            : base(message)
        {
        }
    }
}
