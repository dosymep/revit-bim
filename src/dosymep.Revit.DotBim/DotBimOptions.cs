using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace dosymep.Revit.DotBim;

/// <summary>
/// Options that control exporting a Revit model to the dotbim (<c>.bim</c>) file format.
/// </summary>
public sealed class DotBimOptions {
    /// <summary>
    /// Gets the view used for export.
    /// </summary>
    /// <remarks>
    /// Must be a <see cref="View3D"/> at runtime.
    /// </remarks>
    public required View View { get; init; }

    /// <summary>
    /// Gets a value indicating whether linked models are included in the export.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to export linked models; otherwise, <see langword="false"/>.
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool WithLinks { get; init; }

    /// <summary>
    /// Gets a value indicating whether element parameter information is included in the export.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to write element parameters to each element's info dictionary;
    /// otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    public bool WithElementInfo { get; init; }

    /// <summary>
    /// Gets a value indicating whether document (project) information is included in the export.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to write project information to the file info dictionary;
    /// otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    public bool WithDocumentInfo { get; init; }

    /// <summary>
    /// Gets the category ids included in the export.
    /// </summary>
    /// <value>
    /// Category ids to export. <see langword="null"/> exports every category.
    /// </value>
    public IReadOnlyCollection<ElementId>? CategoryIds { get; init; }

    /// <summary>
    /// Gets the Revit language used for exported names.
    /// </summary>
    /// <value>
    /// A <see cref="LanguageType"/> taken from Revit.
    /// The default value is <see cref="LanguageType.English_USA"/>.
    /// </value>
    public LanguageType NameLanguage { get; init; }
}