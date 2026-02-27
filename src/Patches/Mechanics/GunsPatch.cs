namespace Patches.Mechanics;

using HarmonyLib;
using UnityEngine;

using COAT;
using COAT.Entities;
using COAT.Net;
using COAT.Net.Types;

[HarmonyPatch]
public class GunsPatch
{
    [HarmonyPostfix]
    //[HarmonyPatch(typeof(GunControl), nameof(GunControl.SwitchWeapon), new Type[] {typeof(int), typeof(List<GameObject>)})]
    [HarmonyPatch(typeof(GunControl), nameof(GunControl.SwitchWeapon))]
    static void GunSwitch() => Events.OnWeaponChanged.Fire();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GunControl), nameof(GunControl.ForceWeapon))]
    static void GunForce() => Events.OnWeaponChanged.Fire();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GunColorGetter), nameof(GunColorGetter.UpdateColor))]
    static bool GunColor(GunColorGetter __instance) => __instance.GetComponentInParent<RemotePlayer>() == null;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WeaponIcon), nameof(WeaponIcon.UpdateIcon))]
    static bool GunIcon(WeaponIcon __instance) => __instance.GetComponentInParent<RemotePlayer>() == null;
}

[HarmonyPatch]
public class ArmsPatch
{
    static LocalPlayer lp => Networking.LocalPlayer;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Punch), "ActiveStart")]
    static void Puncn()
    {
        if (LobbyController.Offline) return;

        foreach (var harpoon in NewMovement.Instance.GetComponentsInChildren<Harpoon>()) Bullets.Punch(harpoon, true);
        Bullets.SyncPunch();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Punch), nameof(Punch.GetParryLookTarget))]
    static void Parry() => lp.Parried = true;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HookArm), "Update")]
    static void Hook(HookArm __instance, bool ___forcingFistControl, Vector3 ___hookPoint, bool ___lightTarget, EnemyIdentifier ___caughtEid)
    {
        if (LobbyController.Offline) return;

        lp.Hook = ___forcingFistControl ? ___hookPoint : Vector3.zero;
        if (__instance.state == HookState.Pulling && ___lightTarget && ___caughtEid.name == "Net") ___caughtEid.GetComponent<Enemy>()?.TakeOwnage();
    }
}
