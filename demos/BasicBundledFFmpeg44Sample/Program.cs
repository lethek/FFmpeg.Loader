using FFmpeg.Loader;

try {
    string version = FFmpegLoader
        .SearchApplication() //Search for a bundled copy of FFmpeg
        .ThenSearchEnvironmentPaths("FFMPEG_PATH") //Search paths defined in an FFMPEG_PATH environment variable
        .ThenSearchSystem() //Search pre-defined default system paths (e.g. /usr/lib/x86_64-linux-gnu/)
        .ThenSearchEnvironmentPaths() //Search paths defined in the PATH environment variable
        .Load();

    Console.WriteLine($"Successfully loaded FFmpeg v{version}");

} catch (DllNotFoundException) {
    Console.WriteLine("Could not find FFmpeg");
}
