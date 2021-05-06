using System.IO.Abstractions;
using System.Linq;


namespace FFmpeg.Loader.Locators
{

    internal class LinuxBinaries : BinariesBase
    {
        internal LinuxBinaries() { }


        internal LinuxBinaries(IFileSystem fileSystem)
            : base(fileSystem) { }


        public override string FindFFmpegLibrary(string name, int version, params string[] searchPaths)
        {
            var first = searchPaths
                .SelectMany(x => new[] {
                    x,
                    FileSystem.Path.Combine(x, "runtimes", "linux-x64", "native")
                });

            var paths = new[] {
                FileSystem.Path.Combine("..", "..", "runtimes", "linux-x64", "native"),
                FileSystem.Path.Combine(".", "runtimes", "linux-x64", "native"),
                "."
            };

            return FindLibrary($"lib{name}.so.{version}", first.Concat(paths));
        }
    }

}
