# FFmpeg.Loader

[![GitHub license](https://img.shields.io/github/license/lethek/FFmpeg.Loader)](https://github.com/lethek/FFmpeg.Loader/blob/main/LICENSE)
[![NuGet Stats](https://img.shields.io/nuget/v/FFmpeg.Loader.svg)](https://www.nuget.org/packages/FFmpeg.Loader)
[![Build & Publish](https://github.com/lethek/FFmpeg.Loader/actions/workflows/dotnet.yml/badge.svg)](https://github.com/lethek/FFmpeg.Loader/actions/workflows/dotnet.yml)

FFmpegLoader will find and load FFmpeg libaries with the [FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen) bindings. It provides a simple and customizable way of doing this, cross-platform.

This library was designed for use with [FFmpeg.Native](https://github.com/quamotion/ffmpeg-win32) as a more flexible alternative to FFmpeg.Native's tooling for finding FFmpeg libs. However please note, FFmpeg.Native is unnecessary if FFmpeg libraries have been installed elsewhere (it's just a convenient method of distributing the binaries with your application): just point this project's FFmpegLoader at the appropriate directory if it can't find the binaries on its own.

# Features

* Searches for appropriate FFmpeg libraries in any number of customizable locations.
* Registers the located binaries for use with [FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen).
* Works around a restriction on some Windows systems which prevents DLL libraries being loaded from non-default locations.

# Setup

Install the **FFmpeg.Loader** package from [NuGet.org](https://www.nuget.org/packages/FFmpeg.Loader/) into your project using the following dotnet CLI:

```
dotnet add package FFmpeg.Loader
```

Or using the Package Manager Console with the following command:

```
PM> Install-Package FFmpeg.Loader
```

**FFmpeg libraries are not included in this package.** You will need to either also include a distribution package like [FFmpeg.Native](https://github.com/quamotion/ffmpeg-win32), or install them externally. For example to install externally:
* On Ubuntu or Debian: `sudo apt install ffmpeg`
* On Mac OS-X: `brew install ffmpeg`
* On Windows, download and extract a Windows build from the [official FFmpeg site](https://ffmpeg.org/download.html#build-windows)

# Usage

See the code samples below for basic usage of FFmpegLoader. If the libraries cannot be located, then a `DllNotFoundException` is thrown.

The *default* paths that FFmpegLoader searches for FFmpeg libraries are the assembly's directory in your executing application, and also several specific subdirectories relative to that, dependent on the current OS and architecture. Aside from the assembly directory, the subdirectories searched match the location that FFmpeg.Native installs to, i.e.:
* .\runtimes\win7-x64\native\{name}-{version}.dll
* .\runtimes\win7-x86\native\{name}-{version}.dll
* ./runtimes/linux-x64/native/lib{name}.so.{version}
* ./runtimes/osx-x64/native/lib{name}.{version}.dylib

```csharp
using FFmpeg.Loader;
```

```csharp
//Search a set of default paths for FFmpeg libraries, relative to the FFmpeg.Loader assembly and then set FFmpeg.AutoGen to load the first matching FFmpeg binaries.
FFmpegLoader.SearchDefaults().Load();

//Search a set of paths for FFmpeg libraries, and then set FFmpeg.AutoGen to load the first matching FFmpeg binaries.
FFmpegLoader.SearchPaths("/usr/lib/x86_64-linux-gnu", "/usr/bin/ffmpeg").Load();

//The following two examples are functionally identical and combines both of the approaches above.
//They first search a default set of paths relative to the FFmpeg.Loader assembly, and then search a list of manually specified paths.
//Finally they set FFmpeg.AutoGen to load the first matching FFmpeg binaries.
FFmpegLoader.SearchDefaults().ThenSearchPaths("/usr/lib/x86_64-linux-gnu", "/usr/bin/ffmpeg").Load();
FFmpegLoader.SearchDefaults().ThenSearchPaths("/usr/lib/x86_64-linux-gnu").ThenSearchPaths("/usr/bin/ffmpeg").Load();

//The following two examples are functionally identical and search the same paths as above, but search the manually specified paths first
//and then fall back on searching a set of default paths.
//As above, they finally set FFmpeg.AutoGen to load the first matching FFmpeg binaries.
FFmpegLoader.SearchPaths("/usr/lib/x86_64-linux-gnu", "/usr/bin/ffmpeg").ThenSearchDefaults().Load();
FFmpegLoader.SearchPaths("/usr/lib/x86_64-linux-gnu").ThenSearchPaths("/usr/bin/ffmpeg").ThenSearchDefaults().Load();
```

After defining the initial search paths, there are several more methods and overloads you may find useful:

```csharp
//Returns an instance with additional search-locations. This method can be chained as many times as necessary.
//Additional locations are a predefined set of defaults relative to the specified rootDir parameter.
//If rootDir is null then the FFmpegLoader assembly folder is used for resolving relative paths.
FFmpegLoaderSearch ThenSearchDefaults(string rootDir = null);

//Returns an instance with additional search-locations. This method can be chained as many times as necessary.
//Values provided in searchPaths are expected to be either absolute or relative to the directory containing the FFmpegLoader assembly.
FFmpegLoaderSearch ThenSearchPaths(params string[] searchPaths);

//Locates a specific FFmpeg library with a specific version. Returns null if no matching library is found.
IFileInfo Find(string name, int version);

//Locates a specific FFmpeg library with a version number provided by FFmpeg.AutoGen. Returns null if no matching library is found.
IFileInfo Find(string name);

//Search the defined search-locations for an FFmpeg library with a specific name and version and set FFmpeg.AutoGen to load from there.
//Throws DllNotFoundException if no matching library is found.
string Load(string name, int version);

//Search the defined search-locations for an FFmpeg library with a specific name (version provided by FFmpeg.AutoGen) and set FFmpeg.AutoGen to load from there.
//Throws DllNotFoundException if no matching library is found.
string Load(string name = "avutil");
```

# FFmpeg versions

This library searches for specific FFmpeg library versions. If your FFmpeg DLLs have different version numbers than expected, then they won't be found. The versions it looks for are pulled from the FFmpeg.AutoGen project's [`ffmpeg.LibraryVersionMap[]`](https://raw.githubusercontent.com/Ruslan-B/FFmpeg.AutoGen/master/FFmpeg.AutoGen/FFmpeg.libraries.g.cs) dictionary.

At the time of writing this, FFmpeg.AutoGen 4.x currently specifies:
Library   |Version
---       |---
avcodec   |58
avdevice  |58
avfilter  |7
avformat  |58
avutil    |56
postproc  |55
swresample|3
swscale   |5

If any of your FFmpeg binaries are of a different version, you may need to explicitly install a newer/older version of FFmpeg.AutoGen into your project.

# Attribution

This project contains some C# code that was originally based on, and modified, from the LGPL-3.0 licensed [FFmpeg.Native](https://github.com/quamotion/ffmpeg-win32) project.
