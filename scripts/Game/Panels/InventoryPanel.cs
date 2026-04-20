using Godot;
using System.Collections.Generic;
using System.Linq;

namespace TalismanOfDeath.Game.Panels;

/// <summary>
/// Handles inventory item display and management
/// </summary>
public partial class InventoryPanel : Panel
{
    private RichTextLabel? _itemsList;
    private List<string> _items = new();
    
    public override void _Ready()
    {
        _itemsList = GetNode<RichTextLabel>("%ItemsList");
        
        // Set placeholder items
        _items = new List<string> { "Sword", "Potion", "Provisions" };
        UpdateDisplay();
    }
    
    public void AddItem(string itemName)
    {
        if (!_items.Contains(itemName))
        {
            _items.Add(itemName);
            UpdateDisplay();
        }
    }
    
    public void RemoveItem(string itemName)
    {
        if (_items.Remove(itemName))
        {
            UpdateDisplay();
        }
    }
    
    public void SetItems(List<string> items)
    {
        _items = new List<string>(items);
        UpdateDisplay();
    }
    
    public void SetPlaceholderItems(string itemsText)
    {
        if (_itemsList != null)
        {
            _itemsList.Text = itemsText;
        }
    }
    
    private void UpdateDisplay()
    {
        if (_itemsList != null && _items.Count > 0)
        {
            var itemsText = string.Join("\n", _items.Select(item => $"• {item}"));
            _itemsList.Text = itemsText;
        }
        else if (_itemsList != null)
        {
            _itemsList.Text = "No items";
        }
    }
}