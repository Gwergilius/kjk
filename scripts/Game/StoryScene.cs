using Godot;
using TalismanOfDeath.Data;
using TalismanOfDeath.Game.Panels;

namespace TalismanOfDeath.Game;

public partial class StoryScene : Control
{
    [Signal] public delegate void NextPressedEventHandler(string target);

    private ImagePanel? _imagePanel;
    private StoryPanel? _storyPanel;
    private Button? _nextButton;
    private string _nextTarget = "";
    private bool _heightUpdatePending;

    public override void _Ready()
    {
        _imagePanel = GetNode<ImagePanel>("%ImagePanel");
        _storyPanel = GetNode<StoryPanel>("%StoryPanel");
        _nextButton = GetNode<Button>("%NextButton");
        _nextButton!.Pressed += OnNextPressed;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized)
            ScheduleStoryHeightUpdate();
    }

    public void DisplaySection(SectionData section, Texture2D? image)
    {
        _imagePanel!.ShowImage(image);
        _storyPanel!.UpdateStoryText(section.Text);

        var first = section.Choices.Count > 0 ? section.Choices[0] : null;
        _nextTarget = first?.Target ?? "";
        _nextButton!.Text = first?.Text ?? "Next >";
        _nextButton.Visible = first != null;

        CallDeferred(nameof(UpdateStoryHeight));
    }

    private void ScheduleStoryHeightUpdate()
    {
        if (_heightUpdatePending) return;
        _heightUpdatePending = true;
        CallDeferred(nameof(UpdateStoryHeight));
    }

    private void UpdateStoryHeight()
    {
        _heightUpdatePending = false;
        if (_storyPanel == null) return;
        var maxH = Size.Y / 3f;
        var contentH = (float)_storyPanel.GetContentHeight();
        _storyPanel.CustomMinimumSize = new Vector2(0, Mathf.Min(contentH, maxH));
    }

    private void OnNextPressed()
    {
        if (!string.IsNullOrEmpty(_nextTarget))
            EmitSignal(SignalName.NextPressed, _nextTarget);
    }
}
