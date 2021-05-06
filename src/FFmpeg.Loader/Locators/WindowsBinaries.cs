using System;
using System.IO.Abstractions;
using System.Linq;


namespace FFmpeg.Loader.Locators
{

    internal class WindowsBinaries : BinariesBase
    {
        internal WindowsBinaries() { }


        internal WindowsBinaries(IFileSystem fileSystem)
            : base(fileSystem) { }


        public override string FindFFmpegLibrary(string name, int version, params string[] searchPaths)
        {
            string runtime = Environment.Is64BitProcess ? "win7-x64" : "win7-x86";

            var first = searchPaths
                .SelectMany(x => new[] {
                    x,
                    FileSystem.Path.Combine(x, "runtimes", runtime, "native")
                });

            var paths = new[] {
                FileSystem.Path.Combine("..", "..", "runtimes", runtime, "native"),
                FileSystem.Path.Combine(".", "runtimes", runtime, "native"),
                "."
            };

            return FindLibrary($"{name}-{version}.dll", first.Concat(paths));
        }
    }

}
