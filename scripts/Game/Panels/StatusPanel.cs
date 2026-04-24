using Godot;
using TalismanOfDeath.Data;

namespace TalismanOfDeath.Game.Panels;

public partial class StatusPanel : PanelContainer
{
    private Label? _skillLabel;
    private Label? _staminaLabel;
    private Label? _luckLabel;
    private Label? _goldLabel;
    private Button? _eatProvisionsButton;

    private CharacterSheet? _characterSheet;

    [Signal] public delegate void TestLuckRequestedEventHandler();
    [Signal] public delegate void RollStatsRequestedEventHandler();
    [Signal] public delegate void EatProvisionsRequestedEventHandler();

    public override void _Ready()
    {
        _skillLabel = GetNode<Label>("%SkillLabel");
        _staminaLabel = GetNode<Label>("%StaminaLabel");
        _luckLabel = GetNode<Label>("%LuckLabel");
        _goldLabel = GetNode<Label>("%GoldLabel");
        _eatProvisionsButton = GetNode<Button>("%EatProvisionsButton");

        GetNode<Button>("%RollStatsButton").Pressed += () => EmitSignal(SignalName.RollStatsRequested);
        GetNode<Button>("%TestLuckButton").Pressed += () => EmitSignal(SignalName.TestLuckRequested);
        _eatProvisionsButton.Pressed += () => EmitSignal(SignalName.EatProvisionsRequested);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationTranslationChanged)
            RefreshStats();
    }

    public void SetCharacterSheet(CharacterSheet characterSheet)
    {
        if (_characterSheet != null)
            _characterSheet.StatsChanged -= RefreshStats;

        _characterSheet = characterSheet;
        _characterSheet.StatsChanged += RefreshStats;
        RefreshStats();
    }

    private void RefreshStats()
    {
        if (_characterSheet == null) return;

        _skillLabel!.Text = StatText("SKILL_LABEL", _characterSheet.Skill, _characterSheet.InitialSkill);
        _staminaLabel!.Text = StatText("STAMINA_LABEL", _characterSheet.Stamina, _characterSheet.InitialStamina);
        _luckLabel!.Text = StatText("LUCK_LABEL", _characterSheet.Luck, _characterSheet.InitialLuck);
        _goldLabel!.Text = $"{Tr("GOLD_LABEL")} {_characterSheet.Gold} gp";

        if (_eatProvisionsButton != null)
            _eatProvisionsButton.Disabled = !_characterSheet.Inventory.Exists(i => i.Contains("Provisions"));
    }

    private string StatText(string key, int current, int initial)
    {
        var text = $"{Tr(key)} {current}";
        if (current < initial)
            text += $" ({Tr("MAX_LABEL")} {initial})";
        return text;
    }
}
