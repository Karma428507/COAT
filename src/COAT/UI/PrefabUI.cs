namespace COAT.UI;

using COAT.Assets;
using COAT.Net;
using COAT.Patches;
using COAT.UI.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Utils.Rect;

/// <summary> Handles the pre-built UI. </summary>
public static class PrefabUI
{
    /// <summary> The index for the home screen UI. </summary>
    public const int UI_SCREEN_HOME = 1;

    /// <summary> Object containing the entire interface. </summary>
    public static Transform Root;
    /// <summary> The Canvas to load the UI to. </summary>
    private static Transform CanvaseTransform;

    /// <summary> Initializes the prefab UI handler. </summary>
    public static void Load()
    {
        Root = Tools.Create("PrefabUI").transform;
        Events.OnMainMenuLoaded += InitiateCanvas;
    }

    /// <summary> Copies from the root to the canvas. </summary>
    internal static void InitiateCanvas()
    {
        GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in objs)
            if (obj != null && obj.name == "Canvas")
                CanvaseTransform = obj.transform;

        if (CanvaseTransform == null)
            return;

        for (int i = 0; i < Root.childCount; i++)
            Tools.Instantiate(Root.GetChild(i).gameObject, CanvaseTransform);
    }

    /// <summary> Enables the specified UI. </summary>
    public static void EnableUI(int uiIndex)
    {
        for (int i=0; i < CanvaseTransform.childCount; i++)
        {
            GameObject obj = CanvaseTransform.GetChild(i).gameObject;

            switch(uiIndex)
            {
                case UI_SCREEN_HOME:
                    if (obj.name.Contains("Main Menu Coat"))
                    {
                        obj.SetActive(true);

                        // I only did this to disable the main menu, idk if there's a better way
                        if (Tools.Scene == "Main Menu")
                        {
                            MenuEsc menuEsc = obj.GetComponent<MenuEsc>();
                            menuEsc.previousPage = Tools.ObjFindMainScene("Canvas/Main Menu (1)");
                            Tools.ObjFindMainScene("Canvas/Main Menu (1)").SetActive(false);
                        }
                        
                        return;
                    }

                    break;
            }
        }

        Log.Error("Cannot find the specified prefab UI.");
    }
}