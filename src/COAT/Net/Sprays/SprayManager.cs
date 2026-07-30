namespace COAT.Net.Sprays;

using COAT.Assets;
using COAT.Net;
using COAT.UI.Menus.Sub;
using COAT.UI.Physical;
using COAT.Utils;
using COAT.Net.Files;

using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary> Saves sprays of players and loads sprays of the local player. </summary>
public class SprayManager
{
    /// <summary> Spray currently selected by the local player. </summary>
    public static SprayFile CurrentSpray;
    /// <summary> Other sprays located in the sprays folder. </summary>
    public static List<SprayFile> Loaded = new();
    /// <summary> Folder containing the local sprays. </summary>
    public static string Folder => Path.Combine(Path.GetDirectoryName(Plugin.Instance.Location), "sprays");
    /// <summary> Whether the current spray has been uploaded. </summary>
    public static bool Uploaded;

    /// <summary> Sprays spawned by players. </summary>
    public static Dictionary<uint, Spray> Sprays = new();
    /// <summary> Cached sprays of other players. </summary>
    public static Dictionary<uint, SprayFile> Cache = new();

    /// <summary> Sound that is played when creating a spray. </summary>
    public static AudioClip puh;

    /// <summary> Subscribes to various events to synchronize sprays or clear cache. </summary>
    public static void Load()
    {
        LoadSprayFiles();
        SpraySettings.Load();

        Tools.ResFind<AudioClip>(clip => clip.name == "Explosion Harmless", clip => puh = clip);

        // clear the cache in offline game & upload the current spray if it was changed
        Events.OnLoaded += () =>
        {
            if (LobbyController.Offline) Cache.Clear();
            else UploadLocal();

            foreach (var spray in Sprays.Values)
                if (spray != null) spray.Lifetime = 60f;
        };

        Events.OnLobbyEntered += () =>
        {
            Uploaded = LobbyController.IsOwner;
            Cache.Clear();
            Cache.Add(Tools.AccId, CurrentSpray);
        };
    }

    /// <summary> Loads sprays from the sprays folder. </summary>
    public static void LoadSprayFiles()
    {
        Loaded.Clear();

        var folder = Folder;
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        else foreach (var file in Directory.EnumerateFiles(folder))
                if (SprayFile.SUPPORTED.Contains(Path.GetExtension(file))) Loaded.Add(new(file));

        Log.Info($"Loaded {Loaded.Count} sprays: {string.Join(", ", Loaded.ConvertAll(s => s.Name))}");
    }

    /// <summary> Sets the current spray. </summary>
    public static void SetSpray(SprayFile spray)
    {
        CurrentSpray = spray;
        Uploaded = false;

        Cache.Remove(Tools.AccId);
        Cache.Add(Tools.AccId, CurrentSpray);
    }

    /// <summary> Spawns someone's spray in the given position. </summary>
    public static Spray Spawn(uint owner, Vector3 position, Vector3 direction)
    {
        if (Sprays.TryGetValue(owner, out var spray))
        {
            spray.Lifetime = 58f;
            Sprays.Remove(owner);
        }
        if (!Cache.ContainsKey(owner))
        {
            if (owner == Tools.AccId) // seems like the player is in offline game
                Cache.Add(owner, CurrentSpray);
            else
                NetRequester.Request(NetFile.NET_FILE_TYPE_SPRAY, owner);
        }

        spray = Spray.Spawn(owner, position, direction);
        Sprays.Add(owner, spray);

        return spray;
    }

    /// <summary> Spawns the local player's spray in the given position. </summary>
    public static Spray Spawn(Vector3 position, Vector3 direction)
    {
        if (CurrentSpray == null)
        {
            Localization.Hud("sprays.empty"); // You haven't chosen a spray. Please, choose on in settings.
            return null;
        }
        return Spawn(Tools.AccId, position, direction);
    }

    /// <summary> Uploads the current spray to the server. </summary>
    public static void UploadLocal()
    {
        // It was that easy?
        if (LobbyController.Offline)
            return;

        // there is no point in sending the spray to the distributor if you haven't changed it
        if (Uploaded || CurrentSpray == null) return;
        Log.Info("Uploading the current spray...");

        NetLoader.Upload(Tools.AccId, NetFile.NET_FILE_TYPE_SPRAY, CurrentSpray.Data);
        Uploaded = true;
    }

    /// <summary> Handles the downloaded spray and decides where to send it next. </summary>
    public static void HandleSpray(uint owner, byte[] data)
    {
        Cache.Remove(owner);
        Cache.Add(owner, new(data));

        // update the existing spray if there is one
        if (Sprays.TryGetValue(owner, out var spray)) spray.UpdateSprite();
    }
}