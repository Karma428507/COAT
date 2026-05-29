namespace COAT.UI.Menus.Sub;

using COAT.Assets;
using COAT.Content;
using COAT.Input;
using COAT.Net;
using COAT.Sprays;
using COAT.UI.Elements;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Elements.Pal;
using static Elements.Rect;
using static UnityEngine.ParticleSystem.PlaybackState;

/// <summary> Other setting UI (rework post release). </summary>
public class GeneralSettings : SettingsPage<GeneralSettings>
{
    static PrefsManager pm => PrefsManager.Instance;

    /// <summary> General settings buttons. </summary>
    private Button lang, feed, knkl;

    /// <summary> Default team table. </summary>
    GameObject Defa;
    /// <summary> "Default team:" text. </summary>
    GameObject DTXT;

    private void Start()
    {
        UIB.Table("Settings", transform, Size(600f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 600f, 800f), null, fill: false);

            // All UI below the LineBreak.
            UIB.Text("General", table, new(10f, 360f, 560f, 40f), Pal.white, 40, TextAnchor.MiddleLeft);

            UIB.Button("#settings.reset", table, new(-40f, 310f, 425f, 40f), clicked: ResetGeneral);

            lang = UIB.Button("Language", table, new(-40f, 260f, 425f, 40f), clicked: () => // this may look like a ctrl+c ctrl+v but.. its actually not. i only realised after that its 1:1
            {
                pm.SetString("jaket.locale", Bundle.Codes[Settings.Language = ++Settings.Language % Bundle.Codes.Length]);
                Rebuild();
            });

            UIB.Toggle("#settings.freeze", table, new(-90f, -180, 320f, 32f, new(.5f, 1f)), 22, _ =>
            {
                pm.SetBool("jaket.disable-freeze", Settings.DisableFreezeFrames = _);
            }).isOn = Settings.DisableFreezeFrames;

            UIB.Image1("LineBreak", table, new(0f, 190f, 560f, 4f), Pal.white, null, true);

            UIB.Text("Appearance", table, new(10f, 160f, 560f, 40f), Pal.white, 40, TextAnchor.MiddleLeft);

            UIB.Text("FEEDBACKER:", table, new(-40f, 120f, 405f, 42f), align: TextAnchor.MiddleLeft);
            feed = UIB.Button("FEEDBACKER:", "", table, Wtf(-280f, 80f), clicked: () =>
            {
                pm.SetInt("jaket.feed-color", Settings.FeedColor = ++Settings.FeedColor % 3);
                Rebuild();
            });

            UIB.Text("KNUCKLE:", table, new(-40f, 82f, 405f, 42f), align: TextAnchor.MiddleLeft);
            knkl = UIB.Button("KNUCKLE:", "", table, Wtf(-318f, 80f), clicked: () =>
            {
                pm.SetInt("jaket.knkl-color", Settings.KnuckleColor = ++Settings.KnuckleColor % 3);
                Rebuild();
            });

            Settings.GetDefaultTeam(out Team enumValue);
            Defa = UIB.Table("Default Team", table, new(-40f, 0f, 425f, 40f), enumValue.Color(), team =>
            {
                DTXT = UIB.Text("Default Team:", team, new(0f, 40f, 425f, 40f), enumValue.Color(), 22, TextAnchor.MiddleLeft).gameObject;
                team.gameObject.AddComponent<Button>().onClick.AddListener(() =>
                {
                    ChangeDefaultTeamBy(1);
                    Settings.GetDefaultTeam(out Team enumValue);

                    Defa.GetComponentInChildren<Image>().color = enumValue.Color();
                    DTXT.GetComponent<Text>().color = enumValue.Color();
                });
            }).gameObject;

        });
        Rebuild();
    }

    // <summary> Toggles visibility of the spray settings. </summary>
    public override void Toggle()
    {
        if (Shown && transform.childCount > 0) Rebuild();
    }

    public void Rebuild()
    {
        string Mode(int mode) => Bundle.Get(mode switch
        {
            0 => "settings.default",
            1 => "settings.green",
            2 => "settings.vanilla",
            _ => "lobby-tab.default"
        });

        lang.GetComponentInChildren<Text>().text = Bundle.Locales[Settings.Language];
        feed.GetComponentInChildren<Text>().text = Mode(Settings.FeedColor);
        knkl.GetComponentInChildren<Text>().text = Mode(Settings.KnuckleColor);

        // update the color of the feedbacker and knuckleblaster
        Events.OnWeaponChanged.Fire();
    }

    public void Refresh()
    {
        Rebuild();
    }

    private void ResetGeneral()
    {
        pm.SetString("jaket.locale", Bundle.Codes[Bundle.LoadedLocale]);
        pm.DeleteKey("jaket.disable-freeze");

        Settings.Load();
        Rebuild();
        transform.GetChild(1).GetChild(7).GetComponent<Toggle>().isOn = Settings.DisableFreezeFrames;
    }

    private void ResetAppearance()
    {
        pm.DeleteKey("jaket.feed-color");
        pm.DeleteKey("jaket.knkl-color");
        pm.SetInt("COAT.default-team", 0);

        Settings.Load();
        Rebuild();
    }

    private void ChangeDefaultTeamBy(int value)
    {
        int previous = pm.GetInt("COAT.default-team");
        pm.SetInt("COAT.default-team", previous + value);
    }
}

