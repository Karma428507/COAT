namespace Patches.Level;

using HarmonyLib;
using System.Collections.Generic;
using ULTRAKILL.Cheats;
using UnityEngine;

using COAT;
using COAT.Input;
using COAT.Net;
using COAT.Net.Types;
using COAT.World;

[HarmonyPatch(typeof(Door))]
public class DoorPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Door.Lock))]
    static void Lock(Door __instance) => DoorManager.SendNetStatus(__instance, DoorManager.DOOR_STATUS_LOCKED);

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Door.Unlock))]
    static void Unlock(Door __instance) => DoorManager.SendNetStatus(__instance, DoorManager.DOOR_STATUS_UNLOCKED);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Door), nameof(Door.Optimize))]
    static bool Unload() => LobbyController.Offline;
}
