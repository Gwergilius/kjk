# Project Setup Guide

## 🔧 Opening the Godot Project

1. Download **Godot 4.3+** engine with .NET support
2. **Project Manager** → Import → Select `project.godot`
3. **On first open** Godot automatically generates C# project files

## 💻 C# Development Environment

### Visual Studio 2022
1. From Godot: **Project** → **Tools** → **C#** → **Create solution**
2. **Open in External Editor** → Visual Studio launches
3. Open the **TalismanOfDeath.slnx** file

### VS Code
1. Install **C# Dev Kit** extension  
2. Open workspace folder
3. Automatic **.slnx** file recognition

## 🏃‍♂️ Running the Project

1. **Godot Editor**: F5 or Play button
2. **Visual Studio**: Launch from Godot (cannot run directly from VS)

## 📂 Code Organization Rules

- **scripts/Game/**: Main game logic classes
- **scripts/UI/**: UI components and controllers  
- **scripts/Data/**: Data model classes and structures
- **Namespace convention**: `TalismanOfDeath.{FolderName}`

## 🚨 Important Notes

- **DO NOT** edit the `.godot/` folder - generated content
- **DO NOT** commit `bin/` and `obj/` folders
- **After C# file changes** Godot automatically recompiles