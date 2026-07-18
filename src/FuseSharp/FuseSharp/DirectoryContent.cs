using System.Text;

namespace FuseSharp
{
    /// <summary>
    /// Accumulates directory entries produced by <see cref="FuseFileSystemBase.ReadDir"/>.
    /// Wraps the native FUSE directory filler function.
    /// </summary>
    public readonly unsafe ref struct DirectoryContent
    {
        private const int MAX_NAME_LENGTH = 1024;

        private readonly void* _buffer;
        private readonly delegate* unmanaged[Cdecl]<void*, byte*, void*, long, int> _filler;

        internal DirectoryContent(void* buffer, void* filler)
        {
            _buffer = buffer;
            _filler = (delegate* unmanaged[Cdecl]<void*, byte*, void*, long, int>)filler;
        }

        /// <summary>
        /// Adds an entry with the specified plaintext <paramref name="name"/> to the directory listing.
        /// </summary>
        public void AddEntry(string name)
        {
            Span<byte> utf8 = stackalloc byte[Encoding.UTF8.GetMaxByteCount(Math.Min(name.Length, MAX_NAME_LENGTH)) + 1];
            var byteCount = Encoding.UTF8.GetBytes(name, utf8);
            utf8[byteCount] = 0;

            fixed (byte* namePtr = utf8)
                _ = _filler(_buffer, namePtr, null, 0L);
        }

        /// <summary>
        /// Adds an entry with the specified null-terminated UTF-8 <paramref name="name"/> to the directory listing.
        /// </summary>
        public void AddEntry(ReadOnlySpan<byte> name)
        {
            if (name.IsEmpty || name[^1] != 0)
            {
                Span<byte> utf8 = stackalloc byte[name.Length + 1];
                name.CopyTo(utf8);
                utf8[name.Length] = 0;

                fixed (byte* namePtr = utf8)
                    _ = _filler(_buffer, namePtr, null, 0L);
            }
            else
            {
                fixed (byte* namePtr = name)
                    _ = _filler(_buffer, namePtr, null, 0L);
            }
        }
    }
}
