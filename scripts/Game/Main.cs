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
    private DiceRollDialog? _diceRollDialog;
    
    // UI elements in Main scene
    private Label? _sectionLabel;
    private Button? _languageButton;
    private Button? _saveButton;
    private Button? _loadButton;

    private int _currentSection = 1;
    private LocalizationManager? _localizationManager;
    private CharacterSheet? _characterSheet;

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
        
        // Create and initialize character sheet
        _characterSheet = new CharacterSheet();
        _characterSheet.RollInitialStats(); // Roll starting stats
        
        // Get panel references (modularized scene instances)
        _imagePanel = GetNode<ImagePanel>("%ImagePanel");
        _storyPanel = GetNode<StoryPanel>("%StoryPanel");
        _statusPanel = GetNode<StatusPanel>("%StatusPanel");
        _inventoryPanel = GetNode<InventoryPanel>("%InventoryPanel");
        _choicesPanel = GetNode<ChoicesPanel>("%ChoicesPanel");
        _diceRollDialog = GetNode<DiceRollDialog>("%DiceRollDialog");
        
        // Set character sheet references in panels
        _statusPanel!.SetCharacterSheet(_characterSheet);
        _inventoryPanel!.SetCharacterSheet(_characterSheet);
        
        // Get UI element references
        _sectionLabel = GetNode<Label>("%SectionLabel");
        _languageButton = GetNode<Button>("%LanguageButton");
        _saveButton = GetNode<Button>("%SaveButton");
        _loadButton = GetNode<Button>("%LoadButton");

        // Connect panel signals
        _imagePanel!.ImageClicked += OnImageClicked;
        _choicesPanel!.ChoiceSelected += OnChoiceSelected;
        _statusPanel!.TestLuckRequested += OnTestLuckRequested;
        _statusPanel!.RollStatsRequested += OnRollStatsRequested;
        _statusPanel!.EatProvisionsRequested += OnEatProvisionsRequested;
        
        // Connect main UI signals
        _languageButton!.Pressed += OnLanguageButtonPressed;
        _saveButton!.Pressed += OnSaveButtonPressed;
        _loadButton!.Pressed += OnLoadButtonPressed;

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
    
    // StatusPanel signal handlers
    private void OnTestLuckRequested()
    {
        if (_characterSheet == null || _diceRollDialog == null) return;
        
        // Show dice roll dialog for luck test
        _diceRollDialog.SetupRoll(_characterSheet, "luck", _characterSheet.Luck);
        _diceRollDialog.DiceRollCompleted += OnLuckTestCompleted;
        _diceRollDialog.Show();
    }
    
    private void OnRollStatsRequested()
    {
        if (_characterSheet == null || _diceRollDialog == null) return;
        
        // Show dice roll dialog for stat rolling
        _diceRollDialog.SetupRoll(_characterSheet, "general");
        _diceRollDialog.DiceRollCompleted += OnStatsRollCompleted;
        _diceRollDialog.Show();
    }
    
    private void OnEatProvisionsRequested()
    {
        if (_characterSheet == null) return;
        
        if (_characterSheet.EatProvisions())
        {
            GD.Print("Ate provisions and restored 4 Stamina!");
        }
        else
        {
            GD.Print("No provisions available!");
        }
    }
    
    // Dice roll completion handlers
    private void OnLuckTestCompleted(int die1, int die2, int total)
    {
        if (_characterSheet == null) return;
        
        // Test Your Luck mechanic already handled in CharacterSheet.TestYourLuck()
        var lucky = total <= _characterSheet.Luck + 1; // +1 because luck was already decremented
        
        // Disconnect the signal
        _diceRollDialog!.DiceRollCompleted -= OnLuckTestCompleted;
        
        GD.Print($"Luck test result: {(lucky ? "Lucky" : "Unlucky")} (rolled {total})");
    }
    
    private void OnStatsRollCompleted(int die1, int die2, int total)
    {
        if (_characterSheet == null) return;
        
        // Re-roll all initial stats
        _characterSheet.RollInitialStats();
        
        // Disconnect the signal
        _diceRollDialog!.DiceRollCompleted -= OnStatsRollCompleted;
        
        GD.Print("New character stats rolled!");
    }
    
    // Save/Load handlers
    private void OnSaveButtonPressed()
    {
        SaveGame();
    }
    
    private void OnLoadButtonPressed()
    {
        LoadGame();
    }
    
    private void SaveGame()
    {
        if (_characterSheet == null) return;
        
        try
        {
            var saveData = new Godot.Collections.Dictionary<string, Variant>
            {
                ["CurrentSection"] = _currentSection,
                ["CharacterData"] = _characterSheet.SaveCharacterData(),
                ["SaveTimestamp"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            var file = FileAccess.Open("user://talisman_save.dat", FileAccess.ModeFlags.Write);
            if (file != null)
            {
                file.StoreVar(saveData);
                file.Close();
                GD.Print("Game saved successfully!");
            }
            else
            {
                GD.PrintErr("Failed to open save file for writing!");
            }
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"Save failed: {e.Message}");
        }
    }
    
    private void LoadGame()
    {
        if (_characterSheet == null) return;
        
        try
        {
            var file = FileAccess.Open("user://talisman_save.dat", FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.Print("No save file found!");
                return;
            }
            
            var saveData = file.GetVar().AsGodotDictionary<string, Variant>();
            file.Close();
            
            if (saveData != null)
            {
                // Load current section
                if (saveData.ContainsKey("CurrentSection"))
                {
                    var section = saveData["CurrentSection"].AsInt32();
                    _currentSection = section;
                }
                
                // Load character data
                if (saveData.ContainsKey("CharacterData"))
                {
                    var characterData = saveData["CharacterData"].AsGodotDictionary<string, Variant>();
                    _characterSheet.LoadCharacterData(characterData);
                }
                
                // Update display
                DisplaySection(_currentSection);
                
                var timestamp = "Unknown";
                if (saveData.ContainsKey("SaveTimestamp"))
                    timestamp = saveData["SaveTimestamp"].AsString();
                    
                GD.Print($"Game loaded successfully! (Saved: {timestamp})");
            }
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"Load failed: {e.Message}");
        }
    }
}