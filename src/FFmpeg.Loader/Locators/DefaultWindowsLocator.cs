using System;
using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class DefaultWindowsLocator : CustomWindowsLocator
{
    internal DefaultWindowsLocator(string rootDir)
        : base(rootDir, DefaultPaths) { }


    internal DefaultWindowsLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string Runtime = Environment.Is64BitProcess ? "win7-x64" : "win7-x86";


    private static readonly string[] DefaultPaths = {
        ".",
        $"./runtimes/{Runtime}/native"
    };
}