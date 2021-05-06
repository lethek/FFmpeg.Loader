using System.IO.Abstractions;
using System.Linq;


namespace FFmpeg.Loader.Locators
{

    internal class MacOsBinaries : BinariesBase
    {
        internal MacOsBinaries() { }


        internal MacOsBinaries(IFileSystem fileSystem)
            : base(fileSystem) { }


        public override string FindFFmpegLibrary(string name, int version, params string[] searchPaths)
        {
            var first = searchPaths
                .SelectMany(x => new[] {
                    x, 
                    FileSystem.Path.Combine(x, "runtimes", "osx-x64", "native")
                });

            var paths = new[] {
                FileSystem.Path.Combine("..", "..", "runtimes", "osx-x64", "native"),
                FileSystem.Path.Combine(".", "runtimes", "osx-x64", "native"),
                "."
            };

            return FindLibrary($"lib{name}.{version}.dylib", first.Concat(paths));
        }
    }

}
