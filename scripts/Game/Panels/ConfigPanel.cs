using Godot;

namespace TalismanOfDeath.Game.Panels;

public partial class ConfigPanel : PanelContainer
{
    private OptionButton? _languageButton;
    private Button? _saveButton;
    private Button? _loadButton;
    private HSlider? _fontSizeSlider;
    private Label? _fontSizeValue;
    private Button? _closeButton;

    [Signal] public delegate void LanguageSelectedEventHandler(int index);
    [Signal] public delegate void SaveRequestedEventHandler();
    [Signal] public delegate void LoadRequestedEventHandler();
    [Signal] public delegate void FontSizeChangedEventHandler(int size);

    public override void _Ready()
    {
        _languageButton = GetNode<OptionButton>("%LanguageButton");
        _saveButton = GetNode<Button>("%SaveButton");
        _loadButton = GetNode<Button>("%LoadButton");
        _fontSizeSlider = GetNode<HSlider>("%FontSizeSlider");
        _fontSizeValue = GetNode<Label>("%FontSizeValue");
        _closeButton = GetNode<Button>("%CloseButton");

        _languageButton.ItemSelected += index => EmitSignal(SignalName.LanguageSelected, (int)index);
        _saveButton.Pressed += () => EmitSignal(SignalName.SaveRequested);
        _loadButton.Pressed += () => EmitSignal(SignalName.LoadRequested);
        _closeButton.Pressed += () => Visible = false;
        _fontSizeSlider.ValueChanged += OnSliderChanged;
    }

    public void Initialize(string[] localeDisplayNames, int selectedIndex, int currentFontSize)
    {
        foreach (var name in localeDisplayNames)
            _languageButton!.AddItem(name);
        _languageButton!.Selected = selectedIndex;

        _fontSizeSlider!.Value = currentFontSize;
        _fontSizeValue!.Text = currentFontSize.ToString();
    }

    public void SetLanguage(int index)
    {
        if (_languageButton != null)
            _languageButton.Selected = index;
    }

    public void Toggle() => Visible = !Visible;

    private void OnSliderChanged(double value)
    {
        var size = (int)value;
        _fontSizeValue!.Text = size.ToString();
        EmitSignal(SignalName.FontSizeChanged, size);
    }
}
