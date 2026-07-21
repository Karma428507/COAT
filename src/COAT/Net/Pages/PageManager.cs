using System.Collections.Generic;

namespace COAT.Pages;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class Page
{
    /// <summary> Page for every players excluding the client. </summary>
    public static List<PlayerPage> Players;
    /// <summary> The client's player page. </summary>
    public static PlayerPage Player;
    /// <summary> Page for the main world settings (doors, deactive arenas). </summary>
    public static WorldPage World;
    /// <summary> Page for information in specific levels. </summary>
    public static SpecialPage? Special;

    // Pages to work on later
    /// <summary> Page for the enemies. </summary>
    public static object Enemies;
    /// <summary> Page for sandbox enemies specifically. </summary>
    public static object SandboxEnemies;
    /// <summary> Page for sandbox creations and settings. </summary>
    public static object Sandbox;

    public static void Load()
    {
        
    }

    public void Initialize()
    {

    }

    /// <summary> Converts the name used for organizing data to a 16 bit number. </summary>
    public short GetDataID()
    {
        return 0x0000;
    }
}
