using Avalonia.Controls;
using Avalonia.Input;
using Cryptie.Client.Core.Filters;


namespace Cryptie.Client.Tests.Core.Filters;

public class DigitOnlyInputFilterTests
{
    [Fact]
    public void Attach_Throws_If_Not_TextBox()
    {
        var control = new Button();
        Assert.Throws<ArgumentException>(() => DigitOnlyInputFilter.Attach(control));
    }

    [Fact]
    public void OnTextInput_Handles_NonDigit()
    {
        var args = new TextInputEventArgs { Text = "abc123" };
        InvokeOnTextInput(null, args);
        Assert.True(args.Handled);
    }

    [Fact]
    public void OnTextInput_DoesNotHandle_Digits()
    {
        var args = new TextInputEventArgs { Text = "123456" };
        InvokeOnTextInput(null, args);
        Assert.False(args.Handled);
    }

    [Fact]
    public void OnTextInput_DoesNothing_If_Empty()
    {
        var args = new TextInputEventArgs { Text = null };
        InvokeOnTextInput(null, args);
        Assert.False(args.Handled);
    }

    [Fact]
    public void InsertTextAtSelection_Inserts_Correctly()
    {
        var tb = new TextBox { Text = "abc", SelectionStart = 1, SelectionEnd = 2 };
        InvokePrivateStaticMethod("InsertTextAtSelection", tb, "123");
        Assert.Equal("a123c", tb.Text);
        Assert.Equal(4, tb.SelectionStart);
        Assert.Equal(4, tb.SelectionEnd);
    }
    
    private static void InvokePrivateStaticMethod(string methodName, params object[] parameters)
    {
        var method = typeof(DigitOnlyInputFilter).GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        method!.Invoke(null, parameters);
    }

    private static void InvokeOnTextInput(object? sender, TextInputEventArgs e)
    {
        InvokePrivateStaticMethod("OnTextInput", sender, e);
    }
}
