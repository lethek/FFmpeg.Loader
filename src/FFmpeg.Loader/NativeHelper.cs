using System.Runtime.InteropServices;


namespace FFmpeg.Loader;

internal static class NativeHelper
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SetDllDirectory(string? dir);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int AddDllDirectory(string dir);


    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetDefaultDllDirectories(uint flags);


    internal static int GetLastError()
        => Marshal.GetLastWin32Error();


    internal const uint LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200;
    internal const uint LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400;
    internal const uint LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;
    internal const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;
}