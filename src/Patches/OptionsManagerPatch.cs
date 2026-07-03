namespace COAT.Patches;

using HarmonyLib;

using COAT;
using COAT.Net;

[HarmonyPatch]
class OptionsManagerPatch
{
    public static bool InUI = false;

    // Looking at the code, this is going to take a while
    // and it would be better to work on when I have the in game UI replaced
    [HarmonyPrefix]
    [HarmonyPatch(typeof(OptionsManager), "Pause")]
    public static bool CanEscape(OptionsManager __instance)
    {
        if (LobbyController.Online && InUI)
            return InUI = false;

        return true;
    }
}
