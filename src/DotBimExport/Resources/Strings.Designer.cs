using System.Globalization;
using System.Resources;

namespace DotBimExport.Resources;

/// <summary>
/// Strongly typed access to localized UI strings.
/// </summary>
public static class Strings {
    private static readonly ResourceManager ResourceManager =
        new("DotBimExport.Resources.Strings", typeof(Strings).Assembly);

    public static string MainWindowTitle => ResourceManager.GetString(nameof(MainWindowTitle), CultureInfo.CurrentUICulture)!;

    public static string View => ResourceManager.GetString(nameof(View), CultureInfo.CurrentUICulture)!;

    public static string NameLanguage => ResourceManager.GetString(nameof(NameLanguage), CultureInfo.CurrentUICulture)!;

    public static string ExportElementInfo => ResourceManager.GetString(nameof(ExportElementInfo), CultureInfo.CurrentUICulture)!;

    public static string ExportDocumentInfo => ResourceManager.GetString(nameof(ExportDocumentInfo), CultureInfo.CurrentUICulture)!;

    public static string ExportLinks => ResourceManager.GetString(nameof(ExportLinks), CultureInfo.CurrentUICulture)!;

    public static string Categories => ResourceManager.GetString(nameof(Categories), CultureInfo.CurrentUICulture)!;

    public static string SelectAll => ResourceManager.GetString(nameof(SelectAll), CultureInfo.CurrentUICulture)!;

    public static string ClearSelection => ResourceManager.GetString(nameof(ClearSelection), CultureInfo.CurrentUICulture)!;

    public static string Export => ResourceManager.GetString(nameof(Export), CultureInfo.CurrentUICulture)!;

    public static string Cancel => ResourceManager.GetString(nameof(Cancel), CultureInfo.CurrentUICulture)!;

    public static string No3DViews => ResourceManager.GetString(nameof(No3DViews), CultureInfo.CurrentUICulture)!;

    public static string SelectView => ResourceManager.GetString(nameof(SelectView), CultureInfo.CurrentUICulture)!;

    public static string SelectNameLanguage => ResourceManager.GetString(nameof(SelectNameLanguage), CultureInfo.CurrentUICulture)!;

    public static string SelectCategory => ResourceManager.GetString(nameof(SelectCategory), CultureInfo.CurrentUICulture)!;

    public static string RibbonButtonText => ResourceManager.GetString(nameof(RibbonButtonText), CultureInfo.CurrentUICulture)!;

    public static string RibbonToolTip => ResourceManager.GetString(nameof(RibbonToolTip), CultureInfo.CurrentUICulture)!;

    public static string SaveFileFilter => ResourceManager.GetString(nameof(SaveFileFilter), CultureInfo.CurrentUICulture)!;
}
