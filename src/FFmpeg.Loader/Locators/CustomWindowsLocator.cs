using System.Collections.Generic;
using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class CustomWindowsLocator : BaseLocator
{
    public CustomWindowsLocator(string rootDir, IEnumerable<string> paths) : base(rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    public CustomWindowsLocator(IFileSystem fileSystem, string rootDir, IEnumerable<string> paths) : base(fileSystem, rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    public override IFileInfo FindFFmpegLibrary(string name, int version)
        => SearchPathsForFile($"{name}-{version}.dll", Paths);


    protected override char[] PathSeparatorChars { get; } = { ';' };


    public List<string> Paths { get; }
}