namespace COAT.World;

using COAT.Assets;
using COAT.Content;
using COAT.Net;
using COAT.Utils;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Adds a green sandbox like terminal. </summary>
public class Terminal
{
    public static void Load()
    {
        Events.OnLoaded += () => Events.Post(LevelHander);
    }

    private static void LevelHander()
    {
        if (LobbyController.Offline) return;

        if (Mapping.Scene == "uk_construct")
        {
            GameObject oldShop = Tools.ObjFindMainScene("Sandbox Shop");
            Vector3 pos = oldShop.transform.position;
            Tools.Destroy(oldShop);

            GameObject newShop = Tools.Instantiate(ModAssets.MultiplayerTerminal, pos);
            newShop.transform.rotation *= Quaternion.Euler(0, 0, 180);
        }
    }
}