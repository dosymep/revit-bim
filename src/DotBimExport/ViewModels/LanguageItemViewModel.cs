using System.Globalization;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace DotBimExport.ViewModels;

public sealed class LanguageItemViewModel : ObservableObject {
    public LanguageItemViewModel(LanguageType language) {
        Name = GetLanguageName(language);
        Language = language;
    }

    public string Name { get; }
    public LanguageType Language { get; }


    private static CultureInfo GetCultureInfo(LanguageType languageType) {
        string cultureCode = languageType switch {
            LanguageType.English_USA => "en-US",
#if REVIT2018_OR_GREATER
            LanguageType.English_GB => "en-GB",
#endif
            LanguageType.German => "de-DE",
            LanguageType.Spanish => "es-ES",
            LanguageType.French => "fr-FR",
            LanguageType.Italian => "it-IT",
            LanguageType.Dutch => "nl-NL",
            LanguageType.Chinese_Simplified => "zh-CN",
            LanguageType.Chinese_Traditional => "zh-TW",
            LanguageType.Japanese => "ja-JP",
            LanguageType.Korean => "ko-KR",
            LanguageType.Russian => "ru-RU",
            LanguageType.Czech => "cs-CZ",
            LanguageType.Polish => "pl-PL",
            LanguageType.Hungarian => "hu-HU",
            LanguageType.Brazilian_Portuguese => "pt-BR",
            _ => "en-US" // Fallback for LanguageType.Unknown
        };

        return new CultureInfo(cultureCode);
    }

    private static string GetLanguageName(LanguageType languageType, bool native = false) {
        CultureInfo culture = GetCultureInfo(languageType);

        // Returns e.g., "Deutsch" if native is true, or "German" if false
        return native ? culture.NativeName : culture.EnglishName;
    }
}