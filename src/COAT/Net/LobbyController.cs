namespace COAT.Net;

using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using COAT.Assets;
using COAT.UI;
using COAT.UI.Menus;
//using COAT.Net;
//using COAT.World;
using COAT.IO;

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

    /// <summary> Whether a lobby is creating right now. </summary>
    public static bool CreatingLobby;
    /// <summary> Whether a list of public lobbies is being fetched right now. </summary>
    public static bool FetchingLobbies;

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
    /// <summary> Use later </summary>
    public static bool IsCOATLobby => Lobby?.Data.Any(pair => pair.Key == "mk_lobby") == true;

    /// <summary> Scales health to increase difficulty. </summary>
    public static void ScaleHealth(ref float health) => health *= 1f + Math.Min(Lobby?.MemberCount - 1 ?? 1, 1) * PPP;
    /// <summary> Whether the given lobby is created via Multikill. </summary>
    public static bool IsMultikillLobby(Lobby lobby) => lobby.Data.Any(pair => pair.Key == "mk_lobby");

    /// <summary> Creates the necessary listeners for proper work. </summary>
    public static void Load()
    {
        // get the owner id when entering the lobby
        SteamMatchmaking.OnLobbyEntered += lobby =>
        {
            if (lobby.Owner.Id != 0L) LastOwner = lobby.Owner.Id;

            if (lobby.GetData("banned").Contains(Tools.AccId.ToString()))
            {
                LeaveLobby();
                Bundle.Hud2NS("lobby.banned");
            }
            if (IsMultikillLobby(lobby))
            {
                LeaveLobby();
                Bundle.Hud("lobby.mk");
            }
        };
        // and leave the lobby if the owner has left it
        SteamMatchmaking.OnLobbyMemberLeave += (lobby, member) =>
        {
            if (member.Id == LastOwner) LeaveLobby();
        };

        // put the level name in the lobby data so that it can be seen in the public lobbies list
        Events.OnLoaded += () => Lobby?.SetData("level", MapMap(Tools.Scene));
        // if the player exits to the main menu, then this is equivalent to leaving the lobby
        Events.OnMainMenuLoaded += () => LeaveLobby(false);
        // creates a server if specified
        Events.OnLoadingStarted += () =>
        {
            UI.PopAllStack();

            if (ServerDiffifcultySelect.loadViaServer)
            {
                Log.Debug("Creating server...");
                CreateLobby(GamemodeList.creationLobby);
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

    #region control

    /// <summary> Asynchronously creates a new lobby with custom settings and connects to it. </summary>
    public static void CreateLobby(SudoLobby sudoLobby)
    {
        if (Lobby != null || CreatingLobby) return;
        CreatingLobby = true;

        sudoLobby.Debug();
        SteamMatchmaking.CreateLobbyAsync(8).ContinueWith(task =>
        {
            CreatingLobby = false; IsOwner = true;
            Lobby = task.Result;

            Lobby?.SetJoinable(true);
            switch (sudoLobby.type)
            {
                case 0: Lobby?.SetPrivate(); break;
                case 1: Lobby?.SetFriendsOnly(); break;
                case 2: Lobby?.SetPublic(); break;
            }
            Lobby?.SetData("jaket", "true");
            Lobby?.SetData("name", "[COAT] " + sudoLobby.name);
            Lobby?.SetData("level", MapMap(Tools.Scene));
            Lobby?.SetData("pvp", sudoLobby.pvp ? "True" : "False");
            Lobby?.SetData("cheats", sudoLobby.cheats ? "True" : "False");
            Lobby?.SetData("mods", sudoLobby.modded ? "True" : "False");
            Lobby?.SetData("heal-bosses", sudoLobby.healBosses ? "True" : "False");
            Lobby?.SetData("banned", "");
            Lobby?.SetData("mute", "");
            Lobby?.SetData("BlacklistedMods", string.Join(' ', Settings.PersonalBlacklistedMods));
        });
    }

    /// <summary> Leaves the lobby. If the player is the owner, then all other players will be thrown into the main menu. </summary>
    public static void LeaveLobby(bool loadMainMenu = true)
    {
        Log.Debug("Leaving the lobby...");

        if (Online) // free up resources allocated for packets that have not been sent
        {
            Networking.Server.Close();
            Networking.Client.Close();

            Lobby?.Leave();
            Lobby = null; 
        }

        // load the main menu if the client has left the lobby
        if (!IsOwner && loadMainMenu) Tools.Load("Main Menu");

        Networking.Clear();
        Events.OnLobbyAction.Fire();
    }

    /// <summary> Opens Steam overlay with a selection of a friend to invite to the lobby. </summary>
    public static void InviteFriend() => SteamFriends.OpenGameInviteOverlay(Lobby.Value.Id);

    /// <summary> Asynchronously connects the player to the given lobby. </summary>
    public static void JoinLobby(Lobby lobby)
    {
        if (lobby.GetData("banned").Contains(Tools.AccId.ToString())) { Bundle.Hud2NS("lobby.banned"); return; } // check if ur banned first so u dont accidentally leave the lobby ur in for no reason
        if (Lobby?.Id == lobby.Id) { Bundle.Hud("lobby.join-yourself"); return; }
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
    }

    #endregion
    #region codes

    /// <summary> Copies the lobby code to the clipboard. </summary>
    public static void CopyCode()
    {
        GUIUtility.systemCopyBuffer = Lobby?.Id.ToString();
        if (Online) Bundle.Hud("lobby.copied");
    }

    /// <summary> Joins by the lobby code from the clipboard. </summary>
    public static void JoinByCode()
    {
        if (ulong.TryParse(GUIUtility.systemCopyBuffer, out var code)) JoinLobby(new(code));
        else Bundle.Hud("lobby.failed");
    }

    #endregion
    #region browser

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

    /// <summary> Maps the map name so that it is more understandable to an average player. </summary>
    public static string MapMap(string map) => map switch
    {
        "Tutorial" => "Tutorial",
        "uk_construct" => "Sandbox",
        "Endless" => "Cyber Grind",
        "CreditsMuseum2" => "Museum",
        _ => map.Substring("Level ".Length)
    };

    #endregion
}

public class SudoLobby
{
    public string name;
    public byte type;
    public bool pvp;
    public bool cheats;
    public bool modded;
    public bool healBosses;

    public SudoLobby(bool pvp, bool cheats, bool modded, bool healBosses)
    {
        name = $"{SteamClient.Name}'s Lobby";
        type = 0;
        this.pvp = pvp;
        this.cheats = cheats;
        this.modded = modded;
        this.healBosses = healBosses;
    }

    public SudoLobby() : this(false, false, true, false) { }

    public void Debug()
    {
        Log.Debug($"Sudo lobby {name}\n");
        Log.Debug($"\tType: {type}\n");
        Log.Debug($"\tPvP: {pvp}\n");
        Log.Debug($"\tCheats: {cheats}\n");
        Log.Debug($"\tMods: {modded}\n");
        Log.Debug($"\tHeal: {healBosses}\n");
    }
}