namespace COAT.Net.Files;

using COAT.Content;
using COAT.IO;
using COAT.Net;
using COAT.UI.Menus.Sub;
using COAT.Utils;

using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


/// <summary> Manages file downloads and uploads. </summary>
public class NetLoader
{
    /// <summary> Size of the packet that contains an net file chunk. </summary>
    public const int CHUNK_SIZE = 512;
    /// <summary> List of all streams for spray loading. </summary>
    public static Dictionary<uint, Writer> Streams = new();

    /// <summary> Uploads the given spray to the clients or server. </summary>
    public void Upload(uint owner, byte[] data, Action<IntPtr, int> result = null)
    {
        // initialize a new stream
        Networking.Send(PacketType.NetFileChunk, w =>
        {
            w.Id(owner);
            w.Bool(true);
            w.Int(data.Length);
        }, result, 9);

        // send data over the stream
        for (int i = 0; i < data.Length; i += CHUNK_SIZE) Networking.Send(PacketType.NetFileChunk, w =>
        {
            w.Id(owner);
            w.Bool(false);
            w.Bytes(data, i, Mathf.Min(CHUNK_SIZE, data.Length - i));
        }, result, CHUNK_SIZE + 5);
    }

    /// <summary> Loads a spray from the client or server. </summary>
    public void Download(Reader r)
    {
        if (!SpraySettings.Enabled) return;

        var id = r.Id(); // id of the spray owner
        if (r.Bool()) // initial packet
        {
            if (Streams.TryGetValue(id, out var stream))
            {
                Log.Warning("Overriding the old stream");
                Marshal.FreeHGlobal(stream.memory);
            }
            Log.Info("Downloading spray#" + id);

            int length = r.Int();
            Streams[id] = new(Marshal.AllocHGlobal(length), length);
        }
        else // data packet
        {
            if (!Streams.TryGetValue(id, out var stream))
            {
                Log.Error("Stream's initial packet was lost!");
                return;
            }

            stream.Bytes(r.Bytes(r.length - 6));
            if (stream.Position >= stream.length)
            {
                // handle the downloaded spray
                Reader.Read(stream.memory, stream.length, r => HandleSpray(id, r.Bytes(r.length)));

                Marshal.FreeHGlobal(stream.memory);
                Streams.Remove(id);
            }

#if DEBUG
            Log.Debug($"Downloaded {100f * stream.Position / stream.length:0.00}%");
#endif
        }
    }
}
