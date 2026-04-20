using Godot;
using System;

namespace TalismanOfDeath.Game.Panels;

/// <summary>
/// Handles section image display and click events
/// </summary>
public partial class ImagePanel : Control
{
    private Button? _sectionImagePlaceholder;
    
    [Signal]
    public delegate void ImageClickedEventHandler();
    
    public override void _Ready()
    {
        _sectionImagePlaceholder = GetNode<Button>("%SectionImagePlaceholder");
        _sectionImagePlaceholder!.Pressed += OnImagePressed;
    }
    
    public void UpdateImage(string imageText)
    {
        if (_sectionImagePlaceholder != null)
        {
            _sectionImagePlaceholder.Text = imageText;
        }
    }
    
    private void OnImagePressed()
    {
        EmitSignal(SignalName.ImageClicked);
        GD.Print("Section image placeholder clicked");
    }
}