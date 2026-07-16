namespace Patches.Level;

using COAT.Net;
using COAT.World;
using COAT.Content;
using COAT.Utils;

using HarmonyLib;

[HarmonyPatch]
public class InteractablesPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ObjectActivator), nameof(ObjectActivator.Activate), new[] { typeof(bool) })]
    static void Activate(ObjectActivator __instance)
    {
        if (LobbyController.Online) World.SyncAction(__instance.gameObject);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FinalDoor), nameof(FinalDoor.Open))]
    static void OpenDoor(FinalDoor __instance)
    {
        if (LobbyController.Online) World.SyncAction(__instance, SyncType.FinalDoorUnlock);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Door), nameof(Door.Open))]
    static void OpenCase(Door __instance)
    {
        var n = __instance.name;
        if (LobbyController.Online && LobbyController.IsOwner &&
           (n.Contains("Glass") || n.Contains("Cover") ||
            n.Contains("Skull") || n.Contains("Quake") ||
            Mapping.Scene == "Level 3-1" || __instance.transform.parent?.parent?.name == "MazeWalls")) World.SyncAction(__instance, SyncType.DoorHandler);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Door), nameof(Door.SimpleOpenOverride))]
    static void OpenSpec(Door __instance)
    {
        if (LobbyController.Online && __instance.name == "BayDoor") World.SyncAction(__instance, SyncType.DoorHandler);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Flammable), nameof(Flammable.Burn))]
    static void ActivateBurn(Flammable __instance, float newHeat)
    {
        if (LobbyController.Online && newHeat == 4f) World.SyncAction(__instance, SyncType.BurnObject);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Breakable), nameof(Breakable.Break), new[] { typeof(float) })]
    static void ActivateBreak(Breakable __instance)
    {
        if (LobbyController.Online) World.SyncAction(__instance, SyncType.BurnObject);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StatueActivator), "Start")]
    static void Activate(StatueActivator __instance)
    {
        if (LobbyController.Online && LobbyController.IsOwner) World.SyncAction(__instance, SyncType.ActivateStatue);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BloodFiller), "FullyFilled")]
    static void FillBlood(BloodFiller __instance)
    {
        if (LobbyController.Online && LobbyController.IsOwner) World.SyncAction(__instance, SyncType.FillTree);
    }
}

[HarmonyPatch(typeof(TramControl))]
public class TramPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TramControl.SpeedUp), typeof(int))]
    static void FightStart(TramControl __instance)
    {
        // find the cart in which the player will appear after respawn
        if (LobbyController.Online && Mapping.Scene == "Level 7-1") World.TunnelRoomba = __instance.transform.parent;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TramControl.SpeedUp), typeof(int))]
    static void Up(TramControl __instance) => World.SyncTram(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TramControl.SpeedDown), typeof(int))]
    static void Down(TramControl __instance) => World.SyncTram(__instance);

    [HarmonyPrefix]
    [HarmonyPatch("FixedUpdate")]
    static bool Update() => LobbyController.Offline; // disable check for player distance
}
