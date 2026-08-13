namespace COAT.Net.Files;

using COAT.Content;
using COAT.IO;
using COAT.Net;
using COAT.Net.Sprays;
using COAT.UI.Menus.Sub;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary> Manages file downloads and uploads. </summary>
public class NetLoader
{
    /// <summary> Size of the packet that contains an net file chunk. </summary>
    public const int CHUNK_SIZE = 512;
    /// <summary> List of all streams for net file loading. </summary>
    public static Dictionary<NetQueue, Writer> Streams = new();
    /// <summary> A list of actions for differnt net file types. </summary>
    public static Dictionary<byte, Action<uint, byte[]>> DownloadEvents = new();

    /// <summary> Mainly defines the download events. </summary>
    public static void Load()
    {
        DownloadEvents[NetFile.NET_FILE_TYPE_NULL] = HandleNull;
        DownloadEvents[NetFile.NET_FILE_TYPE_SPRAY] = SprayManager.HandleSpray;
    }

    /// <summary> Uploads a net file to the clients or server. </summary>
    public static void Upload(uint owner, byte type, byte[] data, Action<IntPtr, int> result = null)
    {
        // initialize a new stream
        Networking.Send(PacketType.NetFileChunk, w =>
        {
            w.Id(owner);
            w.Byte(0);
            w.Byte(type);
            w.Int(data.Length);
        }, result, 10);

        // send data over the stream
        for (int i = 0; i < data.Length; i += CHUNK_SIZE) Networking.Send(PacketType.NetFileChunk, w =>
        {
            w.Id(owner);
            w.Byte((byte)(i + 1)); // 1 - 255
            w.Byte(type);
            w.Bytes(data, i, Mathf.Min(CHUNK_SIZE, data.Length - i));
        }, result, CHUNK_SIZE + 6);
    }

    /// <summary> Loads a spray from the client or server. </summary>
    public static void Download(Reader r)
    {
        var id = r.Id();
        byte index = r.Byte();
        byte type = r.Byte();
        NetQueue queue = new NetQueue(id, type);

        if (type == NetFile.NET_FILE_TYPE_SPRAY && !SpraySettings.Enabled) return;

        if (index == 0) // Initial packet
        {
            if (Streams.TryGetValue(queue, out var stream))
            {
                Log.Warning("Overriding the old stream");
                Marshal.FreeHGlobal(stream.memory);
            }
            Log.Info("Downloading net file#" + id);

            int length = r.Int();
            Streams[queue] = new(Marshal.AllocHGlobal(length), length);
            return;
        }
        else // Data packet
        {
            if (!Streams.TryGetValue(queue, out var stream))
            {
                Log.Error($"Stream's initial packet was lost! i={index}");
                return;
            }

            stream.Bytes(r.Bytes(r.length - 6));
            if (stream.Position >= stream.length)
            {
                // handle the downloaded spray
                Reader.Read(stream.memory, stream.length, r => DownloadEvents[type](id, r.Bytes(r.length)));

                Marshal.FreeHGlobal(stream.memory);
                Streams.Remove(queue);
            }

#if DEBUG
            Log.Debug($"Downloaded {100f * stream.Position / stream.length:0.00}%");
#endif
        }

    }

    /// <summary> Only prints debug information of a complete net file after </summary>
    private static void HandleNull(uint owner, byte[] data)
    {
        Log.Debug("Null net file download completed.");
        Log.Debug($"\t- Owner: {owner}, Data length: {(data != null ? data.Length : "N/A")}");
    }
}
