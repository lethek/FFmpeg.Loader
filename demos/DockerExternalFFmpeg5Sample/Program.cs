using FFmpeg.Loader;

try {
    string version = FFmpegLoader.SearchSystem().Load();

    Console.WriteLine($"Loaded FFmpeg v{version}");

} catch (DllNotFoundException) {
    Console.WriteLine("Could not find FFmpeg");
}
