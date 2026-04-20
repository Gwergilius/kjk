using Godot;

namespace TalismanOfDeath.Game.Panels;

/// <summary>
/// Handles player status display (Section, Skill, Stamina, Luck, Gold)
/// </summary>
public partial class StatusPanel : Panel
{
    private Label? _sectionLabel;
    private Label? _skillLabel;
    private Label? _staminaLabel;
    private Label? _luckLabel;
    private Label? _goldLabel;
    
    public override void _Ready()
    {
        _sectionLabel = GetNode<Label>("%SectionLabel");
        _skillLabel = GetNode<Label>("%SkillLabel");
        _staminaLabel = GetNode<Label>("%StaminaLabel");
        _luckLabel = GetNode<Label>("%LuckLabel");
        _goldLabel = GetNode<Label>("%GoldLabel");
    }
    
    public void UpdateSection(int sectionNumber, string sectionLabelText)
    {
        if (_sectionLabel != null)
        {
            _sectionLabel.Text = $"{sectionLabelText} {sectionNumber}";
        }
    }
    
    public void UpdateStats(int skill, int stamina, int luck, int gold, string[] statLabels)
    {
        if (_skillLabel != null) _skillLabel.Text = $"{statLabels[0]} {skill}";
        if (_staminaLabel != null) _staminaLabel.Text = $"{statLabels[1]} {stamina}";
        if (_luckLabel != null) _luckLabel.Text = $"{statLabels[2]} {luck}";
        if (_goldLabel != null) _goldLabel.Text = $"{statLabels[3]} {gold}";
    }
    
    public void SetupPlaceholderData(string[] labels, int section)
    {
        if (_sectionLabel != null) _sectionLabel.Text = $"{labels[0]} {section}";
        if (_skillLabel != null) _skillLabel.Text = $"{labels[1]} 12";
        if (_staminaLabel != null) _staminaLabel.Text = $"{labels[2]} 20";
        if (_luckLabel != null) _luckLabel.Text = $"{labels[3]} 10";
        if (_goldLabel != null) _goldLabel.Text = $"{labels[4]} 25";
    }
}