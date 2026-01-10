namespace COAT.IO;

using COAT.UI.Menus;
using Discord;
using Steamworks;
using System;
using System.Collections.Generic;
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
        //public GamemodeTypes Gamemode;

        // remove when adding gamemodes
        public bool pvp;
        public bool healBosses;
    }

    private static Dictionary<string, object> lobbyGeneral = new Dictionary<string, object>()
    {
        {"name", $"{SteamClient.Name}'s Lobby"},
        {"cheats", false},
        {"mods", true},
        {"maxplayers", 8},
        {"servertype", 2},
        //{"gamemode", 0},
    };

    static PrefsManager pm => PrefsManager.Instance;

    public static void Load()
    {
        LoadLobby();

        if (false)
            PortOldSave();

        // there's prob a better way of doing this :P
    }

    private static void PortOldSave()
    {
        // work on when reworking how settings is organize
    }

    #region Lobby Data
    public static void SaveLobby()
    {
        pm.SetString("name", ServerCreation.Options.Name);
        pm.SetBool("cheats", ServerCreation.Options.Cheats);
        pm.SetBool("mods", ServerCreation.Options.Mods);
        pm.SetInt("maxplayers", ServerCreation.Options.MaxPlayers);
        pm.SetInt("servertype", ServerCreation.Options.ServerType);
        //pm.SetInt("gamemode", (int)GamemodeList.Options.Gamemode);

        pm.SetBool("pvp", ServerCreation.Options.pvp);
        pm.SetBool("heal", ServerCreation.Options.healBosses);
    }

    public static void LoadLobby()
    {
        ServerCreation.Options.Name = pm.GetString("name", (string)lobbyGeneral["name"]);
        ServerCreation.Options.Cheats = pm.GetBool("cheats", (bool)lobbyGeneral["cheats"]);
        ServerCreation.Options.Mods = pm.GetBool("mods", (bool)lobbyGeneral["mods"]);
        ServerCreation.Options.MaxPlayers = (short)pm.GetInt("maxplayers", (int)lobbyGeneral["maxplayers"]);
        ServerCreation.Options.ServerType = (byte)pm.GetInt("servertype", (int)lobbyGeneral["servertype"]);
        //GamemodeList.Options.Gamemode = (GamemodeTypes)pm.GetInt("gamemode", (int)lobbyGeneral["gamemode"]);

        ServerCreation.Options.pvp = pm.GetBool("pvp", false);
        ServerCreation.Options.healBosses = pm.GetBool("heal", false);
    }

    private static void LobbySetData()
    {
        //if (pm.)
    }
    #endregion
}
