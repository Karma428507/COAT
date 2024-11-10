namespace Coat.Content.Gamemodes;

using System;

/// <summary>
/// Regular Campaign (Normal ultrakill starting from the tutorial)
/// </summary>
public class Regular : Gamemode
{
    public override string Name => "Normal Campaign";

    public override Gamemode_Type Type => Gamemode_Type.REGULAR;
}
