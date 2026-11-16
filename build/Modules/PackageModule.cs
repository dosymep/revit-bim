using Build.Helpers;
using Build.Options;

using Microsoft.Extensions.Options;

using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Modules;

namespace Build.Modules;

[DependsOn<BuildModule>]
public sealed class PackageModule : Module {
    private readonly BuildOptions _options;

    public PackageModule(IOptions<BuildOptions> options) {
        _options = options.Value;
    }

    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken) {
        Directory.CreateDirectory(_options.ArtifactsDirectory);

        string stagingRoot = Path.Combine(_options.ArtifactsDirectory, ".staging");
        try {
            foreach(string revitVersion in _options.RevitVersions) {
                await PackageAsync(stagingRoot, revitVersion, cancellationToken);
            }
        } finally {
            if(Directory.Exists(stagingRoot)) {
                Directory.Delete(stagingRoot, recursive: true);
            }
        }
    }

    private async Task PackageAsync(string stagingRoot, string revitVersion, CancellationToken cancellationToken) {
        string outputDirectory = _options.GetOutputDirectory(revitVersion);
        string addinFileName = $"{_options.ProjectName}.addin";
        string addinPath = Path.Combine(outputDirectory, addinFileName);

        if(!File.Exists(addinPath)) {
            throw new FileNotFoundException($"Add-in manifest was not found: {addinPath}");
        }

        string packageDirectory = Path.Combine(stagingRoot, revitVersion);
        if(Directory.Exists(packageDirectory)) {
            Directory.Delete(packageDirectory, recursive: true);
        }

        Directory.CreateDirectory(packageDirectory);
        File.Copy(addinPath, Path.Combine(packageDirectory, addinFileName));

        FileSystemHelpers.CopyDirectory(
            outputDirectory,
            Path.Combine(packageDirectory, _options.ProjectName),
            filePath => string.Equals(Path.GetFileName(filePath), addinFileName, StringComparison.OrdinalIgnoreCase));

        await FileSystemHelpers.CreateZipAsync(
            packageDirectory, _options.GetArchivePath(revitVersion), cancellationToken);
    }
}