public class ControlSettings : SettingsPage<ControlSettings>
{
    static PrefsManager pm => PrefsManager.Instance;

    private void Start()
    {
        UIB.Table("Settings", transform, Size(600f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 600f, 800f), null, fill: false);

            UIB.Text("Controls", table, new(-60f, 360f, 425f, 42f), Pal.white, 48, TextAnchor.MiddleLeft);

            UIB.Button("Reset", "#settings.reset", table, new(0f, 310f, 560f, 40f), clicked: ResetControls);

            RectTransform ControlsScroll = UIB.Scroll("Controls Scroll", table, new(0f, 10f, 560f, 520f), 445f, 520f).content;
            for (int completedkeybinds = 0; completedkeybinds < Keybinds.KeybindString.Length; completedkeybinds++)
                UIB.KeyButton(Keybinds.KeybindString[completedkeybinds], Keybinds.CurrentKeys[completedkeybinds], ControlsScroll, new(0f, (-20f + completedkeybinds * -40f) + 260, 560f, 40f));
        });

        Rebuild();
    }

    // <summary> Toggles visibility of the spray settings. </summary>
    public override void Toggle()
    {
        if (Shown && transform.childCount > 0) Rebuild();
    }

    public void Rebuild()
    {

    }

    public void Refresh()
    {
        Rebuild();
    }

    private void ResetControls()
    {
        foreach (var name in Keybinds.KeybindString) pm.DeleteKey($"jaket.binds.{name}");

        Settings.Load();
        for (int i = 0; i < Keybinds.KeybindString.Length; i++)
            transform.GetChild(2).GetChild(i + 2).GetChild(0).GetChild(0).GetComponent<Text>().text = Keybinds.KeyName(Keybinds.CurrentKeys[i]);
    }
}

public class ModerationSettings : SettingsPage<ModerationSettings>
{
    static PrefsManager pm => PrefsManager.Instance;

    private void Start()
    {
        UIB.Table("Settings", transform, Size(600f, 800f), table =>
        {
            UIB.Image("Settings Border", table, new(0f, 0f, 600f, 800f), null, fill: false);

            // Old name was "Moderatium" which sounds like a title for the Imperium of Man
            UIB.Text("Moderation", table, new(0f, 356f, 560, 42f), white, 48, TextAnchor.MiddleLeft);

            UIB.Toggle("Enable Moderation", table, new(-90f, -100, 320f, 32f, new(.5f, 1f)), 22, _ =>
            {
                pm.SetBool("COAT.enable-moderation", Settings.EnableModeration = _);
            }).isOn = Settings.EnableModeration;

            UIB.Toggle("Enable Consequences", table, new(-90f, -100, 320f, 32f, new(.5f, 1f)), 22, _ =>
            {
                pm.SetBool("COAT.enable-consequence", Settings.EnableModeration = _);
            }).isOn = Settings.EnableModeration;
        });
        Rebuild();
    }

    // <summary> Toggles visibility of the spray settings. </summary>
    public override void Toggle()
    {
        if (Shown && transform.childCount > 0) Rebuild();
    }

    public void Rebuild()
    {

    }

    public void Refresh()
    {
        Rebuild();
    }
}
