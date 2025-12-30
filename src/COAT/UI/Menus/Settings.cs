namespace COAT.UI.Menus;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using COAT.Assets;
using COAT.Content;
using COAT.Input;
using COAT.Net;
using COAT.UI.Widgets;

using static Pal;
using static Rect;
using COAT.Sprays;


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

    /// <summary> Horriable, just horriable</summary>
    [Obsolete]
    private int SettingsPage = 0;
    private Image GeneralPage, ControlPage, ModerationPage;

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
            {"General", MainOptionList},
            {"Controls", ControlOptionList},
            {"Spray Settings", SprayOptionList},
            {"Moderation (WIP)", ModerationOptionList},
            {"Mod list (WIP)", ModOptionList}
        };

        shadowOptionList = ShadowOptionList.Build(transform, "#settings.general", buttons);

        GeneralPage = UIB.Table("Settings", transform, Size(600f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 600f, 800f), null, fill: false);

            // All UI below the LineBreak.
            UIB.Text("General", table, new(10f, 360f, 560f, 40f), Pal.white, 40, TextAnchor.MiddleLeft);

            UIB.Button("#settings.reset", table, new(-40f, 310f, 425f, 40f), clicked: ResetGeneral);

            lang = UIB.Button("Language", table, new(-40f, 260f, 425f, 40f), clicked: () => // this may look like a ctrl+c ctrl+v but.. its actually not. i only realised after that its 1:1
            {
                pm.SetString("jaket.locale", Bundle.Codes[Language = ++Language % Bundle.Codes.Length]); 
                Rebuild();
            });

            UIB.Toggle("#settings.freeze", table, new(-90f, -180, 320f, 32f, new(.5f, 1f)), 22, _ =>
            {
                pm.SetBool("jaket.disable-freeze", DisableFreezeFrames = _);
            }).isOn = DisableFreezeFrames;

            UIB.Image1("LineBreak", table, new(0f, 190f, 560f, 4f), Pal.white, null, true);

            UIB.Text("Appearance", table, new(10f, 160f, 560f, 40f), Pal.white, 40, TextAnchor.MiddleLeft);

            UIB.Text("FEEDBACKER:", table, new(-40f, 120f, 405f, 42f), align: TextAnchor.MiddleLeft);
            feed = UIB.Button("FEEDBACKER:", "", table, Wtf(-280f, 80f), clicked: () =>
            {
                pm.SetInt("jaket.feed-color", FeedColor = ++FeedColor % 3);
                Rebuild();
            }); 
            
            UIB.Text("KNUCKLE:", table, new(-40f, 82f, 405f, 42f), align: TextAnchor.MiddleLeft);
            knkl = UIB.Button("KNUCKLE:", "", table, Wtf(-318f, 80f), clicked: () =>
            {
                pm.SetInt("jaket.knkl-color", KnuckleColor = ++KnuckleColor % 3);
                Rebuild();
            });

            GetDefaultTeam(out Team enumValue);
            Defa = UIB.Table("Default Team", table, new(-40f, 0f, 425f, 40f), enumValue.Color(), team =>
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

        ControlPage = UIB.Table("Settings", transform, Size(600f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 600f, 800f), null, fill: false);

            UIB.Table("Controls", table, new(0f, 0f, 600f, 800f), controls =>
            {
                UIB.Image("Controls Border", controls, new(0f, 0f, 445f, 640f), null, fill: false);
                UIB.Text("Controls", controls, new(0f, 290f, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);

                UIB.Button("Reset", "#settings.reset", controls, new(0f, 245f, 425f, 40f), clicked: ResetControls);

                RectTransform ControlsScroll = UIB.Scroll("Controls Scroll", controls, new(0f, -60f, 445f, 520f), 445f, 520f).content;
                for (int completedkeybinds = 0; completedkeybinds < Keybinds.KeybindString.Length; completedkeybinds++)
                    UIB.KeyButton(Keybinds.KeybindString[completedkeybinds], Keybinds.CurrentKeys[completedkeybinds], ControlsScroll, new(0f, (-20f + completedkeybinds * -40f) + 260, 400f, 40f));
            });
        });

        ModerationPage = UIB.Table("Settings", transform, Size(600f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 600f, 800f), null, fill: false);

            UIB.Table("Moderatiun", table, new(0f, 0f, 600f, 800f), moderatiun =>
            {
                UIB.Image("Moderatiun Border", moderatiun, new(0f, 0f, 445f, 352f), null, fill: false);
                UIB.Text("Moderatiun", moderatiun, new(0f, 146f, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);
            });
        });

        /*ModsPage = UIB.Table("Settings", transform, Size(600f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 600f, 800f), null, fill: false);

            UIB.Table("Modlist", table, new(0f, -243f, 445f, 274f), modlist =>
            {
                UIB.Image("Modlist Border", modlist, new(0f, 0f, 445f, 274f), null, fill: false);
                UIB.Text("Modlist", modlist, new(0f, 107f, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);

                Field = UIB.Field("Modlist Field", modlist, new(100f, 105f, 212.5f, 40f), 22, OnFocusLost);
                content = UIB.CustSpeedScroll("Modlist Scroll", 3.625f, modlist, new(0f, -24f, 425f, 206f)).content;
            });
        });*/

        ControlPage.enabled = false;
        ModerationPage.enabled = false;

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

        // Change the settings page in the worse way
        switch (SettingsPage)
        {
            case 0:
                GeneralPage.enabled = true;
                ControlPage.enabled = false;
                ModerationPage.enabled = false;

                if (SpraySettings.Instance.enabled)
                    SpraySettings.Instance.Toggle(); 
                break;
            case 1:
                GeneralPage.enabled = false;
                ControlPage.enabled = true;
                ModerationPage.enabled = false;

                if (SpraySettings.Instance.enabled)
                    SpraySettings.Instance.Toggle();
                break;
            case 2:
                GeneralPage.enabled = false;
                ControlPage.enabled = false;
                ModerationPage.enabled = false;

                if (!SpraySettings.Instance.enabled)
                    SpraySettings.Instance.Toggle();
                break;
            case 3:
                GeneralPage.enabled = false;
                ControlPage.enabled = false;
                ModerationPage.enabled = true;

                if (SpraySettings.Instance.enabled)
                    SpraySettings.Instance.Toggle(); 
                break;
        }

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
    private void MenuOptionSet(int option)
    {
        if (SettingsPage == option)
            return;

        SettingsPage = option;
        Rebuild();
    }

    private void MainOptionList() => MenuOptionSet(0);

    private void ControlOptionList() => MenuOptionSet(1);

    private void SprayOptionList() => MenuOptionSet(2);

    private void ModerationOptionList() => MenuOptionSet(3);

    private void ModOptionList()
    {
        HudMessageReceiver.Instance?.SendHudMessage("Mod option list WIP");
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