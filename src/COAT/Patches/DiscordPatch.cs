namespace COAT.Patches;

using COAT;
using Discord;
using HarmonyLib;

using COAT.Net;
using COAT.UI;

[HarmonyPatch]
public class DiscordPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DiscordController), "SendActivity")]
    static void Activity(ref Activity ___cachedActivity)
    {
        // update the discord activity so everyone can know I've been working hard
        if (LobbyController.Online) ___cachedActivity.State = "Playing multiplayer via COAT :3";
    }

    // Maybe bring this back...
    // But with the UI revamp, we could just add the link inside one of the menus
    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(ShopZone), "Start")]
    static void Button(ShopZone __instance)
    {
        // patch only the most common shops
        if (__instance.name != "Shop") return;

        var button = UIB.DiscordButton("Join Jaket Discord", __instance.transform.GetChild(1).GetChild(1).GetChild(0));
        button.transform.localPosition = new(0f, -128f, -20f); // the button is a little stormy

        // add ControllerPointer so that the button can be clicked
        Tools.Destroy(button.gameObject.AddComponent<ShopButton>()); // hacky
    }*/
}