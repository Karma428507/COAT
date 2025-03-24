namespace COAT;

using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;

using Jaket.IO;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

/// <summary> Set of different tools for simplifying life and systematization of code. </summary>
public class Tools
{
    #region networking

    /// <summary> Steam id of the local player. </summary>
    public static SteamId Id => SteamClient.SteamId;
    /// <summary> Account id of the local player. </summary>
    public static uint AccId;

    /// <summary> How could I know that Steamworks do not cache this value? </summary>
    public static void CacheAccId() => AccId = Id.AccountId;
    /// <summary> Returns the name of the player with the given AccountId. </summary>
    public static string Name(uint id) => new Friend(id | 76561197960265728u).Name;

    /// <summary> Shortcut needed in order to track statistics and errors. </summary>
    /*public static void Send(Connection? con, IntPtr data, int size)
    {
        if (con == null)
        {
            Log.Warning("An attempt to send data to the connection equal to null.");
            return;
        }

        con.Value.SendMessage(data, size);
        Stats.Write += size;
    }*/

    #endregion
    #region scene

    /// <summary> Name of the current scene. </summary>
    public static string Scene => SceneHelper.CurrentScene;
    /// <summary> Name of the loading scene. </summary>
    public static string Pending => SceneHelper.PendingScene;

    /// <summary> Loads the given scene. </summary>
    public static void Load(string scene) => SceneHelper.LoadScene(scene);

    /// <summary> Whether the given object is on a scene or is it just an asset. </summary>
    public static bool IsReal(GameObject obj) => obj.scene.name != null;
    public static bool IsReal(Component comp) => IsReal(comp.gameObject);

    #endregion
    #region create, instantiate & destroy

    /// <summary> Creates a new game object and assigns it to the given transform. </summary>
    public static GameObject Create(string name, Transform parent = null)
    {
        GameObject obj = new(name);
        obj.transform.SetParent(parent ?? Plugin.Instance?.transform, false);
        return obj;
    }
    /// <summary> Creates a new game object and adds a component of the given type to it. </summary>
    public static T Create<T>(string name, Transform parent = null) where T : Component => Create(name, parent).AddComponent<T>();

    public static GameObject Instantiate(GameObject obj, Transform parent) => Object.Instantiate(obj, parent);
    public static GameObject Instantiate(GameObject obj, Vector3 position, Quaternion? rotation = null) => Object.Instantiate(obj, position, rotation ?? Quaternion.identity);

    public static void Destroy(Object obj) => Object.Destroy(obj);
    public static void DestroyImmediate(Object obj) => Object.DestroyImmediate(obj);

    #endregion
    #region resources

    public static T[] ResFind<T>() where T : Object => Resources.FindObjectsOfTypeAll<T>();

    /// <summary> Iterates all objects of the type that predicate for the criterion. </summary>
    public static void ResFind<T>(Predicate<T> pred, Action<T> cons) where T : Object
    {
        foreach (var item in ResFind<T>()) if (pred(item)) cons(item);
    }

    public static T ObjFind<T>() where T : Object => Object.FindObjectOfType<T>();
    public static GameObject ObjFind(string name) => GameObject.Find(name);

    /// <summary> Returns the event of pressing the button. </summary>
    public static UnityEvent GetClick(GameObject btn)
    {
        var pointer = btn.GetComponents<MonoBehaviour>()[2]; // so much pain over the private class ControllerPointer
        return AccessTools.Property(pointer.GetType(), "OnPressed").GetValue(pointer) as UnityEvent;
    }

    #endregion
    #region harmony

    /// <summary> Returns the information about a field with the given name. </summary>
    public static FieldInfo Field<T>(string name) => AccessTools.Field(typeof(T), name);

    /// <summary> Gets the value of a field with the given name. </summary>
    public static object Get<T>(string name, T t) => Field<T>(name).GetValue(t);
    /// <summary> Sets the value of a field with the given name. </summary>
    public static void Set<T>(string name, T t, object value) => Field<T>(name).SetValue(t, value);

    /// <summary> Calls a method with the given name. </summary>
    public static void Invoke<T>(string name, T t, params object[] args) => AccessTools.Method(typeof(T), name).Invoke(t, args);
    /// <summary> Calls a method with the given name and a single boolean argument. </summary>
    public static void Invoke<T>(string name, T t, bool arg) => AccessTools.Method(typeof(T), name, new[] { typeof(bool) }).Invoke(t, new object[] { arg });

    #endregion
    #region within

