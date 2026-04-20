using Godot;
using System;
using System.Collections.Generic;
using TalismanOfDeath.Data;

namespace TalismanOfDeath.Game;

/// <summary>
/// Talisman of Death - Gateway-style adventure game interface
/// Starting with section 1
/// </summary>
public partial class Main : Control
{
    // UI Node references (must match unique_name_in_owner from Main.tscn)
    private Button? _sectionImagePlaceholder;
    private Label? _sectionLabel;
    private RichTextLabel? _storyText;
    private VBoxContainer? _statusPanel;
    private VBoxContainer? _inventoryPanel;
    private VBoxContainer? _choicesContainer;
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
        
        // Get node references (using unique names from Main.tscn)
        _sectionImagePlaceholder = GetNode<Button>("%SectionImagePlaceholder");
        _sectionLabel = GetNode<Label>("%SectionLabel");
        _storyText = GetNode<RichTextLabel>("%StoryText");
        _statusPanel = GetNode<VBoxContainer>("%StatusContainer");
        _inventoryPanel = GetNode<VBoxContainer>("%InventoryContainer");
        _choicesContainer = GetNode<VBoxContainer>("%ChoicesContainer");
        _languageButton = GetNode<Button>("%LanguageButton");

        // Connect signals
        _languageButton!.Pressed += OnLanguageButtonPressed;
        _sectionImagePlaceholder!.Pressed += OnSectionImagePressed;

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
        // Clear and setup Status Panel placeholder
        foreach (Node child in _statusPanel!.GetChildren())
        {
            child.QueueFree();
        }
        
        var statusTitle = new Label { Text = _localizationManager!.GetText("STATUS_TITLE") };
        statusTitle.AddThemeStyleboxOverride("normal", new StyleBoxFlat());
        _statusPanel.AddChild(statusTitle);
        
        var sectionInfo = new Label { Text = $"{_localizationManager.GetText("SECTION_LABEL")} {_currentSection}" };
        _statusPanel.AddChild(sectionInfo);
        
        var skillLabel = new Label { Text = $"{_localizationManager.GetText("SKILL_LABEL")} 12" };
        _statusPanel.AddChild(skillLabel);
        
        var staminaLabel = new Label { Text = $"{_localizationManager.GetText("STAMINA_LABEL")} 20" };
        _statusPanel.AddChild(staminaLabel);
        
        var luckLabel = new Label { Text = $"{_localizationManager.GetText("LUCK_LABEL")} 10" };
        _statusPanel.AddChild(luckLabel);
        
        var goldLabel = new Label { Text = $"{_localizationManager.GetText("GOLD_LABEL")} 25" };
        _statusPanel.AddChild(goldLabel);

        // Clear and setup Inventory Panel placeholder
        foreach (Node child in _inventoryPanel!.GetChildren())
        {
            child.QueueFree();
        }
        
        var inventoryTitle = new Label { Text = _localizationManager.GetText("INVENTORY_TITLE") };
        _inventoryPanel.AddChild(inventoryTitle);
        
        var itemsLabel = new Label { Text = $"{_localizationManager.GetText("ITEMS_LABEL")}\n• Sword\n• Potion\n• Provisions" };
        _inventoryPanel.AddChild(itemsLabel);
    }

    private void DisplaySection(int sectionId)
    {
        _currentSection = sectionId;

        // Update section image placeholder
        _sectionImagePlaceholder!.Text = _localizationManager!.GetText("SECTION_IMAGE");
        
        // Update section label
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
            
            _storyText!.Text = sectionText;

            // Setup choices
            SetupChoices(sectionData.Choices);
            
            // Update status panel section info
            RefreshStatusPanel();
        }
        else
        {
            _storyText!.Text = $"[color=red]{_localizationManager.GetText("ERROR_SECTION_NOT_FOUND", sectionId)}[/color]";
            ClearChoices();
        }
    }

    private void RefreshStatusPanel()
    {
        // Update the section number in status panel
        if (_statusPanel!.GetChildCount() > 1)
        {
            var sectionInfo = _statusPanel.GetChild(1) as Label;
            if (sectionInfo != null)
            {
                sectionInfo.Text = $"{_localizationManager!.GetText("SECTION_LABEL")} {_currentSection}";
            }
        }
    }

    private void SetupChoices(List<Choice> choices)
    {
        // Clear existing choice elements
        ClearChoices();

        // Add choice title
        var choicesTitle = new Label { Text = _localizationManager!.GetText("CHOICES_TITLE") };
        _choicesContainer!.AddChild(choicesTitle);

        // Create RichTextLabel for each choice with word wrap
        for (int i = 0; i < choices.Count; i++)
        {
            var choice = choices[i];
            
            // Create clickable choice container with dynamic sizing
            var choiceContainer = new Panel
            {
                SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
                MouseFilter = Control.MouseFilterEnum.Pass
            };
            
            var choiceLabel = new RichTextLabel
            {
                BbcodeEnabled = true,
                FitContent = true,
                ScrollActive = false,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
                CustomMinimumSize = new Vector2(280, 0),
                Text = $"[color=white]{i + 1}. {_localizationManager.GetText(choice.TextKey)}[/color]"
            };
            
            // Setup container layout - manual anchor and offset setting
            choiceLabel.AnchorLeft = 0.0f;
            choiceLabel.AnchorTop = 0.0f;
            choiceLabel.AnchorRight = 1.0f;
            choiceLabel.AnchorBottom = 1.0f;
            choiceLabel.OffsetLeft = 10;
            choiceLabel.OffsetTop = 5;
            choiceLabel.OffsetRight = -10;
            choiceLabel.OffsetBottom = -5;
            
            // Store target section in metadata
            choiceContainer.SetMeta("target", choice.Target);
            choiceContainer.SetMeta("choice_number", i + 1);
            
            // Connect input event for click detection
            choiceContainer.GuiInput += (inputEvent) => OnChoiceInput(choiceContainer, inputEvent);
            
            // Add hover effect
            var styleBox = new StyleBoxFlat
            {
                BgColor = new Color(0.2f, 0.2f, 0.3f, 0.8f),
                BorderWidthLeft = 2,
                BorderWidthRight = 2,
                BorderWidthTop = 2,
                BorderWidthBottom = 2,
                BorderColor = new Color(0.4f, 0.4f, 0.6f, 1.0f),
                CornerRadiusTopLeft = 4,
                CornerRadiusTopRight = 4,
                CornerRadiusBottomLeft = 4,
                CornerRadiusBottomRight = 4
            };
            choiceContainer.AddThemeStyleboxOverride("panel", styleBox);
            
            choiceContainer.AddChild(choiceLabel);
            _choicesContainer!.AddChild(choiceContainer);
        }
    }

    private void ClearChoices()
    {
        foreach (Node child in _choicesContainer!.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void OnChoiceInput(Panel choiceContainer, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            var target = choiceContainer.GetMeta("target").AsInt32();
            var choiceNumber = choiceContainer.GetMeta("choice_number").AsInt32();
            
            GD.Print(_localizationManager!.GetText("LOG_SELECTED_OPTION", choiceNumber, target));
            DisplaySection(target);
        }
    }

    private void OnSectionImagePressed()
    {
        GD.Print("Section image placeholder clicked");
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
        
        // Refresh all UI elements  
        SetupPlaceholderPanels();
        DisplaySection(_currentSection);
    }

    // Debug function - can test from console
    public void GotoSection(int sectionId)
    {
        DisplaySection(sectionId);
    }
}