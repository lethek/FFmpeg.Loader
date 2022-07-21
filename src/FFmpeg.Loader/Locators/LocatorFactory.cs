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


    internal static BaseLocator CreateAppDefaultForCurrentOS(string rootDir)
        => CurrentOS switch {
            OperatingSystem.Windows => new AppDefaultWindowsLocator(rootDir),
            OperatingSystem.Linux => new AppDefaultLinuxLocator(rootDir),
            OperatingSystem.OSX => new AppDefaultMacLocator(rootDir),
            _ => throw new PlatformNotSupportedException()
        };


    internal static BaseLocator CreateSystemDefaultForCurrentOS()
        => CurrentOS switch {
            OperatingSystem.Windows => new SystemDefaultWindowsLocator(),
            OperatingSystem.Linux => new SystemDefaultLinuxLocator(),
            OperatingSystem.OSX => new SystemDefaultMacLocator(),
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