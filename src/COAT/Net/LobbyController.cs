namespace COAT.Net;

using COAT.Assets;
using COAT.Content;
using COAT.IO;
using COAT.Net.Types;
using COAT.UI;
using COAT.UI.Menus;
using COAT.Utils;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Lobby controller with several useful methods and properties. </summary>
public class LobbyController
{
    /// <summary> The current lobby the player is connected to. Null if the player is not connected to any lobby. </summary>
    public static Lobby? Lobby;
    public static bool Online => Lobby != null;
    public static bool Offline => Lobby == null;

    /// <summary> Id of the last lobby owner, needed to track the exit of the host and for other minor things. </summary>
    public static SteamId LastOwner;
    /// <summary> Whether the player owns the lobby. </summary>
    public static bool IsOwner;
    /// <summary> Returns the clients 'Friend' class. </summary>
    public static Friend? Self => Online ? Lobby?.Members.FirstOrDefault(f => f.IsMe) : null;

    /// <summary> Whether a lobby is creating right now. </summary>
    public static bool CreatingLobby;
    /// <summary> Whether a list of public lobbies is being fetched right now. </summary>
    public static bool FetchingLobbies;

    /// <summary> Current lobby name (for hosts only). </summary>
    public static string ServerName;
    /// <summary> The max amount of players for a server (2 - 16). </summary>
    public static int MaxPlayers = 8;

    /// <summary> Creates the necessary listeners for proper work. </summary>
    public static void Load()
    {
        // get the owner id when entering the lobby
        SteamMatchmaking.OnLobbyEntered += lobby =>
        {
            string client = "";

            if (lobby.Owner.Id != 0L) LastOwner = lobby.Owner.Id;

            if (lobby.GetData("banned").Contains(Tools.AccId.ToString()))
            {
                LeaveLobby();
                Message.Hud2NSLocal("lobby.banned");
            }

            if (!IsCoatClient(lobby, ref client))
            {
                LeaveLobby();
                HudMessageReceiver.Instance?.SendHudMessage($"Server is an {client.ToLower()} server");
            }

            SaveManager.LoadPlayerData();
        };

        // and leave the lobby if the owner has left it
        SteamMatchmaking.OnLobbyMemberLeave += (lobby, member) =>
        {
            if (member.Id == LastOwner) LeaveLobby();
        };

        // put the level name in the lobby data so that it can be seen in the public lobbies list
        Events.OnLoaded += () => Lobby?.SetData("level", Mapping.MapMap(Mapping.Scene));
        // if the player exits to the main menu, then this is equivalent to leaving the lobby
        Events.OnMainMenuLoaded += () => LeaveLobby(false);
        // creates a server if specified
        Events.OnLoadingStarted += () =>
        {
            UI.PopAllStack();

            if (ServerDiffifcultySelect.loadViaServer)
            {
                Log.Debug("Creating server...");
                CreateLobby();
                ServerDiffifcultySelect.loadViaServer = false;
            }
        };
    }

    /// <summary> Is there a user with the given id among the members of the lobby. </summary>
    public static bool Contains(uint id) => Lobby?.Members.Any(member => member.Id.AccountId == id) ?? false;

    /// <summary> Returns the member at the given index or null. </summary>
    public static Friend? At(int index) => Lobby?.Members.ElementAt(Math.Min(Math.Max(index, 0), Lobby.Value.MemberCount));

    /// <summary> Returns the index of the local player in the lits of members. </summary>
    public static int IndexOfLocal() => Lobby?.Members.ToList().FindIndex(member => member.IsMe) ?? 0;

    #region server properties

    /// <summary> Whether PvP is allowed in this lobby. </summary>
    public static bool PvPAllowed => Lobby?.GetData("pvp") == "True";
    /// <summary> Whether cheats are allowed in this lobby. </summary>
    public static bool CheatsAllowed => Lobby?.GetData("cheats") == "True";
    /// <summary> Whether mods are allowed in this lobby. </summary>
    public static bool ModsAllowed => Lobby?.GetData("mods") == "True";
    /// <summary> Whether bosses must be healed after death in this lobby. </summary>
    public static bool HealBosses => Lobby?.GetData("heal-bosses") == "True";
    /// <summary> Number of percentages that will be added to the boss's health for each player. </summary>
    public static float PPP;
    /// <summary> Scales health to increase difficulty. </summary>
    public static void ScaleHealth(ref float health) => health *= 1f + Math.Min(Lobby?.MemberCount - 1 ?? 1, 1) * PPP;

    #endregion
    #region control

