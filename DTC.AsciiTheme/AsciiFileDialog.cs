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
using Avalonia.Platform.Storage;
using DTC.AsciiTheme.Controls;

namespace DTC.AsciiTheme;

/// <summary>
/// Provides simple retro-styled file dialog helpers for Avalonia windows.
/// </summary>
public static class AsciiFileDialog
{
    /// <summary>
    /// Shows the custom ASCII-themed open-file dialog and returns the selected file.
    /// </summary>
    /// <param name="owner">The owning window for the dialog.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="fileTypes">Optional file type filters to show in the dialog.</param>
    /// <returns>
    /// The selected <see cref="IStorageFile"/>, or <see langword="null"/> if the dialog is cancelled.
    /// </returns>
    public static async Task<IStorageFile> OpenFileAsync(
        Window owner,
        string title = "Open File",
        IReadOnlyList<FilePickerFileType> fileTypes = null)
    {
        ArgumentNullException.ThrowIfNull(owner);

        var dialog = new AsciiOpenFileDialogWindow(title, fileTypes);
        var selectedPath = await dialog.ShowDialog<string>(owner);

        if (string.IsNullOrWhiteSpace(selectedPath))
        {
            return null;
        }

        return await owner.StorageProvider.TryGetFileFromPathAsync(selectedPath);
    }

    /// <summary>
    /// Shows the custom ASCII-themed save-file dialog and returns the chosen target path.
    /// </summary>
    /// <param name="owner">The owning window for the dialog.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="suggestedFileName">An optional initial file name to prefill in the dialog.</param>
    /// <param name="fileTypes">Optional file type filters to show in the dialog.</param>
    /// <returns>
    /// A <see cref="FileInfo"/> for the chosen target path, or <see langword="null"/> if the dialog is cancelled.
    /// </returns>
    public static async Task<FileInfo> SaveFileAsync(
        Window owner,
        string title = "Save File",
        string suggestedFileName = null,
        IReadOnlyList<FilePickerFileType> fileTypes = null)
    {
        ArgumentNullException.ThrowIfNull(owner);

        var dialog = new AsciiSaveFileDialogWindow(title, suggestedFileName, fileTypes);
        var selectedPath = await dialog.ShowDialog<string>(owner);

        if (string.IsNullOrWhiteSpace(selectedPath))
        {
            return null;
        }

        return new FileInfo(selectedPath);
    }
}
