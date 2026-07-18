using FuseSharp.Native;

namespace FuseSharp
{
    /// <summary>
    /// The base class for user-space file system implementations backed by macFUSE.
    /// </summary>
    /// <remarks>
    /// Only callbacks overridden by the derived class are registered with FUSE;
    /// unimplemented operations are reported to the kernel as missing.
    /// Paths are provided as null-terminated UTF-8 (decomposed by the macOS kernel).
    /// Callbacks return 0 (or a positive byte count where applicable) on success, and a negated
    /// Darwin errno value (e.g. <c>-LibC.ENOENT</c>) on failure.
    /// </remarks>
    public abstract class FuseFileSystemBase
    {
        /// <summary>
        /// Gets the UTF-8 representation of the file system root path.
        /// </summary>
        public static ReadOnlySpan<byte> RootPath => "/"u8;

        /// <summary>
        /// Gets whether FUSE may invoke callbacks concurrently from multiple threads.
        /// </summary>
        public virtual bool SupportsMultiThreading => false;

        public virtual int Access(ReadOnlySpan<byte> path, int mode) => -LibC.ENOSYS;

        public virtual int ChMod(ReadOnlySpan<byte> path, uint mode, FuseFileInfoRef fiRef) => -LibC.ENOSYS;

        public virtual int Chown(ReadOnlySpan<byte> path, uint uid, uint gid, FuseFileInfoRef fiRef) => -LibC.ENOSYS;

        public virtual int Create(ReadOnlySpan<byte> path, uint mode, ref FuseFileInfo fi) => -LibC.ENOSYS;

        public virtual int FAllocate(ReadOnlySpan<byte> path, int mode, ulong offset, long length, ref FuseFileInfo fi) => -LibC.ENOSYS;

        public virtual int Flush(ReadOnlySpan<byte> path, ref FuseFileInfo fi) => -LibC.ENOSYS;

        public virtual int FSync(ReadOnlySpan<byte> path, bool onlyData, ref FuseFileInfo fi) => -LibC.ENOSYS;

        public virtual int FSyncDir(ReadOnlySpan<byte> path, bool onlyData, ref FuseFileInfo fi) => -LibC.ENOSYS;

        public virtual int GetAttr(ReadOnlySpan<byte> path, ref Stat stat, FuseFileInfoRef fiRef) => -LibC.ENOSYS;

        /// <returns>The number of bytes written to <paramref name="value"/> (or the required size when
        /// <paramref name="value"/> is empty), or a negated errno value on failure.</returns>
        public virtual int GetXAttr(ReadOnlySpan<byte> path, ReadOnlySpan<byte> name, Span<byte> value, uint position) => -LibC.ENOSYS;

        /// <inheritdoc cref="GetXAttr"/>
        public virtual int ListXAttr(ReadOnlySpan<byte> path, Span<byte> list) => -LibC.ENOSYS;

        public virtual int Link(ReadOnlySpan<byte> path, ReadOnlySpan<byte> newPath) => -LibC.ENOSYS;

        public virtual int MkDir(ReadOnlySpan<byte> path, uint mode) => -LibC.ENOSYS;

        public virtual int Open(ReadOnlySpan<byte> path, ref FuseFileInfo fi) => -LibC.ENOSYS;

        public virtual int OpenDir(ReadOnlySpan<byte> path, ref FuseFileInfo fi) => -LibC.ENOSYS;

        /// <returns>The number of bytes read, or a negated errno value on failure.</returns>
        public virtual int Read(ReadOnlySpan<byte> path, ulong offset, Span<byte> buffer, ref FuseFileInfo fi) => -LibC.ENOSYS;

        public virtual int ReadDir(ReadOnlySpan<byte> path, ulong offset, DirectoryContent content, ref FuseFileInfo fi) => -LibC.ENOSYS;

        public virtual int ReadLink(ReadOnlySpan<byte> path, Span<byte> buffer) => -LibC.ENOSYS;

        public virtual void Release(ReadOnlySpan<byte> path, ref FuseFileInfo fi)
        {
        }

        public virtual int ReleaseDir(ReadOnlySpan<byte> path, ref FuseFileInfo fi) => 0;

        public virtual int RemoveXAttr(ReadOnlySpan<byte> path, ReadOnlySpan<byte> name) => -LibC.ENOSYS;

        /// <param name="flags">0 for a plain rename, or macOS <c>RENAME_SWAP</c>/<c>RENAME_EXCL</c> flags.</param>
        public virtual int Rename(ReadOnlySpan<byte> path, ReadOnlySpan<byte> newPath, uint flags) => -LibC.ENOSYS;

        public virtual int RmDir(ReadOnlySpan<byte> path) => -LibC.ENOSYS;

        public virtual int SetXAttr(ReadOnlySpan<byte> path, ReadOnlySpan<byte> name, ReadOnlySpan<byte> value, int flags, uint position) => -LibC.ENOSYS;

        public virtual int StatFS(ReadOnlySpan<byte> path, ref StatVfs statfs) => -LibC.ENOSYS;

        public virtual int SymLink(ReadOnlySpan<byte> path, ReadOnlySpan<byte> target) => -LibC.ENOSYS;

        public virtual int Truncate(ReadOnlySpan<byte> path, long length, FuseFileInfoRef fiRef) => -LibC.ENOSYS;

        public virtual int Unlink(ReadOnlySpan<byte> path) => -LibC.ENOSYS;

        public virtual int UpdateTimestamps(ReadOnlySpan<byte> path, ref TimeSpec atime, ref TimeSpec mtime, FuseFileInfoRef fiRef) => -LibC.ENOSYS;

        /// <returns>The number of bytes written, or a negated errno value on failure.</returns>
        public virtual int Write(ReadOnlySpan<byte> path, ulong offset, ReadOnlySpan<byte> buffer, ref FuseFileInfo fi) => -LibC.ENOSYS;

        /// <summary>
        /// Invoked when the file system has been mounted and the FUSE connection is established.
        /// </summary>
        public virtual void Init(ref FuseConnInfo conn)
        {
        }
    }
}
