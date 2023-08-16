using Autodesk.Revit.DB;

namespace dosymep.Revit.Bim.Export;

/// <summary>
/// Bim export options.
/// </summary>
public sealed class BimExportOptions {
    /// <summary>
    /// Exporting View.
    /// </summary>
    public View View { get; set; }
    
    /// <summary>
    /// If true export with links. Default value is false.
    /// </summary>
    public bool WithLinks { get; set; }
    
    /// <summary>
    /// If true export with element params info. Default value is false.
    /// </summary>
    public bool WithElementInfo { get; set; }
    
    /// <summary>
    /// If true export with document params info. Default value is false.
    /// </summary>
    public bool WithDocumentInfo { get; set; }
}