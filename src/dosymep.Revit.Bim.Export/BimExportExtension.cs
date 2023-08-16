using System;

using Autodesk.Revit.DB;

namespace dosymep.Revit.Bim.Export;

/// <summary>
/// Provides document .bim file export extensions.
/// </summary>
public static class BimExportExtension {
    /// <summary>
    /// Exports the document to .bim file format.
    /// </summary>
    /// <param name="document">Exporting document.</param>
    /// <param name="bimFilePath">Output file path.</param>
    /// <param name="options">Bim export options.</param>
    /// <exception cref="System.ArgumentException">When <paramref name="bimFilePath"/> is null or empty or if the path does not end with ".bim".</exception>
    /// <exception cref="System.ArgumentNullException">When <paramref name="document"/> or <paramref name="options"/> is null.</exception>
    public static void Export(this Document document, string bimFilePath, BimExportOptions options) {
        if(document == null) {
            throw new ArgumentNullException(nameof(document));
        }

        if(options == null) {
            throw new ArgumentNullException(nameof(options));
        }

        if(string.IsNullOrEmpty(bimFilePath)) {
            throw new ArgumentException("Value cannot be null or empty.", nameof(bimFilePath));
        }

        if(!bimFilePath.EndsWith(".bim", StringComparison.OrdinalIgnoreCase)) {
            throw new ArgumentException("Path should end up with .bim.", nameof(bimFilePath));
        }

        var customExport = new CustomExporter(
            document,
            new BimExport(document, bimFilePath, options));

        // To skip geometry objects (Curve, Face, etc)
        customExport.IncludeGeometricObjects = false;
        customExport.Export(options.View);
    }
}