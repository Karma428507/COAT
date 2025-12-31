namespace COAT.UI.Menus;

using COAT;
using COAT.Assets;
using COAT.Net;
using COAT.UI;
using COAT.UI.Elements;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Elements.Pal;
using static Elements.Rect;
using static COAT.IO.SaveManager;
using Rect = Elements.Rect;

public class ServerCreation : CanvasSingleton<ServerCreation>, IMenuInterface
{
    public static ServerOptions Options;

    private Toggle pvp, cheats, myEnemy, bosses;
    private Button accessibility, difficulty;
    private InputField field;
    private int gamemode = 1;

    private Image table;
    //private Transform normalMenu;
    private Transform gamemodeMenu;
    
    //private ShadowOptionList shadowOptionList;

    //private GamemodeTypes selectedGamemode = GamemodeTypes.NormalCampain;
    
    private void Start()
    {
        /*Dictionary<string, Action> buttons = new Dictionary<string, Action>();

        foreach (GamemodeTypes type in Enum.GetValues(typeof(GamemodeTypes)))
        {
            GamemodeManager.GamemodeList.TryGetValue(type, out var list);
            buttons.Add(list.Name, () =>
            {
                if (type == GamemodeTypes.PAiN) gamemode = 0;
                else if (type == GamemodeTypes.NormalCampain) gamemode = 1;
                else HudMessageReceiver.Instance?.SendHudMessage("This Gamemode isn't working right now,\nplease fucking deal with it.");

                LoadServerCreator(type);
                GamemodeManager.GetList(gamemode, GameMode => GameMode.LoadSettings(gamemodeMenu));
            });
        }

        UIB.Button("", transform, new Rect(0, 0, 2000, 2000), clicked: Toggle);

        shadowOptionList = ShadowOptionList.Build(transform, "--GAMEMODES--", buttons);

        UIB.Text("[ UNDER CONSTUCTION ]", transform, new(0f, 0f, 1000f, 50f), size: 50).GetComponent<RectTransform>().localRotation = new Quaternion(0f, 0f, .2164f, .9763f);*/

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
                    Options.ServerType = (byte)((int)(++Options.ServerType) % 3);
                    Rebuild();
                });

                // Change to player limit slider later with the max of 16
                difficulty = UIB.Button("WIP", options, Rect.Btn(120));

                pvp = UIB.Toggle("#lobby-tab.allow-pvp", options, Rect.Tgl(160), clicked: allow => Options.pvp = allow);
                cheats = UIB.Toggle("#lobby-tab.allow-cheats", options, Rect.Tgl(200), clicked: allow => Options.Cheats = allow);
                myEnemy = UIB.Toggle("#lobby-tab.allow-mods", options, Rect.Tgl(240), clicked: allow => Options.Mods = allow);
                bosses = UIB.Toggle("#lobby-tab.heal-bosses", options, Rect.Tgl(280), 20, allow => Options.healBosses = allow);

                UIB.Button("Play", options, new Rect(0, -190, 380, 40), Pal.white, 24, clicked: () =>
                {
                    //GamemodeManager.GetList(gamemode, GameMode => GameMode.Start());

                    UI.PushStack(new ServerDiffifcultySelect());
                });
            });

            // gamemode settings menu
            gamemodeMenu = UIB.Image(name, table, new(225, 0, 400, 450), null, fill: false).transform;
            Rebuild();
        });
    }

    private void Rebuild()
    {
        Log.Debug("Rebuilt");

        pvp.isOn = Options.pvp;
        cheats.isOn = Options.Cheats;
        myEnemy.isOn = Options.Mods;
        bosses.isOn = Options.healBosses;

        accessibility.GetComponentInChildren<Text>().text = Bundle.Get(Options.ServerType switch
        {
            0 => "lobby-tab.private",
            1 => "lobby-tab.fr-only",
            2 => "lobby-tab.public",
            _ => "lobby-tab.default"
        });
    }
        
    /*private void LoadServerCreator(GamemodeTypes type)
    {
        //serverCreatorTable.transform.DetachChildren();
        //foreach (Transform child in normalMenu) Destroy(child.gameObject);
        foreach (Transform child in gamemodeMenu) Destroy(child.gameObject);
    }*/

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
