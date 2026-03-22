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
using DTC.AsciiTheme;

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

    private async void HandleShowOkMessageBoxClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var result = await AsciiMessageBox.ShowAsync(
            this,
            "Information",
            "The current operation completed successfully.",
            AsciiMessageBoxButtons.Ok);

        UpdateMessageBoxResult(result);
    }

    private async void HandleShowOkCancelMessageBoxClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var result = await AsciiMessageBox.ShowAsync(
            this,
            "Overwrite File",
            "Overwrite CONFIG.SYS with the new settings from SETUP?",
            AsciiMessageBoxButtons.OkCancel);

        UpdateMessageBoxResult(result);
    }

    private async void HandleShowYesNoMessageBoxClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var result = await AsciiMessageBox.ShowAsync(
            this,
            "Delete File",
            "Delete AUTOEXEC.BAT from drive C:?",
            AsciiMessageBoxButtons.YesNo);

        UpdateMessageBoxResult(result);
    }

    private void UpdateMessageBoxResult(AsciiMessageBoxResult result)
    {
        MessageBoxResultTextBlock.Text = $"Last result: {result}";
    }

    private void HandlePaletteSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Application.Current is null)
        {
            return;
        }

        if (sender is not ComboBox comboBox)
        {
            return;
        }

        if (comboBox.SelectedItem is not ComboBoxItem comboBoxItem)
        {
            return;
        }

        if (comboBoxItem.Tag is not string paletteName)
        {
            return;
        }

        if (!Enum.TryParse<AsciiPalette>(paletteName, ignoreCase: true, out var palette))
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
