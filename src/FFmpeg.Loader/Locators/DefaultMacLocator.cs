using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class DefaultMacLocator : CustomMacLocator
{
    public DefaultMacLocator(string rootDir)
        : base(rootDir, DefaultPaths) { }


    public DefaultMacLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
        ".",
        "./runtimes/osx-x64/native",
    };
}