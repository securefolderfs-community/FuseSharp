using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// Darwin libSystem P/Invoke bindings and POSIX constants used by FUSE file system implementations.
    /// </summary>
    /// <remarks>
    /// Errno and flag values are Darwin-specific and intentionally differ from their Linux counterparts.
    /// Only symbols which are identical across x86_64 and arm64 are bound here (the stat() family has
    /// per-architecture symbol names and is deliberately avoided).
    /// </remarks>
    public static unsafe partial class LibC
    {
        private const string LIBRARY_NAME = "libSystem.dylib";

        #region Errno

        public const int EPERM = 1;
        public const int ENOENT = 2;
        public const int EINTR = 4;
        public const int EIO = 5;
        public const int ENXIO = 6;
        public const int EBADF = 9;
        public const int ENOMEM = 12;
        public const int EACCES = 13;
        public const int EBUSY = 16;
        public const int EEXIST = 17;
        public const int EXDEV = 18;
        public const int ENODEV = 19;
        public const int ENOTDIR = 20;
        public const int EISDIR = 21;
        public const int EINVAL = 22;
        public const int EFBIG = 27;
        public const int ENOSPC = 28;
        public const int EROFS = 30;
        public const int EMLINK = 31;
        public const int ERANGE = 34;
        public const int EAGAIN = 35;
        public const int ENOTSUP = 45;
        public const int ENAMETOOLONG = 63;
        public const int ENOTEMPTY = 66;
        public const int ENOSYS = 78;
        public const int EOVERFLOW = 84;
        public const int ENOATTR = 93;
        public const int EOPNOTSUPP = 102;

        /// <summary>
        /// Gets the error code of the last P/Invoke made with SetLastError enabled.
        /// </summary>
        public static int errno => Marshal.GetLastPInvokeError();

        #endregion

        #region Open flags

        public const int O_RDONLY = 0x0000;
        public const int O_WRONLY = 0x0001;
        public const int O_RDWR = 0x0002;
        public const int O_ACCMODE = 0x0003;
        public const int O_NONBLOCK = 0x0004;
        public const int O_APPEND = 0x0008;
        public const int O_SHLOCK = 0x0010;
        public const int O_EXLOCK = 0x0020;
        public const int O_ASYNC = 0x0040;
        public const int O_SYNC = 0x0080;
        public const int O_NOFOLLOW = 0x0100;
        public const int O_CREAT = 0x0200;
        public const int O_TRUNC = 0x0400;
        public const int O_EXCL = 0x0800;
        public const int O_EVTONLY = 0x8000;
        public const int O_NOCTTY = 0x20000;
        public const int O_DIRECTORY = 0x100000;
        public const int O_SYMLINK = 0x200000;
        public const int O_CLOEXEC = 0x1000000;

        #endregion

        #region File modes

        public const ushort S_IFMT = 0xF000;
        public const ushort S_IFIFO = 0x1000;
        public const ushort S_IFCHR = 0x2000;
        public const ushort S_IFDIR = 0x4000;
        public const ushort S_IFBLK = 0x6000;
        public const ushort S_IFREG = 0x8000;
        public const ushort S_IFLNK = 0xA000;
        public const ushort S_IFSOCK = 0xC000;

        #endregion

        #region Xattr options

        public const int XATTR_NOFOLLOW = 1;
        public const int XATTR_CREATE = 2;
        public const int XATTR_REPLACE = 4;

        #endregion

        #region Mount flags

        public const int MNT_FORCE = 0x00080000;

        #endregion

        public const int AT_FDCWD = -2;

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial int unmount(byte* path, int flags);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial int statvfs(byte* path, StatVfs* buf);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial int chown(byte* path, uint owner, uint group);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial int rename(byte* oldPath, byte* newPath);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial int renamex_np(byte* oldPath, byte* newPath, uint flags);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial int utimensat(int dirFd, byte* path, TimeSpec* times, int flags);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial nint getxattr(byte* path, byte* name, void* value, nuint size, uint position, int options);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial int setxattr(byte* path, byte* name, void* value, nuint size, uint position, int options);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial nint listxattr(byte* path, byte* nameBuffer, nuint size, int options);

        [LibraryImport(LIBRARY_NAME, SetLastError = true)]
        public static partial int removexattr(byte* path, byte* name, int options);

        [LibraryImport(LIBRARY_NAME)]
        public static partial uint getuid();

        [LibraryImport(LIBRARY_NAME)]
        public static partial uint getgid();

        [LibraryImport(LIBRARY_NAME, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial int sysctlbyname(string name, void* oldp, nuint* oldlenp, void* newp, nuint newlen);
    }
}
