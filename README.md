# FFmpeg.Loader

[![NuGet Stats](https://img.shields.io/nuget/v/FFmpeg.Loader.svg)](https://www.nuget.org/packages/FFmpeg.Loader)
[![Build & Publish](https://github.com/lethek/FFmpeg.Loader/actions/workflows/dotnet.yml/badge.svg)](https://github.com/lethek/FFmpeg.Loader/actions/workflows/dotnet.yml)
[![GitHub license](https://img.shields.io/github/license/lethek/FFmpeg.Loader)](https://github.com/lethek/FFmpeg.Loader/blob/main/LICENSE)

FFmpegLoader will find and load FFmpeg libraries with the [FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen) bindings. It's goal is to provide a simple and customizable way of doing this, cross-platform.

This library was originally designed for use with [FFmpeg.Native](https://github.com/quamotion/ffmpeg-win32) as a more flexible alternative to FFmpeg.Native's tooling for finding FFmpeg libs. However please note, FFmpeg.Native is unnecessary if FFmpeg libraries have been installed elsewhere (it's just a convenient method of distributing the binaries with your application): just point this project's FFmpegLoader at the appropriate directory if it can't find the binaries on its own.

# Features

* Searches for appropriate FFmpeg libraries in any number of customizable locations.
* Registers the located binaries for use with [FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen).
* **Works around a restriction on some Windows systems which prevents DLL libraries being loaded from non-default locations.**

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

See the [Code Samples](#code-samples) below for basic usage of FFmpegLoader.

## Notes

The search is only performed when `Load()` or `Find()` are called. The Load method throws `DllNotFoundException` if it can't find the libraries. And the Find method returns `null` if it can't find them.

There is no need to call any Load method more than once (e.g. for multiple individual library files): FFmpeg.AutoGen will use *all* the necessary FFmpeg library files from the same located directory when you attempt to use them.

The default application paths that `SearchApplication()` searches are the assembly's directory in your executing application, and also several specific subdirectories relative to that, dependent on the current OS and architecture. Aside from the assembly directory, these subdirectories searched match the location that FFmpeg.Native installs to, i.e.:
* On Linux (x64): **./runtimes/linux-x64/native/lib{name}.so.{version}**
* On Mac OS-X (x64): **./runtimes/osx-x64/native/lib{name}.{version}.dylib**
* On Windows (x64): **.\runtimes\win7-x64\native\{name}-{version}.dll**
* On Windows (x86): **.\runtimes\win7-x86\native\{name}-{version}.dll**

The default system paths that `SearchSystem()` searches depend on the current OS.
* On Linux systems this will search: **/usr/lib/x86_64-linux-gnu**, **/usr/lib/aarch64-linux-gnu** and **/usr/lib**
* On Mac OS-X systems this will search: **/usr/local/lib**
* On Windows systems this will search: **C:\ffmpeg\bin**

## Code Samples

```csharp
using FFmpeg.Loader;
```

```csharp
//Search a set of default paths for FFmpeg libraries, relative to the FFmpeg.Loader assembly and
//then load the first matching binaries with FFmpeg.AutoGen.
FFmpegLoader.SearchApplication().Load();

//Search a set of default system paths for the FFmpeg libaries and then load the first matching
//binaries.
FFmpegLoader.SearchSystem().Load();

//Search the system's environment PATH for FFmpeg libraries and then load the first matching
//binaries.
FFmpegLoader.SearchEnvironmentPaths().Load();

//Search a set of specified paths for FFmpeg libraries and then load the first matching binaries.
FFmpegLoader.SearchPaths("/home/user/bin/ffmpeg", "/usr/bin/ffmpeg").Load();

//Calls can be chained and searches are conducted in a fallback sequence. When the 1st match is
//found, no further searching is done.
FFmpegLoader
	.SearchApplication()
	.ThenSearchEnvironmentPaths("FFMPEG_PATH")
	.ThenSearchSystem()
	.Load();

//The following two examples are functionally identical and combine both of the approaches above.
//They first search a default set of paths relative to the FFmpeg.Loader assembly, and then search a
//list of manually specified paths.
//Finally they set FFmpeg.AutoGen to load the first matching FFmpeg binaries.
FFmpegLoader.SearchApplication().ThenSearchPaths(@"c:\ffmpeg", "/usr/bin/ffmpeg").Load();
FFmpegLoader.SearchApplication().ThenSearchPaths(@"c:\ffmpeg").ThenSearchPaths("/usr/bin/ffmpeg").Load();
```

After defining the initial search paths, there are several more methods and overloads you may find useful:

```csharp
//Returns an instance with additional search-locations. This method can be chained as many times as 
//necessary.
//Additional locations are a predefined set of defaults relative to the specified rootDir parameter.
//If rootDir is null then the FFmpegLoader assembly folder is used for resolving relative paths.
FFmpegLoaderSearch ThenSearchApplication(string rootDir = null);

//Returns an instance with additional search-locations. This method can be chained as many times as
//necessary (not that there's any point using it in your chain more than once).
//Additional locations are a predefined set of defaults specific to the operating system.
//If rootDir is null then the FFmpegLoader assembly folder is used for resolving relative paths.
FFmpegLoaderSearch ThenSearchSystem();

//Returns an instance with additional search-locations. This method can be chained as many times as
//necessary.
//Values provided in searchPaths are expected to be either absolute or relative to the directory
//containing the FFmpegLoader assembly.
FFmpegLoaderSearch ThenSearchPaths(params string[] searchPaths);

//Returns an instance with additional search-locations. This method can be chained as many times as
//necessary.
//Paths are read from the specified envVar; the appropriate PATH separator for each OS is recognized
//and handled (":" on Linux & OSX, ";" on Windows).
FFmpegLoaderSearch ThenSearchEnvironmentPaths(string envVar = "PATH");

//Locates a specific FFmpeg library with a specific version, but does not load it. Returns null if
//no matching library is found.
IFileInfo Find(string name, int version);

//Locates a specific FFmpeg library with a version number provided by FFmpeg.AutoGen, but does not
//load it. Returns null if no matching library is found.
IFileInfo Find(string name);

//Search the defined search-locations for an FFmpeg library with a specific name and version and
//loads its directory with FFmpeg.AutoGen.
//If a matching library file was found, FFmpegLoader assumes that all other library files you
//require are also present in the same directory.
//Returns FFmpeg's self-reported version string (e.g. "4.4.2-0ubuntu0.22.04.1").
//Throws DllNotFoundException if no matching library is found.
string Load(string name, int version);

//Search the defined search-locations for an FFmpeg library with a specific name (version provided
//by FFmpeg.AutoGen) and loads its directory with FFmpeg.AutoGen.
//If a matching library file was found, FFmpegLoader assumes that all other library files you
//require are also present in the same directory.
//Returns FFmpeg's self-reported version string (e.g. "4.4.2-0ubuntu0.22.04.1").
//Throws DllNotFoundException if no matching library is found.
string Load(string name = "avutil");
```

# FFmpeg versions

This library searches for specific FFmpeg library versions. If your FFmpeg DLLs have different version numbers than expected, then they won't be found. The versions it looks for are pulled from the FFmpeg.AutoGen project's [`ffmpeg.LibraryVersionMap[]`](https://raw.githubusercontent.com/Ruslan-B/FFmpeg.AutoGen/master/FFmpeg.AutoGen/FFmpeg.libraries.g.cs) dictionary.

FFmpeg.AutoGen 4.x currently expects to bind to these library versions:

|Library   |Version|
|----------|-------|
|avcodec   |58     |
|avdevice  |58     |
|avfilter  |7      |
|avformat  |58     |
|avutil    |56     |
|postproc  |55     |
|swresample|3      |
|swscale   |5      |

And FFmpeg.AutoGen 5.x currently expects to bind to these library versions:

|Library   |Version|
|----------|-------|
|avcodec   |59     |
|avdevice  |59     |
|avfilter  |8      |
|avformat  |59     |
|avutil    |57     |
|postproc  |56     |
|swresample|4      |
|swscale   |6      |

If any of your FFmpeg binaries are of a different version, you may need to explicitly install a newer/older version of FFmpeg.AutoGen into your project.

# Attribution

This project contains some C# code that was originally based on, and heavily modified, from the LGPL-3.0 licensed [FFmpeg.Native](https://github.com/quamotion/ffmpeg-win32) project.
