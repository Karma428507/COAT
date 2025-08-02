namespace COAT.Net;

using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

using COAT.Assets;
using COAT.Content;
using COAT.IO;
using COAT.Net.Endpoints;
using COAT.Net.Types;
using COAT.UI.Menus;
using COAT.UI.Overlays;
using System.Linq;

/// <summary> Class responsible for updating endpoints, transmitting packets and managing entities. </summary>
public class Networking
{
    /// <summary> Number of snapshots to be sent per second. </summary>
    public const int SNAPSHOTS_PER_SECOND = 16;
    /// <summary> Number of seconds between snapshots. </summary>
    public const float SNAPSHOTS_SPACING = 1f / SNAPSHOTS_PER_SECOND;

    /// <summary> Server endpoint. Will be updated by the owner of the lobby. </summary>
    public static Server Server = new();
    /// <summary> Client endpoint. Will be updated by players connected to the lobby. </summary>
    public static Client Client = new();

    /// <summary> List of all entities by their id. May contain null. </summary>
    public static Dictionary<uint, Entity> Entities = new();
    /// <summary> Local player singleton. </summary>
    public static LocalPlayer LocalPlayer;
    /// <summary> The list of COAT players in a lobby </summary>
    public static List<uint> COATPLAYERS = new();
    /// <summary> The list of muted players in a lobby </summary>
    public static List<uint> MUTEDPLAYERS = new();

    /// <summary> Whether a scene is loading right now. </summary>
    public static bool Loading;
    /// <summary> Whether multiplayer was used in the current level. </summary>
    public static bool WasMultiplayerUsed;
    /// <summary> Whether multiplayer was used in the current level. </summary>
    public static bool IsViewing = false;

    /// <summary> Loads server, client and event listeners. </summary>
    public static void Load()
    {
        Server.Load();
        Client.Load();

        // create a local player to sync player data
        LocalPlayer = Tools.Create<LocalPlayer>("Local Player");
        // update network logic every tick
        Events.EveryTick += NetworkUpdate;
        Events.EveryDozen += Optimize;

        Events.OnLoaded += () => WasMultiplayerUsed = LobbyController.Online;
        Events.OnLobbyAction += () => WasMultiplayerUsed |= LobbyController.Online;

        Events.OnLoadingStarted += () =>
        {
            if (LobbyController.Online) SceneHelper.SetLoadingSubtext(UnityEngine.Random.value < .042f ? "<3 I love you :3" : "/// MULTIPLAYER VIA COAT ///");
            Loading = true;
        };
        Events.OnLoaded += () =>
        {
            Clear();
            Loading = false;
        };

        // fires when accepting an invitation via the Steam overlay
        SteamFriends.OnGameLobbyJoinRequested += (lobby, id) =>
        {
            string LobbyBannedData = LobbyController.Lobby?.GetData("banned");
            if (LobbyBannedData.Contains($"{id.AccountId}")) return;

            LobbyController.JoinLobby(lobby);
        };

        SteamMatchmaking.OnLobbyEntered += lobby =>
        {
            if (IsViewing)
                return;

            Clear(); // destroy all entities, since the player could join from another lobby
            if (LobbyController.IsOwner)
            {
                // open the server so people can join it
                Server.Open();
            }
            else
            {
                // establishing a connection with the owner of the lobby
                Client.Connect(lobby.Owner.Id);
                // prevent objects from loading before the scene is loaded
                Loading = true;

                MUTEDPLAYERS.Clear();
                COATPLAYERS.Clear(); // clear list so then u can update it
                COATPLAYERS.Add(Tools.AccId); // add yourself to the list
                Send(PacketType.COAT_Request, w => { w.Id(Tools.AccId); }); // request others id's
            }

            Settings.GetDefaultTeam(out Team team);

            Networking.LocalPlayer.Team = team;
            Events.OnTeamChanged.Fire();

            if (PlayerList.Shown) PlayerList.Instance.Rebuild();
        };

        SteamMatchmaking.OnLobbyMemberJoined += (lobby, member) =>
        {
            string LobbyBannedData = LobbyController.Lobby?.GetData("banned");
            if (LobbyBannedData.Contains($"{member.Id.AccountId}")) return;
            
            Bundle.Msg("player.joined", member.Name);
        };

        SteamMatchmaking.OnLobbyMemberLeave += (lobby, member) =>
        {
            string LobbyBannedData = LobbyController.Lobby?.GetData("banned");
            if (LobbyBannedData.Contains($"{member.Id.AccountId}")) return;

            if (COATPLAYERS.Contains(member.Id.AccountId))
                COATPLAYERS.Remove(member.Id.AccountId);

            Bundle.Msg("player.left", member.Name);
            if (!LobbyController.IsOwner) return;

            // returning the exited player's entities back to the host owner & close the connection
            FindCon(member.Id.AccountId)?.Close();
            EachEntity(entity =>
            {
                //if (entity is OwnableEntity oe && oe.Owner == member.Id.AccountId) oe.TakeOwnage();
                // NETKILL the entity (omg i love that word so muchhhh :3)
                if (entity is RemotePlayer rp && rp.Owner == member.Id.AccountId) rp.NetKill(); // NET KILL IS SUCH A BAD ASS NAME OMG I LOVE YOUUUU
            });
        };

        SteamMatchmaking.OnChatMessage += (lobby, member, message) =>
        {
            string LobbyBannedData = LobbyController.Lobby?.GetData("banned");
            string LobbyMutedData = LobbyController.Lobby?.GetData("mute");
            bool bannedormuted = LobbyBannedData.Contains($"{member.Id.AccountId}") || LobbyMutedData.Contains($"{member.Id.AccountId}");
            if (!bannedormuted)
            {
                if (message.Length > ChatUI.MAX_MESSAGE_LENGTH + 8) message = message.Substring(0, ChatUI.MAX_MESSAGE_LENGTH);

                if (message == "#/d")
                {
                    Bundle.Msg("player.died", member.Name);
                    /*if (LobbyController.HealBosses) EachEntity(entity =>
                    {
                        if (entity is Enemy enemy && enemy.IsBoss && !enemy.Dead) enemy.HealBoss();
                    });*/
                }

                else if (message.StartsWith("#/k") && uint.TryParse(message.Substring(3), out uint id))
                {
                    //Administration.Banned.Clear();
                    //Administration.Banned.AddRange(LobbyController.Lobby?.GetData("banned").Split(' ').Select(s => uint.TryParse(s, out uint value) ? value : 0).ToArray());
                    LobbyBannedData = LobbyController.Lobby?.GetData("banned");
                    List<uint> ClientBannedData = Administration.Banned;
                    if (LobbyBannedData.Contains($"{id}")) Bundle.Msg("player.banned", Tools.Name(id));
                    else return;
                    ClientBannedData.Clear();
                    ClientBannedData.AddRange(LobbyBannedData.Split(' ').Select(s => uint.TryParse(s, out uint value) ? value : 0));
                }
                else if (message.StartsWith("#/s") && byte.TryParse(message.Substring(3), out byte team))
                {
                    if (LocalPlayer.Team == (Team)team) StyleHUD.Instance.AddPoints(Mathf.RoundToInt(250f * StyleCalculator.Instance.airTime), Bundle.ParseColors("[#3C3]FRATRICIDE"));
                }
                else if (message.StartsWith("#/r") && byte.TryParse(message.Substring(3), out byte rps))
                    ChatUI.Instance.Receive($"[#FFA500]{member.Name} has chosen {rps switch { 0 => "rock", 1 => "paper", 2 => "scissors", _ => "nothing" }}");
                else if (message.StartsWith("/tts "))
                    ChatUI.Instance.ReceiveTTS(GetTeamColor(member), member, message.Substring(5));
                else
                    ChatUI.Instance.NewReceive(GetTeamColor(member), member, message);
            }
        };
    }

