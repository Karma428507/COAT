namespace COAT.IO;

using COAT.Gamemode;
using COAT.UI.Menus;

using Discord;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary> To manage saved mod data </summary>
public static class SaveManager
{
    public struct ServerOptions
    {
        // General
        public string Name;
        public bool Cheats;
        public bool Mods;
        public short MaxPlayers;
        public byte ServerType;
        public string Gamemode;

        // Remove when working on gamemodes
        public bool PvP;
        public bool HealBosses;
    }

    private static Dictionary<string, object> lobbyGeneral = new Dictionary<string, object>()
    {
        {"name", $"{SteamClient.Name}'s Lobby"},
        {"cheats", false},
        {"mods", true},
        {"maxplayers", 8},
        {"servertype", 2},
        {"gamemode", "COAT:Normal"},
    };

    private static Dictionary<string, object> gamemodeSettings;

    static PrefsManager pm => PrefsManager.Instance;

    public static void Load()
    {
        // Get the normal lobby info
        LoadLobby();

        // Detect the gamemode and load it's data
        string[] gamemodeInfo = ((string)lobbyGeneral["gamemode"]).Split(":");
        Log.Debug($"Mod: {gamemodeInfo[0]}, Name: {gamemodeInfo[1]}");

        if (gamemodeInfo[0] == "COAT")
        {

        }
        else
            Log.Error("Third party gamemodes are not supported at this time");
    }

    #region Lobby Data

    public static void LoadLobby()
    {
        string[] keyList = lobbyGeneral.Keys.ToArray();

        foreach (string keyPartial in keyList)
        {
            string key = $"COAT-lobby-{keyPartial}";
            object obj = lobbyGeneral[keyPartial];

            // I love C# having the weirdest features
            switch (obj)
            {
                case string s:
                    lobbyGeneral[keyPartial] = pm.GetString(key, (string)lobbyGeneral[keyPartial]);
                    break;
                case int i:
                    lobbyGeneral[keyPartial] = pm.GetInt(key, (int)lobbyGeneral[keyPartial]);
                    break;
                case bool b:
                    lobbyGeneral[keyPartial] = pm.GetBool(key, (bool)lobbyGeneral[keyPartial]);
                    break;
                default:
                    Log.Error($"Read an unexpected value type \"{obj.GetType()}\"");
                    break;
            }
        }
    }

    public static void SaveLobby()
    {
        string[] keyList = lobbyGeneral.Keys.ToArray();

        foreach (string keyPartial in keyList)
        {
            string key = $"COAT-lobby-{keyPartial}";
            object obj = lobbyGeneral[keyPartial];

            switch (obj)
            {
                case string s:
                    pm.SetString(key, (string)lobbyGeneral[keyPartial]);
                    break;
                case int i:
                    pm.SetInt(key, (int)lobbyGeneral[keyPartial]);
                    break;
                case bool b:
                    pm.SetBool(key, (bool)lobbyGeneral[keyPartial]);
                    break;
                default:
                    Log.Error($"Read an unexpected value type \"{obj.GetType()}\"");
                    break;
            }
        }
    }

    #endregion
}
