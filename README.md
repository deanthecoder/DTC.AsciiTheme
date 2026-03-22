[![Twitter URL](https://img.shields.io/twitter/url/https/twitter.com/deanthecoder.svg?style=social&label=Follow%20%40deanthecoder)](https://twitter.com/deanthecoder)

# DTC.AsciiTheme
DTC.AsciiTheme is a reusable Avalonia theme library for building apps that feel like classic DOS utilities, BBS tools, setup screens, and text-mode file managers, while still using real Avalonia controls underneath. The goal is not terminal emulation, but a practical control theme that keeps modern bindings, layout, focus, and input behavior intact.

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
Demonstrates `Menu`, `AsciiMessageBox`, `Expander`, `Slider`, `ToolTip`, and the footer `AsciiStatusBar`.

![More tab screenshot](img/more.png)

## What it currently styles
- `AsciiMessageBox`
- `AsciiGroupBox`
- `AsciiStatusBar`
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

## Project layout
- `src/DTC.AsciiTheme/` contains the reusable theme library.
- `samples/DTC.AsciiTheme.Demo/` contains the showcase app used for interactive visual checks.
- `tests/DTC.AsciiTheme.Tests/` contains the automated screenshot-generation test.
- `img/` contains the generated demo screenshots used by this README.