    /// <summary> Kills all players and clears the list of entities. </summary>
    public static void Clear()
    {
        EachPlayer(player => player.Kill());
        Entities.Clear();
        Entities[LocalPlayer.Id] = LocalPlayer;
    }

    /// <summary> Core network logic should have been here, but in fact it is located in the server and client classes. </summary>
    private static void NetworkUpdate()
    {
        // the player isn't connected to the lobby and the logic doesn't need to be updated
        if (LobbyController.Offline) return;

        // update the server or client depending on the role of the player
        if (LobbyController.IsOwner)
            Server.Update();
        else
            Client.Update();
    }

    /// <summary> Optimizes the network by removing the dead entities from the global list. </summary>
    private static void Optimize()
    {
        // there is no need to optimize the network if no one uses it
        if (LobbyController.Offline) return;

        List<uint> toRemove = new();

        Entities.Values.DoIf(e => e == null ||
            (e.Dead && e.LastUpdate < Time.time - 1f && !e.gameObject.activeSelf),
            e => toRemove.Add(e.Id));
        if (DeadBullet.Instance.LastUpdate < Time.time - 1f)
            Entities.DoIf(pair => pair.Value == DeadBullet.Instance, pair => toRemove.Add(pair.Key));

        toRemove.ForEach(id => Entities.Remove(id));
    }

    #region iteration

    /// <summary> Iterates each server connection. </summary>
    public static void EachConnection(Action<Connection> cons)
    {
        foreach (var con in Server.Manager?.Connected) cons(con);
    }

    /// <summary> Iterates each non-null entity. </summary>
    public static void EachEntity(Action<Entity> cons)
    {
        foreach (var entity in Entities.Values) if (entity != null && !entity.Dead) cons(entity);
    }

    /// <summary> Iterates each non-null entity that fits the given predicate. </summary>
    public static void EachEntity(Predicate<Entity> pred, Action<Entity> cons) => EachEntity(entity =>
    {
        if (pred(entity)) cons(entity);
    });

    /// <summary> Iterates each player. </summary>
    public static void EachPlayer(Action<RemotePlayer> cons) => EachEntity(entity =>
    {
        if (entity is RemotePlayer player) cons(player);
    });

    #endregion
    #region tools

    /// <summary> Returns the team of the given friend. </summary>
    public static Team GetTeam(Friend friend) => friend.IsMe
        ? LocalPlayer.Team
        : (Entities.TryGetValue(friend.Id.AccountId, out var entity) && entity && entity is RemotePlayer player ? player.Team : Team.Yellow);

    /// <summary> Returns the hex color of the friend's team. </summary>
    public static string GetTeamColor(Friend friend) => ColorUtility.ToHtmlStringRGBA(GetTeam(friend).Color());

    /// <summary> Finds a connection by id or returns null if there is no such connection. </summary>
    public static Connection? FindCon(uint id)
    {
        foreach (var con in Server.Manager.Connected)
            if (con.ConnectionName == id.ToString()) return con;
        return null;
    }

    /// <summary> Forwards the packet to all clients or the host. </summary>
    public static void Redirect(IntPtr data, int size)
    {
        if (LobbyController.IsOwner)
            EachConnection(con => Tools.Send(con, data, size));
        else
            Tools.Send(Client.Manager.Connection, data, size);
    }

    /// <summary> Allocates memory, writes the packet there and sends it. </summary>
    public static void Send(PacketType packetType, Action<Writer> cons = null, Action<IntPtr, int> result = null, int size = 47) =>
        Writer.Write(w => { w.Enum(packetType); cons?.Invoke(w); }, result ?? Redirect, cons == null ? 1 : size + 1);

    #endregion
}