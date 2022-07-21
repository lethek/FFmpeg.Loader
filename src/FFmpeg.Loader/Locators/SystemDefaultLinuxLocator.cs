using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class SystemDefaultLinuxLocator : CustomLinuxLocator
{
    public SystemDefaultLinuxLocator()
        : base(null, DefaultPaths) { }


    public SystemDefaultLinuxLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
        "/usr/lib/x86_64-linux-gnu",
        "/usr/lib/aarch64-linux-gnu",
        "/usr/lib"
    };
}