# FuseSharp

FuseSharp is a **FUSE** binding for **macOS** ([macFUSE](https://macfuse.github.io/)) written in modern C#.
It provides a clean API designed specifically to enable the development of userspace filesystems in .NET applications.

> **Note:** This fork is a complete rewrite of the original FuseSharp for .NET 10. It binds directly against
> macFUSE's FUSE 2.x compatibility library (`/usr/local/lib/libfuse.2.dylib`) using `LibraryImport` and
> `[UnmanagedCallersOnly]` function pointers. The original native `Adaptor` shim, the `Mono.Posix` dependency,
> and the delegate-based marshaling layer have been removed entirely - no native components need to be built
> or shipped alongside the managed assembly, and both `osx-arm64` and `osx-x64` are supported.

## Requirements

* macOS with [macFUSE](https://macfuse.github.io/) installed
* .NET 10 or later

## Usage

Derive from `FuseFileSystemBase` and override the operations your filesystem supports. Only overridden
callbacks are registered with FUSE; everything else is reported to the kernel as not implemented.

```csharp
using FuseSharp;
using FuseSharp.Native;

sealed class MyFileSystem : FuseFileSystemBase
{
    public override bool SupportsMultiThreading => true;

    public override int GetAttr(ReadOnlySpan<byte> path, ref Stat stat, FuseFileInfoRef fiRef)
    {
        if (!path.SequenceEqual(RootPath))
            return -LibC.ENOENT;

        stat.st_mode = (ushort)(LibC.S_IFDIR | 0x1ED); // drwxr-xr-x
        stat.st_nlink = 2;
        return 0;
    }

    public override int ReadDir(ReadOnlySpan<byte> path, ulong offset, DirectoryContent content, ref FuseFileInfo fi)
    {
        content.AddEntry(".");
        content.AddEntry("..");
        return 0;
    }
}
```

Mount and unmount:

```csharp
if (!Fuse.CheckDependencies())
    throw new InvalidOperationException("macFUSE is not installed.");

IFuseMount mount = Fuse.Mount("/path/to/mountpoint", new MyFileSystem(), new MountOptions()
{
    Options = "default_permissions,volname=MyVolume"
});

// ... the filesystem is served on a background thread ...

bool unmounted = await mount.UnmountAsync(force: true);
```

## Project overview

| Component | Purpose |
|----------:|:--------|
| `FuseFileSystemBase` | The abstract base class filesystem implementations derive from. Callbacks receive UTF-8 paths as spans and return 0 or a negated Darwin errno. |
| `Fuse` | Static entry point: `CheckDependencies`, `Mount`, `LazyUnmount`. |
| `IFuseMount` | A handle to a mounted filesystem with `UnmountAsync`. |
| `Native/LibFuse` | `LibraryImport` bindings for `libfuse.2.dylib` (`fuse_mount`, `fuse_new`, `fuse_loop_mt`, ...). |
| `Native/LibC` | Darwin libSystem bindings and POSIX constants (errno values, open flags, xattr, ...). |
| `Native/FuseOperations` | A blittable mirror of macFUSE's `struct fuse_operations` (FUSE API 26), layout-verified against the installed headers. |

## Mount options

The `MountOptions.Options` string is passed to FUSE verbatim (comma-separated). Commonly useful options
(see the [macFUSE mount options wiki](https://github.com/osxfuse/osxfuse/wiki/Mount-options)):

| Option | Description |
|--------|-------------|
| `volname=NAME` | The volume name shown in Finder. |
| `fsname=NAME` | The file system name reported by `mount`. |
| `default_permissions` | Let the kernel enforce permissions based on `GetAttr` results. |
| `allow_other` / `allow_root` | Allow other users (requires the `allow_other` macFUSE tunable). |
| `ro` | Mount read-only. |
| `noappledouble` | Do not create AppleDouble (`._`) companion files. |
| `iosize=N` | I/O size in bytes (512 bytes - 32 MB, power of 2). |
| `debug` | Print FUSE debugging output. |

## License

See [LICENSE](LICENSE). Based on FuseSharp by Códice Software / PlasticSCM.
