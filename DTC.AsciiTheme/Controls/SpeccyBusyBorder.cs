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

using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace DTC.AsciiTheme.Controls;

public class SpeccyBusyBorder : Decorator
{
    /// <summary>
    /// A short neutral pause before the loader effect begins.
    /// </summary>
    private const int WarmupDurationMs = 250;

    /// <summary>
    /// The length of the opening flash phase before the first red/cyan data bars.
    /// </summary>
    private const int InitialFlashDurationMs = 3250;

    /// <summary>
    /// The length of each red/cyan loading-bar run.
    /// </summary>
    private const int IntroDataDurationMs = 3000;

    /// <summary>
    /// The length of the intermediate flash phase between the two red/cyan bar runs.
    /// </summary>
    private const int SecondaryFlashDurationMs = 2000;

    /// <summary>
    /// The speed of the flash alternation.
    /// </summary>
    private const int IntroPulseMs = 1000;

    /// <summary>
    /// Controls how quickly the bar pattern scrolls across the border.
    /// </summary>
    private const int AnimationDivisor = 10;

    /// <summary>
    /// The thickness of each colored red/cyan stripe during the earlier loading phase.
    /// </summary>
    private const int IntroBarWidth = 12;

    /// <summary>
    /// The gap between each red/cyan stripe during the earlier loading phase.
    /// </summary>
    private const int IntroGapWidth = 12;

    /// <summary>
    /// The thickness of each colored blue/yellow stripe during the final data phase.
    /// </summary>
    private const int DataBarWidth = 8;

    /// <summary>
    /// The gap between each blue/yellow stripe during the final data phase.
    /// </summary>
    private const int DataGapWidth = 8;

    private static readonly IBrush s_defaultBackgroundBrush = new SolidColorBrush(Color.Parse("#D7D7D7"));
    private static readonly IBrush s_introCyanBrush = new SolidColorBrush(Color.Parse("#00D7D7"));
    private static readonly IBrush s_introRedBrush = new SolidColorBrush(Color.Parse("#D70000"));
    private static readonly IBrush s_dataBlueBrush = new SolidColorBrush(Color.Parse("#0000D7"));
    private static readonly IBrush s_dataYellowBrush = new SolidColorBrush(Color.Parse("#D7D700"));

    private readonly Random m_random = new();
    private readonly Stopwatch m_stopwatch = new();
    private readonly DispatcherTimer m_timer;
    private bool m_isAttachedToVisualTree;
    private int m_finalPhaseOffset;

    public static readonly StyledProperty<IBrush> BackgroundProperty =
        AvaloniaProperty.Register<SpeccyBusyBorder, IBrush>(nameof(Background), s_defaultBackgroundBrush);

    public static readonly StyledProperty<bool> IsBusyProperty =
        AvaloniaProperty.Register<SpeccyBusyBorder, bool>(nameof(IsBusy));

    static SpeccyBusyBorder()
    {
        AffectsRender<SpeccyBusyBorder>(BackgroundProperty, IsBusyProperty, Decorator.PaddingProperty);
        AffectsMeasure<SpeccyBusyBorder>(Decorator.PaddingProperty);

        IsBusyProperty.Changed.AddClassHandler<SpeccyBusyBorder>((border, _) => border.UpdateBusyState());
    }

    public SpeccyBusyBorder()
    {
        m_timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(40),
        };
        m_timer.Tick += HandleTimerTick;

