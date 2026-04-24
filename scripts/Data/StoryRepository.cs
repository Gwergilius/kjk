using Godot;
using System.Collections.Generic;
using System.Linq;
using Gwergilius.Common.Extensions;
using TalismanOfDeath.Content;

namespace TalismanOfDeath.Data;

public class StoryRepository
{
    private readonly Dictionary<(string id, string lang), SectionData> _sections = new();
    public string StartSection { get; private set; } = "1";

    public void LoadAll(string[] languages)
    {
        foreach (var lang in languages)
            LoadLanguage(lang);
    }

    private void LoadLanguage(string lang)
    {
        var result = typeof(ContentAssembly).GetYamlResource<GameFile>($"data/{lang}.yaml");
        if (!result.IsSuccess)
        {
            GD.PrintErr($"StoryRepository: {string.Join("; ", result.Errors)}");
            return;
        }

        var file = result.Value.FirstOrDefault();
        if (file == null) return;

        if (lang == "en" && !string.IsNullOrEmpty(file.Start))
            StartSection = file.Start;

        int count = 0;
        foreach (var entry in file.Sections)
        {
            var choices = new List<Choice>();
            foreach (var kvp in entry.Choices)
                choices.Add(new Choice { Text = kvp.Value, Target = kvp.Key });

            _sections[(entry.Id, lang)] = new SectionData
            {
                Id = entry.Id,
                Type = ParseSceneType(entry.Type),
                Image = entry.Image,
                Text = entry.Text,
                Choices = choices
            };
            count++;
        }

        GD.Print($"StoryRepository: loaded {count} sections for lang={lang}");
    }

    public SectionData? GetSection(string id, string lang)
    {
        if (_sections.TryGetValue((id, lang), out var section))
            return section;
        if (lang != "en" && _sections.TryGetValue((id, "en"), out var fallback))
            return fallback;
        return null;
    }

    private static SceneType ParseSceneType(string type) =>
        type.ToLowerInvariant() switch
        {
            "backstory" => SceneType.Backstory,
            "story" => SceneType.Story,
            _ => SceneType.Location
        };

    private class GameFile
    {
        public string Start { get; set; } = string.Empty;
        public List<SceneEntry> Sections { get; set; } = new();
    }

    private class SceneEntry
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "location";
        public string? Image { get; set; }
        public string Text { get; set; } = "";
        public Dictionary<string, string> Choices { get; set; } = new();
    }
}
