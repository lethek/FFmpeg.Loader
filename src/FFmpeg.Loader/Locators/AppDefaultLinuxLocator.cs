using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class AppDefaultLinuxLocator : CustomLinuxLocator
{
    public AppDefaultLinuxLocator(string? rootDir)
        : base(rootDir, DefaultPaths) { }


    public AppDefaultLinuxLocator(IFileSystem fileSystem, string? rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
        ".",
        "./runtimes/linux-x64/native"
    };
}