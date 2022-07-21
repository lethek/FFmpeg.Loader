using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class SystemDefaultMacLocator : CustomMacLocator
{
    public SystemDefaultMacLocator()
        : base(null, DefaultPaths) { }


    public SystemDefaultMacLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
        "/usr/local/lib"
    };
}