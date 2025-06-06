namespace COAT.UI.Menus;

using Steamworks.Data;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using COAT.Assets;
using COAT.Net;
//using Jaket.World;

using static Pal;
using static Rect;
using UnityEngine.UI.Extensions;
using COAT.Gamemodes;
using System.Collections.Generic;
using COAT.UI.Menus;
using UnityEngine.SceneManagement;

/// <summary> Browser for public lobbies that receives the list via Steam API and displays it in the scrollbar. </summary>
public class Home : CanvasSingleton<Home>, IMenuInterface
{
    /// <summary> List of lobbies currently displayed. </summary>
    public Lobby[] Lobbies;
    /// <summary> Button that updates the lobby list. </summary>
    private Button refresh;
    /// <summary> String by which the lobby will be searched. </summary>
    private string search = "";
    /// <summary> Content of the lobby list. </summary>
    private RectTransform content;
    private RectTransform optionList;
    private Button newServer;

    /// <summary> Panels to organize the UI </summary>
    private UnityEngine.UI.Image filters;
    private UnityEngine.UI.Image lobbyList;
    private UnityEngine.UI.Image misc;

    private void Start()
    {
        UIB.Table("List", transform, Size(1400, 800f), table =>
        {
            UIB.Image(name, table, new(0, 0, 1400f, 800f), null, fill: false);

            // Top row of buttons
            UIB.IconButton("X", table, new COAT.UI.Rect(630f, 330f, 100f, 100f), red, clicked: UI.PopStack);

            filters = UIB.Table("FilterPanel", table, new COAT.UI.Rect(-60f, 330f, 1240f, 100f), list =>
            {
                UIB.Image(name, list, new(0, 0, 1240f, 100f), null, fill: false);

                refresh = UIB.IconButton("Refresh", list, new COAT.UI.Rect(-512, 0f, 200f, 86f), blue, clicked: Refresh);
                UIB.Field("#lobby-list.search", list, new(96f, 0f, 1016f, 86f), cons: text =>
                {
                    search = text.Trim().ToLower();
                    Rebuild();
                });
            });

            UIB.Text("<size=25>Options</size>", table, new COAT.UI.Rect(510f, 230, 340, 60), align: TextAnchor.MiddleCenter);

            misc = UIB.Table("OptionList", table, new COAT.UI.Rect(510f, -90f, 340, 580f), list =>
            {
                UIB.Image(name, list, new(0, 0, 340, 580f), null, fill: false);

                float height = (4 * 88) + 24f;
                float y = 40;

                optionList = UIB.Scroll("List", list, new(0f, 0, 340f, 573f)).content;
                optionList.sizeDelta = new(336f, height - 28f);

                UIB.Button("Settings", optionList, new(0, y -= 88, 320f, 80f, new(.5f, 1f)),
                    yellow, 24, clicked: () => UI.PushStack(Settings.Instance));

                /*UIB.Button("Spray Settings", optionList, new(0, y -= 88, 320f, 80f, new(.5f, 1f)),
                    green, 24, clicked: SpraySettings.Instance.Toggle);*/

                UIB.Button("Join By Code", optionList, new(0, y -= 88, 320f, 80f, new(.5f, 1f)),
                    orange, 24, clicked: LobbyController.JoinByCode);

                newServer = UIB.Button("New Server", optionList, new(0, y -= 88, 320f, 80f, new(.5f, 1f)),
                    red, 24, clicked: () => UI.PushStack(GamemodeList.Instance));

                UIB.Button("Dummy", optionList, new(0, y -= 88, 320f, 80f, new(.5f, 1f)),
                    black, 24, clicked: () => Tools.Dummy());
            });

            // Body portion
            lobbyList = UIB.Table("LobbyList", table, new COAT.UI.Rect(-180f, -60f, 1000f, 640f), list =>
            {
                UIB.Image(name, list, new(0, 0, 1000f, 640f), null, fill: false);
                content = UIB.Scroll("List", list, new(0f, 0f, 1000f, 640f)).content;
            });
        });
        Refresh();
    }

    /// <summary> Toggles visibility of the lobby list. </summary>
    public void Toggle()
    {
        gameObject.SetActive(Shown = !Shown);
        //Movement.UpdateState();

        if (Shown && transform.childCount > 0) Refresh();
    }

    /// <summary> Rebuilds the lobby list to match the list on Steam servers. </summary>
    public void Rebuild()
    {
        if (LobbyController.Online)
            newServer.image.GetComponentInChildren<Text>().color = newServer.image.color = UnityEngine.Color.gray;
        else
            newServer.image.GetComponentInChildren<Text>().color = newServer.image.color = UnityEngine.Color.red;

        //refresh.GetComponentInChildren<Text>().text = Bundle.Get(LobbyController.FetchingLobbies ? "lobby-list.wait" : "lobby-list.refresh");

        // destroy old lobby entries if the search is completed
        if (!LobbyController.FetchingLobbies)
            foreach (Transform child in content)
                Destroy(child.gameObject);
        if (Lobbies == null) return;

        // look for the lobby using the search string
        var lobbies = search == "" ? Lobbies : Array.FindAll(Lobbies, lobby => lobby.GetData("name").ToLower().Contains(search));
        if (lobbies.Length <= 0) return;

        float height = (lobbies.Length * 120);
        content.sizeDelta = new(1000f, height);

        float y = 60 * (lobbies.Length - 1);
        foreach (var lobby in lobbies)
        {
            if (lobby.GetData("level") == "enu") return;
            bool isMultikill = LobbyController.IsMultikillLobby(lobby);
            string serverName = isMultikill ? "[MULTIKILL] " + lobby.GetData("lobbyName") : lobby.GetData("name");

            UIB.Table("LobbyEntry", content, new COAT.UI.Rect(0, y, 960, 100), entry =>
            {
                UIB.Image(name, entry, new(0, 0, 960, 100), blue, fill: false);

                // text
                var full = lobby.MemberCount <= 2 ? Green : lobby.MemberCount <= 4 ? Orange : Red;
                var info = $"<color=#BBBBBB>{lobby.GetData("level")}</color> <color={full}>{lobby.MemberCount}/{lobby.MaxMembers}</color> ";
                UIB.Text(info, entry, new(0, 30, 960, 50), align: TextAnchor.MiddleRight);
                UIB.Text($" <size=50>{serverName}</size>", entry, new(-100, 20, 740, 50), align: TextAnchor.MiddleLeft);
                UIB.Text("<color=#BBBBBB> TBD (most likely for filters)</color>", entry, new(-100, -30, 740, 50), align: TextAnchor.MiddleLeft);

                // buttons

                UIB.Button("Play", entry, new COAT.UI.Rect(380, -15, 180, 50), align: TextAnchor.MiddleCenter,
                    clicked: () => { if (isMultikill) Bundle.Hud("lobby.mk"); else LobbyController.JoinLobby(lobby); });
            });

            y -= 120;
        }
        return;

    //rip:
    //    UIB.Text("No lobbies are open", lobbyList.transform, new(0, 0, 1000, 650), align: TextAnchor.MiddleCenter);
    }

    /// <summary> Updates the list of public lobbies and rebuilds the menu. </summary>
    public void Refresh()
    {
        LobbyController.FetchLobbies(lobbies =>
        {
            Lobbies = lobbies;
            Rebuild();
        });
        Rebuild();
    }
}