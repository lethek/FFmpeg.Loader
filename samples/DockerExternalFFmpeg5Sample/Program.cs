using FFmpeg.Loader;

try {
    string version = FFmpegLoader
        .SearchDefaults()
        .ThenSearchPaths("/usr/lib/x86_64-linux-gnu") //On recent versions of Debian & Ubuntu, FFmpeg libs installed using apt are usually in this dir
        .Load();

    Console.WriteLine($"Loaded FFmpeg v{version}");

} catch (DllNotFoundException) {
    Console.WriteLine("Could not find FFmpeg");
}
