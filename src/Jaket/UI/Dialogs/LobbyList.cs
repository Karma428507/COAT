namespace Jaket.UI.Dialogs;

using Steamworks.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

using Jaket.Assets;
using Jaket.Net;
using Jaket.World;

using static Pal;
using static Rect;
using Jaket.UI.Elements;
using ULTRAKILL.Cheats;
using Steamworks;

/// <summary> Browser for public lobbies that receives the list via Steam API and displays it in the scrollbar. </summary>
public class LobbyList : CanvasSingleton<LobbyList>
{
    /// <summary> List of lobbies currently displayed. </summary>
    public Lobby[] Lobbies;
    /// <summary> Button that updates the lobby list. </summary>
    private Button refresh;
    /// <summary> String by which the lobby will be searched. </summary>
    private string search = "";
    /// <summary> Content of the lobby list. </summary>
    private RectTransform content;

    /// <summary> Panels to organize the UI </summary>
    private UnityEngine.UI.Image filters;
    private UnityEngine.UI.Image lobbyList;
    private UnityEngine.UI.Image friends;
    private UnityEngine.UI.Image misc;

    private void Start()
    {
        UIB.Table("List"/*, "#lobby-list.name"*/, transform, Size(1400, 800f), table =>
        {
            // Top row of buttons
            UIB.IconButton("X", table, new Jaket.UI.Rect(630f, 330f, 100f, 100f), red, clicked: Toggle);
            filters = UIB.Table("FilterPanel", table, new Jaket.UI.Rect(-60f, 330f, 1240f, 100f), list =>
            {
                refresh = UIB.Button("", list, new(100f, 0, 184f, 40f, new(0f, 1f)), clicked: Refresh);
                //UIB.Field("#lobby-list.search", list, new(392f, 0, 384f, 40f, new(0f, 1f)), cons: text =>
                //{
                //    search = text.Trim().ToLower();
                //    Rebuild();
                //});
            });

            // Body portion
            lobbyList = UIB.Table("LobbyList", table, new Jaket.UI.Rect(-180f, -60f, 1000f, 640f), list =>
            {
                content = UIB.Scroll("List", list, new(0f, 0, 1000f, 640f/*, new(.5f, 0f), new(.5f, 0f)*/)).content;
            });

            
            friends = UIB.Table("FriendList", table, new Jaket.UI.Rect(510f, 60f, 340f, 400f), list =>
            {

            });

            // 560 - 800 = 240 - 20 = 220
            misc = UIB.Table("OptionList", table, new Jaket.UI.Rect(510f, -270, 340f, 220), list =>
            {

            });

            //

            // Panel debugging
            filters.color = UnityEngine.Color.white;
            //lobbyList.color = UnityEngine.Color.white;
            friends.color = UnityEngine.Color.white;
            misc.color = UnityEngine.Color.white;
        }).color = new(UnityEngine.Color.cyan.r, UnityEngine.Color.cyan.g, UnityEngine.Color.cyan.b, 100f);
        Refresh();
    }

    /// <summary> Toggles visibility of the lobby list. </summary>
    public void Toggle()
    {
        if (!Shown) UI.HideCentralGroup();

        gameObject.SetActive(Shown = !Shown);
        Movement.UpdateState();

        if (Shown && transform.childCount > 0) Refresh();
    }

    /// <summary> Rebuilds the lobby list to match the list on Steam servers. </summary>
    public void Rebuild()
    {
        refresh.GetComponentInChildren<Text>().text = Bundle.Get(LobbyController.FetchingLobbies ? "lobby-list.wait" : "lobby-list.refresh");

        // destroy old lobby entries if the search is completed
        if (!LobbyController.FetchingLobbies) foreach (Transform child in content) Destroy(child.gameObject);
        if (Lobbies == null) return;

        // look for the lobby using the search string
        var lobbies = search == "" ? Lobbies : Array.FindAll(Lobbies, lobby => lobby.GetData("name").ToLower().Contains(search));

        if (lobbies.Length <= 0)
        {
            UIB.Text("No lobbies are open", lobbyList.transform, new(0, 0, 1000, 650), align: TextAnchor.MiddleCenter);
            return;
        }

        float height = (lobbies.Length * 120) - 20;
        content.sizeDelta = new(1000f, height);

        float y = 60 * (lobbies.Length - 1);
        foreach (var lobby in lobbies)
        {
            bool isMultikill = LobbyController.IsMultikillLobby(lobby);
            string serverName = isMultikill ? "[MULTIKILL] " + lobby.GetData("lobbyName") : lobby.GetData("name");

            UIB.Table("LobbyEntry", content, new Jaket.UI.Rect(0, y, 1000, 100), entry =>
            {
                // text
                var full = lobby.MemberCount <= 2 ? Green : lobby.MemberCount <= 4 ? Orange : Red;
                var info = $"<color=#BBBBBB>{lobby.GetData("level")}</color> <color={full}>{lobby.MemberCount}/{lobby.MaxMembers}</color> ";
                UIB.Text(info, entry, new(0, 30, 1000, 50), align: TextAnchor.MiddleRight);
                UIB.Text($" <size=50>{serverName}</size>", entry, new(0, 20f, 1000, 50), align: TextAnchor.MiddleLeft);
                UIB.Text("<color=#BBBBBB> TBD (most likely for filters)</color>", entry, new(0, -30, 1000, 50), align: TextAnchor.MiddleLeft);

                // buttons
                UIB.Button("\0", entry, new Jaket.UI.Rect(0, 0, 1000, 100), null, 24, TextAnchor.UpperLeft,
                    () =>
                    {
                        // Work on later
                        foreach (Friend member in lobby.Members)
                        {

                            Log.Debug($"ID: {member.Id}");
                            Log.Debug($"Name: {member.Name}");
                        }
                    });

                UIB.Button("Play", entry, new Jaket.UI.Rect(380, -15, 200, 50), align: TextAnchor.MiddleCenter,
                    clicked: () => { if (isMultikill) Bundle.Hud("lobby.mk"); else LobbyController.JoinLobby(lobby); });
            });

            y -= 120;
        }
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

    /// <summary> Displays the members of a selected lobby </summary>
    public void DisplayMembers(Lobby lobby)
    {
        Log.Debug($"Member length: {lobby.MemberCount}");

        foreach (Friend member in lobby.Members)
        {
            
            Log.Debug($"ID: {member.Id}");
            Log.Debug($"Name: {member.Name}");
        }
    }
}
