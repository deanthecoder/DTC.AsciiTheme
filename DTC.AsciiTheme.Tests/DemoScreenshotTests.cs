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

using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using DTC.AsciiTheme.Controls;

namespace DTC.AsciiTheme.Tests;

[TestFixture]
public sealed class DemoScreenshotTests
{
    private static readonly DirectoryInfo RepositoryDirectory = new(
        GetRepositoryRootPath());

    private static readonly DirectoryInfo ImageDirectory = new(
        Path.Combine(RepositoryDirectory.FullName, "img"));

    [Test]
    public async Task CaptureDemoTabScreenshots()
    {
        var session = HeadlessUnitTestSession.GetOrStartForAssembly(Assembly.GetExecutingAssembly());

        await session.Dispatch(async () =>
        {
            ImageDirectory.Create();

            var window = new Demo.MainWindow
            {
                Width = 960,
                Height = 640,
            };

            window.Show();

            try
            {
                await WaitForRenderAsync();

                var tabControl = window.FindControl<TabControl>("DemoTabControl");
                Assert.That(tabControl, Is.Not.Null, "Expected the demo window to expose DemoTabControl.");

                var tabItems = tabControl!.GetLogicalDescendants()
                                         .OfType<TabItem>()
                                         .ToList();
                Assert.That(tabItems.Count, Is.GreaterThan(0), "Expected the demo tab control to contain tab items.");
                var textTab = tabItems.Single(item => NormalizeHeader(item.Header) == "text");

                foreach (var tabItem in tabItems)
                {
                    tabControl.SelectedItem = tabItem;
                    tabItem.Focus();
                    await WaitForRenderAsync();
                }

                foreach (var tabItem in tabItems)
                {
                    tabControl.SelectedItem = tabItem;
                    tabItem.Focus();
                    await WaitForRenderAsync();
                    SaveScreenshot(window, tabItem);
                }

                var paletteScreenshots = new[]
                {
                    (AsciiPalette.Blue, "text-blue.png"),
                    (AsciiPalette.Mono, "text-mono.png"),
                    (AsciiPalette.Green, "text-green.png"),
                    (AsciiPalette.Plasma, "text-plasma.png"),
                    (AsciiPalette.Grey, "text-grey.png"),
                    (AsciiPalette.ZX, "text-zx.png"),
                };

                foreach (var (palette, fileName) in paletteScreenshots)
                {
                    AsciiPaletteManager.Apply(Avalonia.Application.Current!, palette);
                    tabControl.SelectedItem = textTab;
                    textTab.Focus();
                    await WaitForRenderAsync();
                    await WaitForRenderAsync();
                    SaveScreenshot(window, fileName);
                }

                var openFileDialog = new AsciiOpenFileDialogWindow(
                    "Open File",
                    [
                        new Avalonia.Platform.Storage.FilePickerFileType("All files")
                        {
                            Patterns = ["*.*"],
                        },
                        new Avalonia.Platform.Storage.FilePickerFileType("Text files")
                        {
                            Patterns = ["*.txt", "*.ini", "*.cfg", "*.log", "*.bat", "*.sys"],
                        },
                    ]);

                openFileDialog.Show();

                try
                {
                    await WaitForRenderAsync();
                    await WaitForRenderAsync();
                    SaveScreenshot(openFileDialog, "open-dialog.png");
                }
                finally
                {
                    openFileDialog.Close();
                }
            }
            finally
            {
                window.Close();
            }

            return 0;
        }, CancellationToken.None);
    }

    private static void SaveScreenshot(Window window, TabItem tabItem)
    {
        var fileName = $"{NormalizeHeader(tabItem.Header)}.png";
        SaveScreenshot(window, fileName);
    }

    private static void SaveScreenshot(Window window, string fileName)
    {
        using var frame = window.CaptureRenderedFrame();
        Assert.That(frame, Is.Not.Null, $"Expected a rendered frame for screenshot '{fileName}'.");

        var file = new FileInfo(Path.Combine(ImageDirectory.FullName, fileName));
        using (var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
        {
            frame!.Save(stream);
        }

        file.Refresh();
        Assert.That(file.Exists, Is.True, $"Expected screenshot '{file.FullName}' to be written.");
        Assert.That(file.Length, Is.GreaterThan(0L), $"Expected screenshot '{file.FullName}' to contain data.");
    }

    private static string NormalizeHeader(object header)
    {
        var text = header?.ToString() ?? string.Empty;
        var sanitized = text.Replace("_", string.Empty, StringComparison.Ordinal)
                            .Replace(" ", string.Empty, StringComparison.Ordinal);

        var normalized = sanitized.ToLowerInvariant();

        return normalized switch
        {
            "scrolling" => "scrollviewer",
            _ => normalized,
        };
    }

    private static async Task WaitForRenderAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Render);
        await Task.Delay(50);
    }

    private static string GetRepositoryRootPath([CallerFilePath] string sourceFilePath = "")
    {
        var sourceDirectory = Path.GetDirectoryName(sourceFilePath) ?? string.Empty;
        return Path.GetFullPath(Path.Combine(sourceDirectory, ".."));
    }
}
