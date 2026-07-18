using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// Represents the macFUSE <c>struct fuse_file_info</c> (40 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FuseFileInfo
    {
        public int flags;           // Offset 0 (+4 bytes of padding)
        private nuint fh_old;       // Offset 8
        public int writepage;       // Offset 16
        private uint bitfields;     // Offset 20
        public ulong fh;            // Offset 24
        public ulong lock_owner;    // Offset 32

        /// <summary>
        /// Gets or sets whether to use direct I/O for this file, bypassing the page cache.
        /// </summary>
        public bool direct_io
        {
            readonly get => (bitfields & 1u) != 0u;
            set => bitfields = value ? bitfields | 1u : bitfields & ~1u;
        }

        /// <summary>
        /// Gets or sets whether previously cached file data does not need to be invalidated.
        /// </summary>
        public bool keep_cache
        {
            readonly get => (bitfields & 2u) != 0u;
            set => bitfields = value ? bitfields | 2u : bitfields & ~2u;
        }

        /// <summary>
        /// Gets whether this operation is a flush and not a true release.
        /// </summary>
        public readonly bool flush => (bitfields & 4u) != 0u;

        /// <summary>
        /// Gets or sets whether the file is not seekable.
        /// </summary>
        public bool nonseekable
        {
            readonly get => (bitfields & 8u) != 0u;
            set => bitfields = value ? bitfields | 8u : bitfields & ~8u;
        }
    }

    /// <summary>
    /// A nullable reference to a <see cref="FuseFileInfo"/>. Callbacks which may be invoked both with
    /// and without an open file handle (e.g. getattr/fgetattr) receive an instance of this type.
    /// </summary>
    public readonly unsafe ref struct FuseFileInfoRef
    {
        private readonly FuseFileInfo* _fi;

        /// <summary>
        /// Gets whether no file handle information is associated with the operation.
        /// </summary>
        public bool IsNull => _fi is null;

        /// <summary>
        /// Gets a reference to the underlying <see cref="FuseFileInfo"/>. Only valid when <see cref="IsNull"/> is false.
        /// </summary>
        public ref FuseFileInfo Value => ref *_fi;

        public FuseFileInfoRef(FuseFileInfo* fi)
        {
            _fi = fi;
        }
    }
}
