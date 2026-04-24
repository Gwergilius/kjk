using Godot;
using System.Collections.Generic;
using TalismanOfDeath.Data;

namespace TalismanOfDeath.Game.Panels;

public partial class InventoryPanel : PanelContainer
{
    private RichTextLabel? _itemsList;
    private CharacterSheet? _characterSheet;

    public override void _Ready()
    {
        _itemsList = GetNode<RichTextLabel>("%ItemsList");
    }

    public override void _Notification(int what)
    {
        if (what == NotificationTranslationChanged)
            RefreshInventory();
    }

    public void SetCharacterSheet(CharacterSheet characterSheet)
    {
        if (_characterSheet != null)
            _characterSheet.InventoryChanged -= RefreshInventory;

        _characterSheet = characterSheet;
        _characterSheet.InventoryChanged += RefreshInventory;
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        if (_itemsList == null) return;

        if (_characterSheet == null || _characterSheet.Inventory.Count == 0)
        {
            _itemsList.Text = $"[color=gray]{Tr("INVENTORY_EMPTY")}[/color]";
            return;
        }

        var grouped = new Dictionary<string, int>();
        foreach (var item in _characterSheet.Inventory)
        {
            var parts = item.Split(' ');
            if (item.Contains("Provisions") && parts.Length >= 2 && int.TryParse(parts[0], out int count))
            {
                grouped["Provisions"] = grouped.GetValueOrDefault("Provisions") + count;
            }
            else
            {
                grouped[item] = grouped.GetValueOrDefault(item) + 1;
            }
        }

        var sb = new System.Text.StringBuilder();
        foreach (var (name, qty) in grouped)
        {
            if (name == "Provisions")
                sb.AppendLine($"• {qty} {name}");
            else if (qty > 1)
                sb.AppendLine($"• {name} x{qty}");
            else
                sb.AppendLine($"• {name}");
        }

        _itemsList.Text = sb.ToString().TrimEnd();
    }
}
