using System.Collections.Generic;
using System.IO.Abstractions;
using System.Reflection;


namespace FFmpeg.Loader.Locators
{

    internal abstract class BinariesBase
    {
        protected BinariesBase()
            : this(new FileSystem()) { }


        protected BinariesBase(IFileSystem fileSystem)
            => FileSystem = fileSystem;


        public abstract string FindFFmpegLibrary(string name, int version, params string[] searchPaths);


        internal string FindLibrary(string fileName, IEnumerable<string> relativePaths)
        {
            var assembly = typeof(BinariesBase).GetTypeInfo().Assembly;
            var assemblyLocation = assembly.Location;
            var assemblyDirectory = FileSystem.Path.GetDirectoryName(assemblyLocation);

            foreach (string relativePath in relativePaths) {
                string fullFileName = FileSystem.Path.Combine(assemblyDirectory, relativePath, fileName);
                if (FileSystem.File.Exists(fullFileName)) {
                    return fullFileName;
                }
            }

            //Couldn't find the library.
            return null;
        }


        internal IFileSystem FileSystem { get; }
    }

}
