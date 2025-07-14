namespace COAT.Net.Types.Players;

using COAT.IO;
using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

// maybe move somewhere else later?
public static class PlayerData
{
    public static Dictionary<uint, CoatPlayerData> CoatData = new Dictionary<uint, CoatPlayerData>();
    public static CoatPlayerData LocalData;

    /// <summary> Loads the saved data to the local data </summary>
    public static void Load()
    {
        LocalData = new CoatPlayerData();
        LocalData.Name = PrefsManager.Instance.GetString("COAT.username");
    }

    /// <summary> Removes a player ID when they leave </summary>

    public static void OnEnter()
    {

    }

    /// <summary> Removes a player ID when they leave </summary>
    public static void OnLeave()
    {

    }

    public static void Write(Writer w)
    {
        w.Byte(0);
        w.Byte(0);
        w.Byte(0);
        w.Int(0);
    }

    public static void Read(uint player, Reader r)
    {
        CoatPlayerData data = new CoatPlayerData();
        r.Byte();   // R, G
        r.Byte();   // B, ?
        r.Byte();   // ?
        // add tts info
        r.Int();    // ?
        //Encoding.Unicode.GetBytes(r.String))
    }
}

public struct CoatPlayerData
{
    /* Packet structure
     *
     *  1 B     | Packet type
     *  4 Bi    | Color R
     *  4 Bi    | Color G
     *  4 Bi    | Color B
     *  4 Bi    | Reserved
     *  5 B     | Reserved
     *  16 B    | Username (zero or EOL termination)
     *
     */

    public Color Color;

    public string Name;

    // Admin stuff
    public bool SentRequest;
}