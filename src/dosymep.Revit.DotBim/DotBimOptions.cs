using Autodesk.Revit.DB;

namespace dosymep.Revit.DotBim;

/// <summary>
/// Bim export options.
/// </summary>
public sealed class DotBimOptions {
    /// <summary>
    /// Exporting View.
    /// </summary>
    public required View View { get; init; }

    /// <summary>
    /// If true export with links. Default value is false.
    /// </summary>
    public bool WithLinks { get; init; }

    /// <summary>
    /// If true export with element params info. Default value is false.
    /// </summary>
    public bool WithElementInfo { get; init; }

    /// <summary>
    /// If true export with document params info. Default value is false.
    /// </summary>
    public bool WithDocumentInfo { get; init; }
}