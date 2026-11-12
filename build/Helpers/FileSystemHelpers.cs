using System.IO.Compression;

namespace Build.Helpers;

internal static class FileSystemHelpers {
    public static string EnsureTrailingSlash(string path) {
        return path.EndsWith(Path.DirectorySeparatorChar)
               || path.EndsWith(Path.AltDirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;
    }

    public static async Task CreateZipAsync(
        string sourceDirectory,
        string zipPath,
        CancellationToken cancellationToken) {
        if(File.Exists(zipPath)) {
            File.Delete(zipPath);
        }

        await ZipFile.CreateFromDirectoryAsync(
            sourceDirectory,
            zipPath,
            CompressionLevel.Optimal,
            includeBaseDirectory: true,
            cancellationToken: cancellationToken);
    }
}
