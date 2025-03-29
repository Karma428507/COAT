namespace COAT.UI.Fragments;

using UnityEngine;

//using Jaket.Net;
using COAT.UI;
using COAT.UI.Menus;

using static Rect;
using UnityEngine.UI;
using System.Linq;
using TMPro;

/// <summary> Access to the mod functions through the main menu. </summary>
public class MainMenuAccess : CanvasSingleton<MainMenuAccess>
{
    public override ushort Flags => UI_FLAG_MENU;
    /// <summary> Table containing the access buttons. </summary>
    private Transform table;
    /// <summary> Loads the leftside element </summary>
    private GameObject leftside;
    /// <summary> Main menu table. </summary>
    private GameObject menu;

    private void Start()
    {
        // Activate this every time the main menu is loaded
        Events.OnMainMenuLoaded += Rebuild;
        Rebuild();
    }

    public void Rebuild()
    {
        // Sets the parent for the leftside UI and remove the text
        leftside = Tools.ObjFindByScene("Main Menu", "Canvas").transform.Find("Main Menu (1)/LeftSide").gameObject;
                
        // disable the V1 wake text
        leftside.transform.Find("Text (2)").gameObject.SetActive(false);

        // create a button to show the lobby list
        var multiplayerButton = Tools.Instantiate(leftside.transform.Find("Options").gameObject, leftside.transform);
        multiplayerButton.name = "COAT Multiplayer";
        multiplayerButton.transform.localPosition = new(0f, -145f, 0f);
        multiplayerButton.GetComponentInChildren<TMP_Text>().text = "MULTIPLAYER";
        multiplayerButton.GetComponentInChildren<Button>().onClick = new();
        multiplayerButton.GetComponentInChildren<Button>().onClick.AddListener(() => UI.PushStack(Home.Instance));

        // have it replace the V1 wake text in the object activation sequence
        // this also prevents the wake text from re-enabling itelf
        leftside.GetComponent<ObjectActivateInSequence>().objectsToActivate[2] = multiplayerButton;

        // Add a UI button image later
        table = UIB.Rect("Access Table", leftside.transform, new(127.5f, 0f, 420f, 70f)).transform;
        // I would set the position to this but is sends the table very far away
        //table.gameObject.transform.position = new(210, -180f, 0f);
        table.gameObject.AddComponent<HudOpenEffect>();
    }

    //private void Update() => table.gameObject.SetActive(menu.activeSelf);

    /// <summary> Toggles visibility of the access table. </summary>
    public void Toggle()
    {
        gameObject.SetActive(Shown = !Shown);
        Log.Debug($"Toggle value: {Shown}");

        Rebuild();

        // did this because of a bug
        /*if (Shown) {
            Log.Debug("This is active...");
            Tools.ObjFind("Main Menu (1)").SetActive(true);
        }*/
    }

    private void LoadCoatMenu()
    {
        //Tools.ObjFind("Main Menu (1)").SetActive(false);
        //Home.Instance.Toggle();
        UI.PushStack(Home.Instance);
    }
}