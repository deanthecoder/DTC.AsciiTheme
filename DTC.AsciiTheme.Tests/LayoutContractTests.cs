using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Avalonia.Threading;

namespace DTC.AsciiTheme.Tests;

[TestFixture]
public sealed class LayoutContractTests
{
    [Test]
    public async Task DemoControlsKeepExpectedHeights()
    {
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

                var tabControl = window.FindControl<TabControl>("DemoTabControl");
                Assert.That(tabControl, Is.Not.Null, "Expected the demo window to expose DemoTabControl.");

                var tabItems = tabControl!.GetLogicalDescendants()
                                         .OfType<TabItem>()
                                         .ToList();

                var inputsTab = tabItems.Single(item => NormalizeHeader(item.Header) == "inputs");
                var textTab = tabItems.Single(item => NormalizeHeader(item.Header) == "text");

                tabControl.SelectedItem = inputsTab;
                inputsTab.Focus();
                await WaitForRenderAsync();

                var inputTextBoxes = window.GetLogicalDescendants()
                                           .OfType<TextBox>()
                                           .Where(control => control.IsVisible)
                                           .ToList();

                var demoInputTextBox = inputTextBoxes.First(control => control.Width >= 300);

                tabControl.SelectedItem = textTab;
                textTab.Focus();
                await WaitForRenderAsync();

                var labelDemoTextBox = window.FindControl<TextBox>("LabelDemoTextBox");
                var labels = window.GetLogicalDescendants()
                                   .OfType<Label>()
                                   .Where(control => control.IsVisible)
                                   .ToList();

                var fileNameLabel = labels.First(label => string.Equals(label.Content?.ToString(), "_File name", StringComparison.Ordinal));

                Assert.Multiple(() =>
                {
                    Assert.That(demoInputTextBox.Bounds.Height, Is.EqualTo(34).Within(1), "Inputs tab TextBox height drifted.");
                    Assert.That(labelDemoTextBox, Is.Not.Null, "Expected the text demo textbox to exist.");
                    Assert.That(labelDemoTextBox!.Bounds.Height, Is.EqualTo(34).Within(1), "Text tab demo TextBox height drifted.");
                    Assert.That(fileNameLabel.Bounds.Height, Is.EqualTo(16).Within(1), "Text tab Label height drifted.");
                });
            }
            finally
            {
                window.Close();
            }

