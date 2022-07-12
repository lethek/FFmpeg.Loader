using System.Collections.Generic;
using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class CustomLinuxLocator : BaseLocator
{
    public CustomLinuxLocator(string rootDir, IEnumerable<string> paths) : base(rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    public CustomLinuxLocator(IFileSystem fileSystem, string rootDir, IEnumerable<string> paths) : base(fileSystem, rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    public override IFileInfo FindFFmpegLibrary(string name, int version)
        => SearchPathsForFile($"lib{name}.so.{version}", Paths);


    protected override char[] PathSeparatorChars { get; } = { ':' };


    public List<string> Paths { get; }
}