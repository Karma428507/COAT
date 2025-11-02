namespace COAT.UI.Menus;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using COAT.Assets;
using COAT.World;
using COAT.Content;

using static Pal;
using static Rect;
using COAT.Net;
using System.Net.Sockets;
using COAT.UI.Widgets;

/// <summary> Global mod settings not related to the lobby. </summary>
public class Settings : CanvasSingleton<Settings>, IMenuInterface
{
    static PrefsManager pm => PrefsManager.Instance;

    /// <summary> The list of personally blacklisted mods for a client, acts as a preset when they make a lobby. </summary>
    public static List<string> PersonalBlacklistedMods = new(); 
    #region general

    /// <summary> Id of the currently selected language. </summary>
    public static int Language;
    /// <summary> 0 - default (depending on whether the player is in the lobby or not), 1 - always green, 2 - always blue/red. </summary>
    public static int FeedColor, KnuckleColor;
    /// <summary> The team that coat defaults to when u join a lobby. </summary>
    public static int DefaultTeam;
    /// <summary> Whether freeze frames are disabled. </summary>
    public static bool DisableFreezeFrames;

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

    /// <summary> List of blacklisted mods. </summary>
    RectTransform content;
    /// <summary> Default team table. </summary>
    GameObject Defa;
    /// <summary> "Default team:" text. </summary>
    GameObject DTXT;
    /// <summary> Input field for typing blacklisted mods. </summary>
    InputField Field;
    /// <summary> List of buttons (for main and sprays for now). </summary>
    ShadowOptionList shadowOptionList;

    /// <summary> Loads and applies all settings. </summary>
    public static void Load()
    {
        Language = Bundle.LoadedLocale;
        FeedColor = pm.GetInt("jaket.feed-color");
        KnuckleColor = pm.GetInt("jaket.knkl-color");
        DefaultTeam = pm.GetInt("COAT.default-team");
        DisableFreezeFrames = pm.GetBool("jaket.disable-freeze", true);

        DollAssets.Mixer?.SetFloat("Volume", TTSVolume / 2f - 30f);
    }

