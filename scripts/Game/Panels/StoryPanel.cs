using Godot;
using TalismanOfDeath.Services;

namespace TalismanOfDeath.Game.Panels;

public partial class StoryPanel : Control
{
    private RichTextLabel? _storyText;
    private ScrollContainer? _storyArea;

    private const int ScrollStep = 40;

    public override void _Ready()
    {
        _storyText = GetNode<RichTextLabel>("%StoryText");
        _storyArea = GetNode<ScrollContainer>("MarginContainer/StoryArea");
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (_storyArea == null) return;
        if (@event is not InputEventKey { Pressed: true } key) return;

        switch (key.Keycode)
        {
            case Key.Up:
                _storyArea.ScrollVertical -= ScrollStep;
                GetViewport().SetInputAsHandled();
                break;
            case Key.Down:
                _storyArea.ScrollVertical += ScrollStep;
                GetViewport().SetInputAsHandled();
                break;
            case Key.Pageup:
                _storyArea.ScrollVertical -= (int)_storyArea.Size.Y;
                GetViewport().SetInputAsHandled();
                break;
            case Key.Pagedown:
                _storyArea.ScrollVertical += (int)_storyArea.Size.Y;
                GetViewport().SetInputAsHandled();
                break;
        }
    }

    public void UpdateStoryText(string text)
    {
        if (_storyText != null)
        {
            _storyText.Text = MarkdownConverter.ToBBCode(MarkdownConverter.ExpandParagraphs(text));
            if (_storyArea != null)
                _storyArea.ScrollVertical = 0;
        }
    }

    public void ShowLoadingText(string loadingText)
    {
        if (_storyText != null)
        {
            _storyText.Text = loadingText;
        }
    }

    public int GetContentHeight() => _storyText?.GetContentHeight() ?? 0;
}