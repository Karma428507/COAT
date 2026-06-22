namespace COAT.UI;

using COAT.Net;
using COAT.Patches;
using COAT.UI.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utils.Rect;

/// <summary> Replaces some in-game UI. </summary>
public static class ReplacementUI
{
    /// <summary> Original sandbox button. </summary>
    private static GameObject sandboxOriginal;

    public static void Start()
    {
        // Saves the sandbox button data
        GameObject sandbox;
        sandbox = Tools.ObjFindMainScene("Canvas/Chapter Select/Chapters/Sandbox");
        sandboxOriginal = sandbox;

        // Activate this every time the main menu is loaded
        Events.OnMainMenuLoaded += () => {
            RebuildMainMenu();
            DisplaySecondOption(false);
        };

        Events.OnLoaded += () =>
        {
            if (Tools.Scene == "EarlyAccessEnd" && LobbyController.Online)
                RebuildEarlyAccessEnd();
        };
    }

    /// <summary> Creates the menu button </summary>
    private static void RebuildMainMenu()
    {
        Transform table;
        GameObject leftside;
        GameObject play;

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
        table.gameObject.AddComponent<HudOpenEffect>();
    }

    /// <summary> Creates the menu button </summary>
    private static void RebuildEarlyAccessEnd()
    {
        const string ScreenPath = "Canvas/UnderwaterOverlay/Panel/Skippables";
        const string DiscordPath = $"{ScreenPath}/Panel/Discord";
        const string TwitterPath = $"{ScreenPath}/Panel/Twitter";
        const string YoutubePath = $"{ScreenPath}/Panel/Youtube";

        GameObject screen = Tools.ObjFindMainScene(ScreenPath);
        Button button = Tools.ObjFindMainScene($"{ScreenPath}/Quit Mission").GetComponent<Button>();

        // Changes the text to be more COAT centric
        screen.GetComponentInChildren<TextMeshProUGUI>().text = """
                    <size=48><b>THANKS FOR PLAYING COAT</b></size>

                    ACT I is being worked on currently with it being released in the next major updates
                    
                    For news on it's progress, go to
                    """;

        // Changes social media links
        Tools.ObjFindMainScene(DiscordPath).GetComponent<WebButton>().url = "https://discord.gg/DUEmvMXfq2";
        Tools.ObjFindMainScene(TwitterPath).GetComponent<WebButton>().url = "https://github.com/Karma428507/COAT";

        // Change color/properties/text
        //Tools.ObjFindMainScene(TwitterPath).GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
        Tools.ObjFindMainScene(TwitterPath).GetComponentInChildren<TextMeshProUGUI>().text = "GITHUB";
        Tools.ObjFindMainScene(YoutubePath).GetComponent<Image>().color = new Color(.7f, 0f, 0f);
        Tools.ObjFindMainScene(YoutubePath).GetComponent<Button>().interactable = false;

        // Adds buttons for host and client
        if (LobbyController.IsOwner)
        {
            Button newButton = GameObject.Instantiate(button, screen.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "GO TO SANDBOX";
            Vector3 vector3 = button.transform.position;
            vector3.y += 135f;
            button.transform.position = vector3;

            // idk why this isn't working
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(() => Tools.Load("uk_construct"));
        }
        else
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "LEAVE SERVER";
        }
    }

    public static void DisplaySecondOption(bool value)
    {
        GameObject parent, container, sandbox, museum;
        parent = Tools.ObjFindMainScene("Canvas/Chapter Select/Chapters");

        if (!value)
        {
            // Do these REALLY need brackets?
            try
            {
                container = Tools.ObjFindMainScene("Canvas/Chapter Select/Chapters/Container");
            }
            catch
            {
                container = null;
            }
            
            if (container == null) return;

            Tools.Destroy(container);
            Tools.Instantiate(sandboxOriginal, parent.transform);
            return;
        }

        sandbox = Tools.ObjFindMainScene("Canvas/Chapter Select/Chapters/Sandbox");
        Tools.Destroy(sandbox);
        container = Tools.Instantiate(sandboxOriginal, parent.transform);
        container.name = "Container";
        container.transform.position = sandboxOriginal.transform.position;

        sandbox = Tools.Instantiate(sandboxOriginal, container.transform);
        Vector2 size = sandbox.GetComponent<RectTransform>().sizeDelta;
        size.x -= 20;
        size.x /= 2;
        sandbox.GetComponent<RectTransform>().sizeDelta = size;

        museum = Tools.Instantiate(sandbox, container.transform);
        Vector2 position = new Vector2();
        position.x += 100;
        museum.transform.localPosition = position;
        museum.name = "Museum";

        museum.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        museum.GetComponent<Button>().onClick.AddListener(() => Tools.Load("CreditsMuseum2"));
        museum.GetComponentInChildren<TextMeshProUGUI>().text = "MUSEUM";

        position = new Vector2(0, 0);
        position.x -= 100;
        sandbox.transform.localPosition = position;
    }
}