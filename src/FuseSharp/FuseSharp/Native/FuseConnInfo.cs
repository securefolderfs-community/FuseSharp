using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// Represents the macFUSE <c>struct fuse_conn_info</c> (128 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 128)]
    public struct FuseConnInfo
    {
        public uint proto_major;    // Offset 0
        public uint proto_minor;    // Offset 4
        public uint async_read;     // Offset 8
        public uint max_write;      // Offset 12
        public uint max_readahead;  // Offset 16
        // The remaining bytes are reserved
    }
}
