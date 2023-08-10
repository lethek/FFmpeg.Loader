using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class CustomMacLocator : BaseLocator
{
    public CustomMacLocator(string? rootDir, IEnumerable<string> paths) : base(rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    public CustomMacLocator(IFileSystem fileSystem, string? rootDir, IEnumerable<string> paths) : base(fileSystem, rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    public override IFileInfo? FindFFmpegLibrary(string name, int version)
        => SearchPathsForFile($"lib{name}.{version}.dylib", Paths);


    protected override char[] PathSeparatorChars { get; } = { ':' };


    public List<string> Paths { get; }
}