namespace COAT.Patches;

using HarmonyLib;

using COAT;
using COAT.Net;

[HarmonyPatch]
class OptionsManagerPatch
{
    public static bool InUI = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(OptionsManager), "Pause")]
    public static bool CanEscape(OptionsManager __instance)
    {
        if (LobbyController.Online && InUI)
            return InUI = false;

        return true;
    }
}
