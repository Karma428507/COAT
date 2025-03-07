using System;
using System.Collections.Generic;
using System.Text;

namespace COAT.Gamemodes
{
    public enum GamemodeTypes
    {
        NormalCampain,
        UltraCompany,
        TeamKill2,
        HideAndSeek,
        TheArena
    }

    public class GamemodeManager
    {
        public static readonly Dictionary<GamemodeTypes, Gamemode> GamemodeList = new Dictionary<GamemodeTypes, Gamemode>()
        {
            {GamemodeTypes.NormalCampain, new NormalCampain()},
            {GamemodeTypes.UltraCompany, new UltraCompany()},
            {GamemodeTypes.TeamKill2, new TeamKill2()},
            {GamemodeTypes.HideAndSeek, new HideAndSeek()},
            {GamemodeTypes.TheArena, new TheArena()},
        };
    }
    public abstract class Gamemode
    {
        public virtual string Name { get; set; }

        public virtual GamemodeTypes Type { get; set; }
    }

    // Put this into seperate files later

    public class NormalCampain : Gamemode
    {
        public override string Name => "Normal Campain";
        public override GamemodeTypes Type => GamemodeTypes.NormalCampain;
    }

    public class UltraCompany : Gamemode
    {
        public override string Name => "Ultra Company";
        public override GamemodeTypes Type => GamemodeTypes.UltraCompany;
    }

    public class TeamKill2 : Gamemode
    {
        public override string Name => "Team Kill 2";
        public override GamemodeTypes Type => GamemodeTypes.TeamKill2;
    }

    public class HideAndSeek : Gamemode
    {
        public override string Name => "Hide and Seek";
        public override GamemodeTypes Type => GamemodeTypes.HideAndSeek;
    }

    public class TheArena : Gamemode
    {
        public override string Name => "The Arena";
        public override GamemodeTypes Type => GamemodeTypes.TheArena;
    }
}
