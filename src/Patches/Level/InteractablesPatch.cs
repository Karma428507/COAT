namespace Patches.Level;

using HarmonyLib;
using UnityEngine;

using System;
using System.Collections.Generic;

using COAT;
using COAT.Input;
using COAT.Net;
using COAT.World;

[HarmonyPatch(typeof(Breakable))]
public class BreakablePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Breakable.Break), new[] {typeof(float)})]
    static void Enter(Breakable __instance)
    {
        if (LobbyController.Offline)
            return;

        Vector3 pos = __instance.transform.position;

        Log.Debug($"Broken object at {pos}");
    }
}