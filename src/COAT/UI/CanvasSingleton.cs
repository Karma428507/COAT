namespace COAT.UI;

using System;
using UnityEngine.UI;

/// <summary> Singleton based on canvas. Used for interface construction. </summary>
public abstract class CanvasSingleton<T> : MonoSingleton<T> where T : CanvasSingleton<T>
{
    /// <summary> Shows the UI based on the toggle condition (uses bits 0-4 as priority) </summary>
    public const ushort UI_FLAG_OVERLAY = 1 << 7;
    /// <summary> UI that only displays in the main menu </summary>
    public const ushort UI_FLAG_MENU = 1 << 6;
    /// <summary> UI that are shown ingame and stops movement (only one can be active at a time and uses esc to exit) </summary>
    public const ushort UI_FLAG_INGAME = 1 << 5;

    /// <summary> Used in UI management to help overlay order and toggling </summary>
    public abstract ushort Flags { get; }
    /// <summary> Whether the canvas is a dialog or fragment. </summary>
    public static bool Dialog { get; private set; }
    /// <summary> Whether the canvas is visible or hidden. </summary>
    public static bool Shown;

    /// <summary> Creates an instance of this singleton. </summary>
    /// <param name="woh"> Width or height will be used to scale the canvas. True - width, false - height. </param>
    /// <param name="dialog"> Dialogs lock the mouse and movement while fragments don't do this. </param>
    public static void Build(string name, bool woh, bool dialog, Func<string, bool> hideCond = null, Action hide = null)
    {
        // initialize the singleton and create a canvas
        UIB.Canvas(name, UI.Root, woh: woh ? 0f : 1f).gameObject.AddComponent<T>();
        Instance.GetComponent<GraphicRaycaster>().enabled = Dialog = dialog;

        hideCond ??= _ => true;
        hide ??= () => Instance.gameObject.SetActive(Shown = false);
        
        // Ignore this, this is when I make the UI manager more advanced
        void Check() {
            if (hideCond(Tools.Scene))
                hide();

            switch (Instance.Flags)
            {
                case UI_FLAG_OVERLAY:
                case UI_FLAG_INGAME:
                    if (Tools.Scene == "Main Menu")
                        hide();
                    break;
                case UI_FLAG_MENU:
                    if (Tools.Scene != "Main Menu")
                        hide();
                    break;
            }
        }

        Check();
        Events.OnLoaded += Check;
    }
}

/// <summary> I hate generics, I just want to make an array for menus </summary>
public interface IMenuInterface
{
    public void Toggle();
}