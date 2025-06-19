using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
using Cryptie.Client.Features.Messages.Models;
using Cryptie.Client.Features.Messages.ViewModels;

namespace Cryptie.Client.Features.Messages.Services;

internal class ThemeService : IThemeService
{
    private const string FileName = "settings.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly SettingsModel _model;

    public ThemeService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appData, "Cryptie");
        Directory.CreateDirectory(folder);
        _filePath = Path.Combine(folder, FileName);

        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _model = JsonSerializer.Deserialize<SettingsModel>(json, JsonOptions) ?? new SettingsModel();
        }
        else
        {
            _model = new SettingsModel();
        }

        ApplyTheme(_model.SelectedTheme);
    }

    public IReadOnlyList<string> AvailableThemes { get; } = new List<string> { "System", "Light", "Dark" };

    public string CurrentTheme
    {
        get => _model.SelectedTheme;
        set
        {
            if (value == _model.SelectedTheme) return;
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
        var json = JsonSerializer.Serialize(_model, JsonOptions);
        File.WriteAllText(_filePath, json);
    }
}