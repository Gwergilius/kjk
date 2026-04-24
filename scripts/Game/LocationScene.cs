using Godot;
using TalismanOfDeath.Data;
using TalismanOfDeath.Game.Panels;

namespace TalismanOfDeath.Game;

public partial class LocationScene : Control
{
    [Signal] public delegate void ChoiceSelectedEventHandler(string target, int choiceNumber);
    [Signal] public delegate void TestLuckRequestedEventHandler();
    [Signal] public delegate void RollStatsRequestedEventHandler();
    [Signal] public delegate void EatProvisionsRequestedEventHandler();

    private ImagePanel? _imagePanel;
    private StoryPanel? _storyPanel;
    private StatusPanel? _statusPanel;
    private InventoryPanel? _inventoryPanel;
    private ChoicesPanel? _choicesPanel;
    private bool _heightUpdatePending;

    public override void _Ready()
    {
        _imagePanel = GetNode<ImagePanel>("%ImagePanel");
        _storyPanel = GetNode<StoryPanel>("%StoryPanel");
        _statusPanel = GetNode<StatusPanel>("%StatusPanel");
        _inventoryPanel = GetNode<InventoryPanel>("%InventoryPanel");
        _choicesPanel = GetNode<ChoicesPanel>("%ChoicesPanel");

        _choicesPanel!.ChoiceSelected += (target, num) => EmitSignal(SignalName.ChoiceSelected, target, num);
        _statusPanel!.TestLuckRequested += () => EmitSignal(SignalName.TestLuckRequested);
        _statusPanel!.RollStatsRequested += () => EmitSignal(SignalName.RollStatsRequested);
        _statusPanel!.EatProvisionsRequested += () => EmitSignal(SignalName.EatProvisionsRequested);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized)
            ScheduleStoryHeightUpdate();
    }

    public void SetCharacterSheet(CharacterSheet cs)
    {
        _statusPanel!.SetCharacterSheet(cs);
        _inventoryPanel!.SetCharacterSheet(cs);
    }

    public void DisplaySection(SectionData section, Texture2D? image)
    {
        _imagePanel!.ShowImage(image);
        _storyPanel!.UpdateStoryText(section.Text);
        _choicesPanel!.SetupChoices(section.Choices);
        CallDeferred(nameof(UpdateStoryHeight));
    }

    public void ShowError(string errorText)
    {
        _storyPanel!.UpdateStoryText(errorText);
        _choicesPanel!.ClearChoices();
        _imagePanel!.ShowImage(null);
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
}
