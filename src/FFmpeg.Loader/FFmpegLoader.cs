using FFmpeg.Loader.Locators;


namespace FFmpeg.Loader;

/// <summary>
/// FFmpegLoader is your entry-point to start configuring search-locations for FFmpeg shared libraries, using a Fluent Builder API.
/// </summary>
public static class FFmpegLoader
{
    /// <summary>
    /// Sets the initial list of search-locations: a predefined set of defaults based on the current operating system and relative to the specified <paramref name="rootDir"/>
    /// directory (the directory containing the FFmpegLoader assembly if <paramref name="rootDir"/> is <see langword="null" />).
    /// </summary>
    /// <param name="rootDir">The root-directory to use when resolving default relative paths. E.g. typically the application's root directory for binaries.
    /// If <see langword="null" /> then the directory which contains th FFmpegLoader assembly is used.</param>
    /// <returns>A new instance of <see cref="FFmpegLoaderSearch"/> with the initial search-locations.
    /// Call <see cref="FFmpegLoaderSearch.ThenSearchApplication">ThenSearchApplication</see>, <see cref="FFmpegLoaderSearch.ThenSearchSystem">ThenSearchSystem</see>,
    /// <see cref="FFmpegLoaderSearch.ThenSearchPaths">ThenSearchPaths</see> or <see cref="FFmpegLoaderSearch.ThenSearchEnvironmentPaths">ThenSearchEnvironmentPaths</see>
    /// on that to add additional search locations.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if using an unsupported operating system, i.e. anything other than Windows, Linux or Mac OSX.</exception>
    public static FFmpegLoaderSearch SearchApplication(string? rootDir = null)
        => new(new[] {
            LocatorFactory.CreateAppDefaultForCurrentOS(rootDir)
        });


    /// <summary>
    /// Sets the initial list of search-locations: a predefined set of defaults based on the current operating system.
    /// </summary>
    /// <returns>A new instance of <see cref="FFmpegLoaderSearch"/> with the initial search-locations.
    /// Call <see cref="FFmpegLoaderSearch.ThenSearchApplication">ThenSearchApplication</see>, <see cref="FFmpegLoaderSearch.ThenSearchSystem">ThenSearchSystem</see>,
    /// <see cref="FFmpegLoaderSearch.ThenSearchPaths">ThenSearchPaths</see> or <see cref="FFmpegLoaderSearch.ThenSearchEnvironmentPaths">ThenSearchEnvironmentPaths</see>
    /// on that to add additional search locations.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if using an unsupported operating system, i.e. anything other than Windows, Linux or Mac OSX.</exception>
    public static FFmpegLoaderSearch SearchSystem()
        => new(new[] {
            LocatorFactory.CreateSystemDefaultForCurrentOS()
        });


    /// <summary>
    /// Sets the initial list of search-locations: a custom set of paths for the current operating system. Search paths are expected to be either absolute or relative
    /// to the directory containing the FFmpegLoader assembly.
    /// </summary>
    /// <param name="searchPaths">Additional search-locations. Search paths are expected to be absolute or relative to the directory containing the FFmpegLoader assembly.</param>
    /// <returns>A new instance of <see cref="FFmpegLoaderSearch"/> with the initial search-locations.
    /// Call <see cref="FFmpegLoaderSearch.ThenSearchApplication">ThenSearchApplication</see>, <see cref="FFmpegLoaderSearch.ThenSearchSystem">ThenSearchSystem</see>,
    /// <see cref="FFmpegLoaderSearch.ThenSearchPaths">ThenSearchPaths</see> or <see cref="FFmpegLoaderSearch.ThenSearchEnvironmentPaths">ThenSearchEnvironmentPaths</see>
    /// on that to add additional search locations.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if using an unsupported operating system, i.e. anything other than Windows, Linux or Mac OSX.</exception>
    public static FFmpegLoaderSearch SearchPaths(params string?[] searchPaths)
        => new(new[] {
            LocatorFactory.CreateCustomForCurrentOS(null, searchPaths)
        });


    /// <summary>
    /// Sets the initial list of search-locations: paths from an environment variable (by default the PATH environment variable).
    /// </summary>
    /// <param name="envVar">Name of the environment variable to pull search paths from. The default is the standard PATH variable.</param>
    /// <returns>A new instance of <see cref="FFmpegLoaderSearch"/> with the initial search-locations.
    /// Call <see cref="FFmpegLoaderSearch.ThenSearchApplication">ThenSearchApplication</see>, <see cref="FFmpegLoaderSearch.ThenSearchSystem">ThenSearchSystem</see>,
    /// <see cref="FFmpegLoaderSearch.ThenSearchPaths">ThenSearchPaths</see> or <see cref="FFmpegLoaderSearch.ThenSearchEnvironmentPaths">ThenSearchEnvironmentPaths</see>
    /// on that to add additional search locations.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if using an unsupported operating system, i.e. anything other than Windows, Linux or Mac OSX.</exception>
    public static FFmpegLoaderSearch SearchEnvironmentPaths(string envVar = "PATH")
        => new(new[] {
            LocatorFactory.CreateCustomForCurrentOS(null, new[] { Environment.GetEnvironmentVariable(envVar) })
        });
}
