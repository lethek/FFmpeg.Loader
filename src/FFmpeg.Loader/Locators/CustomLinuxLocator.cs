﻿using System.Collections.Generic;
using System.IO.Abstractions;

namespace FFmpeg.Loader.Locators;

internal class CustomLinuxLocator : BaseLocator
{
    internal CustomLinuxLocator(string rootDir, IEnumerable<string> paths) : base(rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    internal CustomLinuxLocator(IFileSystem fileSystem, string rootDir, IEnumerable<string> paths) : base(fileSystem, rootDir)
        => Paths = new List<string>(FlattenPathList(paths));


    public override IFileInfo FindFFmpegLibrary(string name, int version)
        => FindLibrary($"lib{name}.so.{version}", Paths);


    protected override char[] PathSeparatorChars { get; } = { ':' };


    public List<string> Paths { get; }
}