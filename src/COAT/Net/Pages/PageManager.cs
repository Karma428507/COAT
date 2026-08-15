namespace COAT.Net.Pages;

using COAT.IO;

using Steamworks;
using System.Collections.Generic;
using System.Linq;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class PageManager
{
    /// <summary> Null index. </summary>
    public const int PAGE_INDEX_NULL = 0x00;
    /// <summary> Index for the world page. </summary>
    public const int PAGE_INDEX_WORLD = 0x01;
    /// <summary> Index for the special page. </summary>
    public const int PAGE_INDEX_SPECIAL = 0x01;
    /// <summary> Index for the enemies page. </summary>
    public const int PAGE_INDEX_ENEMIES = 0x01;
    /// <summary> Index for the sandbox enemies page. </summary>
    public const int PAGE_INDEX_SANDBOX_ENEMIES = 0x01;
    /// <summary> Index for the sandbox page. </summary>
    public const int PAGE_INDEX_SANDBOX = 0x01;

    /// <summary> Page for the main world settings (doors, deactive arenas). </summary>
    public static WorldPage World;
    /// <summary> Page for information in specific levels. </summary>
    public static SpecialPage? Special;

    // Pages to work on later
    /// <summary> Page for the enemies. </summary>
    public static Page Enemies;
    /// <summary> Page for sandbox enemies specifically. </summary>
    public static Page SandboxEnemies;
    /// <summary> Page for sandbox creations and settings. </summary>
    public static Page Sandbox;

    public static void Load()
    {
    }
}