    /// <summary> Asynchronously creates a new lobby with custom settings and connects to it. </summary>
    public static void CreateLobby()
    {
        if (Lobby != null || CreatingLobby) return;
        Dictionary<string, object> saveData = SaveManager.LobbyGeneral;
        CreatingLobby = true;

        SteamMatchmaking.CreateLobbyAsync((int)saveData["maxplayers"]).ContinueWith(task =>
        {
            CreatingLobby = false; IsOwner = true;
            Lobby = task.Result;

            Lobby?.SetJoinable(true);

            // Standardized way to differentiate different clients
            Lobby?.SetData("client", "COAT");

            // general non-savable data
            Lobby?.SetData("banned", "");
            Lobby?.SetData("mute", "");
            Lobby?.SetData("blacklisted-mods", string.Join(' ', Settings.PersonalBlacklistedMods));

            // have this data be added manually in the manager
            switch ((int)saveData["servertype"])
            {
                case 0: Lobby?.SetPrivate(); break;
                case 1: Lobby?.SetFriendsOnly(); break;
                case 2: Lobby?.SetPublic(); break;
            }

            // general savable data
            ServerName = (string)saveData["name"];
            Lobby?.SetData("name", "<color=#20AAFF>[COAT]</color> " + (string)saveData["name"]);
            Lobby?.SetData("cheats", (bool)saveData["cheats"] ? "True" : "False");
            Lobby?.SetData("mods", (bool)saveData["mods"] ? "True" : "False");

            // Only normal gamemodes would display the level
            Lobby?.SetData("level", Mapping.MapMap(Mapping.Scene));

            // normal campaign savable data
            Lobby?.SetData("pvp", (bool)saveData["pvp-temp"] ? "True" : "False");
            Lobby?.SetData("heal-bosses", (bool)saveData["heal-temp"] ? "True" : "False");
        });
    }

    /// <summary> Leaves the lobby. If the player is the owner, then all other players will be thrown into the main menu. </summary>
    public static void LeaveLobby(bool loadMainMenu = true)
    {
        if (Online) // free up resources allocated for packets that have not been sent
        {
            Log.Debug("Leaving the lobby...");
            
            Networking.Server.Close();
            Networking.Client.Close();

            Lobby?.Leave();
            Lobby = null; 
        }

        // load the main menu if the client has left the lobby
        if (!IsOwner && loadMainMenu) Mapping.Load("Main Menu");

        Networking.Clear();
        Events.OnLobbyAction.Fire();
    }

    /// <summary> Opens Steam overlay with a selection of a friend to invite to the lobby. </summary>
    public static void InviteFriend() => SteamFriends.OpenGameInviteOverlay(Lobby.Value.Id);

    /// <summary> Asynchronously connects the player to the given lobby. </summary>
    public static void JoinLobby(Lobby lobby)
    {
        if (lobby.GetData("banned").Contains(Tools.AccId.ToString())) { Message.Hud2NSLocal("lobby.banned"); return; } // check if ur banned first so u dont accidentally leave the lobby ur in for no reason
        if (Lobby?.Id == lobby.Id) { Message.HudLocal("lobby.join-yourself"); return; }
        Log.Debug("Joining a lobby...");

        // leave the previous lobby before join the new, but don't load the main menu
        if (Online) LeaveLobby(false);

        lobby.Join().ContinueWith(task =>
        {
            if (task.Result == RoomEnter.Success)
            {
                IsOwner = false;
                Lobby = lobby;
            }
            else Log.Warning($"Couldn't join a lobby. Result is {task.Result}");
        });
        
        SaveManager.SaveLobby();
    }

    #endregion
    #region codes

    /// <summary> Copies the lobby code to the clipboard. </summary>
    public static void CopyCode()
    {
        GUIUtility.systemCopyBuffer = Lobby?.Id.ToString();
        if (Online) Message.HudLocal("lobby.copied");
    }

    /// <summary> Joins by the lobby code from the clipboard. </summary>
    public static void JoinByCode()
    {
        if (ulong.TryParse(GUIUtility.systemCopyBuffer, out var code)) JoinLobby(new(code));
        else Message.HudLocal("lobby.failed");
    }

    #endregion
    #region browser utilities

    /// <summary> Asynchronously fetches a list of public lobbies. </summary>
    public static void FetchLobbies(Action<Lobby[]> done)
    {
        FetchingLobbies = true;
        SteamMatchmaking.LobbyList.RequestAsync().ContinueWith(task =>
        {
            FetchingLobbies = false;
            done(task.Result.ToArray());
        });
    }

    /// <summary> Detects the server's client and gives a string for custom clients. </summary>
    public static bool IsCoatClient(Lobby lobby, ref string client)
    {
        // For clients that follow the jaket standard
        if (lobby.Data.Any(pair => pair.Key == "client"))
        {
            if (lobby.GetData("client") == "COAT")
                return true;

            client = lobby.GetData("client").ToUpper();
            return false;
        }

        // For multikill
        if (lobby.Data.Any(pair => pair.Key == "mk_lobby"))
        {
            client = "MULTIKILL";
            return false;
        }

        // For polarite
        if (lobby.Data.Any(pair => pair.Key == "LobbyName"))
        {
            client = "POLARITE";
            return false;
        }

        client = "UNKNOWN";
        return false;
    }

    #endregion
}
