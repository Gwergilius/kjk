using Godot;
using System.Collections.Generic;

namespace TalismanOfDeath.Game;

/// <summary>
/// Manages game localization and language switching with built-in translations
/// </summary>
public partial class LocalizationManager : Node
{
    public static LocalizationManager Instance { get; private set; } = null!;
    
    // Available languages
    public static readonly string[] AvailableLanguages = { "en", "hu" };
    public static readonly string[] LanguageNames = { "English", "Magyar" };
    
    private string _currentLanguage = "en";
    
    // Translation dictionary - all translations in code for reliability
    private readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        ["GAME_TITLE"] = new Dictionary<string, string>
        {
            ["en"] = "TALISMAN OF DEATH",
            ["hu"] = "A HALÁLTALIZMÁN"
        },
        ["SECTION_1_TEXT"] = new Dictionary<string, string>
        {
            ["en"] = "When you regain consciousness you look around: you are standing in a huge vaulted hall deep underground. The air is cool and you wonder what terrible fate awaits you.\n\nYou are completely alone in this world without a single friend and you have no idea what devilish dangers may lurk here so far from your home.\n\nAt the end of the hall you see two arched exits. Light flashes in the distance and a hellish roar echoes throughout the hall.",
            ["hu"] = "Mikor magadhoz térsz körülnézel: hatalmas bolthajtásos csarnokban állsz mélyen a föld alatt. A levegő hűvös és azon töprengsz vajon milyen borzalmas vég vár rád.\n\nTeljesen egyedül vagy ezen a világon egyetlen barát nélkül és fogalmad sincs milyen ördögi veszélyek leselkedhetnek rád itt oly távol az otthonodtól.\n\nA csarnok végében két boltíves kijáratot látsz. Fény villan a távolban és a csarnokon pokoli üvöltés visszhangzik végig."
        },
        ["SECTION_1_CHOICE_1"] = new Dictionary<string, string>
        {
            ["en"] = "Rush out through the corridor closest to you",
            ["hu"] = "Kirohansz a hozzád közelebb lévő folyosón"
        },
        ["SECTION_1_CHOICE_2"] = new Dictionary<string, string>
        {
            ["en"] = "Stay where you are and draw your sword",
            ["hu"] = "Ott maradsz és kardot rántasz"
        },
        ["SECTION_17_TEXT"] = new Dictionary<string, string>
        {
            ["en"] = "As you rush toward the ominous darkness of the arched corridor you hear a click followed by a terrible rumble. The ground shakes beneath your feet.",
            ["hu"] = "Amint a boltíves folyosó vészjósló sötétje felé rohansz egy kattanást hallasz amit félelmetes robaj követ. A lábad alatt megremeg a föld."
        },
        ["SECTION_17_CHOICE_1"] = new Dictionary<string, string>
        {
            ["en"] = "Throw yourself into the darkness through the arch",
            ["hu"] = "Ha a boltíven át beleveted magad a sötétbe"
        },
        ["SECTION_17_CHOICE_2"] = new Dictionary<string, string>
        {
            ["en"] = "Stop and hesitate",
            ["hu"] = "Ha megtorpansz"
        },
        ["SECTION_30_TEXT"] = new Dictionary<string, string>
        {
            ["en"] = "The footsteps get closer and closer. Suddenly a thunderous sound echoes throughout the cave. Looking around you see that both arched exits are blocked by huge stone slabs. You are trapped.",
            ["hu"] = "A lépések egyre közelednek. Hirtelen mennydörgéshez hasonló hang visszhangzik végig a barlangon. Körülnézve látod hogy mindkét boltíves kijáratot hatalmas kőtáblák zárják le. Csapdába kerültél."
        },
        ["SECTION_30_CHOICE_1"] = new Dictionary<string, string>
        {
            ["en"] = "Continue",
            ["hu"] = "Folytatás"
        },
        ["SECTION_NOT_IMPLEMENTED"] = new Dictionary<string, string>
        {
            ["en"] = "This section is not yet implemented!",
            ["hu"] = "Ez a bekezdés még nem implementálva!"
        },
        ["SECTION_NOT_IMPLEMENTED_DESC"] = new Dictionary<string, string>
        {
            ["en"] = "This would be section {0}.",
            ["hu"] = "Ez itt a {0}. bekezdés helye lenne."
        },
        ["BACK_TO_BEGINNING"] = new Dictionary<string, string>
        {
            ["en"] = "Back to beginning",
            ["hu"] = "Vissza az elejére"
        },
        ["ERROR_SECTION_NOT_FOUND"] = new Dictionary<string, string>
        {
            ["en"] = "ERROR: Section {0} not found!",
            ["hu"] = "HIBA: {0}. bekezdés nem található!"
        },
        ["LANGUAGE"] = new Dictionary<string, string>
        {
            ["en"] = "Language",
            ["hu"] = "Nyelv"
        },
        ["LOG_ADVENTURE_BEGINS"] = new Dictionary<string, string>
        {
            ["en"] = "Talisman of Death - Adventure begins!",
            ["hu"] = "A Haláltalizmán - Kaland kezdődik!"
        },
        ["LOG_SELECTED_OPTION"] = new Dictionary<string, string>
        {
            ["en"] = "Selected option {0} -> Jump to section {1}",
            ["hu"] = "Választottad a {0}. opciót -> Ugrás a {1}. bekezdésre"
        }
    };
    
    public override void _Ready()
    {
        Instance = this;
        
        // Set default language based on system locale, fallback to English
        var systemLocale = OS.GetLocaleLanguage();
        var defaultLanguage = systemLocale == "hu" ? "hu" : "en";
        
        SetLanguage(defaultLanguage);
    }
    
    /// <summary>
    /// Changes the current language
    /// </summary>
    /// <param name="languageCode">Language code (en, hu)</param>
    public void SetLanguage(string languageCode)
    {
        if (System.Array.IndexOf(AvailableLanguages, languageCode) >= 0)
        {
            _currentLanguage = languageCode;
            GD.Print($"Language changed to: {languageCode}");
            
            // Emit signal to notify other nodes about language change
            GetTree().CallGroup("localized_nodes", "_on_language_changed");
        }
        else
        {
            GD.PrintErr($"Unsupported language: {languageCode}");
        }
    }
    
    /// <summary>
    /// Gets the current language code
    /// </summary>
    public string GetCurrentLanguage()
    {
        return _currentLanguage;
    }
    
    /// <summary>
    /// Gets localized text by key
    /// </summary>
    /// <param name="key">Translation key</param>
    /// <param name="args">Optional formatting arguments</param>
    public string GetText(string key, params object[] args)
    {
        if (_translations.TryGetValue(key, out var languageDict))
        {
            if (languageDict.TryGetValue(_currentLanguage, out var text))
            {
                return args.Length > 0 ? string.Format(text, args) : text;
            }
            // Fallback to English if current language not found
            if (languageDict.TryGetValue("en", out var fallbackText))
            {
                return args.Length > 0 ? string.Format(fallbackText, args) : fallbackText;
            }
        }
        
        GD.PrintErr($"Translation not found for key: {key}");
        return $"[MISSING: {key}]";
    }
    
    /// <summary>
    /// Gets the display name for a language code
    /// </summary>
    /// <param name="languageCode">Language code</param>
    public string GetLanguageName(string languageCode)
    {
        var index = System.Array.IndexOf(AvailableLanguages, languageCode);
        return index >= 0 ? LanguageNames[index] : languageCode;
    }
}