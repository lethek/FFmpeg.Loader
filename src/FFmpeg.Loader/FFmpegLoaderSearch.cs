using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FFmpeg.AutoGen;
using FFmpeg.Loader.Locators;

namespace FFmpeg.Loader;

public record FFmpegLoaderSearch
{
    internal FFmpegLoaderSearch(IEnumerable<BaseLocator> locators)
        => Locators = new List<BaseLocator>(locators).ToImmutableList();


    /// <summary>
    /// Adds to the list of search-locations: a predefined set of defaults based on the current operating system and relative to the specified <paramref name="rootDir"/>
    /// directory (the directory containing the FFmpegLoader assembly if <paramref name="rootDir"/> is <see langword="null" />).
    /// </summary>
    /// <param name="rootDir">The root-directory to use when resolving default relative paths. E.g. typically the application's root directory for binaries.
    /// If <see langword="null" /> then the directory which contains th FFmpegLoader assembly is used.</param>
    /// <returns>A new instance of <see cref="FFmpegLoaderSearch"/> with the additional search-locations.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if using an unsupported operating system, i.e. anything other than Windows, Linux or Mac OSX.</exception>
    public FFmpegLoaderSearch ThenSearchApplication(string rootDir = null)
        => this with {
            Locators = Locators.Add(LocatorFactory.CreateAppDefaultForCurrentOS(rootDir))
        };


    /// <summary>
    /// Add to the list of search-locations: a predefined set of defaults based on the current operating system.
    /// </summary>
    /// <returns>A new instance of <see cref="FFmpegLoaderSearch"/> with the initial search-locations.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if using an unsupported operating system, i.e. anything other than Windows, Linux or Mac OSX.</exception>
    public FFmpegLoaderSearch ThenSearchSystem()
        => this with {
            Locators = Locators.Add(LocatorFactory.CreateSystemDefaultForCurrentOS())
        };


    /// <summary>
    /// Adds to the list of search-locations: a custom set of paths for the current operating system. Search paths are expected to be either absolute or relative
    /// to the directory containing the FFmpegLoader assembly.
    /// </summary>
    /// <param name="searchPaths">Additional search-locations. Search paths are expected to be absolute or relative to the directory containing the FFmpegLoader assembly.</param>
    /// <returns>A new instance of <see cref="FFmpegLoaderSearch"/> with the additional search-locations.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if using an unsupported operating system, i.e. anything other than Windows, Linux or Mac OSX.</exception>
    public FFmpegLoaderSearch ThenSearchPaths(params string[] searchPaths)
        => this with {
            Locators = Locators.Add(LocatorFactory.CreateCustomForCurrentOS(null, searchPaths))
        };


    /// <summary>
    /// Adds to the list of search-locations: paths from an environment variable (by default the PATH environment variable).
    /// </summary>
    /// <param name="envVar">Name of the environment variable to pull search paths from. The default is the standard PATH variable.</param>
    /// <returns>A new instance of <see cref="FFmpegLoaderSearch"/> with the initial search-locations.
    /// Call <see cref="ThenSearchApplication">ThenSearchApplication</see>, <see cref="FFmpegLoaderSearch.ThenSearchPaths">ThenSearchPaths</see> or
    /// <see cref="FFmpegLoaderSearch.ThenSearchEnvironmentPaths">ThenSearchEnvironmentPaths</see> on that to add additional search locations.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if using an unsupported operating system, i.e. anything other than Windows, Linux or Mac OSX.</exception>
    public FFmpegLoaderSearch ThenSearchEnvironmentPaths(string envVar = "PATH")
        => this with {
            Locators = Locators.Add(LocatorFactory.CreateCustomForCurrentOS(null, new[] { Environment.GetEnvironmentVariable(envVar) }))
        };


    public FFmpegLoaderSearch DownloadIfMissing(string dstDir, string target = null, string version = null, string license = License.LGPL)
    {
        if (target == null) {
            var platform = PlatformInformation.GetCurrentOS() switch {
                OperatingSystem.Windows => "win",
                OperatingSystem.Linux => "linux",
                OperatingSystem.OSX => throw new NotImplementedException(),
                _ => throw new PlatformNotSupportedException()
            };
            var architecture = RuntimeInformation.OSArchitecture switch {
                Architecture.X64 => "64",
                Architecture.X86 => "32",
                Architecture.Arm64 => "arm64",
                _ => throw new PlatformNotSupportedException()
            };
            target = platform + architecture;
        }

        version ??= ffmpeg.LibraryVersionMap["avcodec"] switch {
            58 => "4.4",
            59 => "5.0",
            _ => "5.0"
        };

        string ext = target.StartsWith("win", StringComparison.OrdinalIgnoreCase) ? "zip" : "tar.xz";

        return this with {
            DownloadFrom = $"https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-n{version}-latest-{target}-{license}-shared-{version}.{ext}",
            DownloadTo = dstDir
        };
    }