    private void Start()
    {
        Dictionary<string, Action> buttons = new Dictionary<string, Action>()
        {
            {"Main Settings", MainOptionList},
            {"Spray Settings", SprayOptionList}
        };

        shadowOptionList = ShadowOptionList.Build(transform, "#settings.general", buttons);

        UIB.Table("Settings", transform, Size(1400f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 1400f, 800f), null, fill: false);

            // All UI above the LineBreak.
            #region top
            UIB.IconButton("X", table, new COAT.UI.Rect(630f, 330f, 100f, 100f), red, clicked: UI.PopStack);

            UIB.Table("Settings Text Box", table, new COAT.UI.Rect(-60f, 330f, 1240f, 100f), list => { UIB.Image("Settings Text Box Border", list, new(0, 0, 1240f, 100f), null, fill: false); });

            UIB.Image1("LineBreak", table, new(0f, 277f, 1355f, 4f), Pal.white, null, true);
            UIB.Text("Settings", table, new(-427f, 333.5f, 445f, 186f), Pal.white, 75, TextAnchor.MiddleLeft);
            #endregion

            // All UI below the LineBreak.
            #region bottom
            #region General and Player Apyearence
            UIB.Table("General", table, new(-457f, 161f, 445f, 199f), general =>
            {
                UIB.Image("General Border", general, new(0f, 0f, 445f, 199f), null, fill: false);
                UIB.Text("General", general, new(0f, 69.5f/*this isnt a joke, my calculations made this number*/, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);

                UIB.Button("#settings.reset", general, new(0f, 25f, 425f, 40f), clicked: ResetGeneral);

                lang = UIB.Button("Language", general, new(0f, -25f, 425f, 40f), clicked: () => // this may look like a ctrl+c ctrl+v but.. its actually not. i only realised after that its 1:1
                {
                    pm.SetString("jaket.locale", Bundle.Codes[Language = ++Language % Bundle.Codes.Length]); 
                    Rebuild();
                });

                UIB.Toggle("#settings.freeze", general, Tgl(171f), 22, _ =>
                {
                    pm.SetBool("jaket.disable-freeze", DisableFreezeFrames = _);
                }).isOn = DisableFreezeFrames;
            });

            UIB.Table("Player Apyearence", table, new(-457, -166f, 445f, 427f), playerapyearence =>
            {
                UIB.Image("Player Apyearence Border", playerapyearence, new(0f, 0f, 445f, 427f), null, fill: false);
                UIB.Text("Apyearence", playerapyearence, new(0f, 183.5f, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);

                UIB.Text("FEEDBACKER:", playerapyearence, new(0f, 140f, 405f, 42f), align: TextAnchor.MiddleLeft);
                feed = UIB.Button("FEEDBACKER:", "", playerapyearence, Wtf(-75f, 240f), clicked: () =>
                {
                    pm.SetInt("jaket.feed-color", FeedColor = ++FeedColor % 3);
                    Rebuild();
                }); 

                UIB.Text("KNUCKLE:", playerapyearence, new(0f, 92f, 405f, 42f), align: TextAnchor.MiddleLeft);
                knkl = UIB.Button("KNUCKLE:", "", playerapyearence, Wtf(-123f, 240f), clicked: () =>
                {
                    pm.SetInt("jaket.knkl-color", KnuckleColor = ++KnuckleColor % 3);
                    Rebuild();
                });

                GetDefaultTeam(out Team enumValue);
                Defa = UIB.Table("Default Team", playerapyearence, new(0f, -183f, 425f, 40f), enumValue.Color(), team =>
                {
                    DTXT = UIB.Text("Default Team:", team, new(0f, 40f, 425f, 40f), enumValue.Color(), 22, TextAnchor.MiddleLeft).gameObject;
                    team.gameObject.AddComponent<Button>().onClick.AddListener(() => 
                    {
                       ChangeDefaultTeamBy(1);
                       GetDefaultTeam(out Team enumValue);

                       Defa.GetComponentInChildren<Image>().color = enumValue.Color();
                       DTXT.GetComponent<Text>().color = enumValue.Color();
                   });
                }).gameObject;
            });
            #endregion

            #region Moderatiun and Modlist
            UIB.Table("Moderatiun", table, new(0f, 84f, 445f, 352f), moderatiun =>
            {
                UIB.Image("Moderatiun Border", moderatiun, new(0f, 0f, 445f, 352f), null, fill: false);
                UIB.Text("Moderatiun", moderatiun, new(0f, 146f, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);
            });

            UIB.Table("Modlist", table, new(0f, -243f, 445f, 274f), modlist =>
            {
                UIB.Image("Modlist Border", modlist, new(0f, 0f, 445f, 274f), null, fill: false);
                UIB.Text("Modlist", modlist, new(0f, 107f, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);

                Field = UIB.Field("Modlist Field", modlist, new(100f, 105f, 212.5f, 40f), 22, OnFocusLost);
                content = UIB.CustSpeedScroll("Modlist Scroll", 3.625f, modlist, new(0f, -24f, 425f, 206f)).content;
            });
            #endregion

            UIB.Table("Controls", table, new(457f, -60f, 445f, 640f), controls =>
            {
                UIB.Image("Controls Border", controls, new(0f, 0f, 445f, 640f), null, fill: false);
                UIB.Text("Controls", controls, new(0f, 290f, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);

                UIB.Button("Reset", "#settings.reset", controls, new(0f, 245f, 425f, 40f), clicked: ResetControls);

                RectTransform ControlsScroll = UIB.Scroll("Controls Scroll", controls, new(0f, -60f, 445f, 520f), 445f, 520f).content;
                for (int completedkeybinds = 0; completedkeybinds < Keybinds.KeybindString.Length; completedkeybinds++)
                    UIB.KeyButton(Keybinds.KeybindString[completedkeybinds], Keybinds.CurrentKeys[completedkeybinds], ControlsScroll, new(0f, (-20f + completedkeybinds * -40f) + 260, 400f, 40f));
            });
            #endregion
        });

        COAT.Version.Label(transform);
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

        text.text = Keybinds.KeyName(key);
        pm.SetInt($"jaket.binds.{path}", (int)key);
        Load();
    }

    // <summary> Toggles visibility of the settings. </summary>
    public void Toggle()
    {
        gameObject.SetActive(Shown = !Shown);
        Movement.UpdateState();
    }

    /// <summary> checks if the player presses enter, clicks off, etc. </summary>
    private void OnFocusLost(string text)
    {
        // this code is so cooked so i hid it behind a method lmao
        Tools.OnFocusLost(
        () => // OnEnter
        {
            Administration.BlacklistMod(text);
            //PersonalBlacklistedMods.Add("d: " + text);
            Field.text = "TYPE MOD NAME HERE";
            Rebuild();
        });
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

        //for (int i = 0; i < 10; i++) this is for debugging
        //    Administration.BlacklistMod($"e{i}");

        foreach (Transform child in content)
            Destroy(child.gameObject); // I LOVE DESTORYING CHILDREN!!!!

        // builds the blacklisted mods!! :D
        int y = 0;
        foreach (string mod in PersonalBlacklistedMods)
        {
            y++;
            Log.Info($"building mod: {mod}");

            Button button = null;
            button = UIB.Button(mod, content, new(0f, -20f - (y * 50), 425f, 40f), clicked: () => { Administration.BlacklistMod(mod); Rebuild(); });

            content.sizeDelta = new Vector2(0f, y * 50);
        }

        foreach (RectTransform child in content)
            child.anchoredPosition += new Vector2(0f, (y + 2) * 25);
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

    private void ChangeDefaultTeamBy(int value)
    {
        int previous = pm.GetInt("COAT.default-team");
        pm.SetInt("COAT.default-team", previous + value);
    }

    public static void GetDefaultTeam(out Team team)
    {
        int DefaultTeamInt = pm.GetInt("COAT.default-team");
        Team enumValue;
        if (Enum.IsDefined(typeof(Team), DefaultTeamInt))
            enumValue = (Team)DefaultTeamInt;
        else
        {
            pm.SetInt("COAT.default-team", 0);
            enumValue = Team.Yellow;
        }

        team = enumValue;
    }

    #region Button nonsense
    private void MainOptionList()
    {

    }

    private void SprayOptionList()
    {

    }
    #endregion
    #region reset

    private void ResetGeneral()
    {
        pm.SetString("jaket.locale", Bundle.Codes[Bundle.LoadedLocale]);
        pm.DeleteKey("jaket.disable-freeze");

        Load();
        Rebuild();
        transform.GetChild(1).GetChild(7).GetComponent<Toggle>().isOn = DisableFreezeFrames;
    }

    private void ResetApyearence()
    {
        pm.DeleteKey("jaket.feed-color");
        pm.DeleteKey("jaket.knkl-color");
        pm.SetInt("COAT.default-team", 0);

        Load();
        Rebuild();
    }

    private void ResetControls()
    {
        foreach (var name in Keybinds.KeybindString) pm.DeleteKey($"jaket.binds.{name}");

        Load();
        for (int i = 0; i < Keybinds.KeybindString.Length; i++)
            transform.GetChild(2).GetChild(i + 2).GetChild(0).GetChild(0).GetComponent<Text>().text = Keybinds.KeyName(Keybinds.CurrentKeys[i]);
    }

    #endregion
}