using Godot;

namespace TalismanOfDeath.Game.Panels;

public partial class ImagePanel : Control
{
    private TextureRect? _sectionImage;
    private Button? _placeholder;

    [Signal] public delegate void ImageClickedEventHandler();

    public override void _Ready()
    {
        _sectionImage = GetNode<TextureRect>("%SectionImage");
        _placeholder = GetNode<Button>("%SectionImagePlaceholder");
        _placeholder.Pressed += () => EmitSignal(SignalName.ImageClicked);
    }

    public void ShowImage(Texture2D? texture)
    {
        _sectionImage!.Texture = texture;
        _sectionImage.Visible = texture != null;
        _placeholder!.Visible = texture == null;
    }
}
