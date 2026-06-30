namespace COAT.World;

using COAT.Content;
using COAT.IO;
using COAT.Net;
using COAT.Net.Types;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Class that manages how doors behave and sync in the levels. </summary>
public class DoorManager
{
    public static void Load()
    {
        Events.OnLoaded += () => {
            if (LobbyController.Online && LobbyController.IsOwner && Tools.Scene != "Main Menu")
                GetDoors();
        };
    }

    private static void GetDoors()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        // Find all objects in the scene (including children)
        List<GameObject> results = new List<GameObject>();

        foreach (GameObject root in activeScene.GetRootGameObjects())
            results.AddRange(RecursiveDoorFind(root));

        Log.Debug($"Found {results.Count} objects containing '{"door"}'.");
        foreach (GameObject obj in results)
        {
            Door hasDoor = obj.GetComponent<Door>();
            

            if (hasDoor == null)
                continue;

            hasDoor.Unlock();
            Log.Debug(obj.name);
        }
    }

    private static List<GameObject> RecursiveDoorFind(GameObject parent)
    {
        List<GameObject> results = new List<GameObject>();

        // Get all descendants
        string name = parent.gameObject.name.ToLower();

        // && !(name.Contains("indoor") || name.Contains("outdoor"))
        if (name.Contains("door"))
            results.Add(parent);

        for (int i = 0; i < parent.transform.childCount; i++)
            if (!parent.transform.GetChild(i).gameObject.name.Contains("Enemies"))
                results.AddRange(RecursiveDoorFind(parent.transform.GetChild(i).gameObject));

        return results;
    }
}