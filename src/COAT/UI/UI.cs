namespace COAT.UI;

using COAT;
using COAT.UI.Fragments;
using COAT.UI.Menus;
using COAT.UI.Overlays;
using COAT.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.Collections.Generic;
using HarmonyLib;
using System.Linq;
using System.Threading.Tasks;
using COAT.Patches;
using COAT.Net;

/// <summary> Class that loads and manages the interface of the mod. </summary>
public class UI
{
    /// <summary> Whether the player is focused on a input field. </summary>
    public static bool Focused => Focus != null && Focus.TryGetComponent<InputField>(out var f) && f.isActiveAndEnabled;
    /// <summary> Whether the player is in any of Jaket dialog. </summary>
    public static bool AnyDialog => ChatUI.Shown || MenuStack.Count > 0 || (OptionsManager.Instance?.paused ?? false);
    /// <summary> Whether any interface that blocks movement is currently visible. </summary>
    public static bool AnyMovementBlocking => AnyDialog || NewMovement.Instance.dead || Movement.Instance.Emoji != 0xFF;

    /// <summary> Object on which the player is focused. </summary>
    public static GameObject Focus => EventSystem.current?.currentSelectedGameObject;
    /// <summary> Object containing the entire interface. </summary>
    public static Transform Root;
    /// <summary> An array for the menu stack </summary>
    public static List<IMenuInterface> MenuStack = new List<IMenuInterface>();
    /// <summary> An array for the overlay </summary>
    public static List<IOverlayInterface> OverlayList = new List<IOverlayInterface>();

    /// <summary> Creates singleton instances of fragments and dialogs. </summary>
    public static void Load()
    {
        Root = Tools.Create("UI").transform;

        
        Home.Build("Lobby List", false, true);
        MainMenuAccess.Build("Main Menu Access", false, true);
        ChatUI.Build("Chat", true, true, hide: () => ChatUI.Instance.Field?.gameObject.SetActive(ChatUI.Shown = false));
        GamemodeList.Build("Gamemode List", false, true);
        PlayerList.Build("Player List", false, true);
        Settings.Build("Settings", false, true);

        Debugging.Build("Debugging Menu", false, false);
        PlayerInfo.Build("Player Information", false, false, scene => scene == "Main Menu", () => { if (PlayerInfo.Shown) PlayerInfo.Instance.Toggle(); });
        SpraySettings.Build("Spray Settings", false, true);
        PlayerIndicators.Build("Player Indicators", false, false, scene => scene == "Main Menu");

        EmojiWheel.Build("Emoji Wheel", false, false);
        Skateboard.Build("Skateboard", false, false);
        Teleporter.Build("Teleporter", false, false, hide: () => { });

        // For overlay UI only
        OverlayList.Add(ChatUI.Instance);

        MainMenuAccess.Instance.Toggle();

        Events.EveryDozen += UpdateOverlayCondition;
    }

    /// <summary> A method used to see if the UI can be used depending on what UI is currently active </summary>
    public static bool CanUseMenu()
    {
        string[] selectors = {"Prelude", "Act I", "Act II", "Act III", "Encore", "Prime"};

        for (int i = 0; i < selectors.Length; i++)
            if (Tools.ObjFindMainScene($"Canvas/Level Select ({selectors[i]})").activeSelf)
                return false;

        return !Tools.ObjFindMainScene("Canvas/Main Menu (1)").activeSelf && !Tools.ObjFindMainScene("Canvas/Chapter Select").activeSelf;
    }

    /// <summary> A method used to see if the UI can be used depending on what UI is currently active </summary>
    public static bool CanUseIngame() => !Tools.ObjFindMainScene("Canvas/PauseMenu").activeSelf && !Tools.ObjFindMainScene("Canvas/OptionsMenu").activeSelf;

    /// <summary> Pushes a menu onto the stack (will check flags) </summary>
    public static void PushStack(IMenuInterface Current)
    {
        if (MenuStack.Count == 0 && Tools.Scene == "Main Menu")
            Tools.ObjFindMainScene("Canvas/Main Menu (1)").SetActive(false);
        else if (MenuStack.Count != 0)
            MenuStack[^1].Toggle();

        MenuStack.Add(Current);
        Current.Toggle();
    }

    /// <summary> Pops the menu stack, no value returned since it doesn't matter </summary>
    public static void PopStack()
    {
        if (MenuStack.Count == 0)
            return;

        MenuStack[^1].Toggle();
        MenuStack.RemoveAt(MenuStack.Count - 1);

        if (MenuStack.Count == 0 && Tools.Scene == "Main Menu")
            Tools.ObjFindMainScene("Canvas/Main Menu (1)").SetActive(true);
        else if (MenuStack.Count != 0)
            MenuStack[^1].Toggle();

        if (Tools.Scene != "Main Menu")
            OptionsManagerPatch.InUI = true;
    }

    public static void PopAllStack()
    {
        MenuStack = new List<IMenuInterface>();
    }

    public static void ToggleUI(IMenuInterface Current)
    {
        if (MenuStack.Count > 0 && MenuStack[^1] == Current)
        {
            MenuStack[^1].Toggle();
            PopAllStack();
        } else
        {
            if (CanUseIngame())
                PushStack(Current);
        }

        Movement.UpdateState();
    }

    public async static void WaitForMenu()
    {
        await Task.Delay(50);

        if (Tools.ObjFindMainScene("Canvas/OptionsMenu").activeSelf)
            Tools.ObjFindMainScene("Canvas/OptionsMenu").SetActive(false);
    }

    protected static void UpdateOverlayCondition()
    {
        if (Tools.Scene == "Main Menu" || LobbyController.Offline)
            return;
    }
}