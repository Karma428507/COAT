namespace Patches.Mechanics;

using HarmonyLib;
using System;
using UnityEngine;

using COAT.Assets;
using COAT.Net;
using COAT.Entities;
using System.Linq;

[HarmonyPatch]
public class BestiaryPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyInfoPage), "Start")]
    static void Start(ref SpawnableObjectsDatabase ___objects)
    {
        // there is no point in adding V3 twice
        if (Array.Exists(___objects.enemies, obj => obj.identifier == "jaket.v3")) return;

        var v3 = ScriptableObject.CreateInstance<SpawnableObject>();
        var entry = BestiaryEntry.Load();

        // set up all sorts of things
        v3.identifier = "jaket.v3";
        v3.enemyType = EnemyType.Filth;

        v3.backgroundColor = ___objects.enemies[11].backgroundColor;
        v3.gridIcon = DollAssets.Icon;

        v3.objectName = entry.name;
        v3.type = entry.type;
        v3.description = entry.description;
        v3.strategy = entry.strategy;
        v3.preview = DollAssets.Preview;

        // insert V3 after the turret in the list
        Array.Resize(ref ___objects.enemies, ___objects.enemies.Length + 1);
        Array.Copy(___objects.enemies, 15, ___objects.enemies, 16, ___objects.enemies.Length - 16);
        ___objects.enemies[15] = v3;
    }

    /// <summary> This prevents enemies not from prelude from being shown in the bestiary or sandbox. </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BestiaryData), "GetEnemy")]
    static void PostGet(ref int __result, EnemyType enemy)
    {
        if (LobbyController.Offline)
            return;

        if (!Enemies.allowedEnemies.Contains(enemy))
            __result = 0;
    }
}

[Serializable]
public class BestiaryEntry
{
    /// <summary> Bestiary entry fields displayed in terminal. </summary>
    public string name, type, description, strategy;
    /// <summary> Loads the V3 bestiary entry from the bundle. </summary>
    public static BestiaryEntry Load() => JsonUtility.FromJson<BestiaryEntry>(DollAssets.Bundle.LoadAsset<TextAsset>("V3-bestiary-entry").text);
}