    /// <summary>
    /// Locates a specific FFmpeg library with a specific version.
    /// </summary>
    /// <param name="name">Name of the FFmpeg library (e.g. avutil, avcodec, swresample, etc.)</param>
    /// <param name="version">The version of the library (e.g. 56)</param>
    /// <returns>An IFileInfo object representing the located library or <see langword="null" /> if none are found.</returns>
    public IFileInfo Find(string name, int version)
        => Locators
            .Select(x => x.FindFFmpegLibrary(name, version))
            .FirstOrDefault(x => x != null);


    /// <summary>
    /// Locates a specific FFmpeg library with a version number provided by FFmpeg.AutoGen.
    /// </summary>
    /// <param name="name">Name of the FFmpeg library (e.g. avutil, avcodec, swresample, etc.)</param>
    /// <returns>An IFileInfo object representing the located library or <see langword="null" /> if none are found.</returns>
    public IFileInfo Find(string name)
        => Find(name, ffmpeg.LibraryVersionMap[name]);



    /// <summary>
    /// Search the defined search-locations for FFmpeg libraries and set FFmpeg.AutoGen to load from there.
    /// </summary>
    /// <param name="name">Name of the FFmpeg library (e.g. avutil, avcodec, swresample, etc.)</param>
    /// <param name="version">The version of the library (e.g. 56)</param>
    /// <returns>FFmpeg's reported version number string.</returns>
    /// <exception cref="DllNotFoundException">Thrown if the required FFmpeg library could not be found in any of the specified search-locations.</exception>
    public async Task<string> LoadAsync(string name, int version)
    {
        var lib = Find(name, version);
        if (lib == null) {
            if (DownloadFrom == null) {
                throw new DllNotFoundException();
            } else {
                if (DownloadFrom.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) {
                    using var client = new HttpClient();
                    using var stream = await client.GetStreamAsync(DownloadFrom);
                    using var archive = new ZipArchive(stream, ZipArchiveMode.Read, false);
                    var entries = archive.Entries.Where(x => FFmpegDownloadBinaries.IsMatch(x.FullName));
                    Directory.CreateDirectory(DownloadTo);
                    foreach (var entry in entries) {
                        entry.ExtractToFile(Path.Combine(DownloadTo, entry.Name));
                    }

                } else if (DownloadFrom.EndsWith(".tar.xz", StringComparison.OrdinalIgnoreCase)) {
                    throw new NotImplementedException();

                } else {
                    throw new NotSupportedException("Unsupported archive format");
                }

                var locator = LocatorFactory.CreateCustomForCurrentOS(DownloadTo, new[] { "bin" });
                lib = locator.FindFFmpegLibrary(name, version) ?? throw new DllNotFoundException();
            }
        }
        var dir = lib.DirectoryName;

        if (LocatorFactory.CurrentOS == OperatingSystem.Windows) {
            NativeHelper.SetDllDirectory(dir);
        }

        ffmpeg.RootPath = dir;
        return ffmpeg.av_version_info();
    }


    /// <summary>
    /// Search the defined search-locations for FFmpeg libraries and set FFmpeg.AutoGen to load from there.
    /// </summary>
    /// <param name="name">Name of the FFmpeg library (e.g. avutil, avcodec, swresample, etc.)</param>
    /// <param name="version">The version of the library (e.g. 56)</param>
    /// <returns>FFmpeg's reported version number string.</returns>
    /// <exception cref="DllNotFoundException">Thrown if the required FFmpeg library could not be found in any of the specified search-locations.</exception>
    public string Load(string name, int version)
    {
        var lib = Find(name, version) ?? throw new DllNotFoundException();
        var dir = lib.DirectoryName;

        if (LocatorFactory.CurrentOS == OperatingSystem.Windows) {
            NativeHelper.SetDllDirectory(dir);
        }

        ffmpeg.RootPath = dir;
        return ffmpeg.av_version_info();
    }


    /// <summary>
    /// Search the defined search-locations for FFmpeg libraries and set FFmpeg.AutoGen to load from there. FFmpeg.AutoGen is queried for supported library version
    /// numbers and only a library with a matching version will be loaded.
    /// </summary>
    /// <param name="name">Name of an FFmpeg library to locate. If not provided, the default is "avutil". Valid values are: avcodec, avdevice, avfilter, avformat, avutil, postproc, swresample, swscale.</param>
    /// <returns>FFmpeg's reported version number string.</returns>
    /// <exception cref="DllNotFoundException">Thrown if the required FFmpeg library (default: avutil) could not be found in any of the specified search-locations.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if an unrecognized library name was provided.</exception>
    public string Load(string name = "avutil")
        => Load(name, ffmpeg.LibraryVersionMap[name]);


    private ImmutableList<BaseLocator> Locators { get; init; } = ImmutableList<BaseLocator>.Empty;
    private string DownloadFrom { get; init; }
    private string DownloadTo { get; init; }


    private static readonly Regex FFmpegDownloadBinaries = new(@"/bin/\w+-\d+\.dll$", RegexOptions.Compiled);
}
