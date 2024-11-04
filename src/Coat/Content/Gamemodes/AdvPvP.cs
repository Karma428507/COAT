namespace Coat.Content.Gamemodes;

using System;

/// <summary>
/// Cyber grind pvp mode, like the final boss in DMC5
/// </summary>
public class AdvPvP : Gamemode
{
    public override string Name => "Cyber PvP";

    public override Gamemode_Type Type => Gamemode_Type.ADV_PVP;
}
