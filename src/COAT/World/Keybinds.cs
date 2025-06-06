namespace COAT.World;

using COAT.Content;
using COAT.Net;
using COAT.UI;
using COAT.UI.Elements;
using COAT.UI.Fragments;
using COAT.UI.Menus;
using COAT.UI.Overlays;

using UnityEngine;


public class Keybinds : MonoSingleton<Keybinds>
{
    static NewMovement nm => NewMovement.Instance;
    static CameraController cc => CameraController.Instance;
    static PrefsManager pm => PrefsManager.Instance;

    /// <summary> Environmental mask needed to prevent the skateboard from riding on water and camera from falling trough the ground. </summary>
    private readonly int mask = LayerMaskDefaults.Get(LMD.Environment);

    /// <summary> Last pointer created by the player. </summary>
    public Pointer Pointers;
    /// <summary> Last spray created by the player. </summary>
    //public Spray Spray;

    /// <summary> List of internal names of all key bindings. </summary>
    public static readonly string[] KeybindString =
    { "chat", "scroll-messages-up", "scroll-messages-down", "lobby-tab", "player-list", "settings", "player-indicators", "player-information", "emoji-wheel", "pointer", "spray", "self-destruction" };

    /// <summary> Array with current control settings. </summary>
    public static KeyCode[] CurrentKeys => new[]
    { ChatKey, ScrollUpKey, ScrollDownKey, LobbyTabKey, PlayerListKey, SettingsKey, PlayerIndicatorsKey, PlayerInfoKey, EmojiWheelKey, PointerKey, SprayKey, SelfDestructionKey, PanHitKey };

    /// <summary> List of all key bindings in the mod. </summary>
    public static KeyCode ChatKey, ScrollUpKey, ScrollDownKey, LobbyTabKey, PlayerListKey, SettingsKey, PlayerIndicatorsKey, PlayerInfoKey, EmojiWheelKey, PointerKey, SprayKey, SelfDestructionKey, PanHitKey;

    /// <summary> Gets the key binding value from its path. </summary>
    public static KeyCode GetKey(string path, KeyCode def) => (KeyCode)pm.GetInt($"jaket.binds.{path}", (int)def);

    /// <summary> Returns the name of the given key. </summary>
    public static string KeyName(KeyCode key) => key switch
    {
        KeyCode.LeftAlt => "LEFT ALT",
        KeyCode.RightAlt => "RIGHT ALT",
        KeyCode.LeftShift => "LEFT SHIFT",
        KeyCode.RightShift => "RIGHT SHIFT",
        KeyCode.LeftControl => "LEFT CONTROL",
        KeyCode.RightControl => "RIGHT CONTROL",
        KeyCode.Return => "ENTER",
        KeyCode.CapsLock => "CAPS LOCK",
        _ => key.ToString().Replace("Alpha", "").Replace("Keypad", "Num ").ToUpper()
    };

    public static void Load()
    {
        Tools.Create<Keybinds>("Keybinds");

        ChatKey = GetKey("chat", KeyCode.Return);
        ScrollUpKey = GetKey("scroll-messages-up", KeyCode.UpArrow);
        ScrollDownKey = GetKey("scroll-messages-down", KeyCode.DownArrow);
        LobbyTabKey = GetKey("lobby-tab", KeyCode.F1);
        PlayerListKey = GetKey("player-list", KeyCode.F2);
        SettingsKey = GetKey("settings", KeyCode.F3);
        PlayerIndicatorsKey = GetKey("player-indicators", KeyCode.Z);
        PlayerInfoKey = GetKey("player-information", KeyCode.X);
        EmojiWheelKey = GetKey("emoji-wheel", KeyCode.B);
        PointerKey = GetKey("pointer", KeyCode.Mouse2);
        SprayKey = GetKey("spray", KeyCode.T);
        SelfDestructionKey = GetKey("self-destruction", KeyCode.K);
        PanHitKey = GetKey("self-destruction", KeyCode.F);
    }

    private void Update()
    {
        // Allows the user to escape from the menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // YOU CAN ESCAPE
            if (Tools.Scene == "Main Menu" && UI.CanUseMenu())
            {
                UI.PopStack();
            }
            else if (Tools.Scene != "Main Menu" && UI.CanUseIngame())
            {
                UI.PopStack();
                Movement.UpdateState();
            }
        }

        if (Tools.Scene == "Main Menu" || LobbyController.Offline) return;

        // Uncomment this when you're working on settings and other UI
        if (Input.GetKeyDown(Keybinds.LobbyTabKey)) UI.ToggleUI(Home.Instance);
        if (Input.GetKeyDown(Keybinds.PlayerListKey)) UI.ToggleUI(PlayerList.Instance);
        if (Input.GetKeyDown(Keybinds.SettingsKey)) UI.ToggleUI(Settings.Instance);

        if (Input.GetKeyDown(Keybinds.ScrollUpKey)) Chat.Instance.ScrollMessages(true);
        if (Input.GetKeyDown(Keybinds.ScrollDownKey)) Chat.Instance.ScrollMessages(false);

        if (UI.Focused || Settings.Instance.Rebinding) return;

        if (Input.GetKeyDown(Keybinds.ChatKey)) Chat.Instance.Toggle();

        if (Input.GetKeyDown(KeyCode.F4)) Debugging.Instance.Toggle();
        if (Input.GetKeyDown(KeyCode.C) && Debugging.Shown) Debugging.Instance.Clear();

        /*if (Input.GetKeyDown(Keybinds.PlayerIndicatorsKey)) PlayerIndicators.Instance.Toggle();
        if (Input.GetKeyDown(Keybinds.PlayerInfoKey)) PlayerInfo.Instance.Toggle();*/

        if (Input.GetKey(Keybinds.EmojiWheelKey) && !Home.Shown && !WeaponWheel.Instance.gameObject.activeSelf)
        {
            Movement.Instance.HoldTime += Time.deltaTime; // if the key has been pressed for 0.25 seconds, show the emoji wheel
            if (!EmojiWheel.Shown && Movement.Instance.HoldTime > .25f) EmojiWheel.Instance.Show();
        }
        else
        {
            Movement.Instance.HoldTime = 0.075f; // 0 is too harsh for people with bad keyboards or weak fingers
            if (EmojiWheel.Shown) EmojiWheel.Instance.Hide();
        }

        bool p = Input.GetKeyDown(Keybinds.PointerKey), s = Input.GetKeyDown(Keybinds.SprayKey);
        if ((p || s) && Physics.Raycast(cc.transform.position, cc.transform.forward, out var hit, float.MaxValue, mask))
        {
            if (p)
            {
                if (Pointers != null) Pointers.Lifetime = 4.5f;
                Pointers = Pointer.Spawn(Networking.LocalPlayer.Team, hit.point, hit.normal);
            }
            //if (s) Spray = SprayManager.Spawn(hit.point, hit.normal);

            if (LobbyController.Online) Networking.Send(p ? PacketType.Point : PacketType.Spray, w =>
            {
                w.Id(Tools.AccId);
                w.Vector(hit.point);
                w.Vector(hit.normal);
            }, size: 32);
        }

        if (Input.GetKeyDown(Keybinds.SelfDestructionKey) && !UI.AnyDialog) nm.GetHurt(4200, false, 0f);

        /*if (Input.GetKeyDown(Keybinds.PanHit) && !UI.AnyDialog)
        {
            if (Physics.Raycast(cc.transform.position, cc.transform.forward, out var hit, 1f, mask))
            {

            } else 

            
        }*/
    }
}