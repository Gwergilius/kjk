using Godot;
using System;
using TalismanOfDeath.Data;

namespace TalismanOfDeath.Game.Panels;

/// <summary>
/// Dialog for showing dice roll animations and results
/// </summary>
public partial class DiceRollDialog : AcceptDialog
{
    [Signal]
    public delegate void DiceRollCompletedEventHandler(int die1, int die2, int total);
    
    private Label? _rollTitle;
    private Label? _die1Value;
    private Label? _die2Value;
    private Label? _totalValue;
    private RichTextLabel? _resultText;
    private Button? _rollButton;
    private Button? _closeButton;
    
    private CharacterSheet? _characterSheet;
    private Timer? _rollTimer;
    private int _rollAnimationSteps = 0;
    private bool _isRolling = false;
    
    private string _rollType = "";
    private int _targetValue = 0;
    
    public override void _Ready()
    {
        _rollTitle = GetNode<Label>("%RollTitle");
        _die1Value = GetNode<Label>("%Die1Value");
        _die2Value = GetNode<Label>("%Die2Value");
        _totalValue = GetNode<Label>("%TotalValue");
        _resultText = GetNode<RichTextLabel>("%ResultText");
        _rollButton = GetNode<Button>("%RollButton");
        _closeButton = GetNode<Button>("%CloseButton");
        
        // Create and configure timer for dice animation
        _rollTimer = new Timer();
        _rollTimer.WaitTime = 0.1;
        _rollTimer.Timeout += OnRollTimerTimeout;
        AddChild(_rollTimer);
        
        // Connect button signals
        _rollButton!.Pressed += OnRollButtonPressed;
        _closeButton!.Pressed += OnCloseButtonPressed;
        
        // Dialog settings
        CloseRequested += OnCloseButtonPressed;
    }
    
    /// <summary>
    /// Setup dice roll dialog for a specific type of roll
    /// </summary>
    public void SetupRoll(CharacterSheet characterSheet, string rollType, int targetValue = 0)
    {
        _characterSheet = characterSheet;
        _rollType = rollType;
        _targetValue = targetValue;
        
        // Reset display
        _die1Value!.Text = "?";
        _die2Value!.Text = "?";
        _totalValue!.Text = "?";
        _rollButton!.Disabled = false;
        _isRolling = false;
        
        // Set title and instructions based on roll type
        switch (rollType.ToLower())
        {
            case "luck":
                _rollTitle!.Text = "Test Your Luck";
                _resultText!.Text = $"[center]Roll {targetValue} or less on two dice[/center]";
                break;
            case "skill":
                _rollTitle!.Text = "Skill Test";
                _resultText!.Text = $"[center]Roll {targetValue} or less on two dice[/center]";
                break;
            case "combat":
                _rollTitle!.Text = "Combat Roll";
                _resultText!.Text = "[center]Roll for combat resolution[/center]";
                break;
            case "general":
            default:
                _rollTitle!.Text = "Roll Dice";
                _resultText!.Text = "[center]Roll two six-sided dice[/center]";
                break;
        }
    }
    
    private void OnRollButtonPressed()
    {
        if (_isRolling || _characterSheet == null) return;
        
        _isRolling = true;
        _rollButton!.Disabled = true;
        _rollAnimationSteps = 0;
        
        // Start rolling animation
        _rollTimer!.Start();
    }
    
    private void OnRollTimerTimeout()
    {
        _rollAnimationSteps++;
        
        // Show random numbers during animation (first 10 steps)
        if (_rollAnimationSteps <= 10)
        {
            _die1Value!.Text = GD.RandRange(1, 6).ToString();
            _die2Value!.Text = GD.RandRange(1, 6).ToString();
            _totalValue!.Text = "?";
        }
        else
        {
            // Final roll
            var (die1, die2, total) = _characterSheet!.RollTwoDice();
            
            _die1Value!.Text = die1.ToString();
            _die2Value!.Text = die2.ToString();
            _totalValue!.Text = total.ToString();
            
            // Show result text
            ShowRollResult(die1, die2, total);
            
            _rollTimer!.Stop();
            _isRolling = false;
            _rollButton!.Text = "Roll Again";
            _rollButton!.Disabled = false;
            
            // Emit completion signal
            EmitSignal(SignalName.DiceRollCompleted, die1, die2, total);
        }
    }
    
    private void ShowRollResult(int die1, int die2, int total)
    {
        string resultText = $"[center][b]{die1} + {die2} = {total}[/b][/center]\n\n";
        
        switch (_rollType.ToLower())
        {
            case "luck":
                if (total <= _targetValue)
                {
                    resultText += "[center][color=green]LUCKY![/color][/center]\n";
                    resultText += $"[center]Rolled {total} ≤ {_targetValue}[/center]";
                }
                else
                {
                    resultText += "[center][color=red]UNLUCKY![/color][/center]\n";
                    resultText += $"[center]Rolled {total} > {_targetValue}[/center]";
                }
                resultText += "\n[center][color=yellow]Luck reduced by 1[/color][/center]";
                break;
                
            case "skill":
                if (total <= _targetValue)
                {
                    resultText += "[center][color=green]SUCCESS![/color][/center]\n";
                    resultText += $"[center]Rolled {total} ≤ {_targetValue}[/center]";
                }
                else
                {
                    resultText += "[center][color=red]FAILURE![/color][/center]\n";
                    resultText += $"[center]Rolled {total} > {_targetValue}[/center]";
                }
                break;
                
            case "combat":
                resultText += $"[center]Combat Strength: {_targetValue + total}[/center]";
                break;
                
            default:
                resultText += "[center]Dice rolled![/center]";
                break;
        }
        
        _resultText!.Text = resultText;
    }
    
    private void OnCloseButtonPressed()
    {
        Hide();
    }
    
    /// <summary>
    /// Show the dice roll dialog
    /// </summary>
    public new void Show()
    {
        PopupCentered();
    }
}