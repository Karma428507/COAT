namespace COAT.Patches;

using HarmonyLib;

using COAT.Net;
using COAT.World;

[HarmonyPatch(typeof(MinotaurChase))]
public class MinotaurPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("IntroEnd")]
    static void Intro(MinotaurChase __instance) => __instance.enabled = LobbyController.Offline || LobbyController.IsOwner;

    [HarmonyPostfix]
    [HarmonyPatch("HammerSwing")]
    static void Hammer() { if (World.Minotaur && World.Minotaur.IsOwner) World.Minotaur.Attack = 0; }

    [HarmonyPostfix]
    [HarmonyPatch("MeatThrow")]
    static void Meat() { if (World.Minotaur && World.Minotaur.IsOwner) World.Minotaur.Attack = 1; }

    [HarmonyPostfix]
    [HarmonyPatch("HandSwing")]
    static void Hand() { if (World.Minotaur && World.Minotaur.IsOwner) World.Minotaur.Attack = 2; }
}
