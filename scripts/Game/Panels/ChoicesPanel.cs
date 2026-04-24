using Godot;
using System.Collections.Generic;
using TalismanOfDeath.Data;

namespace TalismanOfDeath.Game.Panels;

public partial class ChoicesPanel : PanelContainer
{
    private VBoxContainer? _choicesContainer;

    [Signal] public delegate void ChoiceSelectedEventHandler(string target, int choiceNumber);

    public override void _Ready()
    {
        _choicesContainer = GetNode<VBoxContainer>("%ChoicesContainer");
    }

    public void SetupChoices(List<Choice> choices)
    {
        ClearChoices();

        for (int i = 0; i < choices.Count; i++)
        {
            var choice = choices[i];
            var button = new Button
            {
                Text = $"{i + 1}. {choice.Text}",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
                AutowrapMode = TextServer.AutowrapMode.WordSmart,
                CustomMinimumSize = new Vector2(0, 40),
                ClipContents = false
            };

            var target = choice.Target;
            var number = i + 1;
            button.Pressed += () => EmitSignal(SignalName.ChoiceSelected, target, number);

            _choicesContainer!.AddChild(button);
        }
    }

    public void ClearChoices()
    {
        if (_choicesContainer == null) return;
        for (int i = _choicesContainer.GetChildCount() - 1; i >= 2; i--)
            _choicesContainer.GetChild(i).QueueFree();
    }
}
