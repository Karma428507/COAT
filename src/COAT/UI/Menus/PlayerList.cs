namespace COAT.UI.Menus;

using COAT.Assets;
//using COAT.Content;
using COAT.Net;
//using COAT.Net.Types;
using COAT.World;
using Steamworks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using ULTRAKILL.Cheats;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Pal;
using static Rect;

/// <summary> List of all players and teams. </summary>
public class PlayerList : CanvasSingleton<PlayerList>, IMenuInterface
{
    private RectTransform content;
    UnityEngine.UI.Image InfoMenu;
    Transform PlayerListTransform;
    bool ShowPlayerInfoMenu = false;

    private void Start()
    {
        UIB.Table("Teams", "#player-list.team", transform, Tlw(16f + 166f / 2f, 166f), table =>
        {
            UIB.Text("#player-list.info", table, Btn(71f) with { Height = 46f }, size: 16);
        });

        //UIB.Table("List", "#player-list.list", transform, Tlw(198f + 800 / 2f, 800f), table =>
        //{
        //    content = UIB.Scroll("List", table, new Jaket.UI.Rect(0, -1, 336f, 800 - 90f)).content;
        //});


        UIB.Table("List", transform, Size(1400, 800f), table =>
        {
            UIB.Image(name, table, new(0, 0, 1400f, 800f), null, fill: false);

            UIB.Table("List", "#player-list.list", table, new(512f, 0f, 336f, 760f), player =>
            {
                UIB.Image(name, player, new(0, 0, 336f, 760f), null, fill: false);
                content = UIB.Scroll("List", player, new COAT.UI.Rect(0, -25, 625f, 760f - 50f)).content;
            });

            UIB.Table("List", "player settings", table, new(-178, 295, 1004f, 170), player =>
            {
                UIB.Image(name, player, new(0, 0, 1004f, 170f), null, fill: false);

                /*float x = -24f;
                foreach (Team team in System.Enum.GetValues(typeof(Team))) UIB.TeamButton(team, player, new(x += 64f, -130f, 56f, 56f, new(0f, 1f)), () =>
                {
                    Networking.LocalPlayer.Team = team;
                    Events.OnTeamChanged.Fire();

                    Rebuild();
                });*/
            });

            UIB.Table("List", "server settings", table, new(-178, -95, 1004f, 570f), server =>
            {
                UIB.Image(name, server, new(0, 0, 1004f, 570f), null, fill: false);
            });
        });

        Version.Label(transform);
        Rebuild();
    }

    // <summary> Toggles visibility of the player list. </summary>
    public void Toggle()
    {
        gameObject.SetActive(Shown = !Shown);
        Movement.UpdateState();

        if (Shown && transform.childCount > 0) Rebuild();
    }

    /// <summary> Rebuilds the player list to add new players or remove players left the lobby. </summary>
    /*public void Rebuild()
    {
        // destroy old player list
        //if (transform.childCount > 3) Destroy(transform.GetChild(3).gameObject);
        if (content.childCount > 0) foreach (Transform child in content) Destroy(child.gameObject);
        if (LobbyController.Offline) return;


        float height = (LobbyController.Lobby.Value.MemberCount * 88) + 24f;
        float y = 44f;

        content.sizeDelta = new(336f, height - 28f);

        foreach (var member in LobbyController.Lobby?.Members)
        {
            UIB.Table(
                "Player Info", content, new(0, y -= 88, 320f, 80f, new(.5f, 1f)), player => {
                    string name = member.IsFriend ? "❤️" + member.Name : member.Name;

                    if (LobbyController.LastOwner == member.Id)
                        name = "★" + name;

                    // Load the main compodents of the UI
                    UIB.Button("", player, new(0, 0, 320f, 80f),
                        /*Networking.GetTeam(member).Color()*//* Color.white, 24, clicked: () => SteamFriends.OpenUserOverlay(member.Id, "steamid"));

                    UnityEngine.UI.Image PFP = UIB.Image("PFP", player, new(-125, 0, 50, 50));
                    Mask PFPMASK = UIB.Mask($"PFP MASK OF {member.Name}", player, new(-125, 0, 50, 50), UIB.Background);
                    PFP.transform.SetParent(PFPMASK.transform);

                    UIB.Text(name, player, new(30, 12.5f, 230f, 30), align: TextAnchor.MiddleLeft);

                    // Add the + button
                    UIB.Button("+", player, new(135f, -15f, 30f, 30f),
                        /*Networking.GetTeam(member).Color()*//* Color.white, 24,
                        clicked: () => PlayerInfoMenu(member, player));

                    // Add a ban button for the owner
                    //if (LobbyController.IsOwner && LobbyController.LastOwner != member.Id)
                    //    UIB.IconButton("X", player, Icon(45f, 52.5f), red, clicked: () => Administration.Ban(member.Id.AccountId));

                    // To load in the PFPs
                    LoadPFP(member, PFP);
                });
        } 
            }*/

