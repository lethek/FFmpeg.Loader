using System.Runtime.InteropServices;

namespace FFmpeg.Loader;

internal static class PlatformInformation
{
    internal static OperatingSystem GetCurrentOS()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return OperatingSystem.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return OperatingSystem.Linux;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OperatingSystem.OSX;
        return OperatingSystem.Other;
    }
}