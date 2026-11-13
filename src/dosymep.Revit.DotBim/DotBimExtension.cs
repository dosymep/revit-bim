using Autodesk.Revit.DB;

using dosymep.Revit.DotBim.Internal;

namespace dosymep.Revit.DotBim;

/// <summary>
/// Provides extension methods for exporting a Revit <see cref="Document"/> to the dotbim (<c>.bim</c>) file format.
/// </summary>
public static class DotBimExtension {
    /// <summary>
    /// Exports the specified document to a <c>.bim</c> file using Revit's <see cref="CustomExporter"/>.
    /// </summary>
    /// <param name="document">The Revit document to export.</param>
    /// <param name="dotBimFilePath">The output file path. Must end with <c>.bim</c> (case-insensitive).</param>
    /// <param name="options">Export options, including the <see cref="View3D"/> used for export.</param>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when <paramref name="document"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <paramref name="dotBimFilePath"/> is <see langword="null"/> or empty,
    /// or when the path does not end with <c>.bim</c>.
    /// </exception>
    /// <remarks>
    /// Geometric objects such as curves and faces are skipped.
    /// Coordinates are written in meters. The written schema version is 1.2.0.
    /// </remarks>
    public static void Export(this Document document, string dotBimFilePath, DotBimOptions options) {
        if(document == null) {
            throw new ArgumentNullException(nameof(document));
        }

        if(options == null) {
            throw new ArgumentNullException(nameof(options));
        }

        if(string.IsNullOrEmpty(dotBimFilePath)) {
            throw new ArgumentException("Value cannot be null or empty.", nameof(dotBimFilePath));
        }

        if(!dotBimFilePath.EndsWith(".bim", StringComparison.OrdinalIgnoreCase)) {
            throw new ArgumentException("Path should end up with .bim.", nameof(dotBimFilePath));
        }

        CustomExporter customExport = new(
            document,
            new DotBimExportContext(document, dotBimFilePath, options));

        // To skip geometry objects (Curve, Face, etc.)
        customExport.IncludeGeometricObjects = false;
        
#if REVIT2020_OR_GREATER
        customExport.Export(options.View);
#else
        customExport.Export((View3D) options.View);
#endif
    }
}