using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class DefaultLinuxLocator : CustomLinuxLocator
{
    internal DefaultLinuxLocator(string rootDir)
        : base(rootDir, DefaultPaths) { }


    internal DefaultLinuxLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
        ".",
        "./runtimes/linux-x64/native"
    };
}