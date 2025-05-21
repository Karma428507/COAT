namespace COAT.UI.Menus;

using UnityEngine;
using UnityEngine.UI;

using COAT.Assets;
using COAT.World;

using static Pal;
using static Rect;

/// <summary> Global mod settings not related to the lobby. </summary>
public class Settings : CanvasSingleton<Settings>, IMenuInterface
{
    static PrefsManager pm => PrefsManager.Instance;

    #region general

    /// <summary> Id of the currently selected language. </summary>
    public static int Language;
    /// <summary> 0 - default (depending on whether the player is in the lobby or not), 1 - always green, 2 - always blue/red. </summary>
    public static int FeedColor, KnuckleColor;
    /// <summary> Whether freeze frames are disabled. </summary>
    public static bool DisableFreezeFrames;

    #endregion
    #region controls

    /// <summary> List of internal names of all key bindings. </summary>
    public static readonly string[] Keybinds =
    { "chat", "scroll-messages-up", "scroll-messages-down", "lobby-tab", "player-list", "settings", "player-indicators", "player-information", "emoji-wheel", "pointer", "spray", "self-destruction" };

    /// <summary> Array with current control settings. </summary>
    public static KeyCode[] CurrentKeys => new[]
    { Chat, ScrollUp, ScrollDown, LobbyTab, PlayerList, Settingz, PlayerIndicators, PlayerInfo, EmojiWheel, Pointer, Spray, SelfDestruction };

    /// <summary> List of all key bindings in the mod. </summary>
    public static KeyCode Chat, ScrollUp, ScrollDown, LobbyTab, PlayerList, Settingz, PlayerIndicators, PlayerInfo, EmojiWheel, Pointer, Spray, SelfDestruction;

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

    #endregion
    #region tts

    // <summary> Sam's voice volume. Limited by interval from 0 to 100. </summary>
    public static int TTSVolume
    {
        get => pm.GetInt("jaket.tts.volume", 60);
        set
        {
            DollAssets.Mixer?.SetFloat("Volume", value / 2f - 30f); // the value should be between -30 and 20 decibels
            pm.SetInt("jaket.tts.volume", value);
        }
    }

    /// <summary> Whether auto TTS is enabled. </summary>
    public static bool AutoTTS
    {
        get => pm.GetBool("jaket.tts.auto");
        set => pm.SetBool("jaket.tts.auto", value);
    }

    #endregion

    /// <summary> Whether a binding is being reassigned right now. </summary>
    public bool Rebinding;
    /// <summary> Components of a key button and the path to the keybind. </summary>
    private string path; Text text; Image background;
    /// <summary> General settings buttons. </summary>
    private Button lang, feed, knkl;

    /// <summary> Loads and applies all settings. </summary>
    public static void Load()
    {
        Language = Bundle.LoadedLocale;
        FeedColor = pm.GetInt("jaket.feed-color");
        KnuckleColor = pm.GetInt("jaket.knkl-color");
        DisableFreezeFrames = pm.GetBool("jaket.disable-freeze", true);

        Chat = GetKey("chat", KeyCode.Return);
        ScrollUp = GetKey("scroll-messages-up", KeyCode.UpArrow);
        ScrollDown = GetKey("scroll-messages-down", KeyCode.DownArrow);
        LobbyTab = GetKey("lobby-tab", KeyCode.F1);
        PlayerList = GetKey("player-list", KeyCode.F2);
        Settingz = GetKey("settings", KeyCode.F3);
        PlayerIndicators = GetKey("player-indicators", KeyCode.Z);
        PlayerInfo = GetKey("player-information", KeyCode.X);
        EmojiWheel = GetKey("emoji-wheel", KeyCode.B);
        Pointer = GetKey("pointer", KeyCode.Mouse2);
        Spray = GetKey("spray", KeyCode.T);
        SelfDestruction = GetKey("self-destruction", KeyCode.K);

        DollAssets.Mixer?.SetFloat("Volume", TTSVolume / 2f - 30f);
    }

