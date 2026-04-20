using Godot;
using System;
using System.Collections.Generic;
using TalismanOfDeath.Data;
using TalismanOfDeath.Game.Panels;

namespace TalismanOfDeath.Game;

/// <summary>
/// Talisman of Death - Modularized panel architecture
/// Main coordinator for all game panels
/// </summary>
public partial class Main : Control
{
    // Panel references (modularized scene instances)
    private ImagePanel? _imagePanel;
    private StoryPanel? _storyPanel;
    private StatusPanel? _statusPanel;
    private InventoryPanel? _inventoryPanel;
    private ChoicesPanel? _choicesPanel;
    
    // UI elements in Main scene
    private Label? _sectionLabel;
    private Button? _languageButton;

    private int _currentSection = 1;
    private LocalizationManager? _localizationManager;

    // Section storage with localization keys
    private readonly Dictionary<int, SectionData> _sections = new()
    {
        {1, new SectionData
        {
            TextKey = "SECTION_1_TEXT",
            Choices = new List<Choice>
            {
                new() { TextKey = "SECTION_1_CHOICE_1", Target = 17 },
                new() { TextKey = "SECTION_1_CHOICE_2", Target = 30 }
            }
        }},
        {17, new SectionData
        {
            TextKey = "SECTION_17_TEXT",
            Choices = new List<Choice>
            {
                new() { TextKey = "SECTION_17_CHOICE_1", Target = 42 },
                new() { TextKey = "SECTION_17_CHOICE_2", Target = 1 }
            }
        }},
        {30, new SectionData
        {
            TextKey = "SECTION_30_TEXT",
            Choices = new List<Choice>
            {
                new() { TextKey = "SECTION_30_CHOICE_1", Target = 1 },
                new() { TextKey = "SECTION_30_CHOICE_2", Target = 17 }
            }
        }},
        {42, new SectionData
        {
            TextKey = "SECTION_42_TEXT",
            Choices = new List<Choice>
            {
                new() { TextKey = "SECTION_42_CHOICE_1", Target = 1 }
            }
        }}
    };

    public override void _Ready()
    {
        // Create and add LocalizationManager
        _localizationManager = new LocalizationManager();
        AddChild(_localizationManager);
        
        // Get panel references (modularized scene instances)
        _imagePanel = GetNode<ImagePanel>("%ImagePanel");
        _storyPanel = GetNode<StoryPanel>("%StoryPanel");
        _statusPanel = GetNode<StatusPanel>("%StatusPanel");
        _inventoryPanel = GetNode<InventoryPanel>("%InventoryPanel");
        _choicesPanel = GetNode<ChoicesPanel>("%ChoicesPanel");
        
        // Get UI element references
        _sectionLabel = GetNode<Label>("%SectionLabel");
        _languageButton = GetNode<Button>("%LanguageButton");

        // Connect panel signals
        _imagePanel!.ImageClicked += OnImageClicked;
        _choicesPanel!.ChoiceSelected += OnChoiceSelected;
        
        // Connect main UI signals
        _languageButton!.Pressed += OnLanguageButtonPressed;

        // Add to localized nodes group for language change notifications
        AddToGroup("localized_nodes");
        
        // Setup initial UI state
        UpdateLanguageButton();
        SetupPlaceholderPanels();
        
        GD.Print(_localizationManager.GetText("LOG_ADVENTURE_BEGINS"));
        DisplaySection(1);
    }

    private void SetupPlaceholderPanels()
    {
        // Setup status panel with placeholder data
        string[] statusLabels = {
            _localizationManager!.GetText("SECTION_LABEL"),
            _localizationManager.GetText("SKILL_LABEL"),
            _localizationManager.GetText("STAMINA_LABEL"),
            _localizationManager.GetText("LUCK_LABEL"),
            _localizationManager.GetText("GOLD_LABEL")
        };
        _statusPanel!.SetupPlaceholderData(statusLabels, _currentSection);

        // Setup inventory panel placeholder  
        var itemsText = $"{_localizationManager.GetText("ITEMS_LABEL")}\n• Sword\n• Potion\n• Provisions";
        _inventoryPanel!.SetPlaceholderItems(itemsText);
    }

    private void DisplaySection(int sectionId)
    {
        _currentSection = sectionId;

        // Update image panel
        _imagePanel!.UpdateImage(_localizationManager!.GetText("SECTION_IMAGE"));
        
        // Update section label in bottom status bar
        _sectionLabel!.Text = $"{_localizationManager.GetText("SECTION_LABEL")} {sectionId}";

        if (_sections.TryGetValue(sectionId, out var sectionData))
        {
            // Build section text (without section number prefix)
            string sectionText;
            
            if (sectionData.TextKey == "SECTION_NOT_IMPLEMENTED")
            {
                sectionText = $"[color=red]{_localizationManager.GetText(sectionData.TextKey)}[/color]\n\n";
                if (!string.IsNullOrEmpty(sectionData.AdditionalTextKey))
                {
                    sectionText += _localizationManager.GetText(sectionData.AdditionalTextKey, sectionData.AdditionalTextArgs ?? new object[0]);
                }
            }
            else
            {
                sectionText = _localizationManager.GetText(sectionData.TextKey);
            }
            
            // Update story panel
            _storyPanel!.UpdateStoryText(sectionText);

            // Setup choices panel
            _choicesPanel!.SetupChoices(sectionData.Choices, (key) => _localizationManager.GetText(key));
            
            // Update status panel with current section
            _statusPanel!.UpdateSection(sectionId, _localizationManager.GetText("SECTION_LABEL"));
        }
        else
        {
            var errorText = $"[color=red]{_localizationManager.GetText("ERROR_SECTION_NOT_FOUND", sectionId)}[/color]";
            _storyPanel!.UpdateStoryText(errorText);
            _choicesPanel!.ClearChoices();
        }
    }

    private void OnChoiceSelected(int target, int choiceNumber)
    {
        GD.Print(_localizationManager!.GetText("LOG_SELECTED_OPTION", choiceNumber, target));
        DisplaySection(target);
    }

    private void OnImageClicked()
    {
        GD.Print("Section image clicked from ImagePanel");
    }

    private void OnLanguageButtonPressed()
    {
        if (_localizationManager == null) return;
        
        var currentLang = _localizationManager.GetCurrentLanguage();
        var availableLanguages = LocalizationManager.AvailableLanguages;
        var currentIndex = Array.IndexOf(availableLanguages, currentLang);
        var nextIndex = (currentIndex + 1) % availableLanguages.Length;
        var nextLanguage = availableLanguages[nextIndex];
        
        _localizationManager.SetLanguage(nextLanguage);
    }

    private void UpdateLanguageButton()
    {
        var currentLang = _localizationManager!.GetCurrentLanguage();
        var displayName = _localizationManager.GetLanguageName(currentLang);
        _languageButton!.Text = $"{_localizationManager.GetText("LANGUAGE")}: {displayName}";
    }

    // Called when language changes
    public void _on_language_changed()
    {
        if (_localizationManager == null) return;
        
        // Update language button text
        UpdateLanguageButton();
        
        // Refresh all panels  
        SetupPlaceholderPanels();
        DisplaySection(_currentSection);
    }

    // Debug function - can test from console
    public void GotoSection(int sectionId)
    {
        DisplaySection(sectionId);
    }
}