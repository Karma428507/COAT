namespace COAT.Content;

using UnityEngine;

using COAT.Net;

/// <summary> All teams. Teams needed for PvP mechanics. </summary>
public enum Team
{
    // NOTE: dont add more than 8 teams
    Yellow, 
    Red, 
    Green,
    Blue,
    Pink, 
    
    // COAT
    Purple,
}

/// <summary> Extension class that allows you to get team data. </summary>
public static class TeamExtensions
{
    /// <summary> Returns the team color, used only in the interface. </summary>
    public static Color Color(this Team team) => team switch
    {
        Team.Yellow => new(1f, .8f, .3f),
        Team.Red    => new(1f, .2f, .1f),
        Team.Green  => new(0f, .9f, .4f),
        Team.Blue   => new(0f, .5f,  1f),
        Team.Pink   => new(1f, .4f, .8f),
        Team.Purple => new(.7f, 0f, 1f),
        _ => new(1f, 1f, 1f)
    };

    /// <summary> Whether this team is allied with the player. </summary>
    public static bool Ally(this Team team) => team == Networking.LocalPlayer.Team || !LobbyController.PvPAllowed;
}