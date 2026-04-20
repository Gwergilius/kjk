# Localization Guide

## 📁 Localization System

### Built-in C# Translation System
- **Implementation**: `LocalizationManager.cs`
- **Method**: Dictionary-based translations in C# code
- **Reliability**: No external file dependencies

## 🔧 Adding New Translations

### 1. Add to LocalizationManager.cs
```csharp
["NEW_TEXT_KEY"] = new Dictionary<string, string>
{
    ["en"] = "English text here",
    ["hu"] = "Magyar szöveg itt"
},
["FORMATTED_TEXT"] = new Dictionary<string, string>
{
    ["en"] = "Hello {0}!",
    ["hu"] = "Szia {0}!"
}
```

### 2. Use in C# Code
```csharp
// Simple text - recommended way
var text = _localizationManager.GetText("NEW_TEXT_KEY");

// Formatted text with arguments
var formatted = _localizationManager.GetText("FORMATTED_TEXT", playerName);

// Alternative - using singleton
var text = LocalizationManager.Instance.GetText("NEW_TEXT_KEY");
```

## 🌍 Language Management

### Available Languages
- **en**: English
- **hu**: Hungarian (Magyar)

### Adding New Languages
1. Add language code to `LocalizationManager.AvailableLanguages` array
2. Add display name to `LocalizationManager.LanguageNames` array  
3. Add translations to all dictionary entries in `_translations`

### Language Switching
```csharp
// Get current language
var currentLang = LocalizationManager.Instance.GetCurrentLanguage();

// Switch to specific language  
LocalizationManager.Instance.SetLanguage("hu");
```

## 🎯 Best Practices

1. **Use descriptive keys**: `SECTION_1_TEXT` instead of `TEXT_1`
2. **Group related keys**: All section 1 content starts with `SECTION_1_`
3. **Handle plurals separately**: Create separate keys for singular/plural
4. **Keep formatting consistent**: Use {0}, {1} for placeholders
5. **Test both languages**: Always verify both translations work correctly
6. **Add fallbacks**: English is used as fallback if translation missing

## 🔄 Dynamic Updates

The game automatically refreshes all text when language is changed:
- Section content
- Button text  
- UI labels
- Error messages

All nodes in the "localized_nodes" group receive `_on_language_changed()` callback.

## ✅ Advantages of Built-in System

- **No file dependencies**: All translations embedded in code
- **Reliable loading**: No risk of missing translation files
- **Type safety**: Compile-time checking of translation keys  
- **Performance**: Fast dictionary lookups
- **Fallback support**: Automatic fallback to English
- **Error handling**: Missing keys clearly marked