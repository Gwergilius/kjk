using Godot;
using System;
using System.Collections.Generic;
using TalismanOfDeath.Data;

namespace TalismanOfDeath.Game.Panels;

/// <summary>
/// Handles choice display and selection management
/// </summary>
public partial class ChoicesPanel : Panel
{
    private VBoxContainer? _choicesContainer;
    
    [Signal]
    public delegate void ChoiceSelectedEventHandler(int target, int choiceNumber);
    
    public override void _Ready()
    {
        _choicesContainer = GetNode<VBoxContainer>("%ChoicesContainer");
    }
    
    public void SetupChoices(List<Choice> choices, Func<string, string> getLocalizedText)
    {
        // Clear existing choice elements
        ClearChoices();
        
        // Create Button for each choice with proper text wrapping
        for (int i = 0; i < choices.Count; i++)
        {
            var choice = choices[i];
            
            var choiceButton = new Button
            {
                Text = $"{i + 1}. {getLocalizedText(choice.TextKey)}",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
                AutowrapMode = TextServer.AutowrapMode.WordSmart,
                CustomMinimumSize = new Vector2(0, 40),
                ClipContents = false
            };
            
            // Store choice data for the event
            var target = choice.Target;
            var choiceNumber = i + 1;
            
            // Connect button press signal
            choiceButton.Pressed += () => OnChoicePressed(target, choiceNumber);
            
            _choicesContainer!.AddChild(choiceButton);
        }
    }
    
    public void ClearChoices()
    {
        if (_choicesContainer == null) return;
        
        // Keep title and separator, remove only choice buttons
        var childCount = _choicesContainer.GetChildCount();
        for (int i = childCount - 1; i >= 2; i--) // Keep first 2 children (title + separator)
        {
            _choicesContainer.GetChild(i).QueueFree();
        }
    }
    
    private void OnChoicePressed(int target, int choiceNumber)
    {
        EmitSignal(SignalName.ChoiceSelected, target, choiceNumber);
    }
}