using Autodesk.Revit.DB;

using dosymep.Revit.DotBim.Internal;

namespace dosymep.Revit.DotBim;

/// <summary>
/// Provides document .bim file export extensions.
/// </summary>
public static class DotBimExtension {
    /// <summary>
    /// Exports the document to .bim file format.
    /// </summary>
    /// <param name="document">Exporting document.</param>
    /// <param name="dotBimFilePath">Output file path.</param>
    /// <param name="options">Bim export options.</param>
    /// <exception cref="System.ArgumentException">When <paramref name="dotBimFilePath"/> is null or empty or if the path does not end with ".bim".</exception>
    /// <exception cref="System.ArgumentNullException">When <paramref name="document"/> or <paramref name="options"/> is null.</exception>
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
        customExport.Export(options.View);
    }
}