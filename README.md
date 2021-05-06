# FFmpeg.Loader
[![build](https://img.shields.io/azure-devops/build/SMMX/FFmpeg.Loader/3)](https://dev.azure.com/SMMX/FFmpeg.Loader/_build?definitionId=3)
[![test](https://img.shields.io/azure-devops/tests/SMMX/FFmpeg.Loader/3)](https://dev.azure.com/SMMX/FFmpeg.Loader/_build?definitionId=3)
[![nuget](https://img.shields.io/nuget/v/FFmpeg.Loader)](https://www.nuget.org/packages/FFmpeg.Loader/)

Tooling to find and load FFmpeg libraries with the [FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen) bindings.

This library is also intended for use with [FFmpeg.Native](https://github.com/quamotion/ffmpeg-win32) (note: at present, that project's NuGet packages are all *pre-release*). However, FFmpeg.Native is not necessary if FFmpeg libraries have been installed elsewhere: just point this project's FFmpegLoader at the appropriate directory if it can't find the binaries on its own.

# Features
* Searches for appropriate FFmpeg libraries in a number of locations relative to the current executable.
* Can be configured to also first search a custom list of locations.
* Registers the located binaries for use with [FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen).
* Works around a restriction on some Windows systems which prevents a DLL library being loaded from a non-default location.

# Usage
The static `FFmpegLoader.RegisterBinaries(params string[] searchPaths)` method returns FFmpeg's reported version number string. If the libraries fail to load for any reason, an exception such as `DllNotFoundException` is thrown instead.

The *default* paths it searches for FFmpeg libraries are the current executable's directory and several specific subdirectories dependent on the current OS and architecture. These default paths match the location that FFmpeg.Native installs to, e.g.:
* .
* .\runtimes\win7-x64\native\{name}-{version}.dll
* ./runtimes/linux-x64/native/lib{name}.so.{version}
* etc.

```csharp
using FFmpeg.Loader;
```
```csharp
//Search a set of default paths for FFmpeg libraries, relative to the current executable, and set FFmpeg.AutoGen to load from there.
FFmpegLoader.RegisterBinaries();

//Same as above, but it first searches a list of user-provided locations. The method-signature is params string[] so you can supply as many search paths as you need.
FFmpegLoader.RegisterBinaries("./someSubDir", @"C:\ffmpeg", "SomeOtherDir");
```

The `FFmpegLoader` class has several more static convenience methods for finding libaries but not loading them. A `null` value is returned if no matching libraries are found. Their signatures are:

```csharp
//Locates and returns the full path to a specific library with a specific version
string FindLibrary(string name, int version, params string[] searchPaths);

//Locates and returns the full path to a specific library with a version provided by FFmpeg.AutoGen
string FindLibrary(string name, params string[] searchPaths);

//Locates and returns the parent directory for a specific library with a specific version
string FindLibraryDirectory(string name, int version, params string[] searchPaths);

//Locates and returns the parent directory for a specific library with a version provided by FFmpeg.AutoGen
string FindLibraryDirectory(string name, params string[] searchPaths);
```

# FFmpeg versions
This library searches for specific FFmpeg library versions. If your FFmpeg DLLs have different version numbers than expected, then they won't be found. The versions it looks for are pulled from the FFmpeg.AutoGen project's [`ffmpeg.LibraryVersionMap[]`](https://raw.githubusercontent.com/Ruslan-B/FFmpeg.AutoGen/master/FFmpeg.AutoGen/FFmpeg.libraries.g.cs) dictionary.

At the time of writing this, FFmpeg.AutoGen currently specifies:
|Library   |Version|
|---       |---    |
|avcodec   |58     |
|avdevice  |58     |
|avfilter  |7      |
|avformat  |58     |
|avutil    |56     |
|postproc  |55     |
|swresample|3      |
|swscale   |5      |

If any of your FFmpeg binaries are of a different version, you may need to explicitly install a newer/older version of FFmpeg.AutoGen into your project.

# Attribution
This project contains some C# code based on, and modified, from the LGPL-3.0 licensed [FFmpeg.Native](https://github.com/quamotion/ffmpeg-win32) project.
