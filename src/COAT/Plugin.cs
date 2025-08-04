namespace COAT;

using BepInEx;
using BepInEx.Bootstrap;

using COAT.Assets;
using COAT.Chat;
using COAT.Content;
using COAT.IO;
using COAT.Net;
using COAT.Net.Types.Players;
using COAT.World;

using HarmonyLib;
using COAT.Sprays;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Bootloader class needed to avoid destroying the mod by the game. </summary>
[BepInPlugin("Karma.Coat", "COAT", Version.CURRENT)]
public class PluginLoader : BaseUnityPlugin
{
    private void Awake() => SceneManager.sceneLoaded += (_, _) =>
    {
        if (Plugin.Instance == null) Tools.Create<Plugin>("COAT").Location = Info.Location;
    };
}

/// <summary> Plugin main class. Essentially only initializes all other components. </summary>
public class Plugin : MonoBehaviour
{
    /// <summary> Plugin instance available everywhere. </summary>
    public static Plugin Instance;
    /// <summary> Whether the plugin has been initialized. </summary>
    public bool Initialized;
    /// <summary> Path to the dll file of the mod. </summary>
    public string Location;

    /// <summary> Set this to false in release versions </summary>
    public const bool DebugMode = true;

    /// <summary> List of mods compatible with COAT. </summary>
    public static readonly string[] Compatible = { "COAT", "CrosshairColorFixer", "IntroSkip", "Healthbars", "RcHud", "PluginConfigurator", "AngryLevelLoader" }; // TODO: add more later frfr gang ang
    /// <summary> Whether at least on incompatible mod is loaded. </summary>
    public bool HasIncompatibility;
    /// <summary> List of mods that are blacklisted in the lobby. </summary>
    public static readonly string[] Blacklisted = LobbyController.Lobby?.GetData("BlacklistedMods").Split(' ');
    /// <summary> Whether at least one blacklisted mod is loaded. </summary>
    public bool HasBlacklisted;

    private void Awake() => DontDestroyOnLoad(Instance = this); // save the instance of the mod for later use and prevent it from being destroyed by the game

    private void Start()
    {
        // create output points for logs
        Log.Load();
        // note the fact that the mod is loading
        Log.Info("Loading COAT...");

        // adds an event listener to the scene loading
        Events.Load();
        // interface components and assets bundle can only be loaded from the main menu
        Events.OnMainMenuLoaded += Init;
    }

    private void OnApplicationQuit() => Log.Flush();

    private void Init()
    {
        if (Initialized) return;

        // notify players about the availability of an update so that they no longer whine to me about something not working
        //Version.Check4Update();
        Stats.StartRecord();
        Pointers.Allocate();
        Tools.CacheAccId();

        ChatHandler.Load();
        Keybinds.Load();

        Bundle.Load();
        Events.Post(Enemies.Load);
        Events.Post(Items.Load);
        Events.OnLoaded += Weapons.Initialize;
        DollAssets.Load();

        Administration.Load();
        LobbyController.Load();
        Networking.Load();
        Entities.Load();

        World.World.Load();
        WorldActionsList.Load();
        Movement.Load();
        SprayManager.Load();

        PlayerData.Load();

        UI.UIB.Load();
        UI.UI.Load();

        // initialize harmony and patch all the necessary classes
        new Harmony("Meow :3").PatchAll();

        // check if there is any incompatible mods
        HasIncompatibility = Chainloader.PluginInfos.Values.Any(info => !Compatible.Contains(info.Metadata.Name));
        //HasBlacklisted = Chainloader.PluginInfos.Values.Any(info => !Blacklisted.Contains(info.Metadata.Name));

        // mark the plugin as initialized and log a message about it
        Initialized = true;
        Log.Info("COAT initialized!");
    }
}