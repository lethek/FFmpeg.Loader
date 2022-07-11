using System.Collections.Generic;
using System.IO.Abstractions;
using System.Reflection;

namespace FFmpeg.Loader.Locators;

internal abstract class BaseLocator
{
    protected BaseLocator(string rootDir)
        : this(new FileSystem(), rootDir) { }


    protected BaseLocator(IFileSystem fileSystem, string rootDir)
    {
        FileSystem = fileSystem;
        RootDir = rootDir ?? FileSystem.Path.GetDirectoryName(AssemblyLocation);
    }


    public abstract IFileInfo FindFFmpegLibrary(string name, int version);


    internal IFileInfo FindLibrary(string fileName, IEnumerable<string> relativePaths)
    {
        foreach (var relativePath in relativePaths) {
            var fi = FileSystem.FileInfo.FromFileName(FileSystem.Path.Combine(RootDir, relativePath, fileName));
            if (fi.Exists) {
                return fi;
            }
        }

        //Couldn't find the library.
        return null;
    }


    internal IFileSystem FileSystem { get; init; }
    internal string RootDir { get; init; }


    private static readonly string AssemblyLocation = typeof(BaseLocator).GetTypeInfo().Assembly.Location;
}