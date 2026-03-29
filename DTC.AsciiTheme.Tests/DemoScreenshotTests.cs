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
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DTC.AsciiTheme.Controls;
using SkiaSharp;

namespace DTC.AsciiTheme.Tests;

[TestFixture]
public sealed class DemoScreenshotTests
{
    private static readonly DirectoryInfo RepositoryDirectory = new(
        GetRepositoryRootPath());

    private static readonly DirectoryInfo ImageDirectory = new(
        Path.Combine(RepositoryDirectory.FullName, "img"));

    private static readonly FileInfo FfmpegExecutable = new("/opt/homebrew/bin/ffmpeg");
    private static readonly FileInfo VgaFontFile = new(Path.Combine(RepositoryDirectory.FullName, "DTC.AsciiTheme", "Assets", "Fonts", "VGA", "PxPlus_IBM_VGA8.ttf"));

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
                    (AsciiPalette.BBC, "text-bbc.png"),
                    (AsciiPalette.C64, "text-c64.png"),
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

                AsciiPaletteManager.Apply(Avalonia.Application.Current!, AsciiPalette.Blue);

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

                var messageBox = new AsciiMessageBoxWindow(
                    "About DTC.AsciiTheme",
                    "A retro Avalonia theme with classic utility-style controls, dialogs, and palettes.",
                    AsciiMessageBoxButtons.OkCancel);

                var messageBoxTask = messageBox.ShowDialog<AsciiMessageBoxResult>(window);

