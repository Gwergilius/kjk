using Godot;
using System;
using TalismanOfDeath.Data;
using TalismanOfDeath.Game.Panels;

namespace TalismanOfDeath.Game;

public partial class Main : Control
{
    private static readonly string[] AvailableLocales = { "en", "hu" };
    private static readonly string[] LocaleDisplayNames = { "English", "Magyar" };

    private Control? _contentArea;
    private DiceRollDialog? _diceRollDialog;
    private LineEdit? _sectionInput;
    private OptionButton? _languageButton;
    private Button? _saveButton;
    private Button? _loadButton;

    private LocationScene? _locationScene;
    private StoryScene? _storyScene;
    private SceneType _activeSceneType = (SceneType)(-1);

    private string _currentSection = "";
    private string _currentLocale = "en";
    private CharacterSheet? _characterSheet;
    private StoryRepository _storyRepository = new();
    private bool _isReady = false;

    public override void _Ready()
    {

        var systemLang = OS.GetLocaleLanguage();
        _currentLocale = Array.IndexOf(AvailableLocales, systemLang) >= 0 ? systemLang : "en";
        TranslationServer.SetLocale(_currentLocale);

        _storyRepository.LoadAll(AvailableLocales);

        _characterSheet = new CharacterSheet();
        _characterSheet.RollInitialStats();

        _contentArea = GetNode<Control>("MainLayout/ContentArea");
        _diceRollDialog = GetNode<DiceRollDialog>("%DiceRollDialog");
        _sectionInput = GetNode<LineEdit>("%SectionInput");
        _languageButton = GetNode<OptionButton>("%LanguageButton");
        _saveButton = GetNode<Button>("%SaveButton");
        _loadButton = GetNode<Button>("%LoadButton");
        _sectionInput.TextSubmitted += id => DisplaySection(id.Trim());

        _locationScene = GD.Load<PackedScene>("res://scenes/game/LocationScene.tscn").Instantiate<LocationScene>();
        _storyScene = GD.Load<PackedScene>("res://scenes/game/StoryScene.tscn").Instantiate<StoryScene>();

        _locationScene.ChoiceSelected += OnChoiceSelected;
        _locationScene.TestLuckRequested += OnTestLuckRequested;
        _locationScene.RollStatsRequested += OnRollStatsRequested;
        _locationScene.EatProvisionsRequested += OnEatProvisionsRequested;
        _storyScene.NextPressed += DisplaySection;

        _locationScene.Visible = false;
        _storyScene.Visible = false;
        _contentArea.AddChild(_locationScene);
        _contentArea.AddChild(_storyScene);
        _locationScene.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _storyScene.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _locationScene.SetCharacterSheet(_characterSheet);

        _saveButton!.Pressed += OnSaveButtonPressed;
        _loadButton!.Pressed += OnLoadButtonPressed;

        for (int i = 0; i < LocaleDisplayNames.Length; i++)
            _languageButton!.AddItem(LocaleDisplayNames[i], i);
        _languageButton!.Selected = Array.IndexOf(AvailableLocales, _currentLocale);
        _languageButton!.ItemSelected += OnLanguageSelected;

        var root = GetTree().Root;
        root.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
        root.ContentScaleSize = new Vector2I(0, 0);

        _isReady = true;
        DisplaySection(_storyRepository.StartSection);
    }

    public override void _Notification(int what)
    {
        if (!_isReady) return;
        if (what == NotificationTranslationChanged)
            DisplaySection(_currentSection);
    }

    private void DisplaySection(string sectionId)
    {
        _currentSection = sectionId;
        _sectionInput!.Text = sectionId;

        var section = _storyRepository.GetSection(sectionId, _currentLocale);
        var image = section != null ? LoadSectionImage(section.Image) : null;

        if (section == null)
        {
            ActivateScene(SceneType.Location);
            var error = $"[color=red]{string.Format(Tr("ERROR_SECTION_NOT_FOUND"), sectionId)}[/color]";
            _locationScene!.ShowError(error);
            return;
        }

        ActivateScene(section.Type);

        switch (section.Type)
        {
            case SceneType.Story:
                _storyScene!.DisplaySection(section, image);
                break;
            default:
                _locationScene!.DisplaySection(section, image);
                break;
        }
    }

