using System.Collections.Generic;

namespace TalismanOfDeath.Data;

public enum SceneType { Location, Backstory, Story }

public class SectionData
{
    public string Id { get; set; } = string.Empty;
    public SceneType Type { get; set; } = SceneType.Location;
    public string? Image { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<Choice> Choices { get; set; } = new();
}

public class Choice
{
    public string Text { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
}
