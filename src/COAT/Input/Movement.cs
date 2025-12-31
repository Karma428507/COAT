namespace COAT.Input;

using GameConsole;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using COAT.Assets;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI;
using COAT.UI.Menus;
using COAT.UI.Screen;
using System;

/// <summary> Class responsible for additions to control and local display of emotions. </summary>
public class Movement : MonoSingleton<Movement>
{
    static NewMovement nm => NewMovement.Instance;
    static FistControl fc => FistControl.Instance;
    static GunControl gc => GunControl.Instance;
    static CameraController cc => CameraController.Instance;
    static PlayerInput pi => InputManager.Instance.InputSource;
    static CheatsManager cm => CheatsManager.Instance;
    
    /// <summary> Whether the death must be fake on this level. </summary>
    private static bool fakeDeath => nm.endlessMode || Tools.Scene == "Level 0-S";

    /// <summary> Starting and ending position of third person camera. </summary>
    private readonly Vector3 start = new(0f, 6f, 0f), end = new(0f, .1f, 0f);
    /// <summary> Third person camera position. </summary>
    private Vector3 position;
    /// <summary> Third person camera rotation. </summary>
    private Vector2 rotation;
    /// <summary> Third person camera target. If an emote is playing, the camera will aim at the local player, otherwise, at a remote player. </summary>
    private int targetPlayer;

    /// <summary> Creates a singleton of movement. </summary>
    public static void Load()
    {
        // initialize the singleton
        Tools.Create<Movement>("Movement");

        void LevelHandler()
        {
            // disable hook and jump at 0-S
            if (Tools.Scene == "Level 0-S")
            {
                // have this option be setting based
                // because I hate this

                nm.modNoJump = LobbyController.Online;
                HookArm.Instance.gameObject.SetActive(LobbyController.Offline);
            }

            if (fakeDeath)
            {
                // disable restart button for clients
                CanvasController.Instance.transform.Find("PauseMenu/Restart Mission").GetComponent<Button>().interactable = LobbyController.Offline || LobbyController.IsOwner;

                // disable text override component
                //nm.youDiedText.GetComponents<MonoBehaviour>()[1].enabled = false;
            }
        }

        Events.OnLoaded += LevelHandler;

        // update death screen text to display the number of living players in the Cyber Grind
        Instance.InvokeRepeating("GridUpdate", 0f, 1f);
    }

    private void Update()
    {
        if (LobbyController.Offline) return;

        if (pi.Fire1.WasPerformedThisFrame) targetPlayer--;
        if (pi.Fire2.WasPerformedThisFrame) targetPlayer++;

        if (targetPlayer < 0) targetPlayer = LobbyController.Lobby?.MemberCount - 1 ?? 0;
        if (targetPlayer >= LobbyController.Lobby?.MemberCount) targetPlayer = 0;
    }

    private void LateUpdate() // late update is needed to overwrite the time scale value and camera rotation
    {
        // all the following changes are related to the network part of the game and shouldn't affect the local
        if (LobbyController.Offline) return;

        // pause stops time and weapon wheel slows it down, but in multiplayer everything should be real-time
        if (Settings.DisableFreezeFrames || UI.AnyDialog) Time.timeScale = 1f;

        // disable cheats if they are prohibited in the lobby
        if (CheatsController.Instance.cheatsEnabled && !LobbyController.IsOwner && !LobbyController.CheatsAllowed) DisableCheats();
    
        // leave thee lobby if mods are off and u have a "not allowed" mod
        if (Plugin.Instance.HasIncompatibility && !LobbyController.IsOwner && !LobbyController.ModsAllowed)
        {
            LobbyController.Lobby?.SendChatString("[#FF7F50][14]\\[BOT][][] FUCK OFF!");
            LobbyController.LeaveLobby();
            Bundle.Hud2NS("lobby.mods");
        }

        // leave lobby if you have a blacklisted mod 
        if (Plugin.Instance.HasBlacklisted && !LobbyController.IsOwner)
        {
            LobbyController.Lobby?.SendChatString("[#FF7F50][14]\\[BOT][][] FUCK OFF!");
            LobbyController.LeaveLobby();
            Bundle.Hud2NS("lobby.mods");
        }

        // fake Cyber Grind///0-S death
        // very complex
        /*if (nm.dead && nm.blackScreen.color.a < .4f && fakeDeath)
        {
            nm.blackScreen.color = nm.blackScreen.color with { a = nm.blackScreen.color.a + .75f * Time.deltaTime };
            nm.youDiedText.color = nm.youDiedText.color with { a = nm.blackScreen.color.a * 1.25f };
        }*/
    }

