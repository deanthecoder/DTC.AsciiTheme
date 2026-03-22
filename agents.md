# Agents.md — Guidance for AI-assisted Development

_A short guide to the current repository goals and working conventions._

## Summary

This repository contains an **Avalonia theme / control styling project** that makes modern desktop UI feel like a classic old-school ASCII / text-mode application.

The target feel is inspired by DOS utilities, BBS tools, setup screens, Norton-style interfaces, and ANSI-era desktop software.

This project is **not** a terminal emulator and **not** a character-grid renderer. It is a real Avalonia theme that should be usable with normal Avalonia controls.

The preferred implementation approach is:

- re-theme and re-template Avalonia’s built-in controls first,
- keep the result practical and reusable,
- only introduce custom controls where Avalonia does not provide a suitable built-in option.

## Current state

The repository already contains:

- a reusable theme library: `DTC.AsciiTheme`,
- a demo app that previews the controls,
- a README with generated screenshots,
- an NUnit screenshot test project that captures demo tabs into `img/`.

There is already meaningful work in place for controls such as:

- `Button`
- `ToggleButton`
- `TextBox`
- `CheckBox`
- `RadioButton`
- `ComboBox`
- `ListBox`
- `TreeView`
- `ProgressBar`
- `ScrollBar`
- `Menu`
- `TabControl`
- `Expander`
- `Slider`
- `ToolTip`
- `Separator`
- `Label`
- `TextBlock`

Custom controls currently exist only where useful:

- `AsciiGroupBox`
- `AsciiStatusBar`

## Core design goals

- Monospaced retro-style font.
- Disciplined spacing, ideally aligned to the VGA-ish `8x16` feel.
- Sharp contrast and flat fills.
- Text-mode inspired borders and separators.
- Minimal palette.
- Selection and focus states that feel like inverse-video or old utility software.
- Controls that feel like they belong in one coherent DOS-era desktop application.

## Non-goals

Unless explicitly requested, this project is **not** trying to:

- emulate a full terminal,
- render the entire UI onto a character grid,
- clone a specific third-party toolkit pixel-for-pixel,
- add CRT effects, scanlines, or heavy shader treatments,
- replace Avalonia with custom drawing for everything.

## Working approach

When working in this repository:

1. Build the solution first.
2. Prefer improving one control or one small control family at a time.
3. Validate visually in the demo app.
4. Keep metrics, spacing, and colors reusable rather than scattering magic values.
5. Preserve the retro direction even when implementing modern Avalonia behavior.

Prefer these Avalonia primitives:

- `ControlTheme`
- control templates
- styles
- shared resources
- reusable brushes, thicknesses, and metrics

Create a custom control only when:

- Avalonia does not provide the control at all,
- templating the built-in control becomes too fragile,
- or the desired behavior is meaningfully different.

## Important implementation notes

### Fluent baseline

Right now `AsciiTheme` is an override layer, not a totally standalone base theme.

Apps should currently load:

- `FluentTheme`
- then `AsciiTheme`

Do not assume `AsciiTheme` can replace Fluent by itself unless that has been deliberately refactored.

### Font

The font is a core part of the identity.

Requirements:

- supports box-drawing characters,
- looks right at small sizes,
- feels IBM/DOS-like,
- is included and referenced correctly in Avalonia resources.

Prefer reusing the font approach already established in this repository and in related projects like `G33kShell`.

### Palette

Keep the palette limited and reusable.

Prefer:

- a small number of primary theme colors,
- shade overlays using transparent black or transparent white,
- fewer per-control color knobs.

The long-term direction should be a **simpler color mechanism**, where changing a small set of base colors still produces a good-looking theme everywhere.

## Next priorities

When looking for the next useful work, consider these areas first.

### 1. Missing common controls

Check whether any practical retro-app controls are still missing or under-developed, for example:

- `DataGrid`
- richer list or shell-style item views
- numeric entry helpers
- date/time style controls only if genuinely needed

### 2. Dialog story

Think about file and message interactions:

- file open dialog,
- file save dialog,
- message boxes,
- confirm / warning / error dialogs.

Be clear about whether these should be:

- styled custom in-app dialogs,
- wrappers/helpers around Avalonia dialogs,
- or separate sample components.

Native OS dialogs are acceptable when necessary, but if a retro in-app experience is desired, that likely means themed custom windows/components rather than trying to restyle platform-native dialogs.

### 3. Palette simplification

Reduce the number of special-case colors over time.

Prefer a theme model built from:

- base background,
- base foreground,
- accent color,
- highlight color,
- shade overlays.

### 4. Documentation and screenshots

Keep the README and screenshot test up to date when the demo changes materially.

The screenshot test should remain a practical tool for regenerating `img/`, even if it includes small workarounds for preview/headless quirks.

## Testing guidance

Good candidates for tests:

- screenshot generation,
- glyph helper logic,
- simple custom control behavior,
- any reusable logic added outside raw XAML styling.

Visual validation in the demo app remains important.

## Coding preferences

- use `var` where type is obvious,
- avoid single-line `if` statements,
- use `string.Empty` instead of `""`,
- use sentence-style `//` comments with full stops,
- prefer `FileInfo` / `DirectoryInfo` over raw strings where sensible,
- keep methods small and focused.

## Related references

Relevant nearby projects:

- **G33kShell** for font and visual inspiration.
- **DTC.Core** for broader code style and conventions.
- **ReviewG33k** for README and test-project shape.

## Common mistakes to avoid

- Do not replace standard Avalonia controls with custom rendering too early.
- Do not try to style every control at once.
- Do not add unnecessary dependencies.
- Do not drift into modern rounded, soft, or gradient-heavy visuals unless explicitly requested.
- Do not introduce inconsistent spacing or ad-hoc metrics.
- Do not overfit to Rider preview quirks if the real app and demo behavior are otherwise correct, unless those previews are important to the task.
