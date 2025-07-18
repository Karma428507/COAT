namespace COAT.Net;

using HarmonyLib;
using System.Collections.Generic;

using COAT.Content;
using COAT.UI.Overlays;
using COAT.UI.Menus;
using Steamworks;
using System.Linq;
using COAT.Assets;

/// <summary> Class dedicated to protecting the lobby from unfavorable people. </summary>
public class Administration
{
    /// <summary> Max amount of bytes a player can send per second. </summary>
    public const int SPAM_RATE = 32 * 1024;
    /// <summary> Max amount of warnings a player can get before ban. </summary>
    public const int MAX_WARNINGS = 4;

    /// <summary> Max amount of entity bullets per player and common bullets per second. </summary>
    public const int MAX_BULLETS = 10;
    /// <summary> Max amount of entities per player. </summary>
    public const int MAX_ENTITIES = 16;
    /// <summary> Max amount of plushies per player. </summary>
    public const int MAX_PLUSHIES = 6;

    /// <summary> List of banned player ids. </summary>
    public static List<uint> Banned = new();
    /// <summary> List of banned player sprays. </summary>
    //public static List<uint> BannedSprays = new();

    private static Counter spam = new();
    private static Counter warnings = new();
    private static Counter commonBullets = new();
    private static Tree entityBullets = new();
    private static Tree entities = new();
    private static Tree plushies = new();

    /// <summary> List of blacklisted mods in the lobby. </summary>
    //public static string[] BlacklistedMods;

    /// <summary> Subscribes to events to clear lists. </summary>
    public static void Load()
    {
        Events.OnLobbyAction += () =>
        {
            if (LobbyController.IsOwner) return;

            Banned.Clear();
            LobbyController.Lobby?.GetData("banned").Split(' ').Do(sid =>
            {
                if (uint.TryParse(sid, out var id)) Banned.Add(id);
            });
        };
        Events.OnLobbyEntered += () => { Banned.Clear(); /*entityBullets.Clear(); entities.Clear(); plushies.Clear();*/ };
        Events.EverySecond += spam.Clear;
        Events.EverySecond += commonBullets.Clear;
        Events.EveryDozen += warnings.Clear;
    }

    /// <summary> Bans the member from the lobby, or rather asks him to leave, because Valve hasn't added such functionality to their API. </summary>
    public static void Ban(uint id)
    {
        // who does the client think he is?!
        if (!LobbyController.IsOwner) { Chat.StaticReceive("\"yo buddy u aint host, dont push it.\" - Bryan(dev)"); return; }

        Networking.Send(PacketType.Ban, null, (data, size) =>
        {
            var con = Networking.FindCon(id);
            Tools.Send(con, data, size);
            con?.Flush();
            Events.Post2(() => con?.Close());
        });

        Banned.Add(id);
        LobbyController.Lobby?.SendChatString("#/k" + id);
        LobbyController.Lobby?.SetData("banned", string.Join(" ", Banned));
    }

    /// <summary> Kicks the member from the lobby, or rather asks him to leave, because Valve hasn't added such functionality to their API. </summary>
    public static void Kick(uint id)
    {
        // who does the client think he is?!
        if (!LobbyController.IsOwner) { Chat.StaticReceive("\"yo buddy u aint host, dont push it.\" - Bryan(dev)"); return; }

        // send a SteamMatchMaking event for when players are kicked
        //SteamMatchmaking.OnLobbyMemberKicked.Invoke(LobbyController.Lobby, );

        // send a kick msg so jaket users also see it
        Chat.Instance.Send($"<b>{Chat.BOT_PREFIX}</b> Player {Tools.Name(id)} was [#F75][18]\\[ KICKED ][][]");

        // check if either the player is on coat, or not. if so, send kick packet. if not, send ban packet.
        if (Networking.COATPLAYERS.Contains(id))
        {
            Networking.Send(PacketType.COAT_Kick, null, (data, size) =>
            {
                var con = Networking.FindCon(id);
                Tools.Send(con, data, size);
                con?.Flush();
                Events.Post2(() => con?.Close());
            });
        }
        else
        {
            Networking.Send(PacketType.Ban, null, (data, size) =>
            {
                var con = Networking.FindCon(id);
                Tools.Send(con, data, size);
                con?.Flush();
                Events.Post2(() => con?.Close());
            });
        }
    }

