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

namespace DTC.AsciiTheme.Demo;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer m_animationTimer;
    private double m_animationPhase;

    public MainWindow()
    {
        InitializeComponent();
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
}
