# DotBimExport

[![JetBrains Rider](https://img.shields.io/badge/JetBrains-Rider-blue.svg)](https://www.jetbrains.com/rider)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)
[![Revit 2016-2027](https://img.shields.io/badge/Revit-2016--2027-blue.svg)](https://www.autodesk.com/products/revit/overview)

A Revit plugin for exporting 3D views to the [dotbim](https://github.com/paireks/dotbim) (`.bim`) format.
Export is implemented by the `dosymep.Revit.DotBim` library in this repository.

## Features

- Ribbon command that exports the active 3D view to a `.bim` file
- Optional export of linked models
- Optional element and project parameter info
- Support for Revit 2016–2027 (including `2026.5`)

## Install

1. Download `DotBimExport.{version}.zip` from [Releases](https://github.com/dosymep/revit-dotbim/releases) (match your Revit year).
2. Extract the archive into the Revit Addins folder for that version, for example:

   `%AppData%\Autodesk\Revit\Addins\2025\`

3. Restart Revit. The **dotbim** panel appears with **Export DotBim**.

Archive layout:

```text
DotBimExport.2025.zip
  DotBimExport.addin
  DotBimExport/
    DotBimExport.dll
    dosymep.Revit.DotBim.dll
    …
```

## Usage

1. Open a **3D view**.
2. On the **dotbim** panel, click **Export DotBim**.
3. Choose the output `.bim` path.

The plugin exports with links, element info, and document info enabled.

### Library API

`dosymep.Revit.DotBim` exposes an extension method on `Document` if you want to call export from your own code:

```csharp
View view = uiApplication.ActiveUIDocument.ActiveView;
Document document = uiApplication.ActiveUIDocument.Document;

document.Export("path/to/sample.bim",
    new DotBimOptions {
        View = view,
        WithLinks = true,
        WithElementInfo = true,
        WithDocumentInfo = true
    });
```

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `View` | `View` | required | View used for export (must be a `View3D`) |
| `WithLinks` | `bool` | `false` | Include linked models |
| `WithElementInfo` | `bool` | `false` | Include element parameters |
| `WithDocumentInfo` | `bool` | `false` | Include project information |

## Build

The repository includes a [ModularPipelines](https://github.com/thomhurst/ModularPipelines) app in `build/`. It cleans `bin`/`obj`, restores the solution, builds `DotBimExport` for each selected Revit version into `bin/<version>/DotBimExport/`, and packages add-in zip archives under `artifacts/`.

Pipeline stages:

1. **Clean** — remove root `bin/` and `artifacts/`, plus project `bin`/`obj` under `src/`
2. **Restore** — restore NuGet packages for the solution
3. **Build** — compile the plugin (and its library dependency) per Revit version
4. **Package** — zip each version as a Revit add-in layout into `artifacts/`

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) matching `global.json` (net10)
- Matching Revit API reference packs via `Autodesk.Revit.Sdk.Refs`

### Supported Revit versions

Built by default (local pipeline and GitHub Actions): **2016–2027**, including **2026.5**.

### Run the build pipeline

From the repository root:

```bash
dotnet run --project build
```

### Environment variables

| Variable | Default | Description |
| :--- | :--- | :--- |
| `CONFIGURATION` | `Release` | MSBuild configuration |
| `REVIT_VERSIONS` | `2016,…,2027` (incl. `2026.5`) | One or more Revit versions. Separate with commas, semicolons, or spaces |
| `MAX_PARALLELISM` | `max(1, ProcessorCount / 4)` | Max parallel work items for packaging |

#### PowerShell

```powershell
$env:CONFIGURATION = "Release"
$env:REVIT_VERSIONS = "2024,2025"
$env:MAX_PARALLELISM = "2"
dotnet run --project build
```

#### Bash

```bash
CONFIGURATION=Release REVIT_VERSIONS=2024,2025 MAX_PARALLELISM=2 dotnet run --project build
```

### Quick single-version build

Without the pipeline:

```bash
dotnet build revit-dotbim.slnx -c D2024
```

Available configurations: `D2016` … `D2027` (and `D2026.5`).

### Build outputs

```text
bin/
  2024/
    DotBimExport/
  2025/
    DotBimExport/
artifacts/
  DotBimExport.2024.zip
  DotBimExport.2025.zip
```

### Versioning

Base version lives in `Directory.Build.props` (`2.0.0`).

| Context | Example assembly version |
| :--- | :--- |
| Local / CI branch build | `2.0.0-{commit}.{revitVersion}` |
| GitHub tag `v2.1.0` | `2.1.0.{revitVersion}` |

On `v*` tags, GitHub Actions also creates a release and uploads `artifacts/*`.

## Sample

See [`sample/sample.bim`](sample/sample.bim).

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## Credits

See [CREDITS.md](CREDITS.md).

## License

This project is licensed under the [MIT License](LICENSE.md).
