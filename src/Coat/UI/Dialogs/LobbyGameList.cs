namespace Coat.UI.Dialogs;

using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;

using Coat.Content.Gamemodes;
using Coat.Net;

using Jaket;
using Jaket.Assets;
using Jaket.Net;
using Jaket.UI;
using Jaket.World;

using static Jaket.UI.Pal;
using static Jaket.UI.Rect;

/// <summary> Browser for public lobbies that receives the list via Steam API and displays it in the scrollbar. </summary>
public class LobbyGameList : CanvasSingleton<LobbyGameList>
{
    /// <summary> Content of the lobby list. </summary>
    private RectTransform content;

    private void Start()
    {
        UIB.Table("List", "--Coat Gamemodes--", transform, Size(640f, 640f), table =>
        {
            UIB.IconButton("X", table, Icon(292f, 34f), red, clicked: Toggle);
            content = UIB.Scroll("List", table, new(0f, 300, 624f, 544f, new(.5f, 0f), new(.5f, 0f))).content;
        });
        Rebuild();
    }

    // <summary> Toggles visibility of the lobby list. </summary>
    public void Toggle()
    {
        if (!Shown) UI.HideCentralGroup();

        gameObject.SetActive(Shown = !Shown);
        Movement.UpdateState();
        Rebuild();
    }

    /// <summary> Rebuilds the lobby list to match the list on Steam servers. </summary>
    public void Rebuild()
    {
        float height = Gamemode.Modes.Count * 48;
        content.sizeDelta = new(624f, height);

        float y = -24f;
        foreach (var game in Gamemode.Modes)
        {
            var name = " " + game.Name;
            var r = Btn(y += 48f) with { Width = 624f };

            var b = UIB.Button(name, content, r, align: TextAnchor.MiddleLeft, clicked: () => LoadCampaign(game));
        }
    }

    public void LoadCampaign(Gamemode gamemode)
    {
        switch (gamemode.Type)
        {
            case Gamemode_Type.REGULAR:
                Tools.LoadScn("Tutorial");
                break;
            case Gamemode_Type.TF2:
                // Load UI and clear the menu screen
                break;
            default:
                Tools.LoadScn("Tutorial");
                break;
        }

        LobbyController.CreateLobbyCoat(gamemode.Type);
    }
}
