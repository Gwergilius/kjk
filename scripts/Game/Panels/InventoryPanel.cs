using Godot;
using System.Collections.Generic;
using System.Linq;
using TalismanOfDeath.Data;

namespace TalismanOfDeath.Game.Panels;

/// <summary>
/// Handles inventory item display and management
/// </summary>
public partial class InventoryPanel : Panel
{
    private RichTextLabel? _itemsList;
    private CharacterSheet? _characterSheet;
    
    public override void _Ready()
    {
        _itemsList = GetNode<RichTextLabel>("%ItemsList");
    }
    
    /// <summary>
    /// Set the character sheet reference and connect to its signals
    /// </summary>
    public void SetCharacterSheet(CharacterSheet characterSheet)
    {
        // Disconnect from previous character sheet
        if (_characterSheet != null)
        {
            _characterSheet.InventoryChanged -= OnInventoryChanged;
        }
        
        _characterSheet = characterSheet;
        
        if (_characterSheet != null)
        {
            // Connect to character sheet signals
            _characterSheet.InventoryChanged += OnInventoryChanged;
            
            // Update display
            OnInventoryChanged();
        }
    }
    
    private void OnInventoryChanged()
    {
        if (_characterSheet == null || _itemsList == null) return;
        
        var inventoryText = "[b]INVENTORY[/b]\n";
        
        if (_characterSheet.Inventory.Count == 0)
        {
            inventoryText += "[color=gray]Empty[/color]";
        }
        else
        {
            // Group similar items
            var groupedItems = new Dictionary<string, int>();
            foreach (var item in _characterSheet.Inventory)
            {
                // Handle provision count parsing
                if (item.Contains("Provisions"))
                {
                    var parts = item.Split(' ');
                    if (parts.Length >= 2 && int.TryParse(parts[0], out int count))
                    {
                        if (groupedItems.ContainsKey("Provisions"))
                            groupedItems["Provisions"] += count;
                        else
                            groupedItems["Provisions"] = count;
                    }
                    else
                    {
                        groupedItems[item] = groupedItems.ContainsKey(item) ? groupedItems[item] + 1 : 1;
                    }
                }
                else
                {
                    groupedItems[item] = groupedItems.ContainsKey(item) ? groupedItems[item] + 1 : 1;
                }
            }
            
            foreach (var kvp in groupedItems)
            {
                if (kvp.Key == "Provisions")
                {
                    inventoryText += $"• {kvp.Value} {kvp.Key}\n";
                }
                else if (kvp.Value > 1)
                {
                    inventoryText += $"• {kvp.Key} x{kvp.Value}\n";
                }
                else
                {
                    inventoryText += $"• {kvp.Key}\n";
                }
            }
        }
        
        _itemsList.Text = inventoryText;
    }
    
    // Legacy methods for backwards compatibility
    public void AddItem(string itemName)
    {
        _characterSheet?.AddItem(itemName);
    }
    
    public void RemoveItem(string itemName)
    {
        _characterSheet?.RemoveItem(itemName);
    }
    
    public void SetItems(List<string> items)
    {
        if (_characterSheet == null) return;
        
        _characterSheet.Inventory.Clear();
        foreach (var item in items)
        {
            _characterSheet.AddItem(item);
        }
    }
    
    public void SetPlaceholderItems(string itemsText)
    {
        // This is now handled by the character sheet system
        // Update display automatically when character sheet is connected
        OnInventoryChanged();
    }
}