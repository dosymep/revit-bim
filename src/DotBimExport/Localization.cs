using System.Globalization;

using Autodesk.Revit.ApplicationServices;

namespace DotBimExport;

/// <summary>
/// Applies UI culture from the Revit language. Defaults to English.
/// </summary>
internal static class Localization {
    /// <summary>
    /// Sets <see cref="CultureInfo.CurrentUICulture"/> from Revit.
    /// Russian uses <c>ru</c>; every other language uses English.
    /// </summary>
    public static void Apply(LanguageType language) {
        CultureInfo culture = language == LanguageType.Russian
            ? new CultureInfo("ru")
            : new CultureInfo("en");

        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
