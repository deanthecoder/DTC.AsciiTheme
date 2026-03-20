# Agents.md — Guidance for AI-assisted Development

_An overview of the current repository and its related goals, intended to guide AI-assisted development._

## Summary for Agents

This repository contains an **Avalonia theme / control styling project** that aims to make modern desktop UI look like a classic old-school ASCII / text-mode interface.

The target feel is inspired by vintage DOS applications, BBS tools, setup screens, Norton-style interfaces, ANSI text UIs, and similar retro software.  
This is **not** intended to be a terminal emulator, nor a fake screenshot renderer. It should be a real Avalonia UI theme that can be applied to normal Avalonia controls.

The preferred implementation approach is to **re-theme and re-template Avalonia’s built-in controls first**, only introducing custom controls where absolutely necessary.

---

## Getting Started for Agents

When beginning work on this repository:

1. Ensure the solution builds successfully.
2. If no demo app exists, create a minimal Avalonia application to preview the theme.
3. Apply the theme globally in the demo app.
4. Start by implementing a styled `Button` control to establish the visual direction.
5. Use hardcoded values initially if needed to validate appearance quickly.
6. Iterate toward reusable styles and resources after visual validation.

Do not attempt to fully implement all controls at once.  
Focus on one control at a time and ensure it looks correct before expanding.

---

## Definition of Done (Initial Version)

The first usable version of this project should include:

- A working Avalonia theme package (`DTC.AsciiTheme`).
- A demo application showcasing the theme.
- Styled versions of:
  - Button
  - TextBox
  - CheckBox
  - RadioButton
- A consistent retro ASCII visual style across these controls.
- The ability to apply the theme to an Avalonia app with minimal setup.

Visual accuracy and consistency are more important than completeness.

---

## Agent Instructions (Overview)

- Read this file before making architectural decisions.
- Preserve the retro ASCII / text-mode design goal in all UI choices.
- Prefer **restyling existing Avalonia controls** over inventing replacements.
- Keep the design practical and reusable: this should be a usable Avalonia theme, not just a visual experiment.
- Avoid unnecessary complexity in the first versions.
- Match existing coding style and naming conventions used in the repository.
- If generic reusable code emerges, consider whether it belongs in a shared library rather than staying local to this repo.
- Favour incremental delivery: get a convincing v1 working for core controls before expanding scope.
- Unit test logic where sensible. Visual/theme work may also benefit from a demo app or preview surface.
- Do not introduce heavy dependencies without clear value.

---

## Project Goals

The project should provide a reusable Avalonia theme that gives applications a convincing retro ASCII UI appearance.

### Core visual goals

- Monospaced retro-style font.
- Text-mode inspired layout and spacing.
- Box-drawing style borders.
- Flat fills.
- Sharp contrast.
- No modern rounded corners or soft gradients.
- Clear focus states.
- Selection states that resemble inverse video or old UI highlighting.
- Controls that feel like they belong in a DOS-era interface.

### Non-goals

Unless explicitly requested, this project is **not** trying to:

- emulate an entire terminal environment.
- render the UI into a character grid.
- replace Avalonia with a custom scene renderer.
- perfectly clone any specific third-party project.
- mimic CRT distortion, scanlines, or shader-heavy effects by default.
- build a complete window manager.

---

## Implementation Strategy

### Preferred approach

Start by styling and templating standard Avalonia controls.

Examples:

- `Button`
- `ToggleButton`
- `CheckBox`
- `RadioButton`
- `TextBox`
- `ComboBox`
- `ListBox`
- `ScrollBar`
- `Menu`
- `TabControl`
- `GroupBox`
- `TreeView`
- `DataGrid` later, if included

Use Avalonia theming primitives wherever possible:

- `ControlTheme`
- control templates
- styles
- theme resources
- brushes
- shared sizing resources
- reusable glyph/border helpers where appropriate

### Custom controls

Create custom controls only when one of the following is true:

- Avalonia templating cannot achieve the required look cleanly.
- The retro styling would otherwise become fragile or overly hacky.
- The behaviour is meaningfully different from the standard control.

Any custom control should have a clear justification.

---

## Design Principles

