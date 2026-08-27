#nullable enable

namespace COAT.Net.Pages;

using COAT.IO;
using COAT.Net.Files;
using COAT.UI;
using COAT.Utils;
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
    public static WorldPage? World;
    /// <summary> Page for information in specific levels. </summary>
    public static SpecialPage? Special;

    /* Pages to work on later
    /// <summary> Page for the enemies. </summary>
    public static Page Enemies;
    /// <summary> Page for sandbox enemies specifically. </summary>
    public static Page SandboxEnemies;
    /// <summary> Page for sandbox creations and settings. </summary>
    public static Page Sandbox;*/

    public static void Load()
    {
        Special = new SpecialPage();

        Events.OnLoaded += () =>
        {
            if (LobbyController.Offline)
                return;

            if (LobbyController.IsOwner)
                CreatePages();
            else
                ClientReset();
        };

        Events.OnPageDownload += (type, owner, data) =>
        {
            Log.Debug($"Page type: {type}");

            switch (type)
            {
                case NetFile.NET_FILE_TYPE_PAGE_WORLD:
                    Log.Debug("Downloading world page file");
                    break;
                case NetFile.NET_FILE_TYPE_PAGE_SPECIAL:
                    Log.Debug("Downloading special page file");
                    break;
                default:
                    Log.Debug("Treating as null page.");
                    break;
            }
        };
    }

    public static object? GetData(byte pageIndex, string key)
    {
        Page waitForRequest()
        {
            NetRequester.Request(pageIndex);

            while (true)
            {
                switch (pageIndex)
                {
                    case NetFile.NET_FILE_TYPE_PAGE_WORLD:
                        if (World != null)
                            return World;

                        break;
                    case NetFile.NET_FILE_TYPE_PAGE_SPECIAL:
                        if (Special != null)
                            return Special;

                        break;
                }

                // maybe add a counter for this to not be an infinite loop?
            }
        }

        Page? page;

        // Request the page
        switch (pageIndex)
        {
            case NetFile.NET_FILE_TYPE_PAGE_WORLD:
                page = World;
                break;
            case NetFile.NET_FILE_TYPE_PAGE_SPECIAL:
                page = Special;
                break;
            default:
                return null;
        }

        // Wait for it if the player does not already have it
        if (page == null)
            page = waitForRequest();

        return page.GetProperty(key);
    }

    public static void SetData(Page page, string key, object value)
    {

    }

    private static void CreatePages()
    {
        if (World == null)
            World = new WorldPage();
        
        // For when I make special world syncs
        switch (Mapping.Scene)
        {
            default:
                Special = new SpecialPage();
                break;
        }

        // Reload all of the pages
        World.Reload();
        Special.Reload();

        // Fill the request table with the newly updated pages
        RefreshPageRequests();
    }

    private static void ClientReset()
    {
        World = null;
        Special = null;

        NetRequester.Request(NetFile.NET_FILE_TYPE_PAGE_WORLD);
        NetRequester.Request(NetFile.NET_FILE_TYPE_PAGE_SPECIAL);
    }

    private static void RefreshPageRequests()
    {
        Networking.EachConnection(cons =>
        {
            // switch to null when a debug null page is created
            NetQueue queue;

            // World page
            queue = new NetQueue(cons.Id, NetFile.NET_FILE_TYPE_PAGE_WORLD);
            NetRequester.Requests.Add(queue, cons);

            // Special page
            queue = new NetQueue(cons.Id, NetFile.NET_FILE_TYPE_PAGE_SPECIAL);
            NetRequester.Requests.Add(queue, cons);

        });
    }
}