    public static bool Within(Vector3 a, Vector3 b, float dst = 1f) => (a - b).sqrMagnitude < dst * dst;
    public static bool Within(Transform a, Vector3 b, float dst = 1f) => Within(a.position, b, dst);
    public static bool Within(Transform a, Transform b, float dst = 1f) => Within(a.position, b.position, dst);
    public static bool Within(GameObject a, Vector3 b, float dst = 1f) => Within(a.transform.position, b, dst);
    public static bool Within(GameObject a, GameObject b, float dst = 1f) => Within(a.transform.position, b.transform.position, dst);

    #endregion
    #region steam API

    /// <summary>
    /// Converts a member's pfp into a Texture2D
    /// </summary>
    public static async Task<Texture2D> GetSteamPFP(Friend member, Vector2 size, byte type)
    {
        RenderTexture resized = new RenderTexture();
        Texture2D texture = null;
        byte[] bytes;
        uint[] RGBA;
        Image? PFP;

        // Find the optimal pfp or pfp specified by type
        switch (type)
        {
            // small pfp
            case 0:
                PFP = await SteamFriends.GetSmallAvatarAsync(member.Id);
                break;

            // medium pfp
            case 1:
                PFP = await SteamFriends.GetMediumAvatarAsync(member.Id);
                break;

            // large pfp
            case 2:
                PFP = await SteamFriends.GetLargeAvatarAsync(member.Id);
                break;

            // optimal pfp
            default:
                PFP = await SteamFriends.GetSmallAvatarAsync(member.Id);

                if (PFP != null && PFP.Value.Width >= size.x && PFP.Value.Height >= size.y)
                    break;

                PFP = await SteamFriends.GetMediumAvatarAsync(member.Id);

                if (PFP != null && PFP.Value.Width >= size.x && PFP.Value.Height >= size.y)
                    break;

                PFP = await SteamFriends.GetLargeAvatarAsync(member.Id);
                break;
        }

        if (PFP != null)
        {
            bytes = new byte[PFP.Value.Width * PFP.Value.Height * 4];
            RGBA = new uint[PFP.Value.Width * PFP.Value.Height];
            PFP.Value.Data.CopyTo(bytes, 0);

            for (int i = 0; i < PFP.Value.Width * PFP.Value.Height; i++)
            {
                RGBA[i] |= bytes[i * 4 + 0];
                RGBA[i] |= (uint)(bytes[i * 4 + 1] << 8);
                RGBA[i] |= (uint)(bytes[i * 4 + 2] << 16);
                RGBA[i] |= (uint)(bytes[i * 4 + 3] << 24);
            }

            // reverse
            Array.Reverse(RGBA);
            for (int i = 0; i < PFP.Value.Height; i++)
                for (int j = 0; j < PFP.Value.Width / 2; j++)
                    (RGBA[i * PFP.Value.Height + (PFP.Value.Width - j - 1)], RGBA[i * PFP.Value.Height + j]) = (RGBA[i * PFP.Value.Height + j], RGBA[i * PFP.Value.Height + (PFP.Value.Width - j - 1)]);

            for (int i = 0; i < PFP.Value.Width * PFP.Value.Height; i++)
            {
                bytes[i * 4 + 0] = (byte)RGBA[i];
                bytes[i * 4 + 1] = (byte)(RGBA[i] >> 8);
                bytes[i * 4 + 2] = (byte)(RGBA[i] >> 16);
                bytes[i * 4 + 3] = (byte)(RGBA[i] >> 24);
            }

            texture = new Texture2D((int)PFP.Value.Width, (int)PFP.Value.Height, TextureFormat.RGBA32, false);
        }
        else
        {
            bytes = new byte[50 * 50 * 4];
            RGBA = new uint[50 * 50];

            texture = new Texture2D(50, 50, TextureFormat.RGBA32, false);
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                    RGBA[i * 50 + j] = 0xFFFFFFFF;

                RGBA[i * 50 + i] = 0xFF0000FF;
                RGBA[i * 50 + (50 - i - 1)] = 0xFF0000FF;
            }

            for (int i = 0; i < 2500; i++)
            {
                bytes[i * 4 + 0] = (byte)RGBA[i];
                bytes[i * 4 + 1] = (byte)(RGBA[i] >> 8);
                bytes[i * 4 + 2] = (byte)(RGBA[i] >> 16);
                bytes[i * 4 + 3] = (byte)(RGBA[i] >> 24);
            }

        }

        // apply the bytes into the Texture2D
        texture.LoadRawTextureData(bytes);
        texture.Apply();

        // check if there's a need to resize
        if (texture.width != size.x || texture.height != size.y)
        {
            // resize
            resized = new RenderTexture((int)size.x, (int)size.y, 24);
            RenderTexture.active = resized;
            Graphics.Blit(texture, resized);
            texture = new Texture2D((int)size.x, (int)size.y);
            texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
            texture.Apply();
        }

        return texture;
    }

    #endregion
}
