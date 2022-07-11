using System.Collections.Generic;
using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class CustomWindowsLocator : BaseLocator
{
    internal CustomWindowsLocator(string rootDir, IEnumerable<string> paths) : base(rootDir)
        => Paths = new List<string>(paths);


    internal CustomWindowsLocator(IFileSystem fileSystem, string rootDir, IEnumerable<string> paths) : base(fileSystem, rootDir)
        => Paths = new List<string>(paths);


    public override IFileInfo FindFFmpegLibrary(string name, int version)
        => FindLibrary($"{name}-{version}.dll", Paths);


    public List<string> Paths { get; set; }
}