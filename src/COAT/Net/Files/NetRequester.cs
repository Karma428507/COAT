namespace COAT.Net.Files;

using COAT.Content;
using COAT.Net.Sprays;

using Steamworks.Data;
using System.Collections.Generic;

/// <summary> Manages download requests. </summary>
public class NetRequester
{
    /// <summary> List of requests for spray by id. </summary>
    public static Dictionary<uint, List<Connection>> Requests = new();

    /// <summary> Processes all spray requests. </summary>
    public static void ProcessRequests()
    {
        foreach (var owner in Requests.Keys)
        {
            if (SprayManager.Cache.TryGetValue(owner, out var spray))
                Upload(owner, spray.Data, (data, size) => Requests[owner].ForEach(con => Tools.Send(con, data, size)));
            else
                Log.Error($"Couldn't find the requested spray. Spray id is {owner}");
        }

        Requests.Clear(); // clear all requests, because they are processed
    }

    /// <summary> Handles the downloaded spray and decides where to send it next. </summary>
    public static void HandleSpray(uint owner, byte[] data)
    {
        SprayManager.Cache.Remove(owner);
        SprayManager.Cache.Add(owner, new(data));

        // update the existing spray if there is one
        if (SprayManager.Sprays.TryGetValue(owner, out var spray)) spray.UpdateSprite();
    }

    /// <summary> Requests someone's spray from the host. </summary>
    public static void Request(uint owner) => Networking.Send(PacketType.NetFileRequest, w => w.Id(owner), size: 4);
}
