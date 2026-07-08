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
using UnityEngine.UIElements;

/// <summary> Class that manages how doors behave and sync in the levels. </summary>
public class DoorManager
{
    /// <summary> Status constant for unlocked doors. </summary>
    public const byte DOOR_STATUS_UNLOCKED = 0x00;
    /// <summary> Status constant for locked doors. </summary>
    public const byte DOOR_STATUS_LOCKED = 0x01;

    /// <summary> This is a dictionary to keep track of the door's position and there net status. </summary>
    public static Dictionary<Vector3, byte> DoorDictionary = new Dictionary<Vector3, byte>();

    public static void Load()
    {
        // Idk what to put here rn
    }

    public static void GetDoors()
    {
        // The main scene
        Scene activeScene = SceneManager.GetActiveScene();
        // A list of all door objects
        List<GameObject> results = new List<GameObject>();

        // Recursive function to find the doors in a gameobject's parent
        void RecursiveDoorFind(GameObject parent)
        {
            if (parent.gameObject.GetComponent<Door>() != null)
                results.Add(parent);

            for (int i = 0; i < parent.transform.childCount; i++)
                RecursiveDoorFind(parent.transform.GetChild(i).gameObject);
        }

        // Clears so it wouldn't get crowded
        DoorDictionary.Clear();

        // Find all doors and put it in the results array
        foreach (GameObject root in activeScene.GetRootGameObjects())
            RecursiveDoorFind(root);

        Log.Debug($"Found {results.Count} doors.");
        foreach (GameObject obj in results)
        {
            Door door = obj.GetComponent<Door>();
            Vector3 position = obj.transform.position;
            bool locked = door.locked;

            DoorDictionary.Add(position, locked ? DOOR_STATUS_LOCKED : DOOR_STATUS_UNLOCKED);
            Log.Debug($"\t{obj.name} - {door.doorType.ToString()}");
        }
    }

    public static void SendNetStatus(Door __instance, byte status)
    {
        if (!LobbyController.Online)
            return;
        
        Vector3 position = __instance.gameObject.transform.position;

        if (!DoorDictionary.ContainsKey(position))
        {
            Log.Error("The door is not registered in the level's door manager");
            return;
        }

        DoorDictionary[position] = status;

        World.SyncAction(SyncType.DoorHandler, __instance, status);
    }

    public static void ReceiveNetStatus(Vector3 position, byte status)
    {
        Door door = null;
        
        if(!DoorDictionary.ContainsKey(position))
        {
            Log.Error("The door is not registered in the level's door manager");
            return;
        }

        DoorDictionary[position] = status;

        Tools.ResFind<Door>(d => d.transform.position == position, d => door = d);

        if (door == null)
            return;

        switch (status)
        {
            case DOOR_STATUS_UNLOCKED:
                door.Unlock();
                break;
            case DOOR_STATUS_LOCKED:
                door.Lock();
                break;
        }
    }
}