using Godot;
using System;
using System.Collections.Generic;
using TalismanOfDeath.Data;

namespace TalismanOfDeath.Game;

/// <summary>
/// Talisman of Death - Simple prototype with localization
/// Starting with section 1
/// </summary>
public partial class Main : Control
{
    [Export] private RichTextLabel? _storyText;
    [Export] private VBoxContainer? _choicesContainer;
    [Export] private Button? _choice1Button;
    [Export] private Button? _choice2Button;
    [Export] private Button? _languageButton;
    [Export] private Label? _titleLabel;

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
                new() { TextKey = "SECTION_17_CHOICE_1", Target = 41 },
                new() { TextKey = "SECTION_17_CHOICE_2", Target = 21 }
            }
        }},
        {30, new SectionData
        {
            TextKey = "SECTION_30_TEXT",
            Choices = new List<Choice>
            {
                new() { TextKey = "SECTION_30_CHOICE_1", Target = 13 }
            }
        }},
        {41, new SectionData
        {
            TextKey = "SECTION_NOT_IMPLEMENTED",
            AdditionalTextKey = "SECTION_NOT_IMPLEMENTED_DESC",
            AdditionalTextArgs = new object[] { 41 },
            Choices = new List<Choice>
            {
                new() { TextKey = "BACK_TO_BEGINNING", Target = 1 }
            }
        }},
        {21, new SectionData
        {
            TextKey = "SECTION_NOT_IMPLEMENTED",
            AdditionalTextKey = "SECTION_NOT_IMPLEMENTED_DESC",
            AdditionalTextArgs = new object[] { 21 },
            Choices = new List<Choice>
            {
                new() { TextKey = "BACK_TO_BEGINNING", Target = 1 }
            }
        }},
        {13, new SectionData
        {
            TextKey = "SECTION_NOT_IMPLEMENTED",
            AdditionalTextKey = "SECTION_NOT_IMPLEMENTED_DESC",
            AdditionalTextArgs = new object[] { 13 },
            Choices = new List<Choice>
            {
                new() { TextKey = "BACK_TO_BEGINNING", Target = 1 }
            }
        }}
    };

    public override void _Ready()
    {
        // Create and add LocalizationManager
        _localizationManager = new LocalizationManager();
        AddChild(_localizationManager);
        
        // Get node references
        _storyText = GetNode<RichTextLabel>("%StoryText");
        _choicesContainer = GetNode<VBoxContainer>("%ChoicesContainer");
        _choice1Button = GetNode<Button>("%Choice1");
        _choice2Button = GetNode<Button>("%Choice2");
        _languageButton = GetNode<Button>("%LanguageButton");
        _titleLabel = GetNode<Label>("GameContainer/VBox/Title");

        // Connect signals
        _choice1Button.Pressed += OnChoice1Pressed;
        _choice2Button.Pressed += OnChoice2Pressed;
        _languageButton.Pressed += OnLanguageButtonPressed;

        // Add to localized nodes group for language change notifications
        AddToGroup("localized_nodes");
        
        // Set initial UI text
        _titleLabel!.Text = _localizationManager.GetText("GAME_TITLE");
        var currentLang = _localizationManager.GetCurrentLanguage();
        var displayName = _localizationManager.GetLanguageName(currentLang);
        _languageButton!.Text = $"{_localizationManager.GetText("LANGUAGE")}: {displayName}";
        
        GD.Print(_localizationManager.GetText("LOG_ADVENTURE_BEGINS"));
        DisplaySection(1);
    }

    private void DisplaySection(int sectionId)
    {
        _currentSection = sectionId;

        if (_sections.TryGetValue(sectionId, out var sectionData))
        {
            // Build section text
            var sectionText = $"[center][b]{sectionId}.[/b][/center]\n\n";
            
            if (sectionData.TextKey == "SECTION_NOT_IMPLEMENTED")
            {
                sectionText += $"[color=red]{_localizationManager!.GetText(sectionData.TextKey)}[/color]\n\n";
                if (!string.IsNullOrEmpty(sectionData.AdditionalTextKey))
                {
                    sectionText += _localizationManager.GetText(sectionData.AdditionalTextKey, sectionData.AdditionalTextArgs ?? new object[0]);
                }
            }
            else
            {
                sectionText += _localizationManager!.GetText(sectionData.TextKey);
            }
            
            _storyText!.Text = sectionText;

            // Setup choices
            SetupChoices(sectionData.Choices);
        }
        else
        {
            _storyText!.Text = $"[color=red]{_localizationManager!.GetText("ERROR_SECTION_NOT_FOUND", sectionId)}[/color]";
            HideChoices();
        }
    }

    private void SetupChoices(List<Choice> choices)
    {
        // Hide both buttons first
        _choice1Button!.Visible = false;
        _choice2Button!.Visible = false;

        // Setup choices based on count
        if (choices.Count >= 1)
        {
            _choice1Button.Text = _localizationManager!.GetText(choices[0].TextKey);
            _choice1Button.Visible = true;
            _choice1Button.SetMeta("target", choices[0].Target);
        }

        if (choices.Count >= 2)
        {
            _choice2Button.Text = _localizationManager!.GetText(choices[1].TextKey);
            _choice2Button.Visible = true;
            _choice2Button.SetMeta("target", choices[1].Target);
        }
    }

    private void HideChoices()
    {
        _choice1Button!.Visible = false;
        _choice2Button!.Visible = false;
    }

    private void OnChoice1Pressed()
    {
        var target = _choice1Button!.GetMeta("target").AsInt32();
        GD.Print(_localizationManager!.GetText("LOG_SELECTED_OPTION", 1, target));
        DisplaySection(target);
    }

    private void OnChoice2Pressed()
    {
        var target = _choice2Button!.GetMeta("target").AsInt32();
        GD.Print(_localizationManager!.GetText("LOG_SELECTED_OPTION", 2, target));
        DisplaySection(target);
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

    // Called when language changes
    public void _on_language_changed()
    {
        if (_localizationManager == null) return;
        
        // Update title
        _titleLabel!.Text = _localizationManager.GetText("GAME_TITLE");
        
        // Update language button text
        var currentLang = _localizationManager.GetCurrentLanguage();
        var displayName = _localizationManager.GetLanguageName(currentLang);
        _languageButton!.Text = $"{_localizationManager.GetText("LANGUAGE")}: {displayName}";
        
        // Refresh current section to update all text
        DisplaySection(_currentSection);
    }

    // Debug function - can test from console
    public void GotoSection(int sectionId)
    {
        DisplaySection(sectionId);
    }
}