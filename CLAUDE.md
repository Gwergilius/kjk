# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Talisman of Death** — A Fighting Fantasy gamebook interactive fiction game built with Godot 4.6 + C#.

Current phase: Prototype B+ (4 hardcoded sections), working toward a full XHTML-parsed book implementation.

## Running and Building

This is a pure Godot project — no Makefile, npm, or external build tools.

- **Run**: Open in Godot Editor, press F5 (or use Play button). Main scene: `res://scenes/Main.tscn`
- **Build C#**: Godot compiles automatically on scene load; or use Build in Godot's top bar
- **External IDE**: Open `TalismanOfDeath.slnx` in Visual Studio 2022 or VS Code with C# Dev Kit

No automated test suite exists yet.

## Architecture

### Panel-Based MVC

`Main.cs` is the central coordinator. It owns game state (`_currentSection`, `_characterSheet`) and wires together six independent panel modules via Godot signals:

```
Main (scripts/Game/Main.cs)
├── ChoicesPanel   — dynamic choice buttons; emits ChoiceSelected signal
├── StoryPanel     — RichTextLabel wrapper for section text
├── StatusPanel    — character stats display + luck/fight/provision buttons
├── InventoryPanel — item list display
├── ImagePanel     — section artwork
└── DiceRollDialog — modal dice roll result UI
```

Each panel has a `.tscn` + `.cs` pair in `scenes/panels/` and `scripts/Game/Panels/`. Panels communicate **only** through signals back to `Main`, which then calls other panels directly.

### Key Data Flow

```
Button press → Panel signal → Main handler → update state → refresh panels
```

Example: `ChoicesPanel.ChoiceSelected` → `Main.OnChoiceSelected()` → `DisplaySection(int)` → calls `StoryPanel`, `ChoicesPanel`, `ImagePanel` to update.

### Core Data Classes (`scripts/Data/`)

- **`CharacterSheet.cs`** — Fighting Fantasy stats (Skill, Stamina, Luck, Gold). Emits signals on stat changes. Encapsulates dice mechanics: `TestLuck()`, `RollInitialStats()`.
- **`GameData.cs`** — `SectionData` (text key + choices list) and `Choice` (localization key + target section int).

### Localization System

`LocalizationManager.cs` is a singleton (autoloaded). Text is embedded as dictionaries keyed by language code (`en`, `hu`). Key naming: `SECTION_1_TEXT`, `SECTION_1_CHOICE_1`, `UI_BUTTON_FIGHT`.

All nodes that need translation join the `"localized_nodes"` group and implement `_on_language_changed()`. Language auto-detects from OS locale on startup.

### Save/Load

Binary serialization via `FileAccess` to `user://talisman_save.dat`. Saves `_currentSection` + full `CharacterSheet` state.

## Display Configuration

1024×768, resizable canvas, GL Compatibility renderer (targets Windows, Web, Android, iOS).

## Documentation

- [docs/Setup.md](docs/Setup.md) — dev environment setup
- [docs/Localization.md](docs/Localization.md) — translation guide and key naming conventions
