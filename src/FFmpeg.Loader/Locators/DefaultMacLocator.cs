using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class DefaultMacLocator : CustomMacLocator
{
    internal DefaultMacLocator(string rootDir)
        : base(rootDir, DefaultPaths) { }


    internal DefaultMacLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
        ".",
        "./runtimes/osx-x64/native",
    };
}