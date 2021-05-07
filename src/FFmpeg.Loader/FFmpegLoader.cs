using System;
using System.IO;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;
using FFmpeg.Loader.Locators;


namespace FFmpeg.Loader
{

    public static class FFmpegLoader
    {
        static FFmpegLoader()
        {
            CurrentOS = GetCurrentOS();

            switch (CurrentOS) {
                case OperatingSystem.Windows:
                    Binaries = new WindowsBinaries();
                    break;
                case OperatingSystem.Linux:
                    Binaries = new LinuxBinaries();
                    break;
                case OperatingSystem.OSX:
                    Binaries = new MacOsBinaries();
                    break;
                default:
                    Binaries = null;
                    break;
            }
        }


        /// <summary>
        /// Search a set of default paths and specified paths for FFmpeg libraries, relative to the current executable,
        /// and set FFmpeg.AutoGen to load from there.
        /// </summary>
        /// <param name="searchPaths">A list of paths to search first before checking the default locations.</param>
        /// <returns>FFmpeg's reported version number string. If the libraries fail to load for any reason, an exception
        /// such as <see cref="DllNotFoundException"/> is thrown instead.</returns>
        public static string RegisterBinaries(params string[] searchPaths)
        {
            var dir = FindLibraryDirectory("avcodec", searchPaths)
                ?? throw new DllNotFoundException();

            if (CurrentOS == OperatingSystem.Windows) {
                NativeHelper.SetDllDirectory(dir);
            }

            ffmpeg.RootPath = dir;
            return ffmpeg.av_version_info();
        }


        /// <summary>
        /// Locates a specific FFmpeg library with a specific version.
        /// </summary>
        /// <param name="name">Name of the FFmpeg library (e.g. avcodec, swresample, etc.)</param>
        /// <param name="version">The version of the library (e.g. 58)</param>
        /// <param name="searchPaths">A list of paths to search first before checking the default locations.</param>
        /// <returns>The full path to the library or null if none are found.</returns>
        public static string FindLibrary(string name, int version, params string[] searchPaths)
            => Binaries != null
                ? Binaries.FindFFmpegLibrary(name, version, searchPaths)
                : throw new PlatformNotSupportedException();


        /// <summary>
        /// Locates a specific FFmpeg library with a version provided by FFmpeg.AutoGen.
        /// </summary>
        /// <param name="name">Name of the FFmpeg library (e.g. avcodec, swresample, etc.)</param>
        /// <param name="searchPaths">A list of paths to search first before checking the default locations.</param>
        /// <returns>The full path to the library or null if none are found.</returns>
        public static string FindLibrary(string name, params string[] searchPaths)
            => FindLibrary(name, ffmpeg.LibraryVersionMap[name], searchPaths);


        /// <summary>
        /// Locates a folder containing a specific FFmpeg library with a specific version.
        /// </summary>
        /// <param name="name">Name of the FFmpeg library (e.g. avcodec, swresample, etc.)</param>
        /// <param name="version">The version of the library (e.g. 58)</param>
        /// <param name="searchPaths">A list of paths to search first before checking the default locations.</param>
        /// <returns>The full path to the parent directory of the library or null if none are found.</returns>
        public static string FindLibraryDirectory(string name, int version, params string[] searchPaths)
        {
            var lib = Path.GetDirectoryName(FindLibrary(name, version, searchPaths));
            return lib != null ? Path.GetFullPath(lib) : null;
        }


        /// <summary>
        /// Locates a folder containing a specific FFmpeg library with a version provided by FFmpeg.AutoGen.
        /// </summary>
        /// <param name="name">Name of the FFmpeg library (e.g. avcodec, swresample, etc.)</param>
        /// <param name="searchPaths">A list of paths to search first before checking the default locations.</param>
        /// <returns>The full path to the parent directory of the library or null if none are found.</returns>
        public static string FindLibraryDirectory(string name, params string[] searchPaths)
        {
            var lib = Path.GetDirectoryName(FindLibrary(name, searchPaths));
            return lib != null ? Path.GetFullPath(lib) : null;
        }


        private static OperatingSystem GetCurrentOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return OperatingSystem.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return OperatingSystem.Linux;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OperatingSystem.OSX;
            return OperatingSystem.Other;
        }


        private static readonly OperatingSystem CurrentOS;
        private static readonly BinariesBase Binaries;
    }

}
