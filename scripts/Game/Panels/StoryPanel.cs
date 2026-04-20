using Godot;

namespace TalismanOfDeath.Game.Panels;

/// <summary>
/// Handles story text display and formatting
/// </summary>
public partial class StoryPanel : Control
{
    private RichTextLabel? _storyText;
    
    public override void _Ready()
    {
        _storyText = GetNode<RichTextLabel>("%StoryText");
    }
    
    public void UpdateStoryText(string text)
    {
        if (_storyText != null)
        {
            _storyText.Text = text;
        }
    }
    
    public void ShowLoadingText(string loadingText)
    {
        if (_storyText != null)
        {
            _storyText.Text = loadingText;
        }
    }
}