namespace COAT.UI.Fragments;

using UnityEngine;

//using Jaket.Net;
using COAT.UI;
using COAT.UI.Menus;

using static Rect;
using UnityEngine.UI;

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
        // I forgot what this does, ignore it
        Events.OnMainMenuLoaded += Toggle;

        // Sets the parent for the leftside UI and remove the text
        leftside = Tools.ObjFind("Main Menu (1)/LeftSide");

        // This was supose to disable the text above the continue button but doesn't
        Tools.ObjFind("Main Menu (1)/LeftSide/Text (2)").SetActive(false);

        // Add a UI button image later
        table = UIB.Rect("Access Table", leftside.transform, new(127.5f, 0f, 420f, 70f)).transform;
        // I would set the position to this but is sends the table very far away
        //table.gameObject.transform.position = new(210, -180f, 0f);
        table.gameObject.AddComponent<HudOpenEffect>();

        UIB.Button("#lobby-tab.list", table, new(0f, 0f, 420f, 70f), clicked: LoadCoatMenu).targetGraphic.color = new(1f, .5f, .5f);
    }

    //private void Update() => table.gameObject.SetActive(menu.activeSelf);

    /// <summary> Toggles visibility of the access table. </summary>
    public void Toggle()
    {
        gameObject.SetActive(Shown = Tools.Scene == "Main Menu");
        Log.Debug($"Toggle value: {Shown}");

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