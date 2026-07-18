using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// Represents the macFUSE <c>struct fuse_operations</c> for <c>FUSE_USE_VERSION 26</c> (464 bytes).
    /// </summary>
    /// <remarks>
    /// The layout has been verified against the installed macFUSE headers with offsetof().
    /// Members which are not routed to managed code are typed as <see cref="IntPtr"/> placeholders.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FuseOperations
    {
        public delegate* unmanaged[Cdecl]<byte*, Stat*, int> getattr;                                       // Offset 0
        public delegate* unmanaged[Cdecl]<byte*, byte*, nuint, int> readlink;                               // Offset 8
        private IntPtr getdir;                                                                              // Offset 16 (deprecated)
        public delegate* unmanaged[Cdecl]<byte*, ushort, int, int> mknod;                                   // Offset 24
        public delegate* unmanaged[Cdecl]<byte*, ushort, int> mkdir;                                        // Offset 32
        public delegate* unmanaged[Cdecl]<byte*, int> unlink;                                               // Offset 40
        public delegate* unmanaged[Cdecl]<byte*, int> rmdir;                                                // Offset 48
        public delegate* unmanaged[Cdecl]<byte*, byte*, int> symlink;                                       // Offset 56
        public delegate* unmanaged[Cdecl]<byte*, byte*, int> rename;                                        // Offset 64
        public delegate* unmanaged[Cdecl]<byte*, byte*, int> link;                                          // Offset 72
        public delegate* unmanaged[Cdecl]<byte*, ushort, int> chmod;                                        // Offset 80
        public delegate* unmanaged[Cdecl]<byte*, uint, uint, int> chown;                                    // Offset 88
        public delegate* unmanaged[Cdecl]<byte*, long, int> truncate;                                       // Offset 96
        private IntPtr utime;                                                                               // Offset 104 (superseded by utimens)
        public delegate* unmanaged[Cdecl]<byte*, FuseFileInfo*, int> open;                                  // Offset 112
        public delegate* unmanaged[Cdecl]<byte*, byte*, nuint, long, FuseFileInfo*, int> read;              // Offset 120
        public delegate* unmanaged[Cdecl]<byte*, byte*, nuint, long, FuseFileInfo*, int> write;             // Offset 128
        public delegate* unmanaged[Cdecl]<byte*, StatVfs*, int> statfs;                                     // Offset 136
        public delegate* unmanaged[Cdecl]<byte*, FuseFileInfo*, int> flush;                                 // Offset 144
        public delegate* unmanaged[Cdecl]<byte*, FuseFileInfo*, int> release;                               // Offset 152
        public delegate* unmanaged[Cdecl]<byte*, int, FuseFileInfo*, int> fsync;                            // Offset 160
        public delegate* unmanaged[Cdecl]<byte*, byte*, byte*, nuint, int, uint, int> setxattr;             // Offset 168
        public delegate* unmanaged[Cdecl]<byte*, byte*, byte*, nuint, uint, int> getxattr;                  // Offset 176
        public delegate* unmanaged[Cdecl]<byte*, byte*, nuint, int> listxattr;                              // Offset 184
        public delegate* unmanaged[Cdecl]<byte*, byte*, int> removexattr;                                   // Offset 192
        public delegate* unmanaged[Cdecl]<byte*, FuseFileInfo*, int> opendir;                               // Offset 200
        public delegate* unmanaged[Cdecl]<byte*, void*, void*, long, FuseFileInfo*, int> readdir;           // Offset 208
        public delegate* unmanaged[Cdecl]<byte*, FuseFileInfo*, int> releasedir;                            // Offset 216
        public delegate* unmanaged[Cdecl]<byte*, int, FuseFileInfo*, int> fsyncdir;                         // Offset 224
        public delegate* unmanaged[Cdecl]<FuseConnInfo*, void*> init;                                       // Offset 232
        public delegate* unmanaged[Cdecl]<void*, void> destroy;                                             // Offset 240
        public delegate* unmanaged[Cdecl]<byte*, int, int> access;                                          // Offset 248
        public delegate* unmanaged[Cdecl]<byte*, ushort, FuseFileInfo*, int> create;                        // Offset 256
        public delegate* unmanaged[Cdecl]<byte*, long, FuseFileInfo*, int> ftruncate;                       // Offset 264
        public delegate* unmanaged[Cdecl]<byte*, Stat*, FuseFileInfo*, int> fgetattr;                       // Offset 272
        private IntPtr @lock;                                                                               // Offset 280
        public delegate* unmanaged[Cdecl]<byte*, TimeSpec*, int> utimens;                                   // Offset 288
        private IntPtr bmap;                                                                                // Offset 296
        private uint flags_bitfield;                                                                        // Offset 304 (+4 bytes of padding)
        private IntPtr ioctl;                                                                               // Offset 312
        private IntPtr poll;                                                                                // Offset 320
        private IntPtr write_buf;                                                                           // Offset 328
        private IntPtr read_buf;                                                                            // Offset 336
        private IntPtr flock;                                                                               // Offset 344
        public delegate* unmanaged[Cdecl]<byte*, int, long, long, FuseFileInfo*, int> fallocate;            // Offset 352
        private IntPtr reserved00;                                                                          // Offset 360
        private IntPtr monitor;                                                                             // Offset 368
        public delegate* unmanaged[Cdecl]<byte*, byte*, uint, int> renamex;                                 // Offset 376
        private IntPtr statfs_x;                                                                            // Offset 384
        public delegate* unmanaged[Cdecl]<byte*, int> setvolname;                                           // Offset 392
        private IntPtr exchange;                                                                            // Offset 400
        private IntPtr getxtimes;                                                                           // Offset 408
        private IntPtr setbkuptime;                                                                         // Offset 416
        private IntPtr setchgtime;                                                                          // Offset 424
        private IntPtr setcrtime;                                                                           // Offset 432
        private IntPtr chflags;                                                                             // Offset 440
        private IntPtr setattr_x;                                                                           // Offset 448
        private IntPtr fsetattr_x;                                                                          // Offset 456
    }
}
