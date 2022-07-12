using FFmpeg.Loader;

try {
    string version = FFmpegLoader.SearchDefaults().Load("asdf");
    Console.WriteLine($"Loaded FFmpeg v{version}");

} catch (DllNotFoundException) {
    Console.WriteLine("Could not find FFmpeg");
}