    private void Start()
    {
        // Old settings UI, remove when finish with new UI
        UIB.Table("General", "#settings.general", transform, Tlw(16f + 328f / 2f, 328f), table =>
        {
            UIB.Button("#settings.reset", table, Btn(68f), clicked: ResetGeneral);

            lang = UIB.Button("", table, Btn(116f), clicked: () =>
            {
                pm.SetString("jaket.locale", Bundle.Codes[Language = ++Language % Bundle.Codes.Length]);
                Rebuild();
            });

            UIB.Text("FEEDBACKER:", table, Btn(164f), align: TextAnchor.MiddleLeft);
            feed = UIB.Button("", table, Stn(164f, 160f), clicked: () =>
            {
                pm.SetInt("jaket.feed-color", FeedColor = ++FeedColor % 3);
                Rebuild();
            });

            UIB.Text("KNUCKLE:", table, Btn(212f), align: TextAnchor.MiddleLeft);
            knkl = UIB.Button("", table, Stn(212f, 160f), clicked: () =>
            {
                pm.SetInt("jaket.knkl-color", KnuckleColor = ++KnuckleColor % 3);
                Rebuild();
            });

            UIB.Toggle("#settings.freeze", table, Tgl(256f), 20, _ =>
            {
                pm.SetBool("jaket.disable-freeze", DisableFreezeFrames = _);
            }).isOn = DisableFreezeFrames;

            //UIB.Button("#settings.sprays", table, Btn(300f), clicked: SpraySettings.Instance.Toggle);
        });
        UIB.Table("Controls", "#settings.controls", transform, Tlw(360f + 576f / 2f, 576f), table =>
        {
            UIB.Button("#settings.reset", table, Btn(68f), clicked: ResetControls);

            for (int i = 0; i < Keybinds.Length; i++)
                UIB.KeyButton(Keybinds[i], CurrentKeys[i], table, Tgl(112f + i * 40f));
        });

        // New UI
        UIB.Table("Settings", transform, Size(1400f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 1400f, 800f), null, fill: false);

            UIB.Image1("LineBreak", table, new(0f, 207f, 1357f, 4f), Pal.white, null, true);
            UIB.Text("Settings", table, new(-457f, 231f, 445f, 186f), Pal.white, 24, TextAnchor.MiddleLeft);

            #region General and Player Apyearence
            UIB.Table("General", table, new(-457f, 97f, 445f, 186f), server =>
            {
                UIB.Image("General Border", server, new(0f, 0f, 445f, 186f), null, fill: false);
            });

            UIB.Table("Player Apyearence", table, new(-457f, -195f, 445f, 370f), server =>
            {
                UIB.Image("Player Apyearence Border", server, new(0f, 0f, 445f, 370f), null, fill: false);
            });
            #endregion

            #region Moderatiun and Modlist
            UIB.Table("Moderatiun", table, new(0f, 29f, 445f, 322f), server =>
            {
                UIB.Image("Moderatiun Border", server, new(0f, 0f, 445f, 322f), null, fill: false);
            });

            UIB.Table("Modlist", table, new(0f, -262.5f, 445f, 234f), server =>
            {
                UIB.Image("Modlist Border", server, new(0f, 0f, 445f, 234f), null, fill: false);
            });
            #endregion

            UIB.Table("Controls", table, new(457f, -95f, 445f, 570f), server =>
            {
                UIB.Image("Controls Border", server, new(0f, 0f, 445f, 570f), null, fill: false);
            });
        });

        Version.Label(transform);
        Rebuild();
    }

    private void OnGUI()
    {
        if (!Rebinding) return;

        var current = Event.current; // receive the event and check whether any key is pressed
        if (!current.isKey && !current.isMouse && !current.shift) return;

        background.color = new(0f, 0f, 0f, .5f);
        Rebinding = false;

        // cancel key binding remapping
        if (current.keyCode == KeyCode.Escape || (current.isMouse && current.button == 0)) return;

        KeyCode key = current.isKey
            ? current.keyCode
            : current.isMouse
                ? KeyCode.Mouse0 + current.button
                : Input.GetKeyDown(KeyCode.LeftShift)
                    ? KeyCode.LeftShift
                    : KeyCode.RightShift;

        text.text = KeyName(key);
        pm.SetInt($"jaket.binds.{path}", (int)key);
        Load();
    }

    // <summary> Toggles visibility of the settings. </summary>
    public void Toggle()
    {
        gameObject.SetActive(Shown = !Shown);
        Movement.UpdateState();
    }

    /// <summary> Rebuilds the settings to update some labels. </summary>
    public void Rebuild()
    {
        string Mode(int mode) => Bundle.Get(mode switch
        {
            0 => "settings.default",
            1 => "settings.green",
            2 => "settings.vanilla",
            _ => "lobby-tab.default"
        });

        lang.GetComponentInChildren<Text>().text = Bundle.Locales[Language];
        feed.GetComponentInChildren<Text>().text = Mode(FeedColor);
        knkl.GetComponentInChildren<Text>().text = Mode(KnuckleColor);

        // update the color of the feedbacker and knuckleblaster
        Events.OnWeaponChanged.Fire();
    }

    // <summary> Starts rebinding the given key. </summary>
    public void Rebind(string path, Text text, Image background)
    {
        this.path = path;
        this.text = text;
        this.background = background;

        background.color = orange;
        Rebinding = true;
    }

    #region reset

    private void ResetGeneral()
    {
        pm.SetString("jaket.locale", Bundle.Codes[Bundle.LoadedLocale]);
        pm.DeleteKey("jaket.feed-color");
        pm.DeleteKey("jaket.knkl-color");
        pm.DeleteKey("jaket.disable-freeze");

        Load();
        Rebuild();
        transform.GetChild(1).GetChild(7).GetComponent<Toggle>().isOn = DisableFreezeFrames;
    }

    private void ResetControls()
    {
        foreach (var name in Keybinds) pm.DeleteKey($"jaket.binds.{name}");

        Load();
        for (int i = 0; i < Keybinds.Length; i++)
            transform.GetChild(2).GetChild(i + 2).GetChild(0).GetChild(0).GetComponent<Text>().text = KeyName(CurrentKeys[i]);
    }

    #endregion
}