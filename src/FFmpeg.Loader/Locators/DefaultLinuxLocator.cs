using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class DefaultLinuxLocator : CustomLinuxLocator
{
    public DefaultLinuxLocator(string rootDir)
        : base(rootDir, DefaultPaths) { }


    public DefaultLinuxLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
        ".",
        "./runtimes/linux-x64/native"
    };
}