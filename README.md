# Nook

Nook is an experimental Windows desktop companion: a small floating island for AI-agent activity, permissions, and peripheral-battery notifications.

> **Status: early prototype.** The first build demonstrates the floating interface. AI agent integrations, USB receiver battery readings, and automatic permission workflows are **not implemented yet**.

## First prototype

- A compact, draggable, always-on-top floating island.
- A friendly oval character with a simple breathing animation.
- Expand/collapse an information panel.
- Right-click menu to expand, collapse, or exit.
- Native Windows app built with .NET 8 and WPF.

## Build

Requires Windows and the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```powershell
dotnet run --project src/Nook/Nook.csproj
```

For a standalone x64 build:

```powershell
dotnet publish src/Nook/Nook.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

GitHub Actions produces a **ZIP with the executable**, not an installer. Download it from the successful workflow run under **Artifacts**.

## Roadmap

1. Make the floating island stable, accessible, and visually polished.
2. Add a clear UI for agents and their status, with explicit permission requests. Integrations must use each provider's supported authorization mechanisms; a ChatGPT subscription is not a generic API key.
3. Discover connected peripherals and implement battery readings only where the device/protocol exposes them reliably. Show **unknown** rather than invent a percentage.
4. Add notifications, settings, localization, and an installer.

Nook is an independent project. This repository does not contain or claim rights to the Coucou source code. Dependencies and any third-party assets must be checked for compatible licenses before inclusion.
