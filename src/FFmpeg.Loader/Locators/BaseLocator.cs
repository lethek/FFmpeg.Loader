using System.IO.Abstractions;
using System.Reflection;

namespace FFmpeg.Loader.Locators;

internal abstract class BaseLocator
{
    protected BaseLocator(string? rootDir)
        : this(new FileSystem(), rootDir) { }


    protected BaseLocator(IFileSystem fileSystem, string? rootDir)
    {
        FileSystem = fileSystem;
        RootDir = rootDir ?? FileSystem.Path.GetDirectoryName(AssemblyLocation)!;
    }


    public abstract IFileInfo? FindFFmpegLibrary(string name, int version);


    protected IFileInfo? SearchPathsForFile(string fileName, IEnumerable<string> relativePaths)
        => relativePaths
            .Select(x => FileSystem.FileInfo.New(FileSystem.Path.Combine(RootDir, x, fileName)))
            .FirstOrDefault(x => x.Exists);


    protected IEnumerable<string> FlattenPathList(IEnumerable<string> paths)
        => paths.SelectMany(x => x.Split(PathSeparatorChars, StringSplitOptions.RemoveEmptyEntries));


    protected abstract char[] PathSeparatorChars { get; }


    internal IFileSystem FileSystem { get; init; }
    internal string RootDir { get; init; }


    private static readonly string AssemblyLocation = typeof(BaseLocator).GetTypeInfo().Assembly.Location;
}