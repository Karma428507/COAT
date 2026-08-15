namespace COAT.IO;

using COAT.Content;
using COAT.Gamemode;
using COAT.Net;
using COAT.UI.Menus;

using Discord;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary> To manage saved mod data </summary>
public static class SaveManager
{
    public static Dictionary<string, object> LobbyGeneral = new Dictionary<string, object>()
    {
        {"name", $"{SteamClient.Name}'s Lobby"},
        {"cheats", false},
        {"mods", true},
        {"maxplayers", 8},
        {"servertype", 2},
        {"gamemode", "COAT:Normal"},

        {"pvp-temp", false},
        {"heal-temp", false},
    };

    private static Dictionary<string, object> gamemodeSettings;

    static PrefsManager pm => PrefsManager.Instance;

    public static void Load()
    {
        // Get the normal lobby info
        LoadLobby();

        // Load the gamemode information
        ReloadGamemodeSettings();
    }

    #region Lobby Data

    public static void LoadLobby()
    {
        string[] keyList = LobbyGeneral.Keys.ToArray();

        foreach (string keyPartial in keyList)
        {
            string key = $"COAT-lobby-{keyPartial}";
            object obj = LobbyGeneral[keyPartial];

            // I love C# having the weirdest features
            switch (obj)
            {
                case string s:
                    LobbyGeneral[keyPartial] = pm.GetString(key, (string)LobbyGeneral[keyPartial]);
                    break;
                case int i:
                    LobbyGeneral[keyPartial] = pm.GetInt(key, (int)LobbyGeneral[keyPartial]);
                    break;
                case bool b:
                    LobbyGeneral[keyPartial] = pm.GetBool(key, (bool)LobbyGeneral[keyPartial]);
                    break;
                default:
                    Log.Error($"Read an unexpected value type \"{obj.GetType()}\"");
                    break;
            }
        }
    }

    public static void SaveLobby()
    {
        string[] keyList = LobbyGeneral.Keys.ToArray();

        foreach (string keyPartial in keyList)
        {
            string key = $"COAT-lobby-{keyPartial}";
            object obj = LobbyGeneral[keyPartial];

            switch (obj)
            {
                case string s:
                    pm.SetString(key, (string)LobbyGeneral[keyPartial]);
                    break;
                case int i:
                    pm.SetInt(key, (int)LobbyGeneral[keyPartial]);
                    break;
                case bool b:
                    pm.SetBool(key, (bool)LobbyGeneral[keyPartial]);
                    break;
                default:
                    Log.Error($"Read an unexpected value type \"{obj.GetType()}\"");
                    break;
            }
        }
    }

    public static void LoadPlayerData()
    {
        // <key, initial output>, this is used for keeping setting the player data
        Dictionary<string, string> memberData = new Dictionary<string, string>()
        {
            {"username", ""},
            {"team", Team.Yellow.ToString() },
            {"team-color", ColorUtility.ToHtmlStringRGBA(TeamExtensions.Color(Team.Yellow))}
        };

        foreach (KeyValuePair<string, string> member in memberData)
            LobbyController.Lobby?.SetMemberData(member.Key,
                pm.GetString($"COAT-player-data-{member.Key}", member.Value));

        foreach (KeyValuePair<string, string> member in memberData)
            Log.Debug($"COAT-player-data-{member.Key}: {pm.GetString($"COAT-player-data-{member.Key}", member.Value)}")
                ;
    }

    public static void SetPlayerData(string key, string value)
    {
        pm.SetString($"COAT-player-data-{key}", value);

        if (LobbyController.Online)
            LobbyController.Lobby?.SetMemberData(key, value);
    }

    #endregion
    #region Gamemode Data

    public static void ReloadGamemodeSettings()
    {
        string[] gamemodeInfo = ((string)LobbyGeneral["gamemode"]).Split(":");
        Log.Debug($"Mod: {gamemodeInfo[0]}, Name: {gamemodeInfo[1]}");

        if (gamemodeInfo[0] == "COAT")
        {
            foreach (Gamemode gm in GamemodeManager.RegisteredGamemodes)
            {
                if (gm.Name == gamemodeInfo[1])
                {
                    gm.GetSettingsCopied();
                    break;
                }
            }
        }
        else
            Log.Error("Third party gamemodes are not supported at this time");
    }

    #endregion
}
