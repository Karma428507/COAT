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
    }

    public override void LoadProperties()
    {
        AddProperty("debug", Mapping.Scene);
        AddProperty("more than nothing", (ushort)10);
        AddProperty("3", 3);
        AddProperty("deadbeef", 0xDEADBEEF);
        AddProperty("LONGGGG", (long)1);
    }

    public override void Reload()
    {
        SetProperty("debug", Mapping.Scene);
        SetProperty("more than nothing", (ushort)10);
        SetProperty("3", 3);
        SetProperty("deadbeef", 0xDEADBEEF);
        SetProperty("LONGGGG", (long)1);
    }
}
