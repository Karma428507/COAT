namespace COAT.Net.Pages;

using COAT.Utils;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class WorldPage : Page
{
    public List<KeyValuePair<Vector3, byte>> DoorList;

    public WorldPage() : base(PageManager.PAGE_INDEX_WORLD)
    {
        // Sets the level name for debugging
        AddProperty("debug", Mapping.Scene);
    }

    public override void Reload()
    {
        SetProperty("debug", Mapping.Scene);
        SetProperty("more than nothing", (ushort)10);
        SetProperty("3", 3);
        SetProperty("LONGGGG", (long)1);

        // Important debugging text
        Log.Debug("AAAAAAAAAAA");
    }
}
