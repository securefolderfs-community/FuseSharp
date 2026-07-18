using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// Represents the Darwin <c>struct stat</c> with 64-bit inodes (144 bytes), as expected by macFUSE.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Stat
    {
        public int st_dev;                  // Offset 0
        public ushort st_mode;              // Offset 4
        public ushort st_nlink;             // Offset 6
        public ulong st_ino;                // Offset 8
        public uint st_uid;                 // Offset 16
        public uint st_gid;                 // Offset 20
        public int st_rdev;                 // Offset 24 (+4 bytes of padding)
        public TimeSpec st_atimespec;       // Offset 32
        public TimeSpec st_mtimespec;       // Offset 48
        public TimeSpec st_ctimespec;       // Offset 64
        public TimeSpec st_birthtimespec;   // Offset 80
        public long st_size;                // Offset 96
        public long st_blocks;              // Offset 104
        public int st_blksize;              // Offset 112
        public uint st_flags;               // Offset 116
        public uint st_gen;                 // Offset 120
        private int st_lspare;              // Offset 124
        private long st_qspare0;            // Offset 128
        private long st_qspare1;            // Offset 136
    }
}