    public static void BlacklistMod(string name)
    {
        //if (!LobbyController.IsOwner && !LobbyController.Online) return;

        bool allow = Settings.PersonalBlacklistedMods.Contains(name);
        if (!allow) { Settings.PersonalBlacklistedMods.Add(name); } else { Settings.PersonalBlacklistedMods.Remove(name); }
        if (LobbyController.Online) LobbyController.Lobby?.SetData("BlacklistedMods", string.Join(" ", Settings.PersonalBlacklistedMods));
    }

    /// <summary> Mutes/Unmutes a player in the lobby, no body can hear their screams >:3 </summary>
    public static void Mute(uint id, bool mute)
    {
        // who does the client think he is?!
        if (!LobbyController.IsOwner) { Chat.StaticReceive("\"yo buddy u aint host, dont push it.\" - Bryan(dev)"); return; }

        // send a packet to disable the chat of the muted person
        //Networking.Send(PacketType.COAT_Mute, w => { w.Id(id); w.Bool(mute); });

        if (mute) Networking.MUTEDPLAYERS.Add(id);
        else Networking.MUTEDPLAYERS.Remove(id);

        if (mute) { Networking.MUTEDPLAYERS.Add(id);
            Chat.Instance.Send($"<b>{Chat.BOT_PREFIX}</b> Player {Tools.Name(id)} was [#F75][18]\\[ MUTED ][][]");
            LobbyController.Lobby?.SetData("mute", string.Join(" ", Networking.MUTEDPLAYERS)); } else {
            Networking.MUTEDPLAYERS.Remove(id);
            Chat.Instance.Send($"<b>{Chat.BOT_PREFIX}</b> Player {Tools.Name(id)} was [#F75][18]\\[ UNMUTED ][][]");
            LobbyController.Lobby?.SetData("mute", string.Join(" ", Networking.MUTEDPLAYERS)); }
    }

    /// <summary> Whether the player is sending a large amount of data. </summary>
    public static bool IsSpam(uint id, int amount) => spam.Count(id, amount) >= SPAM_RATE;

    /// <summary> Clears the amount of data sent by the given player. </summary>
    public static void ClearSpam(uint id) => spam[id] = int.MinValue;

    /// <summary> Whether the player is trying to spam. </summary>
    public static bool IsWarned(uint id) => warnings.Count(id, 1) >= MAX_WARNINGS;

    /// <summary> Whether the player can spawn another common bullet. </summary>
    public static bool CanSpawnBullet(uint owner, int amount) => commonBullets.Count(owner, amount) <= MAX_BULLETS;

    /// <summary> Handles the creations of a new entity by a client. If the client exceeds its limit, the old entity will be destroyed. </ Summary>
    public static void Handle(uint owner, Entity entity)
    {
        void Default(Tree tree, int max)
        {
            if (tree.Count(owner) > max) tree[owner][0].NetKill();
            tree[owner].Add(entity);
        }

        if (entity.Type.IsEnemy() || entity.Type.IsItem())
        {
            // player can only spawn one big enemy at a time
            if (entity.Type.IsBigEnemy() && entities.TryGetValue(owner, out var list)) list.ForEach(e => e.NetKill());

            Default(entities, MAX_ENTITIES);
        }
        else if (entity.Type.IsPlushy()) Default(plushies, MAX_PLUSHIES);
        else if (entity.Type.IsBullet()) Default(entityBullets, MAX_BULLETS);
    }

    /// <summary> Counter of abstract actions done by players. </summary>
    public class Counter : Dictionary<uint, int>
    {
        /// <summary> Counts the number of actions done by the given player and increases it by some value. </summary>
        public new int Count(uint id, int amount)
        {
            TryGetValue(id, out int value);
            return this[id] = value + amount;
        }
    }

    /// <summary> Tree with players ids as roots and entities created by these players as children. </summary>
    public class Tree : Dictionary<uint, List<Entity>>
    {
        /// <summary> Counts the number of living entities the given player has in the tree. </summary>
        public new int Count(uint id)
        {
            if (ContainsKey(id))
                return this[id].Count - this[id].RemoveAll(entity => entity == null || entity.Dead);
            else
            {
                this[id] = new();
                return 0;
            }
        }
    }
}