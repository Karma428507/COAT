namespace Coat.Content.Gamemodes;

using System;

/// <summary>
/// Capture the flags but with skulls.
/// he-who said to use levels 1-3, 4-2 and 5-3
/// </summary>
public class SkullCapture : Gamemode
{
    public override string Name => "Capture The Skull";

    public override Gamemode_Type Type => Gamemode_Type.SKULL_CAPTURE;
}
