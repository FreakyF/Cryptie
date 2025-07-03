using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Cryptie.Client.Features.Settings.Models;
using Cryptie.Client.Features.Settings.Services;

namespace Cryptie.Client.Tests.Features.Settings.Services;

public class ThemeServiceLogicTests
{
    private static ThemeService CreateUninitialized()
    {
        return (ThemeService)RuntimeHelpers
            .GetUninitializedObject(typeof(ThemeService));
    }

    [Fact]
    public void ComputeSha256Hash_KnownInput_ProducesExpectedHash()
    {
        var mi = typeof(ThemeService)
            .GetMethod("ComputeSha256Hash", BindingFlags.Static | BindingFlags.NonPublic)!;

        var actual = (string)mi.Invoke(null, ["hello"])!;
        const string expected = 
            "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Save_WritesSettingsMapToFile()
    {
        var svc = CreateUninitialized();

        var mapField = typeof(ThemeService)
            .GetField("_settingsMap", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var dummyModel = new SettingsModel { SelectedTheme = "MyTheme" };
        var userHash  = "user-hash";
        var map = new Dictionary<string, SettingsModel> { [userHash] = dummyModel };
        mapField.SetValue(svc, map);

        var filePathField = typeof(ThemeService)
            .GetField("_filePath", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        filePathField.SetValue(svc, tmp);

        var saveMi = typeof(ThemeService)
            .GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic)!;
        saveMi.Invoke(svc, null);

        Assert.True(File.Exists(tmp), "Expected settings file to be created");
        var json = File.ReadAllText(tmp);
        var rehydrated = JsonSerializer.Deserialize<Dictionary<string, SettingsModel>>(json)!;
        Assert.Single(rehydrated);
        Assert.True(rehydrated.ContainsKey(userHash));
        Assert.Equal("MyTheme", rehydrated[userHash].SelectedTheme);
    }

    [Fact]
    public void ChangingModelAndSave_PersistsNewTheme()
    {
        var svc = CreateUninitialized();

        var mapField = typeof(ThemeService)
            .GetField("_settingsMap", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var modelField = typeof(ThemeService)
            .GetField("_model", BindingFlags.Instance | BindingFlags.NonPublic)!;
        const string userHash = "user-42";
        var model = new SettingsModel { SelectedTheme = "First" };
        var map = new Dictionary<string, SettingsModel> { [userHash] = model };
        mapField.SetValue(svc, map);
        modelField.SetValue(svc, model);

        var filePathField = typeof(ThemeService)
            .GetField("_filePath", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        filePathField.SetValue(svc, tmp);

        model.SelectedTheme = "Second";
        var saveMi = typeof(ThemeService)
            .GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic)!;
        saveMi.Invoke(svc, null);

        var json = File.ReadAllText(tmp);
        var rehydrated = JsonSerializer.Deserialize<Dictionary<string, SettingsModel>>(json)!;
        Assert.Equal("Second", rehydrated[userHash].SelectedTheme);
    }
}