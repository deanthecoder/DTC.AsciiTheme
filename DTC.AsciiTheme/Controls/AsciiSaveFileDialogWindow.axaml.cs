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

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace DTC.AsciiTheme.Controls;

internal sealed partial class AsciiSaveFileDialogWindow : Window
{
    private readonly ComboBox m_filterComboBox;
    private readonly TextBox m_fileNameTextBox;
    private readonly ListBox m_fileListBox;
    private readonly ListBox m_folderListBox;
    private readonly FilterOption[] m_filters;
    private readonly Button m_saveButton;
    private readonly TextBox m_pathTextBox;
    private DirectoryInfo m_currentDirectory;

    public AsciiSaveFileDialogWindow() : this("Save File", null, null)
    {
    }

    public AsciiSaveFileDialogWindow(
        string title,
        string suggestedFileName,
        IReadOnlyList<FilePickerFileType> fileTypes)
    {
        InitializeComponent();

        Title = title;

        var saveFileGroupBox = this.FindControl<AsciiGroupBox>("SaveFileGroupBox")
                               ?? throw new InvalidOperationException("Expected SaveFileGroupBox to exist.");
        saveFileGroupBox.Header = title;

        m_pathTextBox = this.FindControl<TextBox>("PathTextBox")
                       ?? throw new InvalidOperationException("Expected PathTextBox to exist.");
        m_folderListBox = this.FindControl<ListBox>("FolderListBox")
                         ?? throw new InvalidOperationException("Expected FolderListBox to exist.");
        m_fileListBox = this.FindControl<ListBox>("FileListBox")
                       ?? throw new InvalidOperationException("Expected FileListBox to exist.");
        m_fileNameTextBox = this.FindControl<TextBox>("FileNameTextBox")
                           ?? throw new InvalidOperationException("Expected FileNameTextBox to exist.");
        m_filterComboBox = this.FindControl<ComboBox>("FilterComboBox")
                          ?? throw new InvalidOperationException("Expected FilterComboBox to exist.");
        m_saveButton = this.FindControl<Button>("SaveButton")
                      ?? throw new InvalidOperationException("Expected SaveButton to exist.");

        m_filters = CreateFilters(fileTypes);
        m_filterComboBox.ItemsSource = m_filters;
        m_filterComboBox.SelectedIndex = 0;

        m_currentDirectory = GetInitialDirectory();

        if (!string.IsNullOrWhiteSpace(suggestedFileName))
        {
            m_fileNameTextBox.Text = suggestedFileName;
        }

        m_folderListBox.SelectionChanged += HandleFolderSelectionChanged;
        m_fileListBox.SelectionChanged += HandleFileSelectionChanged;
        m_fileNameTextBox.PropertyChanged += HandleFileNameTextBoxPropertyChanged;
        KeyDown += HandleKeyDown;

        NavigateTo(m_currentDirectory);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void HandleFolderSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (m_folderListBox.SelectedItem is not FileSystemEntry entry)
        {
            return;
        }

        if (!entry.IsDirectory)
        {
            return;
        }
    }

    private void HandleFileSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (m_fileListBox.SelectedItem is not FileSystemEntry entry)
        {
            UpdateSaveButtonState();
            return;
        }

