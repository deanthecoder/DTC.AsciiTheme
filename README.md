[![Twitter URL](https://img.shields.io/twitter/url/https/twitter.com/deanthecoder.svg?style=social&label=Follow%20%40deanthecoder)](https://twitter.com/deanthecoder)

# DTC.AsciiTheme
`DTC.AsciiTheme` is primarily a style/theme layer for normal Avalonia controls, so it fits into a regular Avalonia app with very little learning curve. Most of what you use here is still `Button`, `TextBox`, `ComboBox`, `TreeView`, `DataGrid`, and the rest of the standard control set, just re-skinned to feel like classic DOS utilities, BBS tools, setup screens, and text-mode file managers.

Alongside that, the package also includes a small number of extra helper controls for a more authentic retro look where Avalonia does not provide the exact shape out of the box, such as `AsciiGroupBox`, `AsciiStatusBar`, and `AsciiMessageBox`.

The goal is not terminal emulation, but a practical control theme that keeps modern bindings, layout, focus, and input behavior intact.

![Data tab screenshot](img/data.png)

## Demo tabs
### Buttons
Demonstrates `Button` and `ToggleButton`, including hover, focus, disabled, and latched states using the VGA font and retro shadow treatment.

![Buttons tab screenshot](img/buttons.png)

### Inputs
Demonstrates single-line `TextBox`, password entry, a multiline editor with themed scrollbars, `ComboBox`, `CheckBox`, and `RadioButton`.

![Inputs tab screenshot](img/inputs.png)

### Text
Demonstrates `TextBlock`, `Label`, and `Separator`, including wrapped copy, headings, and form-style label targeting.

![Text tab screenshot](img/text.png)

### Lists
Demonstrates `ListBox` together with the custom `AsciiGroupBox` framing and selection styling.

![Lists tab screenshot](img/lists.png)

### Data
Demonstrates a first-pass `DataGrid` with ASCII-styled headers, sort arrows, row selection, grid lines, and shared scrollbar treatment.

![Data tab screenshot](img/data.png)

### Tree
Demonstrates `TreeView`, ASCII expand/collapse arrows, selection, and nested hierarchy rendering.

![Tree tab screenshot](img/tree.png)

### Progress
Demonstrates horizontal and vertical `ProgressBar`, including animated bars and inline progress text.

![Progress tab screenshot](img/progress.png)

### ScrollViewer

Demonstrates shared `ScrollViewer`/`ScrollBar` styling with a dithered retro image preview.

![ScrollViewer tab screenshot](img/scrollviewer.png)

### More
Demonstrates `AsciiMessageBox`, `Expander`, `Slider`, `ToolTip`, and the footer `AsciiStatusBar`. The demo app itself now uses the themed `Menu` at the top level for `File`, `View`, and `Help`.

![More tab screenshot](img/more.png)

## What it currently styles
### Standard Avalonia controls
- `Button`
- `CheckBox`
- `ComboBox`
- `DataGrid`
- `Expander`
- `Label`
- `ListBox`
- `Menu`
- `ProgressBar`
- `RadioButton`
- `ScrollBar`
- `ScrollViewer`
- `Separator`
- `Slider`
- `TabControl`
- `TextBlock`
- `TextBox`
- `ToggleButton`
- `ToolTip`
- `TreeView`

### Extra retro helper controls
- `AsciiGroupBox`
- `AsciiMessageBox`
- `AsciiStatusBar`

## Build and run
Prereqs: .NET 8 SDK.

```bash
dotnet build DTC.AsciiTheme.sln
dotnet run --project samples/DTC.AsciiTheme.Demo/DTC.AsciiTheme.Demo.csproj
```

## Use in your app
Reference `DTC.AsciiTheme` from your Avalonia app, then load the theme in `App.axaml`:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:fluent="clr-namespace:Avalonia.Themes.Fluent;assembly=Avalonia.Themes.Fluent"
             xmlns:ascii="using:DTC.AsciiTheme"
             x:Class="YourApp.App">
    <Application.Styles>
        <fluent:FluentTheme />
        <ascii:AsciiTheme />
    </Application.Styles>
</Application>
```

That matches the demo app and gives you the normal Avalonia Fluent baseline plus the retro overrides from this theme package.

## Optional palette overlays
The base theme ships with a blue DOS-style palette by default, and also includes optional palette overlays you can load after `AsciiTheme`:

- `avares://DTC.AsciiTheme/Palettes/Blue.axaml`
- `avares://DTC.AsciiTheme/Palettes/Mono.axaml`
- `avares://DTC.AsciiTheme/Palettes/Green.axaml`
- `avares://DTC.AsciiTheme/Palettes/Plasma.axaml`
- `avares://DTC.AsciiTheme/Palettes/Grey.axaml`

Example:

```xml
<Application.Styles>
    <fluent:FluentTheme />
    <ascii:AsciiTheme />
    <StyleInclude Source="avares://DTC.AsciiTheme/Palettes/Green.axaml" />
</Application.Styles>
```

If you want to switch palettes at runtime from code, use `AsciiPaletteManager.Apply(...)`.

## Palette gallery
The screenshots below all use the same `Text` tab content so the palette differences are easy to compare:

### Blue

![Blue palette screenshot](img/text-blue.png)

### Mono

![Mono palette screenshot](img/text-mono.png)

### Green

![Green palette screenshot](img/text-green.png)

### Plasma

![Plasma palette screenshot](img/text-plasma.png)

### Grey

Simple light-grey retro palette with black text and strong inverse/highlight states.

![Grey palette screenshot](img/text-grey.png)

## Project layout
- `src/DTC.AsciiTheme/` contains the reusable theme library.
- `samples/DTC.AsciiTheme.Demo/` contains the showcase app used for interactive visual checks.
- `tests/DTC.AsciiTheme.Tests/` contains the automated screenshot-generation test.
- `img/` contains the generated demo screenshots used by this README.