        Padding = new Thickness(48, 32);
    }

    public IBrush Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public bool IsBusy
    {
        get => GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        m_isAttachedToVisualTree = true;
        UpdateBusyState();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        m_isAttachedToVisualTree = false;
        m_timer.Stop();
        m_stopwatch.Stop();

        base.OnDetachedFromVisualTree(e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var padding = Padding;
        var innerAvailableSize = Deflate(availableSize, padding);

        if (Child is null)
        {
            return new Size(padding.Left + padding.Right, padding.Top + padding.Bottom);
        }

        Child.Measure(innerAvailableSize);
        return Inflate(Child.DesiredSize, padding);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child is not null)
        {
            var childBounds = GetInnerBounds(finalSize, Padding);
            Child.Arrange(childBounds);
        }

        return finalSize;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        var padding = Padding;
        if (padding.Left <= 0 && padding.Top <= 0 && padding.Right <= 0 && padding.Bottom <= 0)
        {
            return;
        }

        if (!IsBusy)
        {
            RenderSolidBorder(context, bounds, padding, Background);
            return;
        }

        RenderBusyBorder(context, bounds, padding);
    }

    private void HandleTimerTick(object sender, EventArgs e)
    {
        if (IsBusy && IsInFinalDataPhase(m_stopwatch.ElapsedMilliseconds))
        {
            m_finalPhaseOffset = m_random.Next(0, DataBarWidth + DataGapWidth);
        }

        InvalidateVisual();
    }

    private void UpdateBusyState()
    {
        if (!m_isAttachedToVisualTree)
        {
            return;
        }

        if (IsBusy)
        {
            m_stopwatch.Restart();
            m_timer.Start();
        }
        else
        {
            m_timer.Stop();
            m_stopwatch.Reset();
            m_finalPhaseOffset = 0;
        }

        InvalidateVisual();
    }

    private void RenderBusyBorder(DrawingContext context, Rect bounds, Thickness padding)
    {
        var elapsedMs = m_stopwatch.ElapsedMilliseconds;
        var pulseOn = ((elapsedMs / IntroPulseMs) % 2) == 0;
        var phaseElapsedMs = elapsedMs - WarmupDurationMs;

        if (elapsedMs < WarmupDurationMs)
        {
            RenderSolidBorder(context, bounds, padding, Background);
            return;
        }

        if (phaseElapsedMs < InitialFlashDurationMs)
        {
            var flashBrush = pulseOn ? s_introRedBrush : s_introCyanBrush;
            RenderSolidBorder(context, bounds, padding, flashBrush);
            return;
        }

        phaseElapsedMs -= InitialFlashDurationMs;
        if (phaseElapsedMs < IntroDataDurationMs)
        {
            var offset = (int)(elapsedMs / AnimationDivisor);

            RenderSolidBorder(context, bounds, padding, s_introCyanBrush);
            RenderStripedBorder(context, bounds, padding, s_introRedBrush, IntroBarWidth, IntroGapWidth, offset);
            return;
        }

        phaseElapsedMs -= IntroDataDurationMs;
        if (phaseElapsedMs < SecondaryFlashDurationMs)
        {
            var flashBrush = pulseOn ? s_introCyanBrush : s_introRedBrush;
            RenderSolidBorder(context, bounds, padding, flashBrush);
            return;
        }

        phaseElapsedMs -= SecondaryFlashDurationMs;
        if (phaseElapsedMs < IntroDataDurationMs)
        {
            var offset = (int)(elapsedMs / AnimationDivisor);

            RenderSolidBorder(context, bounds, padding, s_introRedBrush);
            RenderStripedBorder(context, bounds, padding, s_introCyanBrush, IntroBarWidth, IntroGapWidth, offset);
            return;
        }

        RenderSolidBorder(context, bounds, padding, s_dataBlueBrush);
        RenderStripedBorder(context, bounds, padding, s_dataYellowBrush, DataBarWidth, DataGapWidth, m_finalPhaseOffset);
    }

    private static bool IsInFinalDataPhase(long elapsedMs)
    {
        if (elapsedMs < WarmupDurationMs)
        {
            return false;
        }

        var phaseElapsedMs = elapsedMs - WarmupDurationMs;
        phaseElapsedMs -= InitialFlashDurationMs;
        phaseElapsedMs -= IntroDataDurationMs;
        phaseElapsedMs -= SecondaryFlashDurationMs;
        phaseElapsedMs -= IntroDataDurationMs;

        return phaseElapsedMs >= 0;
    }

    private static void RenderSolidBorder(DrawingContext context, Rect bounds, Thickness padding, IBrush brush)
    {
        var innerBounds = GetInnerBounds(bounds.Size, padding);

        var topHeight = Math.Min(padding.Top, bounds.Height);
        if (topHeight > 0)
        {
            context.FillRectangle(brush, new Rect(bounds.X, bounds.Y, bounds.Width, topHeight));
        }

        var bottomHeight = Math.Min(padding.Bottom, Math.Max(0, bounds.Height - topHeight));
        if (bottomHeight > 0)
        {
            context.FillRectangle(brush, new Rect(bounds.X, innerBounds.Bottom, bounds.Width, bottomHeight));
        }

        var leftWidth = Math.Min(padding.Left, bounds.Width);
        if (leftWidth > 0 && innerBounds.Height > 0)
        {
            context.FillRectangle(brush, new Rect(bounds.X, innerBounds.Y, leftWidth, innerBounds.Height));
        }

        var rightWidth = Math.Min(padding.Right, Math.Max(0, bounds.Width - leftWidth));
        if (rightWidth > 0 && innerBounds.Height > 0)
        {
            context.FillRectangle(brush, new Rect(innerBounds.Right, innerBounds.Y, rightWidth, innerBounds.Height));
        }
    }

    private static void RenderStripedBorder(
        DrawingContext context,
        Rect bounds,
        Thickness padding,
        IBrush brush,
        int barWidth,
        int gapWidth,
        int offset)
    {
        var innerBounds = GetInnerBounds(bounds.Size, padding);
        var patternHeight = barWidth + gapWidth;
        var borderHeight = (int)Math.Ceiling(bounds.Height);
        var y = 0;

        while (y < borderHeight)
        {
            var patternY = Mod(y - offset, patternHeight);
            var runHeight = patternY < barWidth ? barWidth - patternY : patternHeight - patternY;
            var drawHeight = Math.Min(runHeight, borderHeight - y);

            if (patternY < barWidth)
            {
                var bandY = bounds.Y + y;

                if (bandY < innerBounds.Y)
                {
                    var topHeight = Math.Min(drawHeight, innerBounds.Y - bandY);
                    context.FillRectangle(brush, new Rect(bounds.X, bandY, bounds.Width, topHeight));
                }

                var middleStart = Math.Max(bandY, innerBounds.Y);
                var middleEnd = Math.Min(bandY + drawHeight, innerBounds.Bottom);
                var middleHeight = middleEnd - middleStart;
                if (middleHeight > 0)
                {
                    var leftWidth = Math.Min(padding.Left, bounds.Width);
                    if (leftWidth > 0)
                    {
                        context.FillRectangle(brush, new Rect(bounds.X, middleStart, leftWidth, middleHeight));
                    }

                    var rightWidth = Math.Min(padding.Right, bounds.Width - leftWidth);
                    if (rightWidth > 0)
                    {
                        context.FillRectangle(brush, new Rect(innerBounds.Right, middleStart, rightWidth, middleHeight));
                    }
                }

                if (bandY + drawHeight > innerBounds.Bottom)
                {
                    var bottomStart = Math.Max(bandY, innerBounds.Bottom);
                    var bottomHeight = (bandY + drawHeight) - bottomStart;
                    if (bottomHeight > 0)
                    {
                        context.FillRectangle(brush, new Rect(bounds.X, bottomStart, bounds.Width, bottomHeight));
                    }
                }
            }

            y += drawHeight;
        }
    }

    private static Rect GetInnerBounds(Size size, Thickness padding)
    {
        var x = padding.Left;
        var y = padding.Top;
        var width = Math.Max(0, size.Width - padding.Left - padding.Right);
        var height = Math.Max(0, size.Height - padding.Top - padding.Bottom);

        return new Rect(x, y, width, height);
    }

    private static Size Deflate(Size size, Thickness padding)
    {
        var width = double.IsInfinity(size.Width)
            ? size.Width
            : Math.Max(0, size.Width - padding.Left - padding.Right);
        var height = double.IsInfinity(size.Height)
            ? size.Height
            : Math.Max(0, size.Height - padding.Top - padding.Bottom);

        return new Size(width, height);
    }

    private static Size Inflate(Size size, Thickness padding)
    {
        return new Size(size.Width + padding.Left + padding.Right, size.Height + padding.Top + padding.Bottom);
    }

    private static int Mod(int value, int divisor)
    {
        var result = value % divisor;
        if (result < 0)
        {
            result += divisor;
        }

        return result;
    }
}
