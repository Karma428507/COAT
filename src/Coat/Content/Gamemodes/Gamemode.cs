namespace Coat.Content.Gamemodes;

using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using Jaket.Assets;
using Jaket.IO;
using Jaket.Net;
using Jaket.Net.Types;

/// <summary> Set of different tools for simplifying life and systematization of code. </summary>
public abstract class Gamemode
{
    public static List<Gamemode> Modes = new()
    {
        new Regular(),
        new AdvPvP(),
        new TF2(),
        new WickedSeek(),
        new SkullCapture(),
        new Lethal()
    };

    public abstract string Name { get; }

    public abstract Gamemode_Type Type { get; }
}

public enum Gamemode_Type
{
    REGULAR,
    ADV_PVP,
    TF2,
    WICKED_SEEK,
    SKULL_CAPTURE,
    LETHAL
}