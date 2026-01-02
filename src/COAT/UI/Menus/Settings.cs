namespace COAT.UI.Menus;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using COAT.Assets;
using COAT.Content;
using COAT.Input;
using COAT.Net;
using COAT.UI.Elements;
using COAT.UI.Menus.Sub;

using static Elements.Pal;
using static Elements.Rect;

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

    /// <summary> Horriable, just horriable</summary>
    [Obsolete]
    private int SettingsPage = 0;

    /// <summary> List of blacklisted mods. </summary>
    RectTransform content;
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

        // Work on later
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

        /*if (!Shown)
        {
            GeneralSettings.Instance.Toggle(false);
            ControlSettings.Instance.Toggle(false);
            SpraySettings.Instance.Toggle(false);
            ModerationSettings.Instance.Toggle(false);
        }*/
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
        // Change the settings page in the worse way
        switch (SettingsPage)
        {
            case 0:
                GeneralSettings.Instance.Toggle(true);
                ControlSettings.Instance.Toggle(false);
                SpraySettings.Instance.Toggle(false);
                ModerationSettings.Instance.Toggle(false);
                break;
            case 1:
                GeneralSettings.Instance.Toggle(false);
                ControlSettings.Instance.Toggle(true);
                SpraySettings.Instance.Toggle(false);
                ModerationSettings.Instance.Toggle(false);
                break;
            case 2:
                GeneralSettings.Instance.Toggle(false);
                ControlSettings.Instance.Toggle(false);
                SpraySettings.Instance.Toggle(true);
                ModerationSettings.Instance.Toggle(false);
                break;
            case 3:
                GeneralSettings.Instance.Toggle(false);
                ControlSettings.Instance.Toggle(false);
                SpraySettings.Instance.Toggle(false);
                ModerationSettings.Instance.Toggle(true);
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
        HudMessageReceiver.Instance?.SendHudMessage("Mod option list currently unavailable");
    }
    #endregion
}

// Lazy dev stuff
[Obsolete]
public abstract class SettingsPage<T> : CanvasSingleton<T> where T : SettingsPage<T>
{
    public abstract void Toggle();

    public void Toggle(bool value)
    {
        gameObject.SetActive(Shown = value);
        Toggle();
    }
}