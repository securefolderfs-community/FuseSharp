using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// Represents the Darwin <c>struct statvfs</c> (64 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StatVfs
    {
        public ulong f_bsize;   // Offset 0
        public ulong f_frsize;  // Offset 8
        public uint f_blocks;   // Offset 16
        public uint f_bfree;    // Offset 20
        public uint f_bavail;   // Offset 24
        public uint f_files;    // Offset 28
        public uint f_ffree;    // Offset 32
        public uint f_favail;   // Offset 36
        public ulong f_fsid;    // Offset 40
        public ulong f_flag;    // Offset 48
        public ulong f_namemax; // Offset 56
    }
}
