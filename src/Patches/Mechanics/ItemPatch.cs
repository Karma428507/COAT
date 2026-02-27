namespace Patches.Mechanics;

using HarmonyLib;

using COAT;
using COAT.Entities;
using COAT.Net.Types;

[HarmonyPatch(typeof(ItemIdentifier))]
public class ItemPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(MethodType.Constructor)]
    static void Spawn(ItemIdentifier __instance) => Events.Post(() => Items.Sync(__instance));

    [HarmonyPrefix]
    [HarmonyPatch("PickUp")]
    static void PickUp(ItemIdentifier __instance) => __instance.GetComponent<Item>()?.PickUp();
}
