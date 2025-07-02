using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Cryptie.Client.Core.Filters;

public static class DigitOnlyInputFilter
{
    public static void Attach(Control control)
    {
        if (control is not TextBox tb)
        {
            throw new ArgumentException("DigitOnlyInputFilter can only be attached to TextBox", nameof(control));
        }

        tb.AddHandler(InputElement.TextInputEvent, OnTextInput,
            RoutingStrategies.Tunnel, true);

        tb.AddHandler(InputElement.KeyDownEvent, OnKeyDown,
            RoutingStrategies.Tunnel, true);

        tb.PastingFromClipboard += OnPastingFromClipboard;
    }

    private static void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        if (!e.Text.All(char.IsDigit))
        {
            e.Handled = true;
        }
    }

    private static async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key != Key.V || !e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                return;
            }

            e.Handled = true;

            if (sender is not TextBox tb)
            {
                return;
            }

            var clipboard = TopLevel.GetTopLevel(tb)?.Clipboard;
            if (clipboard is null)
            {
                return;
            }

            var txt = await clipboard.GetTextAsync();
            if (string.IsNullOrEmpty(txt))
            {
                return;
            }

            var digits = new string(txt.Where(char.IsDigit).ToArray());
            if (digits.Length == 0)
            {
                return;
            }

            InsertTextAtSelection(tb, digits);
        }
        catch (Exception)
        {
            // Swallow exception: do nothing
        }
    }

    private static async void OnPastingFromClipboard(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not TextBox tb)
            {
                return;
            }

            e.Handled = true;

            var clipboard = TopLevel.GetTopLevel(tb)?.Clipboard;
            if (clipboard is null)
            {
                return;
            }

            var txt = await clipboard.GetTextAsync();
            if (string.IsNullOrEmpty(txt))
            {
                return;
            }

            if (txt.Any(c => !char.IsDigit(c)))
            {
                return;
            }

            InsertTextAtSelection(tb, txt);
        }
        catch (Exception)
        {
            // Swallow exception: do nothing
        }
    }

    private static void InsertTextAtSelection(TextBox tb, string text)
    {
        var start = Math.Max(0, tb.SelectionStart);
        var length = Math.Max(0, tb.SelectionEnd - start);
        var original = tb.Text ?? string.Empty;
        var before = original[..start];
        var after = original[(start + length)..];

        tb.Text = before + text + after;
        tb.SelectionStart = before.Length + text.Length;
        tb.SelectionEnd = tb.SelectionStart;
    }
}