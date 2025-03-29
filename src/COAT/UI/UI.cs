namespace COAT.UI;

using COAT;
using COAT.UI.Fragments;
using COAT.UI.Menus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.Collections.Generic;
using HarmonyLib;
using System.Linq;

//using Jaket.UI.Fragments;
//using Jaket.World;

/// <summary> Used for menu and ingame UI handling </summary>
internal struct Stack
{

}

/// <summary> Class that loads and manages the interface of the mod. </summary>
public class UI
{
    public static Dictionary<string, object> MenuDict = new Dictionary<string, object>();

    /// <summary> Whether the player is focused on a input field. </summary>
    public static bool Focused => Focus != null && Focus.TryGetComponent<InputField>(out var f) && f.isActiveAndEnabled;
    /// <summary> Whether the player is in any of Jaket dialog. </summary>
    //public static bool AnyDialog => Chat.Shown || LobbyTab.Shown || LobbyList.Shown || PlayerList.Shown || Settings.Shown || SpraySettings.Shown || (OptionsManager.Instance?.paused ?? false);
    /// <summary> Whether any interface that blocks movement is currently visible. </summary>
    //public static bool AnyMovementBlocking => AnyDialog || NewMovement.Instance.dead || Movement.Instance.Emoji != 0xFF;

    /// <summary> Object on which the player is focused. </summary>
    public static GameObject Focus => EventSystem.current?.currentSelectedGameObject;
    /// <summary> Object containing the entire interface. </summary>
    public static Transform Root;
    /// <summary> An array for the menu stack </summary>
    public static List<IMenuInterface> MenuStack = new List<IMenuInterface>();

    /// <summary> Creates singleton instances of fragments and dialogs. </summary>
    public static void Load()
    {
        Root = Tools.Create("UI").transform;

        Home.Build("Lobby List", false, true);
        MainMenuAccess.Build("Main Menu Access", false, true);
        // reload
        /*Settings.Load(); // settings must be loaded before building the interface

        Chat.Build("Chat", true, true, hide: () => Chat.Instance.Field?.gameObject.SetActive(Chat.Shown = false));
        LobbyTab.Build("Lobby Tab", false, true);*/
        GamemodeList.Build("Gamemode List", false, true);
        
        /*PlayerList.Build("Player List", false, true);
        Settings.Build("Settings", false, true);
        SpraySettings.Build("Spray Settings", false, true);
        Debugging.Build("Debugging Menu", false, false);

        PlayerIndicators.Build("Player Indicators", false, false, scene => scene == "Main Menu");
        PlayerInfo.Build("Player Information", false, false, scene => scene == "Main Menu", () => { if (PlayerInfo.Shown) PlayerInfo.Instance.Toggle(); });
        EmojiWheel.Build("Emoji Wheel", false, false);
        Skateboard.Build("Skateboard", false, false);
        InteractiveGuide.Build("Interactive Guide", false, false, hide: () => InteractiveGuide.Instance.OfferAssistance());
        Teleporter.Build("Teleporter", false, false, hide: () => { });*/

        MainMenuAccess.Instance.Toggle();
    }

    /// <summary> Hides the interface of the left group. </summary>
    public static void HideLeftGroup()
    {
        //if (LobbyTab.Shown) LobbyTab.Instance.Toggle();
        //if (PlayerList.Shown) PlayerList.Instance.Toggle();
        //if (Settings.Shown) Settings.Instance.Toggle();
    }

    /// <summary> Hides the interface of the central group. </summary>
    public static void HideCentralGroup()
    {
        //if (LobbyList.Shown) LobbyList.Instance.Toggle();
        //if (SpraySettings.Shown) SpraySettings.Instance.Toggle();
        //if (EmojiWheel.Shown) EmojiWheel.Instance.Hide();
        //if (OptionsManager.Instance.paused) OptionsManager.Instance.UnPause();
    }

    /// <summary> Pushes a menu onto the stack (will check flags) </summary>
    public static void PushStack(IMenuInterface Current)
    {
        if (MenuStack.Count == 0)
            Tools.ObjFindByScene("Main Menu", "Canvas").transform.Find("Main Menu (1)").gameObject.SetActive(false);
        else
            MenuStack[MenuStack.Count - 1].Toggle();

        MenuStack.Add(Current);
        Current.Toggle();
    }

    /// <summary> Pops the menu stack, no value returned since it doesn't matter </summary>
    public static void PopStack()
    {
        if (MenuStack.Count == 0)
            return;

        MenuStack[MenuStack.Count - 1].Toggle();
        MenuStack.RemoveAt(MenuStack.Count - 1);

        if (MenuStack.Count == 0)
            Tools.ObjFindByScene("Main Menu", "Canvas").transform.Find("Main Menu (1)").gameObject.SetActive(true);
        else
            MenuStack[MenuStack.Count - 1].Toggle();
    }

    public static void PopAllStack()
    {
        MenuStack.RemoveAll(e => e.GetType() is IMenuInterface);
    }
}