            return 0;
        }, CancellationToken.None);
    }

    [Test]
    public async Task MultilineTextBoxKeepsScrollbarGutter()
    {
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

                var tabControl = window.FindControl<TabControl>("DemoTabControl");
                Assert.That(tabControl, Is.Not.Null, "Expected the demo window to expose DemoTabControl.");

                var inputsTab = tabControl!.GetLogicalDescendants()
                                          .OfType<TabItem>()
                                          .Single(item => NormalizeHeader(item.Header) == "inputs");

                tabControl.SelectedItem = inputsTab;
                inputsTab.Focus();
                await WaitForRenderAsync();

                var multilineTextBox = window.GetLogicalDescendants()
                                             .OfType<TextBox>()
                                             .Where(control => control.IsVisible)
                                             .First(control => control.AcceptsReturn && control.Height >= 120);

                var scrollViewer = multilineTextBox.GetVisualDescendants()
                                                   .OfType<ScrollViewer>()
                                                   .FirstOrDefault(control => control.Name == "PART_ScrollViewer");

                Assert.That(scrollViewer, Is.Not.Null, "Expected multiline TextBox to expose PART_ScrollViewer.");
                AssertScrollViewerGutter(scrollViewer, "Multiline TextBox");
            }
            finally
            {
                window.Close();
            }

            return 0;
        }, CancellationToken.None);
    }

    [Test]
    public async Task ListsTabKeepsScrollbarGutter()
    {
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

                var tabControl = window.FindControl<TabControl>("DemoTabControl");
                Assert.That(tabControl, Is.Not.Null, "Expected the demo window to expose DemoTabControl.");

                var listsTab = tabControl!.GetLogicalDescendants()
                                         .OfType<TabItem>()
                                         .Single(item => NormalizeHeader(item.Header) == "lists");

                tabControl.SelectedItem = listsTab;
                listsTab.Focus();
                await WaitForRenderAsync();

                var listBox = window.GetLogicalDescendants()
                                    .OfType<ListBox>()
                                    .Where(control => control.IsVisible)
                                    .First();

                var scrollViewer = listBox.GetVisualDescendants()
                                          .OfType<ScrollViewer>()
                                          .FirstOrDefault();

                Assert.That(scrollViewer, Is.Not.Null, "Expected list box to expose PART_ScrollViewer.");
                AssertScrollViewerGutter(scrollViewer!, "Lists tab ListBox", expectHorizontalScrollbar: false);
            }
            finally
            {
                window.Close();
            }

            return 0;
        }, CancellationToken.None);
    }

    [Test]
    public async Task TreeTabKeepsScrollbarGutter()
    {
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

                var tabControl = window.FindControl<TabControl>("DemoTabControl");
                Assert.That(tabControl, Is.Not.Null, "Expected the demo window to expose DemoTabControl.");

                var treeTab = tabControl!.GetLogicalDescendants()
                                        .OfType<TabItem>()
                                        .Single(item => NormalizeHeader(item.Header) == "tree");

                tabControl.SelectedItem = treeTab;
                treeTab.Focus();
                await WaitForRenderAsync();

                var treeView = window.GetLogicalDescendants()
                                     .OfType<TreeView>()
                                     .Where(control => control.IsVisible)
                                     .First();

                var scrollViewer = treeView.GetVisualDescendants()
                                           .OfType<ScrollViewer>()
                                           .FirstOrDefault();

                Assert.That(scrollViewer, Is.Not.Null, "Expected tree view to expose PART_ScrollViewer.");
                AssertScrollViewerGutter(scrollViewer!, "Tree tab TreeView", expectHorizontalScrollbar: false);
            }
            finally
            {
                window.Close();
            }

            return 0;
        }, CancellationToken.None);
    }

    private static void AssertScrollViewerGutter(
        ScrollViewer scrollViewer,
        string controlName,
        bool expectHorizontalScrollbar = true)
    {
        var contentPresenter = scrollViewer.GetVisualDescendants()
                                           .OfType<ScrollContentPresenter>()
                                           .FirstOrDefault(control => control.Name == "PART_ContentPresenter");
        var verticalScrollBar = scrollViewer.GetVisualDescendants()
                                            .OfType<ScrollBar>()
                                            .FirstOrDefault(control => control.Name == "PART_VerticalScrollBar");
        var horizontalScrollBar = scrollViewer.GetVisualDescendants()
                                              .OfType<ScrollBar>()
                                              .FirstOrDefault(control => control.Name == "PART_HorizontalScrollBar");

        Assert.Multiple(() =>
        {
            Assert.That(contentPresenter, Is.Not.Null, $"Expected {controlName} content presenter.");
            Assert.That(verticalScrollBar, Is.Not.Null, $"Expected {controlName} vertical scrollbar.");
            if (expectHorizontalScrollbar)
            {
                Assert.That(horizontalScrollBar, Is.Not.Null, $"Expected {controlName} horizontal scrollbar.");
            }
        });

        Assert.That(verticalScrollBar!.Bounds.Width, Is.EqualTo(16).Within(1), $"{controlName} vertical scrollbar width drifted.");
        Assert.That(contentPresenter!.Bounds.Right, Is.LessThanOrEqualTo(verticalScrollBar.Bounds.Left + 1), $"{controlName} content overlaps the vertical scrollbar.");

        if (!expectHorizontalScrollbar)
        {
            return;
        }

        Assert.Multiple(() =>
        {
            Assert.That(horizontalScrollBar!.Bounds.Height, Is.EqualTo(16).Within(1), $"{controlName} horizontal scrollbar height drifted.");
            Assert.That(contentPresenter.Bounds.Bottom, Is.LessThanOrEqualTo(horizontalScrollBar.Bounds.Top + 1), $"{controlName} content overlaps the horizontal scrollbar.");
        });
    }

    private static string NormalizeHeader(object header)
    {
        var text = header?.ToString() ?? string.Empty;
        return text.Replace("_", string.Empty, StringComparison.Ordinal)
                   .Replace(" ", string.Empty, StringComparison.Ordinal)
                   .ToLowerInvariant();
    }

    private static async Task WaitForRenderAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Render);
        await Task.Delay(50);
    }
}
