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
using DTC.AsciiTheme.Controls;

namespace DTC.AsciiTheme;

public static class AsciiMessageBox
{
    public static Task<AsciiMessageBoxResult> ShowAsync(
        Window owner,
        string title,
        string message,
        AsciiMessageBoxButtons buttons = AsciiMessageBoxButtons.Ok)
    {
        ArgumentNullException.ThrowIfNull(owner);

        var dialog = new AsciiMessageBoxWindow(title, message, buttons);
        return dialog.ShowDialog<AsciiMessageBoxResult>(owner);
    }
}
