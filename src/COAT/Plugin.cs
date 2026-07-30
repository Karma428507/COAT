namespace COAT;

using COAT.Assets;
using COAT.Chat;
using COAT.Content;
using COAT.Entities;
using COAT.Gamemode;
using COAT.Input;
using COAT.IO;
using COAT.Net;
using COAT.Net.Sprays;
using COAT.Pages;
using COAT.UI;
using COAT.Utils;
using COAT.World;

using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using COAT.Net.Files;

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
    /// <summary> Path to the dll file of the mod. </summary>
    public string Location;

    /// <summary> List of mods compatible with COAT. </summary>
    public static readonly string[] Compatible = { "COAT", "WesV2", "CrosshairColorFixer", "IntroSkip", "Healthbars", "RcHud", "PluginConfigurator", "AngryLevelLoader" }; // TODO: add more later frfr gang ang
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
        Events.OnMainMenuLoaded *= Init;
    }

    private void Init()
    {
        // Update check
#if UPDATE
        Version.Check4Update();
#endif

        // Initialize the important utilities
        Stats.StartRecord();
        Pointers.Load();
        Tools.CacheAccId();

        // Genral networking
        LobbyController.Load();
        Networking.Load();

        // Registerable components
        GamemodeManager.Load();
        PageManager.Load();

        // Loadable assets and files
        EmbeddedManager.Load();
        Localization.Load();
        ModAssets.Load();
        SaveManager.Load();

        // Player services
        UI.Menus.Settings.Load(); // planning on removing this from settings soon
        Keybinds.Load();
        Movement.Load();

        // Net downloadables
        NetRequester.Load();
        SprayManager.Load();

        // Loads the UI
        UIB.Load();
        UI.UI.Load();
        ReplacementUI.Load();
        PrefabUI.Load();

        // Messaging services
        ChatManager.Load();
        Censoring.Load();

        // Entities stuff and weapons
        Net.Entities.Load();
        Events.Post(Enemies.Load);
        Events.Post(Items.Load);
        Events.OnLoaded += Weapons.Initialize;

        // Optimizations
        Administration.Load();

        // World management
        DoorManager.Load();
        World.World.Load();
        WorldActionsList.Load();

        // initialize harmony and patch all the necessary classes
        new Harmony("COAT Harmony").PatchAll();

        // check if there is any incompatible mods
        HasIncompatibility = Chainloader.PluginInfos.Values.Any(info => !Compatible.Contains(info.Metadata.Name));
        //HasBlacklisted = Chainloader.PluginInfos.Values.Any(info => !Blacklisted.Contains(info.Metadata.Name));

        // mark the plugin as initialized and log a message about it
        Log.Info("COAT initialized!");
    }

    private void OnApplicationQuit() => Log.Flush();
}