            /// <summary> Rebuilds the player list to add new players or remove players left the lobby. </summary>
    public void Rebuild()
    {
        ShowPlayerInfoMenu = false;
        // destroy old player list
        //if (transform.childCount > 3) Destroy(transform.GetChild(3).gameObject);
        if (content.childCount > 0) foreach (Transform child in content) Destroy(child.gameObject);
        //if (LobbyController.Offline) return;

        float height = (LobbyController.Lobby.Value.MemberCount * 88) + 24f;
        float y = 44f;

        InfoMenu = UIB.Table("PlayerInfoMenu", content, new(99999, 99999, 0, 0), parent =>
        {
            UIB.Image("PlayerInfoMenu Border", parent, new(0, 0, 120, 120), null, fill: false).maskable = false;
        });
        InfoMenu.maskable = false;

        content.sizeDelta = new(336f, height - 28f);

        foreach (var member in LobbyController.Lobby?.Members)
        {
            UIB.Table("Player Info", content, new(0, y -= 88, 320f, 80f, new(.5f, 1f)), player => 
            {
                string name = member.IsFriend ? "❤️" + member.Name : member.Name;

                if (LobbyController.LastOwner == member.Id)
                    name = "★" + name;

                // Load the main compodents of the UI
                UIB.Button("", player, new(0, 0, 320f, 80f), /*Networking.GetTeam(member).Color()*/ Color.white, 24, clicked: () => SteamFriends.OpenUserOverlay(member.Id, "steamid"));
                UnityEngine.UI.Image PFP = UIB.Image("PFP", player, new(-125, 0, 50, 50));
                Mask PFPMASK = UIB.Mask($"PFP MASK OF {member.Name}", player, new(-125, 0, 50, 50), UIB.Background);
                PFP.transform.SetParent(PFPMASK.transform);
                UIB.Text(name, player, new(30, 12.5f, 230f, 30), align: TextAnchor.MiddleLeft);

                // Add the + button
                UIB.Button("+", player, new(135f, -15f, 30f, 30f), /*Networking.GetTeam(member).Color()*/ Color.white, 24, clicked: () => PlayerInfoMenu(member, player, !ShowPlayerInfoMenu));

                // Add a ban button for the owner
                //if (LobbyController.IsOwner && LobbyController.LastOwner != member.Id)
                //    UIB.IconButton("X", player, Icon(45f, 52.5f), red, clicked: () => Administration.Ban(member.Id.AccountId));

                // To load in the PFPs
                LoadPFP(member, PFP);
            });
        }
    }

    /*private void DeletePlayerInfoMenu()
    {
        PlayerListTransform = GameObject.Find("COAT/UI/Player List").transform;

        foreach (Transform child in PlayerListTransform)
            if (child.gameObject.name == "PlayerInfoMenu")
                Destroy(child.gameObject);
    }*/

    private void PlayerInfoMenu(Steamworks.Friend member, Transform parent, bool show)
    {
        if (show)
        {
            InfoMenu.transform.SetParent(parent);
            InfoMenu.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(120f, 120f);
            InfoMenu.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(265, 0);

            // Ban Button
            UnityEngine.UI.Button BanButton = UIB.Button("Ban", "B", InfoMenu.transform, new(-25, 25, 45, 45), Color.red, 24, clicked: () => Administration.Ban(member.Id.AccountId));
            BanButton.GetComponent<UnityEngine.UI.Image>().maskable = false;
            BanButton.GetComponentInChildren<Text>().maskable = false;

            // Kick Button
            UnityEngine.UI.Button KickButton = UIB.Button("Kick", "K", InfoMenu.transform, new(25, 25, 45, 45), Pal.orange, 24, clicked: () => Administration.Kick(member.Id.AccountId));
            KickButton.GetComponent<UnityEngine.UI.Image>().maskable = false;
            KickButton.GetComponentInChildren<Text>().maskable = false;

            // Mute Button
            UnityEngine.UI.Button MuteButton = UIB.Button("Mute/Unmute", "Mute", InfoMenu.transform, new(0, -25, 95, 45), Color.yellow, 24, clicked: () => Administration.Mute(member.Id.AccountId, !Networking.MUTEDPLAYERS.Contains(member.Id.AccountId)));
            MuteButton.GetComponentInChildren<Text>().text = $"{(Networking.MUTEDPLAYERS.Contains(member.Id.AccountId) ? "Unmute" : "Mute")}";
            MuteButton.GetComponentInChildren<Text>().maskable = false;
            MuteButton.GetComponent<UnityEngine.UI.Image>().maskable = false;
        }
        else
        {
            InfoMenu.transform.SetParent(parent);
            InfoMenu.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(0, 0);
            InfoMenu.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(99999, 99999);
        }
        ShowPlayerInfoMenu = !ShowPlayerInfoMenu;
    }

    public async void LoadPFP(Friend member, UnityEngine.UI.Image PFP)
    {
        Texture2D image = await Tools.GetSteamPFP(member, new(50, 50), 3);
        PFP.sprite = Sprite.Create(image, new Rect(0, 0, 50, 50), new UnityEngine.Vector2(0.5f, 0.5f));
    }
}