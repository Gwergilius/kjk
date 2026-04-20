using System.Collections.Generic;

namespace TalismanOfDeath.Data;

/// <summary>
/// Data for a single gamebook section/paragraph
/// </summary>
public class SectionData
{
    /// <summary>
    /// The localization key for this section's text
    /// </summary>
    public string TextKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional additional text key (for complex sections)
    /// </summary>
    public string AdditionalTextKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Arguments for formatted additional text
    /// </summary>
    public object[]? AdditionalTextArgs { get; set; }

    /// <summary>
    /// List of available choices for this section
    /// </summary>
    public List<Choice> Choices { get; set; } = new();
}

/// <summary>
/// Data for a player choice option
/// </summary>
public class Choice
{
    /// <summary>
    /// The localization key for this choice button text
    /// </summary>
    public string TextKey { get; set; } = string.Empty;

    /// <summary>
    /// Target section number this choice leads to
    /// </summary>
    public int Target { get; set; } = 1;
}