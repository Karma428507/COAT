using System;
using UnityEngine;
using UnityEngine.UI;
using COAT.Gamemodes;
using COAT.Assets;
using COAT.Net;
using COAT.UI;
using COAT;

//using static Pal;
//using static Rect;

namespace COAT.UI.Menus
{
    public class GamemodeList : CanvasSingleton<GamemodeList>, IMenuInterface
    {
        public static SudoLobby creationLobby;

        private Toggle pvp, cheats, myEnemy, bosses;
        private Button accessibility, difficulty;
        private InputField field;
        private int diff = 0;

        private Image table;
        private Transform normalMenu;
        private Transform gamemodeMenu;

        private GamemodeTypes selectedGamemode = GamemodeTypes.NormalCampain;
        private void Start()
        {
            creationLobby = new SudoLobby();

            UIB.Button("", transform, new Rect(0, 0, 2000, 2000), clicked: Toggle);

            table = UIB.Table("Server Creator", transform, new(300, 0, 900, 500), table =>
            {
                // for an outline
                UIB.Image(name, table, new(0, 0, 900, 500), null, fill: false);

                // main settings menu
                UIB.Table("Server Creator", table, new(-225, 0, 400, 450), options =>
                {
                    UIB.Image(name, options, new(0, 0, 400, 450), null, fill: false);

                    field = UIB.Field("#lobby-tab.name", options, Rect.Tgl(40), cons: name => creationLobby.name = name);
                    field.text = creationLobby.name;

                    accessibility = UIB.Button("#lobby-tab.private", options, Rect.Btn(80), clicked: () =>
                    {
                        creationLobby.type = (byte)((int)(++creationLobby.type) % 3);
                        Rebuild();
                    });

                    difficulty = UIB.Button("N/A", options, Rect.Btn(120), clicked: () =>
                    {
                        //diff = (byte)((int)(++diff) % 5);
                        //Rebuild();
                    });

                    pvp = UIB.Toggle("#lobby-tab.allow-pvp", options, Rect.Tgl(160), clicked: allow => creationLobby.pvp = allow);
                    cheats = UIB.Toggle("#lobby-tab.allow-cheats", options, Rect.Tgl(200), clicked: allow => creationLobby.cheats = allow);
                    myEnemy = UIB.Toggle("#lobby-tab.allow-mods", options, Rect.Tgl(240), clicked: allow => creationLobby.modded = allow);
                    bosses = UIB.Toggle("#lobby-tab.heal-bosses", options, Rect.Tgl(280), 20, allow => creationLobby.healBosses = allow);

                    UIB.Button("Play", options, new Rect(0, -190, 380, 40), null, 24, clicked: () => {UI.PushStack(new ServerDiffifcultySelect());});
                });

                // gamemode settings menu
                UIB.Image(name, table, new(225, 0, 400, 450), null, fill: false);
                Rebuild();

            });

            UIB.Table("Gamemode List", transform, new(-500, 0, 400, 126 * Enum.GetNames(typeof(GamemodeTypes)).Length), table =>
            {
                int y = 48 * Enum.GetNames(typeof(GamemodeTypes)).Length;

                UIB.Image(name, table, new(0, 0, 400, 126 * Enum.GetNames(typeof(GamemodeTypes)).Length), null, fill: false);

                foreach (GamemodeTypes type in Enum.GetValues(typeof(GamemodeTypes)))
                {
                    var div = UIB.Table("LobbyEntry", table, new Rect(0, y, 350f, 100f), entry =>
                    {
                        GamemodeManager.GamemodeList.TryGetValue(type, out var list);
                        UIB.Text(list.Name, entry, new(0, 20f, 350f, 100f));

                        // buttons
                        UIB.Button(" ", entry, new Rect(0, 0, 350f, 100),
                            null, 24, TextAnchor.UpperLeft, () => {
                                // TODO: make this shit work, once we start trying to make gamemodes
                                if (type != GamemodeTypes.NormalCampain)
                                    HudMessageReceiver.Instance?.SendHudMessage("Only the normal campain is working right now,\\nplease fucking deal with it.");
                            });
                    });

                    if (type == selectedGamemode)
                        div.color = Color.yellow;

                    y -= 120;
                }
                
                UIB.Text("[ UNDER CONSTUCTION ]", table, new(0f, 0f, 1000f, 50f), size: 50).GetComponent<RectTransform>().localRotation = new Quaternion(0f, 0f, .2164f, .9763f);
            });
        }

        private void Rebuild()
        {
            pvp.isOn = creationLobby.pvp;
            cheats.isOn = creationLobby.cheats;
            myEnemy.isOn = creationLobby.modded;
            bosses.isOn = creationLobby.healBosses;

            accessibility.GetComponentInChildren<Text>().text = Bundle.Get(creationLobby.type switch
            {
                0 => "lobby-tab.private",
                1 => "lobby-tab.fr-only",
                2 => "lobby-tab.public",
                _ => "lobby-tab.default"
            });

            /*difficulty.GetComponentInChildren<Text>().text = diff switch
            {
                0 => "HARMLESS",
                1 => "LENIENT",
                2 => "STANDARD",
                3 => "VIOLENT",
                4 => "BRUTAL",
                _ => "ULTRAKILL MUST DIE"
            };*/

            //creationLobby.Debug();
        }
        
        private void LoadServerCreator(GamemodeTypes type)
        {
            //serverCreatorTable.transform.DetachChildren();
            foreach (Transform child in normalMenu) Destroy(child.gameObject);
            foreach (Transform child in gamemodeMenu) Destroy(child.gameObject);
        }

        public void Toggle()
        {
            gameObject.SetActive(Shown = !Shown);
            
            //Movement.UpdateState();
        }
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