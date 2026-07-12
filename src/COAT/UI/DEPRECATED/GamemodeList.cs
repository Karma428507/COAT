namespace COAT.UI.Menus;

using COAT;
using COAT.Assets;
using COAT.IO;
using COAT.Net;
using COAT.UI;
using COAT.UI.Elements;
using COAT.UI.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using static COAT.IO.SaveManager;

using static Utils.Pal;
using static Utils.Rect;
using Rect = Utils.Rect;

public class ServerCreation : CanvasSingleton<ServerCreation>, IMenuInterface
{
    public static ServerOptions Options;

    private Toggle pvp, cheats, myEnemy, bosses;
    private Button accessibility, difficulty;
    private InputField field;
    private int gamemode = 1;

    private Image table;
    private Transform gamemodeMenu;
    
    private void Start()
    {
        table = UIB.Table("Server Creator", transform, new(0, 0, 900, 500), table =>
        {
            // for an outline
            UIB.Image(name, table, new(0, 0, 900, 500), null, fill: false);

            // main settings menu
            UIB.Table("Server Creator", table, new(-225, 0, 400, 450), options =>
            {
                UIB.Image(name, options, new(0, 0, 400, 450), null, fill: false);

                field = UIB.Field("#lobby-tab.name", options, Rect.Tgl(40), cons: name => Options.Name = name);
                field.text = Options.Name;

                accessibility = UIB.Button("#lobby-tab.private", options, Rect.Btn(80), clicked: () =>
                {
                    Log.Info($"Server type before: {Options.ServerType}");
                    Options.ServerType = (byte)((int)(++Options.ServerType) % 3);
                    Log.Info($"Server type after: {Options.ServerType}");
                    Rebuild();
                });

                // Change to player limit slider later with the max of 16
                difficulty = UIB.Button("WIP", options, Rect.Btn(120));

                pvp = UIB.Toggle("#lobby-tab.allow-pvp", options, Rect.Tgl(160), clicked: allow => Options.PvP = allow);
                cheats = UIB.Toggle("#lobby-tab.allow-cheats", options, Rect.Tgl(200), clicked: allow => Options.Cheats = allow);
                myEnemy = UIB.Toggle("#lobby-tab.allow-mods", options, Rect.Tgl(240), clicked: allow => Options.Mods = allow);
                bosses = UIB.Toggle("#lobby-tab.heal-bosses", options, Rect.Tgl(280), 20, allow => Options.HealBosses = allow);

                UIB.Button("Play", options, new Rect(0, -190, 380, 40), Pal.white, 24, clicked: () =>
                {
                    Options.Name = field.text;
                    SaveLobby();
                    UI.PushStack(new ServerDiffifcultySelect());
                });
            });

            // gamemode settings menu
            UIB.Table("WIP", "", table, new(225f, 0f, 400f, 450f), wip =>
            {
                UIB.Image("Border", wip, new(0, 0, 400f, 450f), Color.red, fill: false);
                UIB.Text("WIP", wip, new(0f, 0f, 550f, 56f), size: 50);
            }).color = Color.gray * 0.5f;

            // Load the options
            LoadLobby();

            pvp.isOn = Options.PvP;
            cheats.isOn = Options.Cheats;
            myEnemy.isOn = Options.Mods;
            bosses.isOn = Options.HealBosses;
            Rebuild();
        });
    }

    private void Rebuild()
    {
        // Rebuild UI element
        accessibility.GetComponentInChildren<Text>().text = Bundle.Get(Options.ServerType switch
        {
            0 => "lobby-tab.private",
            1 => "lobby-tab.fr-only",
            2 => "lobby-tab.public",
            _ => "lobby-tab.default"
        });
    }

    public void Toggle()
    {
        gameObject.SetActive(Shown = !Shown);
    }
}

public class ServerDiffifcultySelect : IMenuInterface
{
    public static bool loadViaServer = false;

    public void Toggle()
    {
        if (Tools.Scene != "Main Menu")
            return;

        loadViaServer = !Tools.ObjFindMainScene("Canvas/Difficulty Select (1)").activeSelf;
        Tools.ObjFindMainScene("Canvas/Difficulty Select (1)").SetActive(loadViaServer);

        if (Tools.ObjFindMainScene("Canvas/Main Menu (1)").activeSelf)
            Tools.ObjFindMainScene("Canvas/Main Menu (1)").SetActive(false);
    }
}
