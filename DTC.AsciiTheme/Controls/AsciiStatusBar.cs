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

using Avalonia;
using Avalonia.Controls.Primitives;

namespace DTC.AsciiTheme.Controls;

public class AsciiStatusBar : TemplatedControl
{
    public static readonly StyledProperty<string> LeftTextProperty =
        AvaloniaProperty.Register<AsciiStatusBar, string>(nameof(LeftText), string.Empty);

    public static readonly StyledProperty<string> RightTextProperty =
        AvaloniaProperty.Register<AsciiStatusBar, string>(nameof(RightText), string.Empty);

    public string LeftText
    {
        get => GetValue(LeftTextProperty);
        set => SetValue(LeftTextProperty, value);
    }

    public string RightText
    {
        get => GetValue(RightTextProperty);
        set => SetValue(RightTextProperty, value);
    }
}
