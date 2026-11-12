namespace Build.Options;

public sealed class BuildOptions {
    public string Configuration { get; set; } = "Release";
    
    public string[] RevitVersions { get; set; } = [
        "2016", "2017", "2018", "2019", "2020", "2021", "2022",
        "2023", "2024", "2025", "2026", "2026.5", "2027"
    ];

    /// <summary>
    /// Max parallel work items for version/project fan-out. Default: processor count / 4.
    /// </summary>
    public int MaxDegreeOfParallelism { get; set; } = Math.Max(1, Environment.ProcessorCount / 4);

    public string SolutionFileName { get; set; } = "revit-dotbim.slnx";

    public string SolutionRoot { get; set; } = string.Empty;

    public string SolutionPath => Path.Combine(SolutionRoot, SolutionFileName);

    public string ProjectName { get; set; } = "dosymep.Revit.DotBim";

    public string SrcDirectory => Path.Combine(SolutionRoot, "src");

    public string ProjectPath => Path.Combine(SrcDirectory, ProjectName, $"{ProjectName}.csproj");

    public string BinDirectory => Path.Combine(SolutionRoot, "bin");

    public string ArtifactsDirectory => Path.Combine(SolutionRoot, "artifacts");

    public string GetOutputDirectory(string revitVersion) {
        return Path.Combine(BinDirectory, revitVersion, ProjectName);
    }

    public string GetArchivePath(string revitVersion) {
        return Path.Combine(ArtifactsDirectory, $"{ProjectName}.{revitVersion}.zip");
    }
}
