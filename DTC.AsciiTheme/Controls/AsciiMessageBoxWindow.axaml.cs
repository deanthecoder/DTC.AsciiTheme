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
using Avalonia.Markup.Xaml;
using DTC.AsciiTheme;

namespace DTC.AsciiTheme.Controls;

internal sealed partial class AsciiMessageBoxWindow : Window
{
    private readonly StackPanel m_buttonPanel;
    private readonly AsciiMessageBoxResult? m_cancelResult;
    private readonly Button m_defaultButton;
    private readonly AsciiMessageBoxResult m_defaultResult;

    public AsciiMessageBoxWindow() : this("Title", "Message", AsciiMessageBoxButtons.OkCancel)
    {
    }

    public AsciiMessageBoxWindow(string title, string message, AsciiMessageBoxButtons buttons)
    {
        InitializeComponent();

        var messageBoxGroupBox = this.FindControl<AsciiGroupBox>("MessageBoxGroupBox")
                                 ?? throw new InvalidOperationException("Expected MessageBoxGroupBox to exist.");
        var messageTextBlock = this.FindControl<TextBlock>("MessageTextBlock")
                               ?? throw new InvalidOperationException("Expected MessageTextBlock to exist.");
        m_buttonPanel = this.FindControl<StackPanel>("ButtonPanel")
                      ?? throw new InvalidOperationException("Expected ButtonPanel to exist.");

        Title = title;
        messageBoxGroupBox.Header = title;
        messageTextBlock.Text = message;

        var buttonSpecs = GetButtonSpecs(buttons);
        m_defaultResult = buttonSpecs[0].Result;
        m_defaultButton = AddButton(buttonSpecs[0]);

        if (buttonSpecs[0].IsCancel)
            m_cancelResult = buttonSpecs[0].Result;

        for (var i = 1; i < buttonSpecs.Length; i++)
        {
            var buttonSpec = buttonSpecs[i];
            AddButton(buttonSpec);

            if (buttonSpec.IsCancel)
                m_cancelResult = buttonSpec.Result;
        }

        Opened += HandleOpened;
        KeyDown += HandleKeyDown;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void HandleOpened(object sender, EventArgs e)
    {
        m_defaultButton.Focus(NavigationMethod.Tab);
    }

    private void HandleKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Close(m_defaultResult);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && m_cancelResult.HasValue)
        {
            Close(m_cancelResult.Value);
            e.Handled = true;
        }
    }

    private Button AddButton(ButtonSpec buttonSpec)
    {
        var button = new Button
        {
            Content = buttonSpec.Label,
            MinWidth = 120,
        };

        button.Click += (_, _) => Close(buttonSpec.Result);
        m_buttonPanel.Children.Add(button);
        return button;
    }

    private static ButtonSpec[] GetButtonSpecs(AsciiMessageBoxButtons buttons)
    {
        return buttons switch
        {
            AsciiMessageBoxButtons.Ok =>
            [
                new ButtonSpec("OK", AsciiMessageBoxResult.Ok, IsCancel: false)
            ],
            AsciiMessageBoxButtons.OkCancel =>
            [
                new ButtonSpec("OK", AsciiMessageBoxResult.Ok, IsCancel: false),
                new ButtonSpec("Cancel", AsciiMessageBoxResult.Cancel, IsCancel: true)
            ],
            AsciiMessageBoxButtons.YesNo =>
            [
                new ButtonSpec("Yes", AsciiMessageBoxResult.Yes, IsCancel: false),
                new ButtonSpec("No", AsciiMessageBoxResult.No, IsCancel: true)
            ],
            AsciiMessageBoxButtons.YesNoCancel =>
            [
                new ButtonSpec("Yes", AsciiMessageBoxResult.Yes, IsCancel: false),
                new ButtonSpec("No", AsciiMessageBoxResult.No, IsCancel: false),
                new ButtonSpec("Cancel", AsciiMessageBoxResult.Cancel, IsCancel: true)
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null),
        };
    }

    private readonly record struct ButtonSpec(string Label, AsciiMessageBoxResult Result, bool IsCancel);
}
