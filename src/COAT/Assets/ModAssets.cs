namespace COAT.Assets;

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

using COAT.Content;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI.Menus;
using COAT.IO;
using COAT.UI;
using COAT.Utils;

/// <summary> Class that works with the assets bundle of the mod. </summary>
public class ModAssets
{
    static NewMovement nm => NewMovement.Instance;
    static FistControl fc => FistControl.Instance;

    /// <summary> Bundle containing assets for player doll. </summary>
    public static AssetBundle Bundle;

    /// <summary> Player doll and its preview prefabs. </summary>
    public static GameObject Doll, Preview;

    /// <summary> :3 </summary>
    public static GameObject MultiplayerTerminal;

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

    /// <summary> Icons for the emoji selection wheel. </summary>
    public static Sprite[] EmojiIcons, EmojiGlows;

    public static AudioClip SpraySound;

    /// <summary> Loads assets bundle and other necessary stuff. </summary>
    public static void Load()
    {
        Bundle = LoadBundle();

        // Dump asset list
#if DUMP_ASSETS
        Log.Debug("Assets:");
        foreach (string asset in Bundle.GetAllAssetNames())
            Log.Debug($"\t- {asset}");
#endif

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

        LoadAsync<Texture>("coin", tex => CoinTexture = tex);

        LoadAsync<AudioClip>("spray", clip => SpraySound = clip);

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

        LoadAsync<GameObject>("Multiplayer Sandbox Shop.prefab", prefab =>
        {
            Object.DontDestroyOnLoad(prefab);
            FixMaterials(prefab);

            MultiplayerTerminal = prefab;
        });

#if ENABLE_PREFAB_UI
        // Loads the prefab UI into the prefabUI root
        LoadAsyncUI("Main Menu.prefab", "Main Menu Coat");

        // After loading the prefab UI, set them up
        Events.Post(PrefabUI.InitiateCanvas);
#endif
    }

    /// <summary> Finds and loads an assets bundle. </summary>
    private static AssetBundle LoadBundle() =>
        AssetBundle.LoadFromMemory(EmbeddedManager.GetDataFromEmbedded("coat.bundle"));

    /// <summary> Finds and asynchronously loads an asset. </summary>
    private static void LoadAsync<T>(string name, UnityAction<T> cons) where T : Object
    {
        var task = Bundle.LoadAssetAsync<T>(name);
        task.completed += _ => cons(task.asset as T);
    }

    /// <summary> Finds and load UI prefabs into the prefab UI section. </summary>
    private static void LoadAsyncUI(string prefabName, string name)
    {
        LoadAsync<GameObject>(prefabName, prefab =>
        {
            FixMaterials(prefab);
            prefab.SetActive(false);
            Tools.Instantiate(prefab, PrefabUI.Root).name = name;
        });
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