### 1. Real controls first

This should remain a real Avalonia UI toolkit/theme.  
Bindings, focus, keyboard navigation, templating, input behaviour, and layout should continue to work normally.

### 2. Retro feel over literal emulation

Aim for a convincing retro experience without making the codebase brittle.  
The result should feel authentic, even if some implementation details are modern.

### 3. Font matters

The chosen font is a major part of the visual identity.

---

## Font Guidance

The theme depends heavily on a suitable monospaced font.

Requirements:

- Must support box-drawing characters.
- Must render cleanly at small sizes.
- Should resemble classic IBM/DOS-era fonts.

Prefer reusing an existing font from the author's other projects (e.g. G33kShell) if available.

Agents should ensure the font is:

- included in the project,
- properly referenced in Avalonia,
- and applied globally via theme resources.

---

### 4. Keep layout disciplined

The UI will look best when spacing, sizes, borders, and glyph placement are consistent.  
Avoid arbitrary padding and modern default spacing values that weaken the retro look.

### 5. Minimal palette

Prefer a limited and deliberate palette.

Examples may include:

- dark background + bright foreground
- classic blue / cyan / white combinations
- amber / green monochrome variants later
- high-contrast focus and selection states

---

## Suggested Project Structure

- `src/`
  - theme library (`DTC.AsciiTheme`)
  - optional helper controls
  - shared resources
- `samples/` or `demo/`
  - showcase app demonstrating all styled controls
- `tests/`
  - unit tests for logic, helpers, and any custom control behaviour
- `assets/`
  - fonts
  - optional screenshots / preview media

---

## Likely Components

### Theme resources

Shared resources for:

- font family
- font size
- spacing
- border thickness
- brushes
- colour palette
- focus visuals
- selection visuals

### Border and glyph helpers

Potential helpers for:

- box-drawing glyph usage
- directional arrows
- checkbox / radio indicator glyphs
- text-mode separators
- decorative title bars or frames

### Demo application

A sample app is strongly recommended.

It should demonstrate:

- all supported controls
- enabled / disabled / focused / hovered / selected states
- alternative palettes if supported

---

## Recommended Milestones

### Milestone 1 — Core look and feel

- font
- palette
- basic spacing
- borders
- focus visuals

Then implement:

- `Button`
- `TextBox`
- `CheckBox`
- `RadioButton`

### Milestone 2 — Common app controls

- `ListBox`
- `ComboBox`
- `ScrollBar`
- `Menu`
- `GroupBox`

### Milestone 3 — More complex containers

- `TabControl`
- `TreeView`
- optional `Expander`

### Milestone 4 — Advanced controls

- `DataGrid`
- dialog styling
- optional variants/themes

---

## Coding Preferences

- use `var` where type is obvious.
- avoid single-line `if` statements.
- use `string.Empty` instead of `""`.
- sentence-style `//` comments with full stops.
- prefer `FileInfo` / `DirectoryInfo` over raw strings where sensible.
- keep methods small and focused.

---

## Testing Guidance

Good candidates for tests:

- glyph helper logic
- resource lookup helpers
- theme registration logic
- custom control behaviour

Visual validation via demo app is important.

---

## Relationship to Other Projects

Relevant references:

- **G33kShell** (font + visual inspiration)
- **DTC.Core** (shared utilities where applicable)

Reuse existing code where appropriate.

---

## Common Mistakes to Avoid

- Do not replace Avalonia controls with custom rendering prematurely.
- Do not attempt pixel-perfect DOS emulation at the cost of maintainability.
- Do not introduce unnecessary dependencies.
- Do not style all controls at once.
- Do not rely on rounded corners or gradients.
- Avoid inconsistent spacing or mixed visual styles.

---

## Suggested First Task

Create a styled `Button` control that:

- Uses a monospaced font.
- Has a box-style border.
- Uses high-contrast colours.
- Clearly shows hover and focus states.
- Avoids rounded corners and gradients.

This control establishes the visual baseline.

---

## Current Intent

The immediate goal is to create an Avalonia theme (`DTC.AsciiTheme`) that looks convincingly like a classic ASCII UI, while behaving like a proper modern Avalonia theme.