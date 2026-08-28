namespace COAT.Net.Files;

using COAT.Content;
using COAT.Net.Pages;
using COAT.Net.Sprays;
using COAT.UI.Physical;
using COAT.Utils;

using Steamworks.Data;
using System.Collections.Generic;

/// <summary> Manages download requests. </summary>
public class NetRequester
{
    /// <summary> List of requests for spray by id. </summary>
    public static Dictionary<NetQueue, List<Connection>> Requests = new();

    public static void Load()
    {
        Events.EverySecond += ProcessRequests;
    }

    /// <summary> Processes all requests. </summary>
    public static void ProcessRequests()
    {
        foreach (var owner in Requests.Keys)
        {
            void pageUpload(Page page)
            {
                if (page == null)
                {
                    Log.Error("Error, unknown page being uploaded.");
                    return;
                }

                NetLoader.Upload(owner.ID, owner.Type, page.GetFile(), (data, size) => Requests[owner].ForEach(con => Tools.Send(con, data, size)));
            }

            // NetFile types
            switch (owner.Type)
            {
                // Upload spray files
                case NetFile.NET_FILE_TYPE_SPRAY:
                    if (SprayManager.Cache.TryGetValue(owner.ID, out var spray))
                        NetLoader.Upload(owner.ID, owner.Type, spray.Data, (data, size) => Requests[owner].ForEach(con => Tools.Send(con, data, size)));
                    
                    break;

                // Pages
                case NetFile.NET_FILE_TYPE_PAGE_WORLD:
                    pageUpload(PageManager.World);
                    break;
                case NetFile.NET_FILE_TYPE_PAGE_SPECIAL:
                    pageUpload(PageManager.Special);
                    break;

                default:
                    Log.Error($"Unable to upload file with the type of {owner.Type} to {owner.ID}");
                    break;
            }
        }

        Requests.Clear(); // clear all requests, because they are processed
    }

    /// <summary> Requests a file from the host directly. </summary>
    public static void Request(byte type) => Request(type, LobbyController.LastOwner.AccountId);

    /// <summary> Requests a file from the any player by asking the host. </summary>
    public static void Request(byte type, uint owner)
    {
        Networking.Send(PacketType.NetFileRequest, w => { 
            w.Id(owner);
            w.Byte(type);
        }, size: 5);
    }
}
