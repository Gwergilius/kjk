using System.Text.RegularExpressions;

namespace TalismanOfDeath.Services;

public static class MarkdownConverter
{
    private static readonly Regex H3 = new(@"^### (.+)$", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex H2 = new(@"^## (.+)$", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex H1 = new(@"^# (.+)$", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex HRule = new(@"^(---|___|(?<!\*)\*\*\*(?!\*))$", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex Bold = new(@"\*\*(.+?)\*\*", RegexOptions.Compiled);
    private static readonly Regex BoldUnderscore = new(@"__(.+?)__", RegexOptions.Compiled);
    private static readonly Regex Italic = new(@"\*(.+?)\*", RegexOptions.Compiled);
    private static readonly Regex ItalicUnderscore = new(@"_(.+?)_", RegexOptions.Compiled);
    private static readonly Regex Strike = new(@"~~(.+?)~~", RegexOptions.Compiled);
    private static readonly Regex Code = new(@"`(.+?)`", RegexOptions.Compiled);
    private static readonly Regex SingleNewline = new(@"\n(?!\n)", RegexOptions.Compiled);
    private static readonly Regex DialogueLine = new(@"--(?!-)\s*", RegexOptions.Compiled);
    private static readonly Regex ExcessNewlines = new(@"\n{3,}", RegexOptions.Compiled);

    /// <summary>
    /// Converts a subset of Markdown to Godot BBCode.
    /// Supported: # headings, **/__ bold, */_ italic, ~~ strikethrough, ` inline code, --- hr.
    /// </summary>
    public static string ToBBCode(string markdown)
    {
        if (string.IsNullOrEmpty(markdown)) return markdown;

        var text = markdown;

        // Headings (longest prefix first to avoid partial matches)
        text = H3.Replace(text, "[b]$1[/b]");
        text = H2.Replace(text, "[b]$1[/b]");
        text = H1.Replace(text, "[b]$1[/b]");

        // Horizontal rule
        text = HRule.Replace(text, "[hr]");

        // Bold before italic so ** is consumed before single *
        text = Bold.Replace(text, "[b]$1[/b]");
        text = BoldUnderscore.Replace(text, "[b]$1[/b]");

        text = Italic.Replace(text, "[i]$1[/i]");
        text = ItalicUnderscore.Replace(text, "[i]$1[/i]");

        text = Strike.Replace(text, "[s]$1[/s]");
        text = Code.Replace(text, "[code]$1[/code]");

        return text;
    }

    /// <summary>
    /// Expands single newlines to paragraph breaks, with special handling for dialogue lines.
    /// Lines prefixed with -- become a soft line break rendered with an em dash (—).
    /// Already-blank lines (\\n\\n) are left unchanged.
    /// </summary>
    public static string ExpandParagraphs(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        text = SingleNewline.Replace(text, "\n\n");
        text = DialogueLine.Replace(text, "\n— ");
        return ExcessNewlines.Replace(text, "\n\n");
    }

    /// <summary>
    /// Removes Markdown syntax and returns plain text, suitable for contexts that do not support BBCode.
    /// </summary>
    public static string StripMarkdown(string markdown)
    {
        if (string.IsNullOrEmpty(markdown)) return markdown;

        var text = markdown;

        text = H3.Replace(text, "$1");
        text = H2.Replace(text, "$1");
        text = H1.Replace(text, "$1");
        text = HRule.Replace(text, "");
        text = Bold.Replace(text, "$1");
        text = BoldUnderscore.Replace(text, "$1");
        text = Italic.Replace(text, "$1");
        text = ItalicUnderscore.Replace(text, "$1");
        text = Strike.Replace(text, "$1");
        text = Code.Replace(text, "$1");

        return text;
    }
}
