using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FFmpeg.Loader.Locators;

internal static class LocatorFactory
{
    internal static readonly OperatingSystem CurrentOS = GetCurrentOS();


    internal static BaseLocator CreateCustomForCurrentOS(string rootDir, IEnumerable<string> searchPaths)
        => CurrentOS switch {
            OperatingSystem.Windows => new CustomWindowsLocator(rootDir, searchPaths.Where(StringHasValue)),
            OperatingSystem.Linux => new CustomLinuxLocator(rootDir, searchPaths.Where(StringHasValue)),
            OperatingSystem.OSX => new CustomMacLocator(rootDir, searchPaths.Where(StringHasValue)),
            _ => throw new PlatformNotSupportedException()
        };


    internal static BaseLocator CreateDefaultForCurrentOS(string rootDir)
        => CurrentOS switch {
            OperatingSystem.Windows => new DefaultWindowsLocator(rootDir),
            OperatingSystem.Linux => new DefaultLinuxLocator(rootDir),
            OperatingSystem.OSX => new DefaultMacLocator(rootDir),
            _ => throw new PlatformNotSupportedException()
        };


    private static OperatingSystem GetCurrentOS()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return OperatingSystem.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return OperatingSystem.Linux;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OperatingSystem.OSX;
        return OperatingSystem.Other;
    }


    private static bool StringHasValue(string str)
        => !String.IsNullOrEmpty(str);
}