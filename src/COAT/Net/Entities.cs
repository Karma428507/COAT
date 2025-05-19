namespace COAT.Net;

using System;
using System.Collections.Generic;
using System.Text;

using COAT.Content;
using COAT.Net.Types;

/// <summary>
/// A class that provides a list on entities
/// </summary>
public class Entities
{
    public static Dictionary<EntityType, Prov> Providers = new();

    public static void Load()
    {
        // Add the thing for players ONCE DollAssets.cs is fixed
    }

    // Add functions to 'mark' and NPC, get type and the next ID

    /// <summary> Entity provider. </summary>
    public delegate Entity Prov();
}
