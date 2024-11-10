namespace Jaket.Patches.COAT;

using Sandbox.Arm;
using HarmonyLib;
using UnityEngine;

using Jaket.Net;
using Jaket.UI;
using Jaket.UI.Dialogs;
using Jaket.Content;

[HarmonyPatch]
public class BuildingPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BuildMode), "OnPrimaryDown")]
    static void OnBuild(BuildMode __instance)
    {
        // Make it send a packet in a better way
        Chat.Instance.Receive($"Item built: {__instance.Name}");
        Networking.Send(PacketType.COAT_Handshake, w =>
        {
            w.Id(12345);
        });
    }
}