    public static void DisableCheats(bool Hud = true)
    {
        CheatsController.Instance.cheatsEnabled = false;
        cm.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        (Tools.Get("idToCheat", cm) as Dictionary<string, ICheat>).Values.Do(cm.DisableCheat);
        if (Hud) Bundle.Hud("lobby.cheats");
    }

    private void GridUpdate()
    {
        if (LobbyController.Offline || !fakeDeath) return;

        /*int alive = CyberGrind.PlayersAlive();
        nm.youDiedText.text = Bundle.Format("spect", alive.ToString(), EndlessGrid.Instance ? "#spect.cg" : "#spect.0s");

        if (alive > 0) return;
        if (Tools.Scene == "Level 0-S") StatsManager.Instance.Restart();
        else
        {
            var final = nm.GetComponentInChildren<FinalCyberRank>();
            if (final.savedTime == 0f)
            {
                final.GameOver();
                Destroy(nm.blackScreen.gameObject);
            }
        }*/
    }

    #region respawn

    /// <summary> Teleports the local player to the given position and activates movement if it is disabled. </summary>
    public void Teleport(Vector3 position)
    {
        UpdateState();
        nm.transform.position = position;
        nm.rb.velocity = Vector3.zero;

        PlayerActivatorRelay.Instance?.Activate();
        if (GameStateManager.Instance.IsStateActive("pit-falling"))
            GameStateManager.Instance.PopState("pit-falling");

        // this annoying sound makes me cry
        Tools.ObjFind("Hellmap")?.SetActive(false);
    }

    /// <summary> Repeats a part of the checkpoint logic, needed in order to avoid resetting rooms. </summary>
    public void Respawn(Vector3 position, float rotation)
    {
        Teleport(position);
        if (PlayerTracker.Instance.playerType == PlayerType.FPS)
            cc.ResetCamera(rotation);
        else
            PlatformerMovement.Instance.ResetCamera(rotation);

        nm.Respawn();
        nm.GetHealth(0, true);
        cc.StopShake();
        nm.ActivatePlayer();

        // the player is currently fighting the Minotaur in the tunnel, the security system or the brain in the Earthmover
        // Level specific
        //if (World.TunnelRoomba) nm.transform.position = World.TunnelRoomba.position with { y = -112.5f };
        //if (World.SecuritySystem[0]) nm.transform.position = new(0f, 472f, 745f);
        //if (World.Brain && World.Brain.IsFightActive) nm.transform.position = new(0f, 826.5f, 610f);
    }

    /// <summary> Respawns Cyber Grind players and launches a screen flash. </summary>
    public void CyberRespawn()
    {
        Respawn(new(0f, 80f, 62.5f), 0f);
        Teleporter.Instance.Flash();
    }

    #endregion

    #region toggling

    /// <summary> Updates the state machine: toggles movement, cursor and third-person camera. </summary>
    public static void UpdateState()
    {
        bool dialog = UI.AnyDialog, blocking = UI.AnyMovementBlocking;

        ToggleCursor(dialog);
        ToggleHud(Emotes.Instance.Emoji == 0xFF);

        if (nm.dead) return;

        nm.activated = fc.activated = gc.activated = !blocking;
        cc.activated = !blocking && !EmojiWheel.Shown;

        if (blocking) fc.NoFist();
        else fc.YesFist();

        OptionsManager.Instance.frozen = Emotes.Instance.Emoji != 0xFF;
        GameConsole.Console.Instance.enabled = Emotes.Instance.Emoji == 0xFF;
    }

    private static void ToggleCursor(bool enable)
    {
        Cursor.visible = enable;
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private static void ToggleHud(bool enable)
    {
        StyleHUD.Instance.transform.parent.gameObject.SetActive(enable);
        fc.gameObject.SetActive(enable);
        gc.gameObject.SetActive(enable);
    }

    #endregion
    
}
