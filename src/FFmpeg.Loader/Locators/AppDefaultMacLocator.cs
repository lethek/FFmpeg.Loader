using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class AppDefaultMacLocator : CustomMacLocator
{
    public AppDefaultMacLocator(string? rootDir)
        : base(rootDir, DefaultPaths) { }


    public AppDefaultMacLocator(IFileSystem fileSystem, string? rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
        ".",
        "./runtimes/osx-x64/native",
    };
}