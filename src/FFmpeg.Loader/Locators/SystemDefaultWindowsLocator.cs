using System;
using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class SystemDefaultWindowsLocator : CustomWindowsLocator
{
    public SystemDefaultWindowsLocator()
        : base(null, DefaultPaths) { }


    public SystemDefaultWindowsLocator(IFileSystem fileSystem, string rootDir)
        : base(fileSystem, rootDir, DefaultPaths) { }


    private static readonly string[] DefaultPaths = {
    };
}