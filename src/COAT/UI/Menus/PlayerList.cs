namespace COAT.UI.Menus;

using COAT.Assets;
using COAT.Content;
using COAT.Input;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI.Elements;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

using static Elements.Pal;
using static Elements.Rect;
using Rect = Elements.Rect;

/// <summary> List of all players and teams. </summary>
public class PlayerList : CanvasSingleton<PlayerList>, IMenuInterface
{
    private RectTransform content;
    UnityEngine.UI.Image InfoMenu;
    //Transform PlayerListTransform;
    Friend LastInfoed;
    bool ShowPlayerInfoMenu = false;

    private Toggle pvp, cheats, myEnemy, bosses;
    private Button accessibility, difficulty;
    private InputField field;

    private Image clientBlock;

    private void Start()
    {
        UIB.Table("ServerSettings", transform, Size(1400, 800f), table =>
        {
            UIB.Image("ServerSettings Border", table, new(0, 0, 1400f, 800f), null, fill: false);

            UIB.Table("Players", "#player-list.list", table, new(512f, 0f, 336f, 760f), player =>
            {
                UIB.Image("Players Border", player, new(0, 0, 336f, 760f), null, fill: false);
                content = UIB.Scroll("Player Scroll List", player, new(0, -25, 625f, 760f - 50f)).content;
            });

            UIB.Table("Player Settings", "player settings", table, new(-178, 295, 1004f, 170), player =>
            {
                UIB.Image("Player Settings Border", player, new(0, 0, 1004f, 170f), null, fill: false);

                UIB.Text("Team:", player, new(-300, 0f, 100f, 56f));

                float x = -24f;
                foreach (Team team in System.Enum.GetValues(typeof(Team))) UIB.TeamButton(team, player, new(x += 64f, -130f, 56f, 56f, new(0f, 1f)), () =>
                {
                    Networking.LocalPlayer.Team = team;
                    Events.OnTeamChanged.Fire();

                    Rebuild();
                });

                UIB.Table("WIP", "", player, new(197f, -20f, 550f, 100f), wip =>
                {
                    UIB.Image("Border", wip, new(0, 0, 550f, 100f), Color.red, fill: false);
                    UIB.Text("WIP", wip, new(0f, 0f, 550f, 56f), size: 50);
                }).color = Color.gray * 0.5f;
            });

            UIB.Table("Server Settings", "server settings", table, new(-178, -95, 1004f, 570f), server =>
            {
                UIB.Image("Server Settings Border", server, new(0, 0, 1004f, 570f), null, fill: false);

                field = UIB.Field("#lobby-tab.name", server, new(-321f, -60f, 320f, 32f, new(.5f, 1f)), cons: name =>
                    LobbyController.Lobby?.SetData("name", "<color=#20AAFF>[COAT]</color> " + (LobbyController.ServerName = name)));

                accessibility = UIB.Button("#lobby-tab.private", server, new(-321f, -120f, 320f, 40f, new(.5f, 1f)), clicked: () =>
                {
                    ServerCreation.Options.ServerType = (byte)((int)(++ServerCreation.Options.ServerType) % 3);
                    Rebuild();
                });

                // Change to player limit slider later with the max of 16
                difficulty = UIB.Button("WIP", server, new(-321f, -160f, 320f, 40f, new(.5f, 1f)));

                pvp = UIB.Toggle("#lobby-tab.allow-pvp", server, Rect.Tgl(200), clicked: allow =>
                    LobbyController.Lobby?.SetData("pvp", allow ? "True" : "False"));
                cheats = UIB.Toggle("#lobby-tab.allow-cheats", server, Rect.Tgl(240), clicked: allow =>
                    LobbyController.Lobby?.SetData("cheats", allow ? "True" : "False"));
                myEnemy = UIB.Toggle("#lobby-tab.allow-mods", server, Rect.Tgl(280), clicked: allow =>
                    LobbyController.Lobby?.SetData("mods", allow ? "True" : "False"));
                bosses = UIB.Toggle("#lobby-tab.heal-bosses", server, Rect.Tgl(320), 20, allow =>
                    LobbyController.Lobby?.SetData("heal-bosses", allow ? "True" : "False"));

                pvp.isOn = ServerCreation.Options.pvp;
                cheats.isOn = ServerCreation.Options.Cheats;
                myEnemy.isOn = ServerCreation.Options.Mods;
                bosses.isOn = ServerCreation.Options.healBosses;

                UIB.Table("WIP", "", server, new(251f, -20f, 462f, 490f), wip =>
                {
                    UIB.Image("Border", wip, new(0, 0, 462f, 490f), Color.red, fill: false);
                    UIB.Text("WIP", wip, new(0f, 0f, 550f, 56f), size: 50);
                }).color = Color.gray * 0.5f;
            });

            clientBlock = UIB.Table("Client Block", "", table, new(-178, -95, 1004f, 570f), block =>
            {
                UIB.Image("Border", block, new(0, 0, 1004f, 570f), Color.red, fill: false);
                UIB.Text("Host only", block, new(0f, 0f, 570f, 570f), size: 75); 
            });
            clientBlock.color = Color.black * 0.5f;
            clientBlock.enabled = false;
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
    public void Rebuild()
    {
        ShowPlayerInfoMenu = false;
        // destroy old player list
        //if (transform.childCount > 3) Destroy(transform.GetChild(3).gameObject);
        if (content.childCount > 0) foreach (Transform child in content) Destroy(child.gameObject);

        // Set the server information
        field.text = LobbyController.ServerName;

        accessibility.GetComponentInChildren<Text>().text = Bundle.Get(ServerCreation.Options.ServerType switch
        {
            0 => "lobby-tab.private",
            1 => "lobby-tab.fr-only",
            2 => "lobby-tab.public",
            _ => "lobby-tab.default"
        });

        // Blocks off server controls if needed
        if (LobbyController.IsOwner)
            clientBlock.enabled = false;
        else
            clientBlock.enabled = true;

        float height = (LobbyController.Lobby.Value.MemberCount * 88) + 24f;
        float y = 44f;

        InfoMenu = UIB.Table("PlayerInfoMenu", content, new(99999, 99999, 0, 0), parent =>
        {
            UIB.Image("PlayerInfoMenu Border", parent, new(0, 0, 120, 120), null, fill: false).maskable = false;
        });
        InfoMenu.maskable = false; // btw dont remove the .maskable = false;'s cuz if u do the thingies disappear (or get cut off)

        content.sizeDelta = new(336f, height - 28f);

        foreach (var member in LobbyController.Lobby?.Members)
        {
            UIB.Table("PlayerInfo", content, new(0, y -= 88, 320f, 80f, new(.5f, 1f)), player =>
            {
                string name = member.IsFriend ? "❤️" + member.Name : member.Name;

                if (LobbyController.LastOwner == member.Id)
                    name = "★" + name;

                // Load the main compodents of the UI
                UIB.Button("Player", "", player, new(0, 0, 320f, 80f), Networking.GetTeam(member).Color(), 24, clicked: () => SteamFriends.OpenUserOverlay(member.Id, "steamid"));

                // this and the mask is just to add rounded borders btw
                Image PFP = UIB.ImageButton($"PFP OF {member.Name}", player, new(-125, 0, 50, 50), clicked: () => Tools.Dummy(member.Id.AccountId));
                Mask PFPMASK = UIB.Mask($"PFP MASK OF {member.Name}", player, new(-125, 0, 50, 50), UIB.Background);
                PFP.transform.SetParent(PFPMASK.transform);

                // To load in the PFP
                LoadPFP(member, PFP);

                // add player name
                UIB.Text(name, player, new(30, 12.5f, 230f, 30), align: TextAnchor.MiddleLeft);

                // Load the main compodents of the UI
                UIB.Button("Player", "", player, new(0, 0, 320f, 80f), Networking.GetTeam(member).Color(), 24, clicked: () => SteamFriends.OpenUserOverlay(member.Id, "steamid"));

                // add the + button
                UIB.Button("View", "+", player, new(135f, -15f, 30f, 30f), Networking.GetTeam(member).Color(), 24, clicked: () => PlayerInfoMenu(member, player));

                // (OLD) Add a ban button for the owner
                //if (LobbyController.IsOwner && LobbyController.LastOwner != member.Id)
                //    UIB.IconButton("X", player, Icon(45f, 52.5f), red, clicked: () => Administration.Ban(member.Id.AccountId));
            });
        }
    }

    /// <summary> Make the PlayerInfoMenu, actually fucking exist. </summary>
    private void PlayerInfoMenu(Steamworks.Friend member, Transform parent)
    {
        bool show; // bool for checking to either turn on or off the infomenu (also acts as a toggle)
        if (LastInfoed.Id.AccountId == member.Id.AccountId)
        {
            show = !ShowPlayerInfoMenu;
            ShowPlayerInfoMenu = !ShowPlayerInfoMenu;
        }
        else show = ShowPlayerInfoMenu = true;
        LastInfoed = member;

        if (show)
        {
            // set parent to the player table
            InfoMenu.transform.SetParent(parent);
            InfoMenu.GetComponent<RectTransform>().sizeDelta = new Vector2(120f, 120f); // move the info menu back into hell from earth
            InfoMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(265, 0); // (i put it at like 99999f, 99999f, 0f, 0f)

            // if ur not host, you cant ban or kick. so only show those if ur host. if else? mute.
            if (LobbyController.LastOwner.AccountId == Tools.AccId)
            {
                // Ban Button
                Button BanButton = UIB.Button("Ban", "B", InfoMenu.transform, new(-25, 25, 45, 45), Color.red, 24, clicked: () => Administration.Ban(member.Id.AccountId));
                BanButton.GetComponent<Image>().maskable = false;
                BanButton.GetComponentInChildren<Text>().maskable = false;

                // Kick Button
                Button KickButton = UIB.Button("Kick", "K", InfoMenu.transform, new(25, 25, 45, 45), Pal.orange, 24, clicked: () => Administration.Kick(member.Id.AccountId));
                KickButton.GetComponent<Image>().maskable = false;
                KickButton.GetComponentInChildren<Text>().maskable = false;

                // Mute Button
                Button MuteButton = UIB.Button("Mute/Unmute", "Mute", InfoMenu.transform, new(0, -25, 95, 45), Color.yellow, 24, clicked: () => Administration.Mute(member.Id.AccountId, !Networking.MUTEDPLAYERS.Contains(member.Id.AccountId)));
                MuteButton.GetComponentInChildren<Text>().text = $"{(Networking.MUTEDPLAYERS.Contains(member.Id.AccountId) ? "Unmute" : "Mute")}"; // sets the text to "mute" or "unmute" based on if its.. well.. mute or unmute u dumbass
                MuteButton.GetComponentInChildren<Text>().maskable = false;
                MuteButton.GetComponent<Image>().maskable = false;
            }
            else 
            {
                // Mute Button
                UnityEngine.UI.Button MuteButton = UIB.Button("Mute/Unmute", "Mute", InfoMenu.transform, new(0, 0, 95, 95), Color.yellow, 24, clicked: () => Mute(member.Id.AccountId)); // btw, this is a special(like me) mute, client sided.
                MuteButton.GetComponentInChildren<Text>().text = $"{(Networking.MUTEDPLAYERS.Contains(member.Id.AccountId) ? "Unmute" : "Mute")}"; // sets the text to "mute" or "unmute" based on if its.. well.. mute or unmute u dumbass
                MuteButton.GetComponentInChildren<Text>().maskable = false;
                MuteButton.GetComponent<UnityEngine.UI.Image>().maskable = false;
            }
        }
        else
        {
            InfoMenu.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0); // SEND IT BACK TO EARTH!!!
            InfoMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(99999, 99999); // YEAA!!!! (tbh idk why i set the size to 0f, 0f)
        }
    }

    public async void LoadPFP(Friend member, Image PFP)
    {
        Texture2D image = await Tools.GetSteamPFP(member, new(50, 50), 3);
        PFP.sprite = Sprite.Create(image, new(0, 0, 50, 50), new Vector2(0.5f, 0.5f));
    }

    public void Mute(uint id)
    {
        // it looks kinda bad but this just toggles that player being muted
        if (Networking.MUTEDPLAYERS.Contains(id))
            Networking.MUTEDPLAYERS.Remove(id);
        else
            Networking.MUTEDPLAYERS.Add(id);
    }
}