namespace COAT.Assets;

using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Audio;
using UnityEngine.Events;

using COAT.Content;
using COAT.World;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI.Menus;
using COAT.UI.Overlays;
using COAT.IO;

/// <summary> Class that works with the assets bundle of the mod. </summary>
public class DollAssets
{
    // idk if i should remove this
    public const string V1 = "36abcaae9708abc4d9e89e6ec73a2846";

    static NewMovement nm => NewMovement.Instance;
    static FistControl fc => FistControl.Instance;

    /// <summary> Bundle containing assets for player doll. </summary>
    public static AssetBundle Bundle;

    /// <summary> Player doll and its preview prefabs. </summary>
    public static GameObject Doll, Preview;

    /// <summary> Player doll icon. </summary>
    public static Sprite Icon;

    /// <summary> Mixer processing Sam's voice. Used to change volume. </summary>
    public static AudioMixer Mixer;

    /// <summary> Font used by the mod. Differs from the original in support of Cyrillic alphabet. </summary>
    public static Font Font;
    public static TMP_FontAsset FontTMP;

    /// <summary> Shader used by the game for materials. </summary>
    public static Shader Shader;

    /// <summary> Wing textures used to differentiate teams. </summary>
    public static Texture[] WingTextures;

    /// <summary> Hand textures used by local player. </summary>
    public static Texture[] HandTextures;

    /// <summary> Coin texture used by team coins. </summary>
    public static Texture CoinTexture;

    /// <summary> Pain texture used by team coins. </summary>
    public static Texture PainTexture;

    /// <summary> Icons for the emoji selection wheel. </summary>
    public static Sprite[] EmojiIcons, EmojiGlows;

    /// <summary> Loads assets bundle and other necessary stuff. </summary>
    public static void Load()
    {
        Bundle = LoadBundle();

        // cache the shader and the wing textures for future use
        Events.Post(LoadAssets);

        WingTextures = new Texture[6];
        HandTextures = new Texture[4];

        // loading wing textures from the bundle
        for (int i = 0; i < WingTextures.Length; i++)
        {
            var index = i; // C# sucks
            LoadAsync<Texture>("V3-wings-" + ((Team)i).ToString(), tex => WingTextures[index] = tex);
        }

        LoadAsync<Texture>("V3-hand", tex => HandTextures[1] = tex);
        LoadAsync<Texture>("V3-blast", tex => HandTextures[3] = tex);

        LoadAsync<Texture>("PainTexture", tex => PainTexture = tex);

        LoadAsync<Texture>("coin", tex => CoinTexture = tex);

        // load icons for emoji wheel
        EmojiIcons = new Sprite[12];
        EmojiGlows = new Sprite[12];

        for (int i = 0; i < 12; i++)
        {
            var index = i;
            LoadAsync<Sprite>("V3-emoji-" + i, tex => EmojiIcons[index] = tex);
            LoadAsync<Sprite>("V3-emoji-" + i + "-glow", tex => EmojiGlows[index] = tex);
        }

        // I guess async will improve performance a little bit
        LoadAsync<Sprite>("V3-icon", sprite => Icon = sprite);
        LoadAsync<AudioMixer>("sam-audio", mix =>
        {
            Mixer = mix;
            Events.Post(() =>
            {
                Networking.LocalPlayer.Voice.outputAudioMixerGroup = Mixer.FindMatchingGroups("Master")[0];
            });
        });

        Font = Bundle.LoadAsset<Font>("font.ttf");
        FontTMP = TMP_FontAsset.CreateFontAsset(Font);
    }

    private static void LoadAssets()
    {
        Shader = Utils.metalDec20.shader;
        HandTextures[0] = FistControl.Instance.blueArm.ToAsset().GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture;
        HandTextures[2] = FistControl.Instance.redArm.ToAsset().GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture;

        // create prefabs of the player doll and its preview
        LoadAsync<GameObject>("Player Doll.prefab", prefab =>
        {
            Object.DontDestroyOnLoad(prefab);
            FixMaterials(prefab);

            Doll = prefab;
        });

        LoadAsync<GameObject>("Player Doll Preview.prefab", prefab =>
        {
            Object.DontDestroyOnLoad(prefab);
            FixMaterials(prefab);

            Preview = prefab;
        });
    }

    /// <summary> Finds and loads an assets bundle. </summary>
    public static AssetBundle LoadBundle() =>
        AssetBundle.LoadFromFile(FileManager.MergeDLLPath("jaket-assets.bundle"));

    /// <summary> Finds and asynchronously loads an asset. </summary>
    public static void LoadAsync<T>(string name, UnityAction<T> cons) where T : Object
    {
        var task = Bundle.LoadAssetAsync<T>(name);
        task.completed += _ => cons(task.asset as T);
    }

    /// <summary> Changes the colors of materials and their shaders to match the style of the game.. </summary>
    public static void FixMaterials(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>(true))
        {
            // component responsible for drawing the trace
            if (renderer is TrailRenderer) continue;

            // body, rocket & hook materials
            foreach (var mat in renderer.materials)
            {
                mat.color = Color.white;
                mat.shader = Shader;
            }
        }
    }

    /// <summary> Tags after loading from a bundle changes due to a mismatch in the tags list, this method returns everything to its place. </summary>
    public static string MapTag(string tag) => tag switch
    {
        "RoomManager" => "Body",
        "Body" => "Limb",
        "Forward" => "Head",
        _ => tag
    };

    /// <summary> Creates a new player doll from the prefab. </summary>
    public static RemotePlayer CreateDoll()
    {
        // create a doll from the prefab obtained from the bundle
        var obj = Entities.Mark(Doll);

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
            rigidbody.tag = MapTag(rigidbody.gameObject.tag);
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
        /*var obj = Tools.Instantiate(Doll, nm.transform.position, nm.transform.rotation);
        obj.name = "Net"; // idk bro#
        obj.tag = "Dummy";

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
            rigidbody.tag = MapTag(rigidbody.gameObject.tag);
            rigidbody.useGravity = true;
        }

        obj.tag = "Dummy"; // im doing this again incase rigid body is replacing it (i have no idea what the fuck is going on
        // add a script to further control the doll 
        var remotePlayer = obj.AddComponent<RemotePlayer>();
        LocalPlayer localPlayer = new();


        obj.tag = "1";
        remotePlayer.Awake();
        Writer.Write(w => localPlayer.DummyWrite(w), (IntPtr, Int) => Reader.Read(IntPtr, Int, r =>
        {
            remotePlayer.Read(r);
        }), 48);

        remotePlayer.Start();
        remotePlayer.Update();
        remotePlayer.LateUpdate();*/

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

    /// <summary> Returns the hand texture currently in use. Depends on whether the player is in the lobby or not. </summary>
    public static Texture HandTexture(bool feedbacker = true)
    {
        var s = feedbacker ? Settings.FeedColor : Settings.KnuckleColor;
        return HandTextures[(feedbacker ? 0 : 2) + (s == 0 ? (LobbyController.Offline ? 0 : 1) : s == 1 ? 1 : 0)];
    }
}

public static class Utils
{
    private static Material _metalDec20;
    public static Material metalDec20
    {
        get
        {
            if (_metalDec20 == null)
                _metalDec20 = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Material>("Assets/Materials/Environment/Metal/Metal Decoration 20.mat").WaitForCompletion();
            return _metalDec20;
        }
    }
}