                try
                {
                    await WaitForRenderAsync();
                    await WaitForRenderAsync();
                    SaveCompositeScreenshot(window, messageBox, "message-box.png");
                }
                finally
                {
                    messageBox.Close(AsciiMessageBoxResult.Cancel);
                    await messageBoxTask;
                }
            }
            finally
            {
                window.Close();
            }

            return 0;
        }, CancellationToken.None);
    }

    [Test]
    public async Task CaptureDemoTourMedia()
    {
        var frameDirectory = new DirectoryInfo(Path.Combine(ImageDirectory.FullName, "tour-frames"));
        var mp4File = new FileInfo(Path.Combine(ImageDirectory.FullName, "demo-tour.mp4"));
        var gifFile = new FileInfo(Path.Combine(ImageDirectory.FullName, "demo-tour.gif"));

        PrepareFrameDirectory(frameDirectory);

        var session = HeadlessUnitTestSession.GetOrStartForAssembly(Assembly.GetExecutingAssembly());

        await session.Dispatch(async () =>
        {
            var window = new Demo.MainWindow
            {
                Width = 960,
                Height = 640,
            };

            window.Show();

            try
            {
                await WaitForRenderAsync();
                await WaitForRenderAsync();

                var tabControl = window.FindControl<TabControl>("DemoTabControl");
                Assert.That(tabControl, Is.Not.Null, "Expected the demo window to expose DemoTabControl.");

                var tabItems = tabControl!.GetLogicalDescendants()
                                         .OfType<TabItem>()
                                         .ToList();
                Assert.That(tabItems.Count, Is.GreaterThan(0), "Expected the demo tab control to contain tab items.");

                var buttonsTab = tabItems.Single(item => NormalizeHeader(item.Header) == "buttons");
                var inputsTab = tabItems.Single(item => NormalizeHeader(item.Header) == "inputs");
                var dataTab = tabItems.Single(item => NormalizeHeader(item.Header) == "data");
                var progressTab = tabItems.Single(item => NormalizeHeader(item.Header) == "progress");
                var moreTab = tabItems.Single(item => NormalizeHeader(item.Header) == "more");
                var fileMenu = window.GetLogicalDescendants()
                                     .OfType<MenuItem>()
                                     .FirstOrDefault(item => NormalizeHeader(item.Header) == "file");
                Assert.That(fileMenu, Is.Not.Null, "Expected the demo window to expose a File menu.");
                var animatedTextProgressBar = window.FindControl<ProgressBar>("AnimatedTextProgressBar");
                var animatedVerticalProgressBar = window.FindControl<ProgressBar>("AnimatedVerticalProgressBar");
                Assert.That(animatedTextProgressBar, Is.Not.Null, "Expected the demo window to expose AnimatedTextProgressBar.");
                Assert.That(animatedVerticalProgressBar, Is.Not.Null, "Expected the demo window to expose AnimatedVerticalProgressBar.");

                var frameIndex = 1;

                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(buttonsTab, window),
                    frameCount: 18);

                var fileMenuCenter = GetControlCenter(fileMenu!, window);
                frameIndex = await CaptureCursorMoveFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(buttonsTab, window),
                    fileMenuCenter,
                    frameCount: 12);
                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    fileMenuCenter,
                    frameCount: 12);

                var fileMenuOverlayOrigin = new Point(8, 24);
                var openMenuPoint = new Point(fileMenuOverlayOrigin.X + 58, fileMenuOverlayOrigin.Y + 14);
                frameIndex = await CaptureCursorMoveFramesWithMenuOverlayAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    fileMenuCenter,
                    openMenuPoint,
                    frameCount: 12,
                    highlightedIndex: 0);
                frameIndex = await CaptureWindowHoldFramesWithMenuOverlayAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    openMenuPoint,
                    frameCount: 12,
                    highlightedIndex: 0,
                    pressOnLastFrame: true);

                var openFileDialog = new AsciiOpenFileDialogWindow(
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

                openFileDialog.Show();

                try
                {
                    await WaitForRenderAsync();
                    await WaitForRenderAsync();

                    var openButton = openFileDialog.FindControl<Button>("OpenButton");
                    Assert.That(openButton, Is.Not.Null, "Expected the open file dialog to expose OpenButton.");

                    var dialogEntryPoint = new Point(48, 48);
                    frameIndex = await CaptureCompositeCursorMoveFramesAsync(
                        window,
                        openFileDialog,
                        frameDirectory,
                        frameIndex,
                        dialogEntryPoint,
                        GetChildControlCenter(openButton!, openFileDialog),
                        frameCount: 16);
                    frameIndex = await CaptureCompositeHoldFramesAsync(
                        window,
                        openFileDialog,
                        frameDirectory,
                        frameIndex,
                        GetChildControlCenter(openButton!, openFileDialog),
                        frameCount: 30,
                        pressOnLastFrame: true);
                }
                finally
                {
                    openFileDialog.Close();
                }

                frameIndex = await CaptureTabTransitionAsync(window, tabControl, buttonsTab, inputsTab, frameDirectory, frameIndex);
                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(inputsTab, window),
                    frameCount: 14);

                frameIndex = await CaptureTabTransitionAsync(window, tabControl, inputsTab, dataTab, frameDirectory, frameIndex);
                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(dataTab, window),
                    frameCount: 14);

                frameIndex = await CaptureTabTransitionAsync(window, tabControl, dataTab, progressTab, frameDirectory, frameIndex);
                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(progressTab, window),
                    frameCount: 8);
                frameIndex = await CaptureCursorMoveFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(progressTab, window),
                    GetControlCenter(animatedTextProgressBar!, window),
                    frameCount: 8);
                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(animatedTextProgressBar!, window),
                    frameCount: 12);
                frameIndex = await CaptureCursorMoveFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(animatedTextProgressBar!, window),
                    GetControlCenter(animatedVerticalProgressBar!, window),
                    frameCount: 8);
                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(animatedVerticalProgressBar!, window),
                    frameCount: 20);

                frameIndex = await CaptureTabTransitionAsync(window, tabControl, progressTab, moreTab, frameDirectory, frameIndex);

                var showOkCancelButton = moreTab.GetLogicalDescendants()
                                                .OfType<Button>()
                                                .FirstOrDefault(button => string.Equals(button.Content?.ToString(), "Show _OK/Cancel", StringComparison.OrdinalIgnoreCase));
                Assert.That(showOkCancelButton, Is.Not.Null, "Expected the More tab to expose the Show OK/Cancel button.");
                frameIndex = await CaptureCursorMoveFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(moreTab, window),
                    GetControlCenter(showOkCancelButton!, window),
                    frameCount: 12);
                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(showOkCancelButton!, window),
                    frameCount: 16);
                SaveTourFrame(window, frameDirectory, frameIndex++, GetControlCenter(showOkCancelButton!, window), isPressed: true);
                await WaitForRenderAsync();

                var messageBox = new AsciiMessageBoxWindow(
                    "About DTC.AsciiTheme",
                    "A retro Avalonia theme with classic utility-style controls, dialogs, and file pickers.",
                    AsciiMessageBoxButtons.OkCancel);

                var messageBoxTask = messageBox.ShowDialog<AsciiMessageBoxResult>(window);

                try
                {
                    await WaitForRenderAsync();
                    await WaitForRenderAsync();

                    var cancelButton = messageBox.GetLogicalDescendants()
                                             .OfType<Button>()
                                             .FirstOrDefault(button => string.Equals(button.Content?.ToString(), "Cancel", StringComparison.OrdinalIgnoreCase));
                    Assert.That(cancelButton, Is.Not.Null, "Expected the message box to expose a Cancel button.");

                    var dialogEntryPoint = new Point(60, 42);
                    frameIndex = await CaptureCompositeCursorMoveFramesAsync(
                        window,
                        messageBox,
                        frameDirectory,
                        frameIndex,
                        dialogEntryPoint,
                        GetChildControlCenter(cancelButton!, messageBox),
                        frameCount: 16);
                    frameIndex = await CaptureCompositeHoldFramesAsync(
                        window,
                        messageBox,
                        frameDirectory,
                        frameIndex,
                        GetChildControlCenter(cancelButton!, messageBox),
                        frameCount: 30,
                        pressOnLastFrame: true);
                }
                finally
                {
                    messageBox.Close(AsciiMessageBoxResult.Cancel);
                    await messageBoxTask;
                }

                frameIndex = await CaptureTabTransitionAsync(window, tabControl, moreTab, buttonsTab, frameDirectory, frameIndex);
                frameIndex = await CaptureWindowHoldFramesAsync(
                    window,
                    frameDirectory,
                    frameIndex,
                    GetControlCenter(buttonsTab, window),
                    frameCount: 22);
            }
            finally
            {
                window.Close();
            }

            return 0;
        }, CancellationToken.None);

        try
        {
            BuildTourMedia(frameDirectory, mp4File, gifFile);
        }
        finally
        {
            DeleteIfExists(mp4File);
            DeleteDirectoryIfExists(frameDirectory);
        }
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

    private static void SaveCompositeScreenshot(Window ownerWindow, Window childWindow, string fileName)
    {
        using var ownerBitmap = CaptureWindowBitmap(ownerWindow);
        using var childBitmap = CaptureWindowBitmap(childWindow);

        using var surface = SKSurface.Create(new SKImageInfo(ownerBitmap.Width, ownerBitmap.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        Assert.That(surface, Is.Not.Null, $"Expected to create a drawing surface for screenshot '{fileName}'.");

        var canvas = surface!.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(ownerBitmap, 0, 0);

        var childOffset = GetChildWindowOffset(ownerBitmap, childBitmap);
        canvas.DrawBitmap(childBitmap, childOffset.X, childOffset.Y);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        var file = new FileInfo(Path.Combine(ImageDirectory.FullName, fileName));
        using (var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
        {
            data.SaveTo(stream);
        }

        file.Refresh();
        Assert.That(file.Exists, Is.True, $"Expected screenshot '{file.FullName}' to be written.");
        Assert.That(file.Length, Is.GreaterThan(0L), $"Expected screenshot '{file.FullName}' to contain data.");
    }

    private static async Task<int> CaptureTabTransitionAsync(
        Window window,
        TabControl tabControl,
        TabItem sourceTab,
        TabItem targetTab,
        DirectoryInfo frameDirectory,
        int frameIndex)
    {
        var source = GetControlCenter(sourceTab, window);
        var target = GetControlCenter(targetTab, window);

        frameIndex = await CaptureCursorMoveFramesAsync(window, frameDirectory, frameIndex, source, target, frameCount: 10);

        tabControl.SelectedItem = targetTab;
        targetTab.Focus();
        await WaitForRenderAsync();

        SaveTourFrame(window, frameDirectory, frameIndex++, target, isPressed: true);
        await WaitForRenderAsync();
        return frameIndex;
    }

    private static async Task<int> CaptureWindowHoldFramesAsync(
        Window window,
        DirectoryInfo frameDirectory,
        int frameIndex,
        Point cursorPosition,
        int frameCount)
    {
        for (var i = 0; i < frameCount; i++)
        {
            SaveTourFrame(window, frameDirectory, frameIndex++, cursorPosition, isPressed: false);
            await WaitForRenderAsync();
        }
        return frameIndex;
    }

    private static async Task<int> CaptureCompositeHoldFramesAsync(
        Window ownerWindow,
        Window childWindow,
        DirectoryInfo frameDirectory,
        int frameIndex,
        Point childCursorPosition,
        int frameCount,
        bool pressOnLastFrame)
    {
        for (var i = 0; i < frameCount; i++)
        {
            SaveCompositeTourFrame(
                ownerWindow,
                childWindow,
                frameDirectory,
                frameIndex++,
                childCursorPosition,
                isPressed: pressOnLastFrame && i == frameCount - 1);
            await WaitForRenderAsync();
        }
        return frameIndex;
    }

    private static async Task<int> CaptureCursorMoveFramesAsync(
        Window window,
        DirectoryInfo frameDirectory,
        int frameIndex,
        Point start,
        Point end,
        int frameCount)
    {
        for (var i = 0; i < frameCount; i++)
        {
            var progress = CalculateEaseProgress(i, frameCount);
            var x = start.X + ((end.X - start.X) * progress);
            var y = start.Y + ((end.Y - start.Y) * progress);
            var arc = Math.Sin(progress * Math.PI) * 12;
            y -= arc;

            SaveTourFrame(window, frameDirectory, frameIndex++, new Point(x, y), isPressed: false);
            await WaitForRenderAsync();
        }
        return frameIndex;
    }

    private static async Task<int> CaptureCursorMoveFramesWithMenuOverlayAsync(
        Window window,
        DirectoryInfo frameDirectory,
        int frameIndex,
        Point start,
        Point end,
        int frameCount,
        int highlightedIndex)
    {
        for (var i = 0; i < frameCount; i++)
        {
            var progress = CalculateEaseProgress(i, frameCount);
            var x = start.X + ((end.X - start.X) * progress);
            var y = start.Y + ((end.Y - start.Y) * progress);
            var arc = Math.Sin(progress * Math.PI) * 10;
            y -= arc;

            SaveTourFrameWithMenuOverlay(window, frameDirectory, frameIndex++, new Point(x, y), isPressed: false, highlightedIndex);
            await WaitForRenderAsync();
        }

        return frameIndex;
    }

    private static async Task<int> CaptureWindowHoldFramesWithMenuOverlayAsync(
        Window window,
        DirectoryInfo frameDirectory,
        int frameIndex,
        Point cursorPosition,
        int frameCount,
        int highlightedIndex,
        bool pressOnLastFrame)
    {
        for (var i = 0; i < frameCount; i++)
        {
            SaveTourFrameWithMenuOverlay(
                window,
                frameDirectory,
                frameIndex++,
                cursorPosition,
                isPressed: pressOnLastFrame && i == frameCount - 1,
                highlightedIndex);
            await WaitForRenderAsync();
        }

        return frameIndex;
    }

    private static async Task<int> CaptureCompositeCursorMoveFramesAsync(
        Window ownerWindow,
        Window childWindow,
        DirectoryInfo frameDirectory,
        int frameIndex,
        Point start,
        Point end,
        int frameCount)
    {
        for (var i = 0; i < frameCount; i++)
        {
            var progress = CalculateEaseProgress(i, frameCount);
            var x = start.X + ((end.X - start.X) * progress);
            var y = start.Y + ((end.Y - start.Y) * progress);
            var arc = Math.Sin(progress * Math.PI) * 10;
            y -= arc;

            SaveCompositeTourFrame(
                ownerWindow,
                childWindow,
                frameDirectory,
                frameIndex++,
                new Point(x, y),
                isPressed: false);
            await WaitForRenderAsync();
        }

        return frameIndex;
    }

    private static void SaveTourFrame(Window window, DirectoryInfo frameDirectory, int frameIndex, Point cursorPosition, bool isPressed)
    {
        using var bitmap = CaptureWindowBitmap(window);
        DrawCursor(bitmap, cursorPosition, isPressed);
        SaveBitmap(bitmap, frameDirectory, frameIndex);
    }

    private static void SaveTourFrameWithMenuOverlay(
        Window window,
        DirectoryInfo frameDirectory,
        int frameIndex,
        Point cursorPosition,
        bool isPressed,
        int highlightedIndex)
    {
        using var bitmap = CaptureWindowBitmap(window);
        DrawFileMenuOverlay(bitmap, highlightedIndex);
        DrawCursor(bitmap, cursorPosition, isPressed);
        SaveBitmap(bitmap, frameDirectory, frameIndex);
    }

    private static void SaveCompositeTourFrame(
        Window ownerWindow,
        Window childWindow,
        DirectoryInfo frameDirectory,
        int frameIndex,
        Point childCursorPosition,
        bool isPressed)
    {
        using var ownerBitmap = CaptureWindowBitmap(ownerWindow);
        using var childBitmap = CaptureWindowBitmap(childWindow);
        var childOffset = GetChildWindowOffset(ownerBitmap, childBitmap);

        using var surface = SKSurface.Create(new SKImageInfo(ownerBitmap.Width, ownerBitmap.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        Assert.That(surface, Is.Not.Null, $"Expected to create a drawing surface for tour frame {frameIndex}.");

        var canvas = surface!.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(ownerBitmap, 0, 0);
        canvas.DrawBitmap(childBitmap, childOffset.X, childOffset.Y);

        using var image = surface.Snapshot();
        using var composite = SKBitmap.FromImage(image);
        var cursorPosition = new Point(childOffset.X + childCursorPosition.X, childOffset.Y + childCursorPosition.Y);
        DrawCursor(composite, cursorPosition, isPressed);
        SaveBitmap(composite, frameDirectory, frameIndex);
    }

    private static void SaveBitmap(SKBitmap bitmap, DirectoryInfo directory, int frameIndex)
    {
        directory.Create();

        var file = new FileInfo(Path.Combine(directory.FullName, $"frame-{frameIndex:D4}.png"));
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using (var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
        {
            data.SaveTo(stream);
        }

        file.Refresh();
        Assert.That(file.Exists, Is.True, $"Expected tour frame '{file.FullName}' to be written.");
        Assert.That(file.Length, Is.GreaterThan(0L), $"Expected tour frame '{file.FullName}' to contain data.");
    }

    private static void DrawCursor(SKBitmap bitmap, Point cursorPosition, bool isPressed)
    {
        using var canvas = new SKCanvas(bitmap);
        canvas.Save();
        canvas.Translate((float)cursorPosition.X, (float)cursorPosition.Y);

        using var outlinePaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        using var fillPaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        using var clickPaint = new SKPaint
        {
            Color = SKColors.Yellow,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };

        using var shadowPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 96),
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        var shadowPath = CreateCursorPath(1, 1);
        var outlinePath = CreateCursorPath(0, 0);
        var fillPath = CreateCursorPath(0, 0, inset: 1);

        canvas.DrawPath(shadowPath, shadowPaint);
        canvas.DrawPath(outlinePath, outlinePaint);
        canvas.DrawPath(fillPath, fillPaint);

        if (isPressed)
        {
            canvas.DrawCircle(4, 4, 9, clickPaint);
        }

        canvas.Restore();
    }

    private static void DrawFileMenuOverlay(SKBitmap bitmap, int highlightedIndex)
    {
        using var canvas = new SKCanvas(bitmap);

        var overlayRect = new SKRect(8, 24, 152, 96);
        var itemHeight = 24f;
        var fontSize = 16f;

        using var borderPaint = new SKPaint
        {
            Color = new SKColor(0x55, 0xFF, 0xFF),
            IsAntialias = false,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
        };

        using var backgroundPaint = new SKPaint
        {
            Color = new SKColor(0x00, 0x00, 0xAA),
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        using var highlightPaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
        };

        using var textPaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = false,
            Typeface = LoadVgaTypeface(),
            TextSize = fontSize,
        };

        using var highlightTextPaint = new SKPaint
        {
            Color = new SKColor(0x00, 0x00, 0xAA),
            IsAntialias = false,
            Typeface = LoadVgaTypeface(),
            TextSize = fontSize,
        };

        canvas.DrawRect(overlayRect, backgroundPaint);
        canvas.DrawRect(overlayRect, borderPaint);

        var items = new[] { "Open", "Save", "Exit" };
        for (var i = 0; i < items.Length; i++)
        {
            var itemRect = new SKRect(
                overlayRect.Left + 1,
                overlayRect.Top + (i * itemHeight) + 1,
                overlayRect.Right - 1,
                overlayRect.Top + ((i + 1) * itemHeight) + 1);

            if (i == highlightedIndex)
            {
                canvas.DrawRect(itemRect, highlightPaint);
            }

            var paint = i == highlightedIndex ? highlightTextPaint : textPaint;
            canvas.DrawText(items[i], overlayRect.Left + 10, overlayRect.Top + 18 + (i * itemHeight), paint);
        }
    }

    private static SKPath CreateCursorPath(float offsetX, float offsetY, float inset = 0)
    {
        var path = new SKPath();
        path.MoveTo(offsetX + inset, offsetY + inset);
        path.LineTo(offsetX + inset, offsetY + 18 - inset);
        path.LineTo(offsetX + 4, offsetY + 14);
        path.LineTo(offsetX + 7, offsetY + 22 - inset);
        path.LineTo(offsetX + 10 - inset, offsetY + 21 - inset);
        path.LineTo(offsetX + 7, offsetY + 13);
        path.LineTo(offsetX + 14 - inset, offsetY + 13 - inset);
        path.Close();
        return path;
    }

    private static double CalculateEaseProgress(int frameIndex, int frameCount)
    {
        if (frameCount <= 1)
        {
            return 1.0;
        }

        var linear = (double)frameIndex / (frameCount - 1);
        return 0.5 - (Math.Cos(linear * Math.PI) / 2);
    }

    private static SKTypeface LoadVgaTypeface()
    {
        return VgaFontFile.Exists
            ? SKTypeface.FromFile(VgaFontFile.FullName)
            : SKTypeface.Default;
    }

    private static Point GetControlCenter(Control control, Visual relativeTo)
    {
        var translated = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            relativeTo);

        Assert.That(translated, Is.Not.Null, $"Expected to translate control '{control.GetType().Name}' to the target visual.");
        return translated!.Value;
    }

    private static Point GetChildControlCenter(Control control, Window childWindow)
    {
        return GetControlCenter(control, childWindow);
    }

    private static SKPointI GetChildWindowOffset(SKBitmap ownerBitmap, SKBitmap childBitmap)
    {
        var childX = Math.Max(24, (ownerBitmap.Width - childBitmap.Width) / 2);
        var childY = Math.Max(32, (ownerBitmap.Height - childBitmap.Height) / 3);
        return new SKPointI(childX, childY);
    }

    private static void PrepareFrameDirectory(DirectoryInfo frameDirectory)
    {
        if (frameDirectory.Exists)
        {
            frameDirectory.Delete(true);
        }

        frameDirectory.Create();
    }

    private static void BuildTourMedia(DirectoryInfo frameDirectory, FileInfo mp4File, FileInfo gifFile)
    {
        Assert.That(FfmpegExecutable.Exists, Is.True, $"Expected ffmpeg at '{FfmpegExecutable.FullName}'.");

        RunFfmpeg(
            $"-y -framerate 12 -i \"{Path.Combine(frameDirectory.FullName, "frame-%04d.png")}\" -pix_fmt yuv420p -vf \"fps=12\" \"{mp4File.FullName}\"");

        RunFfmpeg(
            $"-y -i \"{mp4File.FullName}\" -vf \"fps=12,split[s0][s1];[s0]palettegen=stats_mode=full[p];[s1][p]paletteuse=dither=bayer:bayer_scale=3\" -loop 0 \"{gifFile.FullName}\"");

        mp4File.Refresh();
        gifFile.Refresh();

        Assert.That(mp4File.Exists, Is.True, $"Expected video '{mp4File.FullName}' to be created.");
        Assert.That(mp4File.Length, Is.GreaterThan(0L), $"Expected video '{mp4File.FullName}' to contain data.");
        Assert.That(gifFile.Exists, Is.True, $"Expected GIF '{gifFile.FullName}' to be created.");
        Assert.That(gifFile.Length, Is.GreaterThan(0L), $"Expected GIF '{gifFile.FullName}' to contain data.");
    }

    private static void DeleteIfExists(FileInfo file)
    {
        if (!file.Exists)
        {
            return;
        }

        file.Delete();
    }

    private static void DeleteDirectoryIfExists(DirectoryInfo directory)
    {
        if (!directory.Exists)
        {
            return;
        }

        directory.Delete(true);
    }

    private static void RunFfmpeg(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = FfmpegExecutable.FullName,
            Arguments = arguments,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo);
        Assert.That(process, Is.Not.Null, "Expected ffmpeg process to start.");

        process!.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        Assert.That(
            process.ExitCode,
            Is.EqualTo(0),
            $"Expected ffmpeg to succeed.{Environment.NewLine}{output}{Environment.NewLine}{error}");
    }

    private static SKBitmap CaptureWindowBitmap(Window window)
    {
        using var frame = window.CaptureRenderedFrame();
        Assert.That(frame, Is.Not.Null, $"Expected a rendered frame for window '{window.GetType().Name}'.");

        using var stream = new MemoryStream();
        frame!.Save(stream);
        stream.Position = 0;

        var bitmap = SKBitmap.Decode(stream);
        Assert.That(bitmap, Is.Not.Null, $"Expected to decode the rendered frame for window '{window.GetType().Name}'.");
        return bitmap!;
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
