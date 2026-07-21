namespace COAT.Pages;

using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class WorldPage : Page
{
    public List<KeyValuePair<Vector3, byte>> DoorList;

    public WorldPage()
    {
        Initialize();
    }
}
