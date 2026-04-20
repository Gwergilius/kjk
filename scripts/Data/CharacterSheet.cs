using Godot;
using System;
using System.Collections.Generic;

namespace TalismanOfDeath.Data;

/// <summary>
/// Represents a Fighting Fantasy character sheet with all stats and mechanics
/// </summary>
public partial class CharacterSheet : GodotObject
{
    #region Stats Properties
    
    /// <summary>
    /// Character's skill value (combat effectiveness)
    /// </summary>
    public int Skill { get; private set; }
    
    /// <summary>
    /// Character's initial maximum skill (never changes)
    /// </summary>
    public int InitialSkill { get; private set; }
    
    /// <summary>
    /// Character's current stamina (health points)
    /// </summary>
    public int Stamina { get; private set; }
    
    /// <summary>
    /// Character's initial maximum stamina
    /// </summary>
    public int InitialStamina { get; private set; }
    
    /// <summary>
    /// Character's luck value
    /// </summary>
    public int Luck { get; private set; }
    
    /// <summary>
    /// Character's initial maximum luck
    /// </summary>
    public int InitialLuck { get; private set; }
    
    /// <summary>
    /// Character's gold pieces
    /// </summary>
    public int Gold { get; private set; }
    
    /// <summary>
    /// List of character's equipment and items
    /// </summary>
    public List<string> Inventory { get; private set; }
    
    #endregion

    #region Signals
    
    [Signal]
    public delegate void StatsChangedEventHandler();
    
    [Signal]
    public delegate void InventoryChangedEventHandler();
    
    [Signal] 
    public delegate void DiceRolledEventHandler(int die1, int die2, int total);
    
    #endregion

    #region Constructor
    
    public CharacterSheet()
    {
        Inventory = new List<string>();
        // Start with basic equipment
        Inventory.Add("Sword");
        Inventory.Add("Leather Armour");
        Inventory.Add("10 Provisions");
        
        // Initialize with default stats - will be properly rolled later
        SetStats(10, 16, 8, 25);
    }
    
    #endregion

    #region Stat Management
    
    /// <summary>
    /// Roll initial stats using Fighting Fantasy dice rules
    /// </summary>
    public void RollInitialStats()
    {
        // Roll Skill: 6 + die
        var skillRoll = RollDice();
        var skill = 6 + skillRoll;
        
        // Roll Stamina: 12 + two dice
        var staminaRoll1 = RollDice();
        var staminaRoll2 = RollDice();
        var stamina = 12 + staminaRoll1 + staminaRoll2;
        
        // Roll Luck: 6 + die  
        var luckRoll = RollDice();
        var luck = 6 + luckRoll;
        
        // Starting gold
        var gold = 25;
        
        SetStats(skill, stamina, luck, gold);
        
        GD.Print($"Initial stats rolled - Skill: {Skill}, Stamina: {Stamina}, Luck: {Luck}, Gold: {Gold}");
    }
    
    /// <summary>
    /// Set character stats (used for loading saved games)
    /// </summary>
    public void SetStats(int skill, int stamina, int luck, int gold)
    {
        Skill = skill;
        InitialSkill = skill;
        Stamina = stamina;
        InitialStamina = stamina;
        Luck = luck;
        InitialLuck = luck;
        Gold = gold;
        
        EmitSignal(SignalName.StatsChanged);
    }
    
    /// <summary>
    /// Modify current stats (for gameplay effects)
    /// </summary>
    public void ModifyStats(int skillChange = 0, int staminaChange = 0, int luckChange = 0, int goldChange = 0)
    {
        Skill = Math.Max(0, Math.Min(InitialSkill, Skill + skillChange));
        Stamina = Math.Max(0, Math.Min(InitialStamina, Stamina + staminaChange));
        Luck = Math.Max(0, Math.Min(InitialLuck, Luck + luckChange));
        Gold = Math.Max(0, Gold + goldChange);
        
        EmitSignal(SignalName.StatsChanged);
    }
    
    /// <summary>
    /// Restore stat to initial value (drinking potion, etc.)
    /// </summary>
    public void RestoreToInitial(bool skill = false, bool stamina = false, bool luck = false)
    {
        if (skill) Skill = InitialSkill;
        if (stamina) Stamina = InitialStamina;
        if (luck) Luck = InitialLuck;
        
        if (skill || stamina || luck)
            EmitSignal(SignalName.StatsChanged);
    }
    
    #endregion

    #region Dice Rolling
    
    /// <summary>
    /// Roll a single d6 die
    /// </summary>
    public int RollDice()
    {
        return GD.RandRange(1, 6);
    }
    
