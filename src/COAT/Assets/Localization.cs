namespace COAT.Assets;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using COAT.IO;
using COAT.UI.Overlay;

/// <summary> Class that loads translations from files in the bundles folder and returns translated lines by keys. </summary>
public class Localization
{
    /// <summary> Language codes used in settings. </summary>
    public static readonly string[] Codes = { "ar", "pt", "en", "fl", "fr", "it", "pl", "ru", "es", "uk" };
    /// <summary> Displayed language name so that everyone can find out their own even without knowledge of English. </summary>
    public static readonly string[] Locales = { "عربي", "Português brasileiro", "English", "Filipino", "Français", "Italiano", "Polski", "Русский", "Español", "Українська" };
    /// <summary> File names containing localization. </summary>
    public static readonly string[] Files = { "arabic", "brazilianportuguese", "english", "filipino", "french", "italian", "polish", "russian", "spanish", "ukrainian" };

    /// <summary> Id of loaded localization. -1 if the localization is not loaded yet. </summary>
    public static int LoadedLocale = -1;
    /// <summary> Dictionary with all lines of loaded localization. </summary>
    private static Dictionary<string, string> props = new();
    /// <summary> Text that will be shown in the hud after scene loading. </summary>
    private static string text2Show;

    /// <summary> Loads the translation specified in the settings. </summary>
    public static void Load()
    {
        #region 2NS

        Events.OnLoaded += () =>
        {
            if (text2Show == null) return;

            HudMessageReceiver.Instance?.SendHudMessage(text2Show);
            text2Show = null;
        };

        #endregion

        string[] lines;
        var locale = PrefsManager.Instance.GetString("jaket.locale", "en");
        int localeId = Array.IndexOf(Codes, locale);

        if (localeId == 255)
        {
            Log.Error($"Couldn't find the bundle for {locale} language code!");
            return;
        }
        
        // Gets the embedded localization file
        lines = EmbeddedManager.GetLinedTextFromEmbedded($"Localization.{Files[localeId]}.properties");
        if (lines == null)
        {
            Log.Error(new IOException($"Couldn't find the embdedded bundle file '{Files[localeId]}.properties'"));
            return;
        }
        
        // Processes the lines
        foreach (var line in lines)
        {
            // skip comments and blank lines
            if (line == "" || line.StartsWith("#")) continue;

            var pair = line.Split('=');
            props.Add(pair[0].Trim(), locale == "ar" ? ParseArabic(pair[1].Trim()) : ParseColors(pair[1].Trim()));
        }

        LoadedLocale = localeId;
        Log.Info($"Loaded {props.Count} lines of {Locales[localeId]} ({locale}) locale");
    }

    /// <summary> Returns a localized line by the key. </summary>
    public static string Get(string key, string fallback = "WHAT") => props.TryGetValue(key, out var line) ? line : fallback;

    /// <summary> Returns a localized & formatted line by the key. </summary>
    public static string Format(string key, params string[] args)
    {
        for (int i = 0; i < args.Length; i++)
            if (args[i].StartsWith("#")) args[i] = Get(args[i].Substring(1), args[i]);

        return string.Format(Get(key), args);
    }

    /// <summary> Sends a localized message to the HUD. </summary>
    public static void Hud(string key, bool silent = false) => HudMessageReceiver.Instance?.SendHudMessage(Get(key), silent: silent);

    /// <summary> Sends a localized & formatted message to the HUD. </summary>
    public static void Hud(string key, bool silent, params string[] args) => HudMessageReceiver.Instance?.SendHudMessage(Format(key, args), silent: silent);

    /// <summary> Sends a localized message to the HUD after scene loading. </summary>
    public static void Hud2NS(string key) => text2Show = Get(key);

    /// <summary> Sends a localized & formatted message to the HUD after scene loading. </summary>
    public static void Hud2NS(string key, params string[] args) => text2Show = Format(key, args);

    /// <summary> Sends a localized message to the chat. </summary>
    public static void Msg(string key) => ChatUI.Instance.Receive(Get(key), false);

    /// <summary> Sends a localized & formatted message to the chat. </summary>
    public static void Msg(string key, params string[] args) => ChatUI.Instance.Receive(Format(key, args), false);
}