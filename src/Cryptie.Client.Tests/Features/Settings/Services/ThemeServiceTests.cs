using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Cryptie.Client.Features.Settings.Models;
using Cryptie.Client.Features.Settings.Services;

namespace Cryptie.Client.Tests.Features.Settings.Services;

public sealed class ThemeServiceTests : IDisposable
{
    private readonly string _appDataFolder;
    private bool _disposed;

    public ThemeServiceTests()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _appDataFolder = Path.Combine(appData, "Cryptie");
        if (Directory.Exists(_appDataFolder))
            Directory.Delete(_appDataFolder, recursive: true);
    }

    ~ThemeServiceTests()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing && Directory.Exists(_appDataFolder))
        {
            Directory.Delete(_appDataFolder, recursive: true);
        }

        _disposed = true;
    }

    private static ThemeService CreateUninitialized()
    {
        return (ThemeService)RuntimeHelpers.GetUninitializedObject(typeof(ThemeService));
    }

    private static string ComputeUserHash()
    {
        var mi = typeof(ThemeService)
            .GetMethod("ComputeSha256Hash", BindingFlags.Static | BindingFlags.NonPublic)!;
        return (string)mi.Invoke(null, [Environment.UserName])!;
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
        const string userHash = "user-hash";
        mapField.SetValue(svc, new Dictionary<string, SettingsModel> { [userHash] = dummyModel });

        var filePathField = typeof(ThemeService)
            .GetField("_filePath", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var tmp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        filePathField.SetValue(svc, tmp);

        var saveMi = typeof(ThemeService)
            .GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic)!;
        saveMi.Invoke(svc, null);

        Assert.True(File.Exists(tmp), "Expected settings file to be created");
        var json = File.ReadAllText(tmp);
        var rehydrated = JsonSerializer.Deserialize<Dictionary<string, SettingsModel>>(json)!;
        Assert.Single(rehydrated);
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
        mapField.SetValue(svc, new Dictionary<string, SettingsModel> { [userHash] = model });
        modelField.SetValue(svc, model);

        var filePathField = typeof(ThemeService)
            .GetField("_filePath", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var tmp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        filePathField.SetValue(svc, tmp);

        model.SelectedTheme = "Second";
        var saveMi = typeof(ThemeService)
            .GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic)!;
        saveMi.Invoke(svc, null);

        var json = File.ReadAllText(tmp);
        var rehydrated = JsonSerializer.Deserialize<Dictionary<string, SettingsModel>>(json)!;
        Assert.Equal("Second", rehydrated[userHash].SelectedTheme);
    }

    [Fact]
    public void Constructor_NoExistingFile_CreatesDefaultEntry_BeforeApplyTheme()
    {
        try
        {
            _ = new ThemeService();
        }
        catch (NullReferenceException)
        {
            // ignore ApplyTheme NRE
        }

        var settingsFile = Path.Combine(_appDataFolder, "settings.json");
        Assert.True(File.Exists(settingsFile), "settings.json should exist");

        var json = File.ReadAllText(settingsFile);
        var map  = JsonSerializer.Deserialize<Dictionary<string, SettingsModel>>(json)!;
        Assert.Single(map);

        var userHash = ComputeUserHash();
        Assert.True(map.ContainsKey(userHash));
        Assert.Equal("Default", map[userHash].SelectedTheme);
    }

    [Fact]
    public void Constructor_WithExistingFile_LoadsIt_AndDoesNotOverwrite()
    {
        Directory.CreateDirectory(_appDataFolder);
        var settingsFile = Path.Combine(_appDataFolder, "settings.json");

        var userHash = ComputeUserHash();
        var preset = new Dictionary<string, SettingsModel>
        {
            [userHash] = new SettingsModel { SelectedTheme = "Dark" }
        };
        File.WriteAllText(settingsFile,
            JsonSerializer.Serialize(preset, new JsonSerializerOptions { WriteIndented = true }));

        try
        {
            _ = new ThemeService();
        }
        catch (NullReferenceException)
        {
            // ignore ApplyTheme NRE
        }

        var json = File.ReadAllText(settingsFile);
        var map  = JsonSerializer.Deserialize<Dictionary<string, SettingsModel>>(json)!;
        Assert.Single(map);
        Assert.Equal("Dark", map[userHash].SelectedTheme);
    }
}