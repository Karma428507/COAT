namespace COAT.UI;

using UnityEngine;

//using Jaket.Net;
using COAT.UI.Elements;
using COAT.UI.Menus;

using static Elements.Rect;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.UI.Extensions;

/// <summary> Access to the mod functions through the main menu. </summary>
public class MainMenuAccess : CanvasSingleton<MainMenuAccess>
{
    /// <summary> Table containing the access buttons. </summary>
    private Transform table;
    /// <summary> Loads the leftside element </summary>
    private GameObject leftside;
    /// <summary> Literally just the "PLAY" button </summary>
    private GameObject play;
    /// <summary> Simple bool to just stop an error from popping up </summary>
    private bool ranOnce = false;

    private void Start()
    {
        // Activate this every time the main menu is loaded
        Events.OnMainMenuLoaded += () => {
            ranOnce = false;
            Rebuild();
        };
        Rebuild();
    }

    /// <summary> Toggles visibility of the access table. </summary>
    public void Toggle()
    {
        gameObject.SetActive(Shown = !Shown);
        Rebuild();
    }

    /// <summary> Creates the menu button </summary>
    public void Rebuild()
    {
        if (ranOnce)
            return;

        // Sets the parent for the leftside UI and remove the text
        leftside = Tools.ObjFindMainScene("Canvas/Main Menu (1)/LeftSide");

        // scale down the continue button to make room for the lobbybutton
        play = Tools.ObjFindMainScene("Canvas/Main Menu (1)/LeftSide/Continue");
        play.GetComponent<RectTransform>().sizeDelta = new Vector2(207.5f, 70f);

        // create a button to show the lobby list
        var LobbyButton = Tools.Instantiate(leftside.transform.Find("Continue").gameObject, leftside.transform);
        LobbyButton.name = "Lobbies";
        LobbyButton.transform.localPosition = new(212.5f, -220f, 0f);
        LobbyButton.GetComponent<RectTransform>().sizeDelta = new Vector2(207.5f, 70f);
        LobbyButton.GetComponentInChildren<TMP_Text>().text = "LOBBIES";
        LobbyButton.GetComponentInChildren<TMP_Text>().color = Color.black;
        LobbyButton.GetComponentInChildren<Image>().color = Color.white;
        LobbyButton.GetComponentInChildren<Button>().onClick = new();
        LobbyButton.GetComponentInChildren<Button>().onClick.AddListener(() => UI.PushStack(Home.Instance));

        // set the lobbybutton as the parent for the continue button so then they activate at the same time
        RectTransform LobbyButtonRect = LobbyButton.GetComponent<RectTransform>();
        play.transform.SetParent(LobbyButtonRect, true);
        play.SetActive(true);

        // activate the lobbybutton (and as well the continue button)
        leftside.GetComponent<ObjectActivateInSequence>().objectsToActivate[4] = LobbyButton;

        // Add a UI button image later
        table = UIB.Rect("Access Table", leftside.transform, new(127.5f, 0f, 420f, 70f)).transform;
        // I would set the position to this but is sends the table very far away
        //table.gameObject.transform.position = new(210, -180f, 0f);
        table.gameObject.AddComponent<HudOpenEffect>();

        ranOnce = true;
    }
}