    private void ActivateScene(SceneType type)
    {
        if (_activeSceneType == type) return;
        _activeSceneType = type;
        _locationScene!.Visible = type != SceneType.Story;
        _storyScene!.Visible = type == SceneType.Story;
    }

    private static Texture2D? LoadSectionImage(string? imageName)
    {
        if (imageName == null) return null;
        var path = $"res://assets/images/{imageName}";
        return ResourceLoader.Exists(path) ? GD.Load<Texture2D>(path) : null;
    }

    private void OnChoiceSelected(string target, int choiceNumber)
    {
        GD.Print($"Choice {choiceNumber} → section {target}");
        DisplaySection(target);
    }

    private void OnLanguageSelected(long index)
    {
        _currentLocale = AvailableLocales[(int)index];
        TranslationServer.SetLocale(_currentLocale);
    }

    private void OnTestLuckRequested()
    {
        if (_characterSheet == null || _diceRollDialog == null) return;
        _diceRollDialog.SetupRoll(_characterSheet, "luck", _characterSheet.Luck);
        _diceRollDialog.DiceRollCompleted += OnLuckTestCompleted;
        _diceRollDialog.Show();
    }

    private void OnRollStatsRequested()
    {
        if (_characterSheet == null || _diceRollDialog == null) return;
        _diceRollDialog.SetupRoll(_characterSheet, "general");
        _diceRollDialog.DiceRollCompleted += OnStatsRollCompleted;
        _diceRollDialog.Show();
    }

    private void OnEatProvisionsRequested()
    {
        if (_characterSheet == null) return;
        if (!_characterSheet.EatProvisions())
            GD.Print("No provisions available!");
    }

    private void OnLuckTestCompleted(int die1, int die2, int total)
    {
        _diceRollDialog!.DiceRollCompleted -= OnLuckTestCompleted;
        var lucky = total <= _characterSheet!.Luck + 1;
        GD.Print($"Luck test: {(lucky ? "Lucky" : "Unlucky")} (rolled {total})");
    }

    private void OnStatsRollCompleted(int die1, int die2, int total)
    {
        _diceRollDialog!.DiceRollCompleted -= OnStatsRollCompleted;
        _characterSheet!.RollInitialStats();
        GD.Print("New stats rolled!");
    }

    private void OnSaveButtonPressed() => SaveGame();
    private void OnLoadButtonPressed() => LoadGame();

    private void SaveGame()
    {
        if (_characterSheet == null) return;
        try
        {
            var saveData = new Godot.Collections.Dictionary<string, Variant>
            {
                ["CurrentSection"] = _currentSection,
                ["CurrentLocale"] = _currentLocale,
                ["CharacterData"] = _characterSheet.SaveCharacterData(),
                ["SaveTimestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            using var file = FileAccess.Open("user://talisman_save.dat", FileAccess.ModeFlags.Write);
            if (file != null) { file.StoreVar(saveData); GD.Print("Game saved."); }
        }
        catch (Exception e) { GD.PrintErr($"Save failed: {e.Message}"); }
    }

    private void LoadGame()
    {
        if (_characterSheet == null) return;
        try
        {
            using var file = FileAccess.Open("user://talisman_save.dat", FileAccess.ModeFlags.Read);
            if (file == null) { GD.Print("No save file found."); return; }

            var saveData = file.GetVar().AsGodotDictionary<string, Variant>();
            if (saveData == null) return;

            if (saveData.TryGetValue("CurrentLocale", out var localeVar))
            {
                _currentLocale = localeVar.AsString();
                TranslationServer.SetLocale(_currentLocale);
                _languageButton!.Selected = Array.IndexOf(AvailableLocales, _currentLocale);
            }
            if (saveData.TryGetValue("CharacterData", out var charVar))
                _characterSheet.LoadCharacterData(charVar.AsGodotDictionary<string, Variant>());
            if (saveData.TryGetValue("CurrentSection", out var sectionVar))
                DisplaySection(sectionVar.AsString());

            GD.Print("Game loaded.");
        }
        catch (Exception e) { GD.PrintErr($"Load failed: {e.Message}"); }
    }
}
