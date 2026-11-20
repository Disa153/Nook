# Nook

Nook is an independent experimental Windows desktop companion with a floating, friendly character.

## Current prototype (0.2 work in progress)

- Draggable oval floating island with soft styling and breathing animation.
- Click the character for different reactions.
- Double-click the island or use the arrow to expand/collapse it.
- Three panels: **Devices**, **Agents**, and **Settings**.
- Settings for reactions, animation and always-on-top (currently session-only).
- Device/agent panels are honest placeholders, **not yet live integrations**.

## Download and test on Windows

Open [GitHub Actions](https://github.com/Disa153/Nook/actions/workflows/windows.yml). Select a successful run and download the `Nook-Windows-x64` artifact. Inside the downloaded artifact ZIP there is another ZIP to extract; run `Nook.exe` from the extracted folder.

This is a portable preview, **not a Windows installer**. The build must succeed before the ZIP is available.

## Build yourself

Requires Windows and .NET 8 SDK:

```powershell
dotnet run --project src/Nook/Nook.csproj
```

## Planned

- Identify connected peripherals using supported Windows interfaces and/or documented vendor protocols. A 2.4 GHz USB receiver does not automatically expose battery percentages.
- Low battery notifications when reliable data is available.
- Proper saved settings, startup behavior, and a polished installer.
- Separate integrations for supported AI agents, with permission explanations and authorization supported by each provider.
- Additional languages and appearance themes.

Nook does not include or claim rights to Coucou source code. Any third-party code or assets must be reviewed for license compatibility.
