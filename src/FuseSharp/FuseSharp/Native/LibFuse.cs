using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FuseSharp.Native
{
    /// <summary>
    /// P/Invoke bindings for the macFUSE FUSE 2.x compatibility library (libfuse.2.dylib).
    /// </summary>
    public static unsafe partial class LibFuse
    {
        private const string LIBRARY_NAME = "fuse.2";

        /// <summary>
        /// The absolute path where macFUSE installs the FUSE 2.x compatibility library.
        /// </summary>
        public const string LibraryPath = "/usr/local/lib/libfuse.2.dylib";

        [StructLayout(LayoutKind.Sequential)]
        public struct FuseArgs
        {
            public int argc;        // Offset 0 (+4 bytes of padding)
            public byte** argv;     // Offset 8
            public int allocated;   // Offset 16 (+4 bytes of padding)
        }

        [LibraryImport(LIBRARY_NAME, EntryPoint = "fuse_mount")]
        [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
        public static partial void* fuse_mount(byte* mountpoint, FuseArgs* args);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "fuse_unmount")]
        [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
        public static partial void fuse_unmount(byte* mountpoint, void* channel);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "fuse_new")]
        [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
        public static partial void* fuse_new(void* channel, FuseArgs* args, FuseOperations* operations, nuint operationsSize, void* userData);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "fuse_destroy")]
        [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
        public static partial void fuse_destroy(void* fuse);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "fuse_loop")]
        [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
        public static partial int fuse_loop(void* fuse);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "fuse_loop_mt")]
        [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
        public static partial int fuse_loop_mt(void* fuse);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "fuse_exit")]
        [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
        public static partial void fuse_exit(void* fuse);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "fuse_get_context")]
        [UnmanagedCallConv(CallConvs = [ typeof(System.Runtime.CompilerServices.CallConvCdecl) ])]
        public static partial FuseContext* fuse_get_context();

#pragma warning disable CA2255
        [ModuleInitializer]
        internal static void InitializeResolver()
        {
            NativeLibrary.SetDllImportResolver(typeof(LibFuse).Assembly, static (libraryName, _, _) =>
            {
                if (libraryName != LIBRARY_NAME)
                    return IntPtr.Zero;

                _ = NativeLibrary.TryLoad(LibraryPath, out var handle);
                return handle;
            });
        }
#pragma warning restore CA2255
    }
}
