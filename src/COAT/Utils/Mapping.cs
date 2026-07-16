namespace COAT.Utils;

using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;

using COAT.IO;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;

/// <summary> Deals with changing scenes. </summary>
public class Mapping
{
    /// <summary> Name of the current scene. </summary>
    public static string Scene => SceneHelper.CurrentScene;
    /// <summary> Name of the loading scene. </summary>
    public static string Pending => SceneHelper.PendingScene;

    /// <summary> Loads the given scene. </summary>
    public static void Load(string scene) => SceneHelper.LoadScene(scene);

}
