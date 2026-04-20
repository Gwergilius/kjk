using Godot;
using TalismanOfDeath.Data;

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
    private Button? _rollStatsButton;
    private Button? _testLuckButton;
    private Button? _eatProvisionsButton;
    
    private CharacterSheet? _characterSheet;
    
    [Signal]
    public delegate void TestLuckRequestedEventHandler();
    
    [Signal]
    public delegate void RollStatsRequestedEventHandler();
    
    [Signal]
    public delegate void EatProvisionsRequestedEventHandler();
    
    public override void _Ready()
    {
        _sectionLabel = GetNode<Label>("%SectionLabel");
        _skillLabel = GetNode<Label>("%SkillLabel");
        _staminaLabel = GetNode<Label>("%StaminaLabel");
        _luckLabel = GetNode<Label>("%LuckLabel");
        _goldLabel = GetNode<Label>("%GoldLabel");
        _rollStatsButton = GetNode<Button>("%RollStatsButton");
        _testLuckButton = GetNode<Button>("%TestLuckButton");
        _eatProvisionsButton = GetNode<Button>("%EatProvisionsButton");
        
        // Connect button signals
        _rollStatsButton!.Pressed += OnRollStatsPressed;
        _testLuckButton!.Pressed += OnTestLuckPressed;
        _eatProvisionsButton!.Pressed += OnEatProvisionsPressed;
    }
    
    /// <summary>
    /// Set the character sheet reference and connect to its signals
    /// </summary>
    public void SetCharacterSheet(CharacterSheet characterSheet)
    {
        // Disconnect from previous character sheet
        if (_characterSheet != null)
        {
            _characterSheet.StatsChanged -= OnStatsChanged;
        }
        
        _characterSheet = characterSheet;
        
        if (_characterSheet != null)
        {
            // Connect to character sheet signals
            _characterSheet.StatsChanged += OnStatsChanged;
            
            // Update display
            OnStatsChanged();
        }
    }
    
    private void OnStatsChanged()
    {
        if (_characterSheet == null) return;
        
        if (_skillLabel != null)
        {
            _skillLabel.Text = $"Skill: {_characterSheet.Skill}";
            if (_characterSheet.Skill < _characterSheet.InitialSkill)
                _skillLabel.Text += $" (Max: {_characterSheet.InitialSkill})";
        }
        
        if (_staminaLabel != null)
        {
            _staminaLabel.Text = $"Stamina: {_characterSheet.Stamina}";
            if (_characterSheet.Stamina < _characterSheet.InitialStamina)
                _staminaLabel.Text += $" (Max: {_characterSheet.InitialStamina})";
        }
        
        if (_luckLabel != null)
        {
            _luckLabel.Text = $"Luck: {_characterSheet.Luck}";
            if (_characterSheet.Luck < _characterSheet.InitialLuck)
                _luckLabel.Text += $" (Max: {_characterSheet.InitialLuck})";
        }
        
        if (_goldLabel != null)
        {
            _goldLabel.Text = $"Gold: {_characterSheet.Gold} gp";
        }
        
        // Enable/disable buttons based on state
        if (_eatProvisionsButton != null)
        {
            _eatProvisionsButton.Disabled = !_characterSheet.HasItem("Provisions") && 
                                           !_characterSheet.Inventory.Exists(item => item.Contains("Provisions"));
        }
    }
    
    public void UpdateSection(int sectionNumber, string sectionLabelText)
    {
        if (_sectionLabel != null)
        {
            _sectionLabel.Text = $"{sectionLabelText} {sectionNumber}";
        }
    }
    
    // Legacy method for backwards compatibility
    public void UpdateStats(int skill, int stamina, int luck, int gold, string[] statLabels)
    {
        // This is now handled by the character sheet system
        // Keep for compatibility but prefer using CharacterSheet
        if (_characterSheet != null)
        {
            _characterSheet.SetStats(skill, stamina, luck, gold);
        }
    }
    
    // Legacy method for backwards compatibility  
    public void SetupPlaceholderData(string[] labels, int section)
    {
        if (_characterSheet == null) return;
        
        if (_sectionLabel != null) _sectionLabel.Text = $"{labels[0]} {section}";
        
        // Use character sheet data instead of hardcoded values
        OnStatsChanged();
    }
    
    private void OnRollStatsPressed()
    {
        EmitSignal(SignalName.RollStatsRequested);
    }
    
    private void OnTestLuckPressed()
    {
        EmitSignal(SignalName.TestLuckRequested);
    }
    
    private void OnEatProvisionsPressed()
    {
        EmitSignal(SignalName.EatProvisionsRequested);
    }
}