using System.Collections.Generic;
using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class CustomMacLocator : BaseLocator
{
    internal CustomMacLocator(string rootDir, IEnumerable<string> paths) : base(rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    internal CustomMacLocator(IFileSystem fileSystem, string rootDir, IEnumerable<string> paths) : base(fileSystem, rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    public override IFileInfo FindFFmpegLibrary(string name, int version)
        => FindLibrary($"lib{name}.{version}.dylib", Paths);


    protected override char[] PathSeparatorChars { get; } = { ':' };


    public List<string> Paths { get; }
}