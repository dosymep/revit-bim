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
            includeBaseDirectory: false,
            cancellationToken: cancellationToken);
    }

    public static void CopyDirectory(
        string sourceDirectory,
        string destinationDirectory,
        Func<string, bool>? excludeFile = null) {
        Directory.CreateDirectory(destinationDirectory);

        foreach(string filePath in Directory.GetFiles(sourceDirectory)) {
            if(excludeFile?.Invoke(filePath) == true) {
                continue;
            }

            File.Copy(
                filePath,
                Path.Combine(destinationDirectory, Path.GetFileName(filePath)),
                overwrite: true);
        }

        foreach(string directoryPath in Directory.GetDirectories(sourceDirectory)) {
            CopyDirectory(
                directoryPath,
                Path.Combine(destinationDirectory, Path.GetFileName(directoryPath)),
                excludeFile);
        }
    }
}
