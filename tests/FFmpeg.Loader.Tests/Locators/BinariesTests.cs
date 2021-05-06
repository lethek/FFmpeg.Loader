using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;

using FFmpeg.Loader.Locators;

using Xunit;


namespace FFmpeg.Loader.Tests.Locators
{

    public class BinariesTests
    {
        public BinariesTests()
        {
            _assemblyDir = Path.GetDirectoryName(typeof(BinariesBase).GetTypeInfo().Assembly.Location);
        }


        [Fact]
        public void FindWindowsBinaries1() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var runtime = Environment.Is64BitProcess ? "win7-x64" : "win7-x86";

            var windowsBinaries = new WindowsBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "..", "..", "runtimes", runtime, "native", "avutil-56.dll")),
                Path.GetFullPath(windowsBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindWindowsBinaries2() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var runtime = Environment.Is64BitProcess ? "win7-x64" : "win7-x86";

            var windowsBinaries = new WindowsBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "runtimes", runtime, "native", "avutil-56.dll")),
                Path.GetFullPath(windowsBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindWindowsBinaries3() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var windowsBinaries = new WindowsBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "avutil-56.dll")), Path.GetFullPath(windowsBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindWindowsBinaries4() {
            var fileSystem = new MockFileSystem();
            //Get the mock file-system root this way so that the test runs correctly no matter what the host-runner's OS is
            var root = fileSystem.Path.Combine(fileSystem.DriveInfo.GetDrives().First().RootDirectory.FullName, "FFmpeg");
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib"));

            var runtime = Environment.Is64BitProcess ? "win7-x64" : "win7-x86";

            var windowsBinaries = new WindowsBinaries(fileSystem);
            Assert.Equal(
                fileSystem.Path.GetFullPath(fileSystem.Path.Combine(root, "runtimes", runtime, "native", "avutil-56.dll")),
                fileSystem.Path.GetFullPath(windowsBinaries.FindFFmpegLibrary("avutil", 56, root))
            );
        }


        [Fact]
        public void FindWindowsBinaries5() {
            var fileSystem = new MockFileSystem();
            //Get the mock file-system root this way so that the test runs correctly no matter what the host-runner's OS is
            var root = fileSystem.Path.Combine(fileSystem.DriveInfo.GetDrives().First().RootDirectory.FullName, "FFmpeg");
            fileSystem.AddFile(fileSystem.Path.Combine(root, "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "libavutil.so.56"), new("libavutil.so.56"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "libavutil.56.dylib"), new("libavutil.56.dylib"));

            var windowsBinaries = new WindowsBinaries(fileSystem);

            Assert.Equal(
                fileSystem.Path.GetFullPath(fileSystem.Path.Combine(root, "avutil-56.dll")),
                fileSystem.Path.GetFullPath(windowsBinaries.FindFFmpegLibrary("avutil", 56, root))
            );
        }


        [Fact]
        public void FindLinuxBinaries1() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var linuxBinaries = new LinuxBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine("..", "..", "runtimes", "linux-x64", "native", "libavutil.so.56")),
                Path.GetFullPath(linuxBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindLinuxBinaries2() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var linuxBinaries = new LinuxBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "runtimes", "linux-x64", "native", "libavutil.so.56")),
                Path.GetFullPath(linuxBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindLinuxBinaries3() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var linuxBinaries = new LinuxBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "libavutil.so.56")), Path.GetFullPath(linuxBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindLinuxBinaries4() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var linuxBinaries = new LinuxBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "linux-x64", "native", "libavutil.so.56")),
                Path.GetFullPath(linuxBinaries.FindFFmpegLibrary("avutil", 56, "/usr/local/bin/ffmpeg"))
            );
        }


        [Fact]
        public void FindLinuxBinaries5() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine("/usr/local/bin/ffmpeg", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var linuxBinaries = new LinuxBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine("/usr/local/bin/ffmpeg", "libavutil.so.56")),
                Path.GetFullPath(linuxBinaries.FindFFmpegLibrary("avutil", 56, "/usr/local/bin/ffmpeg"))
            );
        }


        [Fact]
        public void FindMacOsBinaries1() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "..", "..", "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var macOsBinaries = new MacOsBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine("..", "..", "runtimes", "osx-x64", "native", "libavutil.56.dylib")),
                Path.GetFullPath(macOsBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindMacOsBinaries2() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var macOsBinaries = new MacOsBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "runtimes", "osx-x64", "native", "libavutil.56.dylib")),
                Path.GetFullPath(macOsBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindMacOsBinaries3() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var macOsBinaries = new MacOsBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "libavutil.56.dylib")),
                Path.GetFullPath(macOsBinaries.FindFFmpegLibrary("avutil", 56))
            );
        }


        [Fact]
        public void FindMacOsBinaries4() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var macOsBinaries = new MacOsBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine("/usr/local/bin/ffmpeg", "runtimes", "osx-x64", "native", "libavutil.56.dylib")),
                Path.GetFullPath(macOsBinaries.FindFFmpegLibrary("avutil", 56, "/usr/local/bin/ffmpeg"))
            );
        }


        [Fact]
        public void FindMacOsBinaries5() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine("/usr/local/bin/ffmpeg", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine("/usr/local/bin/ffmpeg", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var macOsBinaries = new MacOsBinaries(fileSystem);

            Assert.Equal(
                Path.GetFullPath(Path.Combine("/usr/local/bin/ffmpeg", "libavutil.56.dylib")),
                Path.GetFullPath(macOsBinaries.FindFFmpegLibrary("avutil", 56, "/usr/local/bin/ffmpeg"))
            );
        }


        private readonly string _assemblyDir;
    }

}
