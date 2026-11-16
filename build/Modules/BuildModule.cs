using Build.Helpers;
using Build.Options;
using Build.Versioning;

using Microsoft.Extensions.Options;

using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Git;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

[DependsOn<RestoreModule>]
public sealed class BuildModule : Module {
    private readonly BuildOptions _options;
    private readonly BuildVersionFactory _versions;

    public BuildModule(IOptions<BuildOptions> options, BuildVersionFactory versions) {
        _options = options.Value;
        _versions = versions;
    }

    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken) {
        var git = context.Git().Information;

        foreach(string revitVersion in _options.RevitVersions) {
            await BuildProjectAsync(context, git, revitVersion, cancellationToken);
        }
    }

    private async Task BuildProjectAsync(
        IModuleContext context,
        IGitInformation git,
        string revitVersion,
        CancellationToken cancellationToken) {
        string outputDirectory = _options.GetOutputDirectory(revitVersion);
        if(Directory.Exists(outputDirectory)) {
            Directory.Delete(outputDirectory, recursive: true);
        }

        await context.DotNet()
            .Build(
                new DotNetBuildOptions {
                    ProjectSolution = _options.ProjectPath,
                    Configuration = _options.Configuration,
                    NoRestore = false,
                    NoIncremental = true,
                    Arguments = ["--force"],
                    Properties = [
                        ("RevitVersion", revitVersion),
                        ("OutputPath", FileSystemHelpers.EnsureTrailingSlash(outputDirectory)),
                        .._versions.PropertiesFor(git, revitVersion),
                    ]
                },
                cancellationToken: cancellationToken);
    }
}
