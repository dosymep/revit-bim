# Contributing

Thanks for your interest in contributing to **dosymep.Revit.DotBim**.

## Before you start

1. Search [existing issues](https://github.com/dosymep/revit-dotbim/issues) and pull requests to avoid duplicates.
2. For larger changes, open an issue first and describe the problem or proposal.
3. Keep changes focused. Prefer small, reviewable pull requests over large mixed diffs.

## Development setup

### Prerequisites

- Windows x64
- [.NET SDK](https://dotnet.microsoft.com/download) matching `global.json`
- Revit API reference packs used by `Autodesk.Revit.Sdk.Refs`
- Autodesk Revit installed for every version you want to run or debug locally

### Clone and build

```bash
git clone https://github.com/dosymep/revit-dotbim.git
cd revit-dotbim
dotnet run --project build
```

By default the ModularPipelines app builds Revit **2016–2027** (including `2026.5`) in **Release**. Override with environment variables:

| Variable | Example |
| :--- | :--- |
| `CONFIGURATION` | `Release` |
| `REVIT_VERSIONS` | `2024,2025` |
| `MAX_PARALLELISM` | `2` |

PowerShell example:

```powershell
$env:REVIT_VERSIONS = "2025"
$env:CONFIGURATION = "Release"
dotnet run --project build
```

For a quick single-configuration build without the pipeline:

```powershell
dotnet build revit-dotbim.slnx -c D2025
```

| Configuration | Revit |
| :--- | :--- |
| `D2016` … `D2027` | Revit 2016–2027 |
| `D2026.5` | Revit 2026.5 |

More details: [README.md](README.md).

## Project layout

| Path | Purpose |
| :--- | :--- |
| `src/dosymep.Revit.DotBim/` | Public API: `DotBimExtension`, `DotBimOptions` |
| `src/dosymep.Revit.DotBim/Internal/` | Export context and Revit helpers (not public API) |
| `build/` | ModularPipelines clean / restore / build / package |
| `sample/` | Sample `.bim` output |
| `docs/` | Screenshots and documentation assets |
| `Directory.Build.props` | Shared version and package metadata |
| `Directory.Packages.props` | Central package versions |

## Coding guidelines

- Match the existing C# style, naming, and file organization (see `.editorconfig`).
- Prefer small, purposeful types over large catch-all classes.
- Keep public API changes documented with English XML comments.
- Do not commit secrets, machine-specific absolute paths, or local export samples with personal paths.
- Update `README.md` when user-facing behavior changes (export options, supported Revit versions, usage examples, build defaults).
- Keep types under `Internal/` internal unless there is a clear public API need.
- Prefer editing package versions in `Directory.Packages.props` rather than pinning them ad hoc in project files.

## Versioning

- Base version: `Directory.Build.props`
- Branch / local CI builds: `{base}-{commit}.{revitVersion}`
- Tag builds (`v*`): `{tagWithoutV}.{revitVersion}` (from `GITHUB_REF` / `GITHUB_REF_NAME`)

Bump the base version in `Directory.Build.props` when preparing a release, then push a matching `v*` tag.

## Testing

There is no automated test suite yet. When changing export behavior:

1. Build for the Revit version you use, for example `REVIT_VERSIONS=2025` or `-c D2025`.
2. Export a small 3D view with and without `WithLinks`, `WithElementInfo`, and `WithDocumentInfo`.
3. Open the resulting `.bim` file in a viewer such as [Online 3D Viewer](https://3dviewer.net/) and check geometry, colors, and info dictionaries.
4. Note the Revit version and options you used in the pull request.

## Pull requests

1. Branch from `main`.
2. Make your changes and ensure the relevant configuration builds locally (`dotnet run --project build` or a targeted `-c D20xx` build).
3. Write a clear PR title and describe:
   - what changed
   - why it changed
   - how you tested it
4. Link related issues.
5. Keep the PR limited to one concern (feature, fix, docs, or refactor).

CI builds all supported Revit versions on pull requests and `main`. Tag pushes (`v*`) also publish GitHub Release assets from `artifacts/`. See `.github/workflows/build.yml`.

## Reporting issues

Include as much of the following as possible:

- Revit version
- `dosymep.Revit.DotBim` version / commit / tag
- `DotBimOptions` used (`View`, `WithLinks`, `WithElementInfo`, `WithDocumentInfo`)
- Expected vs actual behavior
- Minimal steps to reproduce
- Sample model details if relevant (no confidential project data)

## License

By contributing, you agree that your contributions are licensed under the same [MIT License](LICENSE.md) as the project.

See also [CREDITS.md](CREDITS.md).
