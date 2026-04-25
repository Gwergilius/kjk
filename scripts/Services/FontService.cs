using Godot;
using System.Collections.Generic;

namespace TalismanOfDeath.Services;

/// <summary>
/// Autoload singleton. Scenes register themselves via RegisterScene(root);
/// FontService then applies base+offset font sizes to every node tagged with
/// metadata "_font_offset" (integer), both immediately and on each BaseSize change.
/// </summary>
public partial class FontService : Node
{
    public static FontService Instance { get; private set; } = null!;

    [Signal] public delegate void FontSizeChangedEventHandler(int baseSize);

    public int BaseSize { get; private set; } = 16;

    private readonly List<Node> _registeredRoots = new();

    public override void _Ready() => Instance = this;

    public void SetBaseSize(int size)
    {
        BaseSize = size;
        _registeredRoots.RemoveAll(r => !GodotObject.IsInstanceValid(r));
        foreach (var root in _registeredRoots)
            ApplyToTree(root);
        EmitSignal(SignalName.FontSizeChanged, size);
    }

    /// <summary>
    /// Registers a scene root: applies current font sizes immediately,
    /// and re-applies automatically on every future BaseSize change.
    /// </summary>
    public void RegisterScene(Node root)
    {
        _registeredRoots.Add(root);
        ApplyToTree(root);
    }

    /// <summary>
    /// Applies font size to a single node based on its "_font_offset" metadata.
    /// Call this after dynamically adding a node that has the metadata set.
    /// </summary>
    public void ApplyToNode(Node node)
    {
        if (!node.HasMeta("_font_offset")) return;
        int size = BaseSize + node.GetMeta("_font_offset").AsInt32();

        switch (node)
        {
            case Label lbl:
                lbl.AddThemeFontSizeOverride("font_size", size);
                break;
            case RichTextLabel rtl:
                rtl.AddThemeFontSizeOverride("normal_font_size", size);
                break;
            case Button btn:
                btn.AddThemeFontSizeOverride("font_size", size);
                break;
        }
    }

    private void ApplyToTree(Node root)
    {
        ApplyToNode(root);
        foreach (var child in root.GetChildren())
            ApplyToTree(child);
    }
}
