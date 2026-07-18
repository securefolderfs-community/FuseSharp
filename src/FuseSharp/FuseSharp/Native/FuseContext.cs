using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// Represents the macFUSE <c>struct fuse_context</c> (40 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 40)]
    public unsafe struct FuseContext
    {
        public void* fuse;          // Offset 0
        public uint uid;            // Offset 8
        public uint gid;            // Offset 12
        public int pid;             // Offset 16 (+4 bytes of padding)
        public void* private_data;  // Offset 24
        public ushort umask;        // Offset 32
    }
}
