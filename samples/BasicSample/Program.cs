using FFmpeg.Loader;

try {
    string version = FFmpegLoader
        .SearchDefaults() //Search for a bundled copy of FFmpeg
        .ThenSearchEnvironmentPaths("FFMPEG_PATH") //Search paths defined in an FFMPEG_PATH environment variable
        .ThenSearchEnvironmentPaths() //Search paths defined in the PATH environment variable
        .Load();

    Console.WriteLine($"Loaded FFmpeg v{version}");

} catch (DllNotFoundException) {
    Console.WriteLine("Could not find FFmpeg");
}
