using System;
using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class AppDefaultWindowsLocator : CustomWindowsLocator
{
    public AppDefaultWindowsLocator(string rootDir)
        : base(rootDir, DefaultPaths) { }


    public AppDefaultWindowsLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string Runtime = Environment.Is64BitProcess ? "win7-x64" : "win7-x86";


    private static readonly string[] DefaultPaths = {
        ".",
        $"./runtimes/{Runtime}/native"
    };
}