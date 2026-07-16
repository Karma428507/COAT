namespace COAT.Utils;

using COAT.Assets;
using COAT.Content;
using COAT.IO;
using COAT.Net;
using COAT.Net.Types;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary> Handles with multiple types of assets. </summary>
public class AssetUtils
{
    /// <summary> A debug function to fill a list of asset addresses. </summary>
    public static void DumpGameAssets()
    {
        string path = FileManager.MergeDLLPath("Assets.txt");
        List<string> ToWrite = new();

        foreach (var locator in Addressables.ResourceLocators)
        {
            foreach (var key in locator.Keys)
            {
                if (locator.Locate(key, typeof(object), out IList<IResourceLocation> locations))
                {
                    foreach (var location in locations)
                    {
                        ToWrite.Add(location.PrimaryKey);
                    }
                }
            }
        }

        FileManager.CreateAppendFile(path, ToWrite);
    }

    /// <summary> Creates a new player doll from the prefab. </summary>
    public static RemotePlayer CreateDoll()
    {
        // create a doll from the prefab obtained from the bundle
        var obj = Entities.Mark(ModAssets.Doll);

        // add components
        var enemyId = obj.AddComponent<EnemyIdentifier>();
        var machine = obj.AddComponent<Machine>();

        enemyId.enemyClass = EnemyClass.Machine;
        enemyId.enemyType = EnemyType.V2;
        enemyId.dontCountAsKills = true;
        enemyId.weaknesses = new string[0];
        enemyId.burners = new();
        enemyId.activateOnDeath = new GameObject[0];
        machine.destroyOnDeath = new GameObject[0];
        machine.hurtSounds = new AudioClip[0];

        // add enemy identifier to all doll parts so that bullets can hit it
        foreach (var rigidbody in obj.transform.GetChild(0).GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.gameObject.AddComponent<EnemyIdentifierIdentifier>();
            rigidbody.tag = ModAssets.MapTag(rigidbody.gameObject.tag);
        }

        // add a script to further control the doll
        return obj.AddComponent<RemotePlayer>();
    }

    static LocalPlayer localPlayer = new();
    static Dictionary<uint, Entity> ents => Networking.Entities;
    /// <summary> Creates a new player doll from the prefab. </summary>
    public static void ProduceDoll()
    {
        // create a doll from the prefab obtained from the bundle
        // the instance is created on these coordinates so as not to collide with anything after the spawn
        RemotePlayer remotePlayer = new();
        Writer.Write(w =>
        {
            w.Id(localPlayer.Id);
            w.Enum(localPlayer.Type);
            localPlayer.DumWrite(w);
        }, (Memory, Length) =>
        {
            Reader.Read(Memory, Length, r =>
            {
                var id = r.Id();
                var type = r.Enum<EntityType>();

                if (!ents.ContainsKey(id) || ents[id] == null) ents[id] = Entities.Get(id, type);
                ents[id]?.Read(r);
            });
        }, 48);
    }
}
