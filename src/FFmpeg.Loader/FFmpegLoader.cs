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


        public static string FindLibrary(string name, int version, params string[] searchPaths)
            => Binaries != null
                ? Binaries.FindFFmpegLibrary(name, version, searchPaths)
                : throw new PlatformNotSupportedException();


        public static string FindLibrary(string name, params string[] searchPaths)
            => FindLibrary(name, ffmpeg.LibraryVersionMap[name], searchPaths);


        public static string FindLibraryDirectory(string name, int version, params string[] searchPaths)
        {
            var lib = Path.GetDirectoryName(FindLibrary(name, version, searchPaths));
            return lib != null ? Path.GetFullPath(lib) : null;
        }


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
