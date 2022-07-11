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

    public class LocatorTests
    {
        public LocatorTests()
            => _assemblyDir = Path.GetDirectoryName(typeof(BaseLocator).GetTypeInfo().Assembly.Location);


        [Fact]
        public void FindWindowsBinaries1() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var runtime = Environment.Is64BitProcess ? "win7-x64" : "win7-x86";

            var locator = new DefaultWindowsLocator(fileSystem, _assemblyDir);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "runtimes", runtime, "native", "avutil-56.dll")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindWindowsBinaries2() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var locator = new DefaultWindowsLocator(fileSystem, _assemblyDir);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "avutil-56.dll")), 
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindWindowsBinaries3() {
            var fileSystem = new MockFileSystem();
            //Get the mock file-system root this way so that the test runs correctly no matter what the host-runner's OS is
            var root = fileSystem.Path.Combine(fileSystem.DriveInfo.GetDrives().First().RootDirectory.FullName, "FFmpeg");
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib"));

            var runtime = Environment.Is64BitProcess ? "win7-x64" : "win7-x86";

            var locator = new DefaultWindowsLocator(fileSystem, root);

            Assert.Equal(
                fileSystem.Path.GetFullPath(fileSystem.Path.Combine(root, "runtimes", runtime, "native", "avutil-56.dll")),
                fileSystem.Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindWindowsBinaries4() {
            var fileSystem = new MockFileSystem();
            //Get the mock file-system root this way so that the test runs correctly no matter what the host-runner's OS is
            var root = fileSystem.Path.Combine(fileSystem.DriveInfo.GetDrives().First().RootDirectory.FullName, "FFmpeg");
            fileSystem.AddFile(fileSystem.Path.Combine(root, "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "libavutil.so.56"), new("libavutil.so.56"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "libavutil.56.dylib"), new("libavutil.56.dylib"));

            var locator = new DefaultWindowsLocator(fileSystem, root);

            Assert.Equal(
                fileSystem.Path.GetFullPath(fileSystem.Path.Combine(root, "avutil-56.dll")),
                fileSystem.Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindLinuxBinaries1() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var locator = new DefaultLinuxLocator(fileSystem, _assemblyDir);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "runtimes", "linux-x64", "native", "libavutil.so.56")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindLinuxBinaries2() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var locator = new DefaultLinuxLocator(fileSystem, _assemblyDir);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "libavutil.so.56")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindLinuxBinaries3()
        {
            var fileSystem = new MockFileSystem();
            //Get the mock file-system root this way so that the test runs correctly no matter what the host-runner's OS is
            var root = fileSystem.Path.GetFullPath("/usr/local/bin/ffmpeg");
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib"));

            var locator = new DefaultLinuxLocator(fileSystem, root);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(root, "runtimes", "linux-x64", "native", "libavutil.so.56")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindLinuxBinaries4() {
            var fileSystem = new MockFileSystem();
            //Get the mock file-system root this way so that the test runs correctly no matter what the host-runner's OS is
            var root = fileSystem.Path.GetFullPath("/usr/local/bin/ffmpeg");
            fileSystem.AddFile(fileSystem.Path.Combine(root, "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "libavutil.so.56"), new("libavutil.so.56"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "libavutil.56.dylib"), new("libavutil.56.dylib"));

            var locator = new DefaultLinuxLocator(fileSystem, root);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(root, "libavutil.so.56")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindMacOsBinaries1() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var locator = new DefaultMacLocator(fileSystem, _assemblyDir);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "runtimes", "osx-x64", "native", "libavutil.56.dylib")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindMacOsBinaries2() {
            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> {
                    { Path.Combine(_assemblyDir, "avutil-56.dll"), new("avutil-56.dll") },
                    { Path.Combine(_assemblyDir, "libavutil.so.56"), new("libavutil.so.56") },
                    { Path.Combine(_assemblyDir, "libavutil.56.dylib"), new("libavutil.56.dylib") }
                }
            );

            var locator = new DefaultMacLocator(fileSystem, _assemblyDir);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(_assemblyDir, "libavutil.56.dylib")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindMacOsBinaries3() {
            var fileSystem = new MockFileSystem();
            //Get the mock file-system root this way so that the test runs correctly no matter what the host-runner's OS is
            var root = fileSystem.Path.GetFullPath("/usr/local/bin/ffmpeg");
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "win7-x86", "native", "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "win7-x64", "native", "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "linux-x64", "native", "libavutil.so.56"), new("libavutil.so.56"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "runtimes", "osx-x64", "native", "libavutil.56.dylib"), new("libavutil.56.dylib"));

            var locator = new DefaultMacLocator(fileSystem, root);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(root, "runtimes", "osx-x64", "native", "libavutil.56.dylib")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        [Fact]
        public void FindMacOsBinaries4() {
            var fileSystem = new MockFileSystem();
            //Get the mock file-system root this way so that the test runs correctly no matter what the host-runner's OS is
            var root = fileSystem.Path.GetFullPath("/usr/local/bin/ffmpeg");
            fileSystem.AddFile(fileSystem.Path.Combine(root, "avutil-56.dll"), new("avutil-56.dll"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "libavutil.so.56"), new("libavutil.so.56"));
            fileSystem.AddFile(fileSystem.Path.Combine(root, "libavutil.56.dylib"), new("libavutil.56.dylib"));

            var locator = new DefaultMacLocator(fileSystem, root);

            Assert.Equal(
                Path.GetFullPath(Path.Combine(root, "libavutil.56.dylib")),
                Path.GetFullPath(locator.FindFFmpegLibrary("avutil", 56).FullName)
            );
        }


        private readonly string _assemblyDir;
    }

}
