/*using System;
using UnityEngine;
using UnityEngine.UI;
using COAT.Gamemodes;
using Jaket.Assets;
using Jaket.Net;

//using static Pal;
//using static Rect;

namespace Jaket.UI.Dialogs
{
    public class GamemodeList : CanvasSingleton<GamemodeList>
    {
        private SudoLobby creationLobby;

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

                    difficulty = UIB.Button("HARMLESS", options, Rect.Btn(120), clicked: () =>
                    {
                        diff = (byte)((int)(++diff) % 5);
                        Rebuild();
                    });

                    pvp = UIB.Toggle("#lobby-tab.allow-pvp", options, Rect.Tgl(160), clicked: allow => creationLobby.pvp = allow);
                    cheats = UIB.Toggle("#lobby-tab.allow-cheats", options, Rect.Tgl(200), clicked: allow => creationLobby.cheats = allow);
                    myEnemy = UIB.Toggle("#lobby-tab.allow-mods", options, Rect.Tgl(240), clicked: allow => creationLobby.modded = allow);
                    bosses = UIB.Toggle("#lobby-tab.heal-bosses", options, Rect.Tgl(280), 20, allow => creationLobby.healBosses = allow);

                    UIB.Button("Play", options, new Rect(0, -190, 380, 40), null, 24, clicked: () =>
                        {
                            PrefsManager.Instance.SetInt("difficulty", diff);
                            LobbyController.CreateLobby(creationLobby);
                            Tools.Load("Tutorial");
                        });
                });

                // gamemode settings menu
                UIB.Image(name, table, new(225, 0, 400, 450), null, fill: false);

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
                                // change when gamemode is made
                                if (type != GamemodeTypes.NormalCampain)
                                    HudMessageReceiver.Instance?.SendHudMessage("Only the normal campain is active now");
                            });
                    });

                    if (type == selectedGamemode)
                        div.color = Color.yellow;

                    y -= 120;
                }
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

            difficulty.GetComponentInChildren<Text>().text = diff switch
            {
                0 => "HARMLESS",
                1 => "LENIENT",
                2 => "STANDARD",
                3 => "VIOLENT",
                4 => "BRUTAL",
                _ => "ULTRAKILL MUST DIE"
            };

            creationLobby.Debug();
        }
        
        private void LoadServerCreator(GamemodeTypes type)
        {
            //serverCreatorTable.transform.DetachChildren();
            foreach (Transform child in normalMenu) Destroy(child.gameObject);
            foreach (Transform child in gamemodeMenu) Destroy(child.gameObject);
        }

        public void Toggle()
        {
            creationLobby = new SudoLobby();

            if (!Shown)
            {
                UI.HideCentralGroup();
            }

            gameObject.SetActive(Shown = !Shown);
            //Movement.UpdateState();

            //if (Shown && transform.childCount > 0) Refresh();

            Tools.ObjFind("Main Menu (1)").SetActive(!Shown);
            Rebuild();
        }
    }
}
*/