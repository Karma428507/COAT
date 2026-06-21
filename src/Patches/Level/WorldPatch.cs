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

[HarmonyPatch(typeof(ActivateArena))]
public class ArenaPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Door), nameof(Door.Optimize))]
    static bool Unload() => LobbyController.Offline;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ActivateArena.Activate))]
    static void Activate(ActivateArena __instance)
    {
        // do not allow the doors to close because this will cause a lot of desync
        if (LobbyController.Online) __instance.doors = new Door[0];
    }

    [HarmonyPostfix]
    [HarmonyPatch("OnTriggerEnter")]
    static void Enter(ActivateArena __instance, Collider other, ArenaStatus ___astat)
    {
        // there is a large check caused by complex game logic that has to be repeated
        if (DisableEnemySpawns.DisableArenaTriggers || (__instance.waitForStatus > 0 && (___astat == null || ___astat.currentStatus < __instance.waitForStatus))) return;

        // launch the arena when a remote player enters it
        if (!__instance.activated && other.name == "Net" && other.TryGetComponent<RemotePlayer>(out _)) __instance.Activate();
    }
}

[HarmonyPatch(typeof(CheckPoint))]
public class RoomPatch
{
    /// <summary> Fake needed to prohibit the checkpoint to recreate the rooms at the first activation. </summary>
    private class FakeList : List<GameObject> { public new int Count => 0; }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CheckPoint.ActivateCheckPoint))]
    static void Activate(CheckPoint __instance)
    {
        if (LobbyController.Online) __instance.roomsToInherit = new FakeList();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CheckPoint.OnRespawn))]
    static void ClearRooms(CheckPoint __instance)
    {
        if (LobbyController.Online) __instance.newRooms.Clear();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CheckPoint.OnRespawn))]
    static void Respawn(CheckPoint __instance)
    {
        if (LobbyController.Online)
        {
            __instance.onRestart?.Invoke();
            __instance.toActivate?.SetActive(true);

            var trn = __instance.transform;
            Movement.Instance.Respawn(trn.position + trn.right * .1f + Vector3.up * 1.25f, trn.eulerAngles.y);
        }
    }
}