        m_fileNameTextBox.Text = entry.Name;
        UpdateSaveButtonState();
    }

    private void HandleFileNameTextBoxPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == TextBox.TextProperty)
        {
            UpdateSaveButtonState();
        }
    }

    private void HandleFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (m_currentDirectory is null)
        {
            return;
        }

        RefreshFiles();
        UpdateSaveButtonState();
    }

    private void HandleFolderListDoubleTapped(object sender, TappedEventArgs e)
    {
        if (m_folderListBox.SelectedItem is not FileSystemEntry entry)
        {
            return;
        }

        if (entry.IsParent)
        {
            var parentDirectory = m_currentDirectory.Parent;
            if (parentDirectory is not null)
            {
                NavigateTo(parentDirectory);
            }

            return;
        }

        if (!entry.IsDirectory)
        {
            return;
        }

        var nextDirectory = new DirectoryInfo(entry.FullPath);
        if (nextDirectory.Exists)
        {
            NavigateTo(nextDirectory);
        }
    }

    private async void HandleFileListDoubleTapped(object sender, TappedEventArgs e)
    {
        await ConfirmAndCloseIfValidAsync();
    }

    private async void HandleSaveButtonClick(object sender, RoutedEventArgs e)
    {
        await ConfirmAndCloseIfValidAsync();
    }

    private void HandleCancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private async void HandleKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            await ConfirmAndCloseIfValidAsync();
            e.Handled = true;
        }
    }

    private async Task ConfirmAndCloseIfValidAsync()
    {
        if (!TryGetSelectedFilePath(out var selectedFile))
        {
            return;
        }

        if (selectedFile.Exists)
        {
            var result = await AsciiMessageBox.ShowAsync(
                this,
                "Overwrite File",
                $"Overwrite {selectedFile.Name}?",
                AsciiMessageBoxButtons.OkCancel);

            if (result != AsciiMessageBoxResult.Ok)
            {
                return;
            }
        }

        Close(selectedFile.FullName);
    }

    private void NavigateTo(DirectoryInfo directory)
    {
        m_currentDirectory = directory;
        m_pathTextBox.Text = directory.FullName;
        ToolTip.SetTip(m_pathTextBox, directory.FullName);

        RefreshFolders();
        RefreshFiles();
        UpdateSaveButtonState();
    }

    private void RefreshFolders()
    {
        var entries = new List<FileSystemEntry>();

        entries.AddRange(GetDriveEntries());

        if (m_currentDirectory.Parent is not null)
        {
            entries.Add(new FileSystemEntry("[..]", m_currentDirectory.Parent.FullName, true, false, true));
        }

        try
        {
            entries.AddRange(m_currentDirectory.EnumerateDirectories()
                                              .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
                                              .Select(d => new FileSystemEntry($"[{d.Name}]", d.FullName, true, false, false)));
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (DirectoryNotFoundException)
        {
        }

        m_folderListBox.ItemsSource = entries;
        m_folderListBox.SelectedItem = null;
    }

    private void RefreshFiles()
    {
        var entries = new List<FileSystemEntry>();

        try
        {
            var selectedFilter = GetSelectedFilter();
            entries.AddRange(m_currentDirectory.EnumerateFiles()
                                              .Where(file => MatchesFilter(file, selectedFilter))
                                              .OrderBy(file => file.Name, StringComparer.OrdinalIgnoreCase)
                                              .Select(file => new FileSystemEntry(file.Name, file.FullName, false, false, false)));
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (DirectoryNotFoundException)
        {
        }

        m_fileListBox.ItemsSource = entries;
        m_fileListBox.SelectedItem = null;
    }

    private bool TryGetSelectedFilePath(out FileInfo selectedFile)
    {
        selectedFile = null;

        var fileName = m_fileNameTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return false;
        }

        fileName = ApplySelectedFilterExtension(fileName);

        var combinedPath = Path.Combine(m_currentDirectory.FullName, fileName);
        selectedFile = new FileInfo(combinedPath);
        return true;
    }

    private void UpdateSaveButtonState()
    {
        m_saveButton.IsEnabled = TryGetSelectedFilePath(out _);
    }

    private FilterOption GetSelectedFilter()
    {
        return m_filterComboBox.SelectedItem as FilterOption
               ?? m_filters[0];
    }

    private string ApplySelectedFilterExtension(string fileName)
    {
        if (Path.HasExtension(fileName))
        {
            return fileName;
        }

        var extensionPattern = GetSelectedFilter().Patterns.FirstOrDefault(
            pattern => pattern.StartsWith("*.", StringComparison.Ordinal) &&
                       !string.Equals(pattern, "*.*", StringComparison.Ordinal));

        if (string.IsNullOrWhiteSpace(extensionPattern))
        {
            return fileName;
        }

        return $"{fileName}{extensionPattern[1..]}";
    }

    private static FilterOption[] CreateFilters(IReadOnlyList<FilePickerFileType> fileTypes)
    {
        if (fileTypes is null || fileTypes.Count == 0)
        {
            return
            [
                new FilterOption("All files (*.*)", ["*.*"])
            ];
        }

        return fileTypes.Select(type =>
                        {
                            var patterns = type.Patterns?.Count > 0
                                ? type.Patterns.ToArray()
                                : ["*.*"];

                            var patternText = string.Join(", ", patterns);
                            return new FilterOption($"{type.Name} ({patternText})", patterns);
                        })
                        .ToArray();
    }

    private static DirectoryInfo GetInitialDirectory()
    {
        var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        return currentDirectory.Exists
            ? currentDirectory
            : new DirectoryInfo(Path.GetPathRoot(Environment.CurrentDirectory) ?? Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));
    }

    private static IEnumerable<FileSystemEntry> GetDriveEntries()
    {
        if (OperatingSystem.IsWindows())
        {
            foreach (var drive in DriveInfo.GetDrives()
                                           .Where(drive => drive.IsReady)
                                           .OrderBy(drive => drive.Name, StringComparer.OrdinalIgnoreCase))
            {
                yield return new FileSystemEntry($"[{drive.Name.TrimEnd(Path.DirectorySeparatorChar)}]", drive.RootDirectory.FullName, true, true, false);
            }

            yield break;
        }

        yield return new FileSystemEntry("[/]", Path.DirectorySeparatorChar.ToString(), true, true, false);

        var volumesDirectory = new DirectoryInfo("/Volumes");
        if (!volumesDirectory.Exists)
        {
            yield break;
        }

        foreach (var directory in volumesDirectory.EnumerateDirectories()
                                                 .OrderBy(directory => directory.Name, StringComparer.OrdinalIgnoreCase))
        {
            yield return new FileSystemEntry($"[{directory.Name}]", directory.FullName, true, true, false);
        }
    }

    private static bool MatchesFilter(FileInfo file, FilterOption filter)
    {
        return filter.Patterns.Any(pattern => MatchesPattern(file.Name, pattern));
    }

    private static bool MatchesPattern(string fileName, string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern) || pattern == "*.*" || pattern == "*")
        {
            return true;
        }

        if (pattern.StartsWith("*.", StringComparison.Ordinal))
        {
            return fileName.EndsWith(pattern[1..], StringComparison.OrdinalIgnoreCase);
        }

        return string.Equals(fileName, pattern, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record FileSystemEntry(
        string Name,
        string FullPath,
        bool IsDirectory,
        bool IsDrive,
        bool IsParent)
    {
        public override string ToString() => Name;
    }

    private sealed record FilterOption(string Name, IReadOnlyList<string> Patterns)
    {
        public override string ToString() => Name;
    }
}