    /// <summary>
    /// Roll two dice and return individual results plus total
    /// </summary>
    public (int die1, int die2, int total) RollTwoDice()
    {
        var die1 = RollDice();
        var die2 = RollDice();
        var total = die1 + die2;
        
        EmitSignal(SignalName.DiceRolled, die1, die2, total);
        
        return (die1, die2, total);
    }
    
    /// <summary>
    /// Test Your Luck mechanic - roll two dice vs current Luck
    /// </summary>
    public bool TestYourLuck()
    {
        var (die1, die2, total) = RollTwoDice();
        var lucky = total <= Luck;
        
        // Always lose 1 Luck point when testing
        ModifyStats(luckChange: -1);
        
        GD.Print($"Test Your Luck: Rolled {total} vs Luck {Luck + 1} - {(lucky ? "LUCKY!" : "UNLUCKY!")}");
        
        return lucky;
    }
    
    #endregion

    #region Inventory Management
    
    /// <summary>
    /// Add item to inventory
    /// </summary>
    public void AddItem(string item)
    {
        if (!string.IsNullOrWhiteSpace(item))
        {
            Inventory.Add(item);
            EmitSignal(SignalName.InventoryChanged);
        }
    }
    
    /// <summary>
    /// Remove item from inventory
    /// </summary>
    public bool RemoveItem(string item)
    {
        if (Inventory.Remove(item))
        {
            EmitSignal(SignalName.InventoryChanged);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Check if character has specific item
    /// </summary>
    public bool HasItem(string item)
    {
        return Inventory.Contains(item);
    }
    
    /// <summary>
    /// Use provisions to restore stamina
    /// </summary>
    public bool EatProvisions()
    {
        // Check if player has provisions
        var provisionsItem = Inventory.Find(item => item.Contains("Provisions"));
        if (provisionsItem == null) return false;
        
        // Parse number of provisions (e.g., "5 Provisions")
        var parts = provisionsItem.Split(' ');
        if (parts.Length >= 2 && int.TryParse(parts[0], out int count))
        {
            if (count > 1)
            {
                // Reduce provision count
                Inventory.Remove(provisionsItem);
                Inventory.Add($"{count - 1} Provisions");
            }
            else
            {
                // Remove last provision
                Inventory.Remove(provisionsItem);
            }
            
            // Restore 4 stamina points
            ModifyStats(staminaChange: 4);
            EmitSignal(SignalName.InventoryChanged);
            
            GD.Print($"Ate provisions - restored 4 Stamina. Current: {Stamina}");
            return true;
        }
        
        return false;
    }
    
    #endregion

    #region Save/Load
    
    /// <summary>
    /// Export character data for saving
    /// </summary>
    public Godot.Collections.Dictionary<string, Variant> SaveCharacterData()
    {
        return new Godot.Collections.Dictionary<string, Variant>
        {
            ["Skill"] = Skill,
            ["InitialSkill"] = InitialSkill,
            ["Stamina"] = Stamina,
            ["InitialStamina"] = InitialStamina,
            ["Luck"] = Luck,
            ["InitialLuck"] = InitialLuck,
            ["Gold"] = Gold,
            ["Inventory"] = new Godot.Collections.Array<string>(Inventory)
        };
    }
    
    /// <summary>
    /// Load character data from save
    /// </summary>
    public void LoadCharacterData(Godot.Collections.Dictionary<string, Variant> data)
    {
        if (data.ContainsKey("Skill")) Skill = data["Skill"].AsInt32();
        if (data.ContainsKey("InitialSkill")) InitialSkill = data["InitialSkill"].AsInt32();
        if (data.ContainsKey("Stamina")) Stamina = data["Stamina"].AsInt32();
        if (data.ContainsKey("InitialStamina")) InitialStamina = data["InitialStamina"].AsInt32();
        if (data.ContainsKey("Luck")) Luck = data["Luck"].AsInt32();
        if (data.ContainsKey("InitialLuck")) InitialLuck = data["InitialLuck"].AsInt32();
        if (data.ContainsKey("Gold")) Gold = data["Gold"].AsInt32();
        
        if (data.ContainsKey("Inventory"))
        {
            var invArray = data["Inventory"].AsGodotArray<string>();
            Inventory.Clear();
            foreach (var item in invArray)
            {
                Inventory.Add(item);
            }
        }
        
        EmitSignal(SignalName.StatsChanged);
        EmitSignal(SignalName.InventoryChanged);
    }
    
    #endregion
}