using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
using Cryptie.Client.Features.Settings.Models;

namespace Cryptie.Client.Features.Settings.Services;

internal class ThemeService : IThemeService
{
    private const string FileName = "settings.json";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly string _filePath;
    private readonly SettingsModel _model;
    private readonly Dictionary<string, SettingsModel> _settingsMap;

    public ThemeService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appData, "Cryptie");
        Directory.CreateDirectory(folder);
        _filePath = Path.Combine(folder, FileName);

        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _settingsMap = JsonSerializer.Deserialize<Dictionary<string, SettingsModel>>(json, JsonOptions)
                           ?? new Dictionary<string, SettingsModel>();
        }
        else
        {
            _settingsMap = new Dictionary<string, SettingsModel>();
        }

        var username = Environment.UserName;
        var userHash = ComputeSha256Hash(username);

        if (!_settingsMap.TryGetValue(userHash, out var model))
        {
            model = new SettingsModel();
            _settingsMap[userHash] = model;
            Save();
        }

        _model = model;

        ApplyTheme(_model.SelectedTheme);
    }


    public IReadOnlyList<string> AvailableThemes { get; }
        = new List<string> { "Default", "Light", "Dark" };

    public string CurrentTheme
    {
        get => _model.SelectedTheme;
        set
        {
            if (value == _model.SelectedTheme)
            {
                return;
            }

            _model.SelectedTheme = value;
            ApplyTheme(value);
            Save();
        }
    }

    private static void ApplyTheme(string theme)
    {
        Application.Current!.RequestedThemeVariant = theme switch
        {
            "Light" => ThemeVariant.Light,
            "Dark" => ThemeVariant.Dark,
            _ => null
        };
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_settingsMap, JsonOptions);
        File.WriteAllText(_filePath, json);
    }

    private static string ComputeSha256Hash(string raw)
    {
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = SHA256.HashData(bytes);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}