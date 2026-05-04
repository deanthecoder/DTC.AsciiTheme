// Code authored by Dean Edis (DeanTheCoder).
// Anyone is free to copy, modify, use, compile, or distribute this software,
// either in source code form or as a compiled binary, for any non-commercial
// purpose.
//
// If you modify the code, please retain this copyright header,
// and consider contributing back to the repository or letting us know
// about your modifications. Your contributions are valued!
//
// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace DTC.AsciiTheme.Demo;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer m_animationTimer;
    private double m_animationPhase;

    public MainWindow()
    {
        InitializeComponent();
        FileDataGrid.ItemsSource = CreateFileRows();
        m_animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(80),
        };
        m_animationTimer.Tick += HandleAnimationTick;
        Opened += HandleOpened;
        Closed += HandleClosed;
    }

    private void HandleOpened(object sender, EventArgs e)
    {
        FocusedButton.Focus(NavigationMethod.Tab);
        m_animationTimer.Start();
    }

    private void HandleClosed(object sender, EventArgs e)
    {
        m_animationTimer.Stop();
    }

    private void HandleAnimationTick(object sender, EventArgs e)
    {
        m_animationPhase += 0.08;

        AnimatedProgressBar.Value = CalculateWaveValue(m_animationPhase, 18.0, 82.0);
        AnimatedTextProgressBar.Value = CalculateWaveValue(m_animationPhase + 1.2, 12.0, 96.0);
        AnimatedVerticalProgressBar.Value = CalculateWaveValue(m_animationPhase + 2.1, 10.0, 90.0);
    }

    private static double CalculateWaveValue(double phase, double minimum, double maximum)
    {
        var normalized = (Math.Sin(phase) + 1.0) / 2.0;
        return minimum + ((maximum - minimum) * normalized);
    }

    private async void HandleShowOkMessageBoxClick(object sender, RoutedEventArgs e)
    {
        var result = await AsciiMessageBox.ShowAsync(
            this,
            "Information",
            "The current operation completed successfully.");

        UpdateMessageBoxResult(result);
    }

    private async void HandleShowOkCancelMessageBoxClick(object sender, RoutedEventArgs e)
    {
        var result = await AsciiMessageBox.ShowAsync(
            this,
            "Overwrite File",
            "Overwrite CONFIG.SYS with the new settings from SETUP?",
            AsciiMessageBoxButtons.OkCancel);

        UpdateMessageBoxResult(result);
    }

    private async void HandleShowYesNoMessageBoxClick(object sender, RoutedEventArgs e)
    {
        var result = await AsciiMessageBox.ShowAsync(
            this,
            "Delete File",
            "Delete AUTOEXEC.BAT from drive C:?",
            AsciiMessageBoxButtons.YesNo);

        UpdateMessageBoxResult(result);
    }

    private async void HandleShowSpeccyBusyBorderDemoClick(object sender, RoutedEventArgs e)
    {
        var dialog = new SpeccyBusyBorderDemoWindow();
        await dialog.ShowDialog(this);
    }

    private void UpdateMessageBoxResult(AsciiMessageBoxResult result)
    {
        MessageBoxResultTextBlock.Text = $"Last result: {result}";
    }

    private async void HandleOpenMenuClick(object sender, RoutedEventArgs e)
    {
        var file = await AsciiFileDialog.OpenFileAsync(
            this,
            "Open File",
            [
                new FilePickerFileType("All files")
                {
                    Patterns = ["*.*"],
                },
                new FilePickerFileType("Text files")
                {
                    Patterns = ["*.txt", "*.ini", "*.cfg", "*.log", "*.bat", "*.sys"],
                },
            ]);

        if (file is null)
        {
            return;
        }

        var selectedFile = file.TryGetLocalPath() ?? file.Name;

        await AsciiMessageBox.ShowAsync(
            this,
            "Open",
            $"Selected file:\n{selectedFile}");
    }

    private async void HandleSaveMenuClick(object sender, RoutedEventArgs e)
    {
        var file = await AsciiFileDialog.SaveFileAsync(
            this,
            "Save File",
            "CONFIG.NEW",
            [
                new FilePickerFileType("All files")
                {
                    Patterns = ["*.*"],
                },
                new FilePickerFileType("Text files")
                {
                    Patterns = ["*.txt", "*.ini", "*.cfg", "*.log", "*.bat", "*.sys"],
                },
            ]);

        if (file is null)
        {
            return;
        }

        await AsciiMessageBox.ShowAsync(
            this,
            "Save",
            $"Target file:\n{file.FullName}");
    }

    private void HandleExitMenuClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void HandleViewButtonsClick(object sender, RoutedEventArgs e) => SelectTab(0);

    private void HandleViewInputsClick(object sender, RoutedEventArgs e) => SelectTab(1);

    private void HandleViewTextClick(object sender, RoutedEventArgs e) => SelectTab(2);

    private void HandleViewListsClick(object sender, RoutedEventArgs e) => SelectTab(3);

    private void HandleViewDataClick(object sender, RoutedEventArgs e) => SelectTab(4);

    private void HandleViewTreeClick(object sender, RoutedEventArgs e) => SelectTab(5);

    private void HandleViewProgressClick(object sender, RoutedEventArgs e) => SelectTab(6);

    private void HandleViewScrollViewerClick(object sender, RoutedEventArgs e) => SelectTab(7);

    private void HandleViewMoreClick(object sender, RoutedEventArgs e) => SelectTab(8);

    private void HandleThemeBlueClick(object sender, RoutedEventArgs e) => ApplyPalette(AsciiPalette.Blue);

    private void HandleThemeMonoClick(object sender, RoutedEventArgs e) => ApplyPalette(AsciiPalette.Mono);

    private void HandleThemeGreenClick(object sender, RoutedEventArgs e) => ApplyPalette(AsciiPalette.Green);

    private void HandleThemePlasmaClick(object sender, RoutedEventArgs e) => ApplyPalette(AsciiPalette.Plasma);

    private void HandleThemeGreyClick(object sender, RoutedEventArgs e) => ApplyPalette(AsciiPalette.Grey);

    private void HandleThemeBbcClick(object sender, RoutedEventArgs e) => ApplyPalette(AsciiPalette.BBC);

    private void HandleThemeC64Click(object sender, RoutedEventArgs e) => ApplyPalette(AsciiPalette.C64);

    private void HandleThemeZxClick(object sender, RoutedEventArgs e) => ApplyPalette(AsciiPalette.ZX);

    private async void HandleGitHub1989MenuClick(object sender, RoutedEventArgs e)
    {
        var window = new GitHub1989Window();
        await window.ShowDialog(this);
    }

    private async void HandleAboutMenuClick(object sender, RoutedEventArgs e)
    {
        await AsciiMessageBox.ShowAsync(
            this,
            "About",
            "DTC.AsciiTheme\n\nA retro Avalonia theme and helper-control package by DeanTheCoder.\n\nGitHub:\nwww.github.com/deanthecoder/DTC.AsciiTheme");
    }

    private void SelectTab(int index)
    {
        DemoTabControl.SelectedIndex = index;
    }

    private static void ApplyPalette(AsciiPalette palette)
    {
        if (Application.Current is null)
        {
            return;
        }

        AsciiPaletteManager.Apply(Application.Current, palette);
    }

    private static IReadOnlyList<DemoFileRow> CreateFileRows()
    {
        return
        [
            new("AUTOEXEC.BAT", "Batch", "1 KB", "1994-03-12 07:15"),
            new("COMMAND.COM", "System", "54 KB", "1994-03-12 07:15"),
            new("CONFIG.SYS", "Config", "2 KB", "1994-03-12 07:16"),
            new("EDIT.EXE", "Program", "69 KB", "1993-11-08 18:42"),
            new("FDISK.EXE", "Program", "30 KB", "1993-11-08 18:42"),
            new("NETWORK.INI", "Settings", "4 KB", "1995-01-21 09:03"),
            new("PKZIP.EXE", "Program", "42 KB", "1992-08-19 20:10"),
            new("README.TXT", "Text", "7 KB", "1995-01-21 09:10"),
        ];
    }
}

public sealed record DemoFileRow(string Name, string Type, string Size, string Modified);
