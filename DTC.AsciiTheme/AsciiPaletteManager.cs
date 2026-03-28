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
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace DTC.AsciiTheme;

public static class AsciiPaletteManager
{
    private static readonly Uri PaletteBaseUri = new("avares://DTC.AsciiTheme/");
    private const string PaletteUriPrefix = "avares://DTC.AsciiTheme/Palettes/";

    public static void Apply(Application application, AsciiPalette palette)
    {
        ArgumentNullException.ThrowIfNull(application);

        Apply(application.Styles, palette);
    }

    public static void Apply(Styles styles, AsciiPalette palette)
    {
        ArgumentNullException.ThrowIfNull(styles);

        RemoveExistingPaletteIncludes(styles);
        styles.Add(new StyleInclude(PaletteBaseUri)
        {
            Source = GetUri(palette),
        });
    }

    public static Uri GetUri(AsciiPalette palette)
    {
        var paletteFileName = palette switch
        {
            AsciiPalette.Blue => "Blue.axaml",
            AsciiPalette.Mono => "Mono.axaml",
            AsciiPalette.Green => "Green.axaml",
            AsciiPalette.Plasma => "Plasma.axaml",
            AsciiPalette.Grey => "Grey.axaml",
            AsciiPalette.BBC => "BBC.axaml",
            AsciiPalette.C64 => "C64.axaml",
            AsciiPalette.ZX => "ZX.axaml",
            _ => throw new ArgumentOutOfRangeException(nameof(palette), palette, null),
        };

        return new Uri($"{PaletteUriPrefix}{paletteFileName}");
    }

    private static void RemoveExistingPaletteIncludes(Styles styles)
    {
        for (var index = styles.Count - 1; index >= 0; index--)
        {
            if (styles[index] is not StyleInclude { Source: { } source })
            {
                continue;
            }

            if (source.ToString().StartsWith(PaletteUriPrefix, StringComparison.OrdinalIgnoreCase))
            {
                styles.RemoveAt(index);
            }
        }
    }
}
