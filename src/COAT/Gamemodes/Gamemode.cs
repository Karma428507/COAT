using System;
using System.Collections.Generic;
using System.Text;

using COAT;
using UnityEngine;

namespace COAT.Gamemodes
{
    public enum GamemodeTypes
    {
        PAiN,
        NormalCampain,
        UltraCompany,
        TeamKill2,
        HideAndSeek,
        TheArena
    }

    public class GamemodeManager
    {
        /// <summary> Gets the Class of a list off of the index of it. </summary>
        public static void GetList(int gamemode, Action<Gamemode> Class)
        {
            foreach (GamemodeTypes type in Enum.GetValues(typeof(GamemodeTypes)))
            {
                if (Convert.ToInt16(type) == gamemode)
                {
                    GamemodeManager.GamemodeList.TryGetValue(type, out Gamemode List);
                    Class(List);
                }
            }
        }

        public static readonly Dictionary<GamemodeTypes, Gamemode> GamemodeList = new Dictionary<GamemodeTypes, Gamemode>()
        {
            {GamemodeTypes.PAiN, new PAiN()},
            {GamemodeTypes.NormalCampain, new NormalCampain()},
            {GamemodeTypes.UltraCompany, new UltraCompany()},
            {GamemodeTypes.TeamKill2, new TeamKill2()},
            {GamemodeTypes.HideAndSeek, new HideAndSeek()},
            {GamemodeTypes.TheArena, new TheArena()},
        };
    }
    public abstract class Gamemode
    {
        /// <summary> Name of the gamemode, used in UI and debugging. </summary>
        public virtual string Name { get; set; }

        /// <summary> What gamemode it is. </summary>
        public virtual GamemodeTypes Type { get; set; }

        /// <summary> Code ran when host client presses "PLAY"
        public abstract void Start();

        /// <summary> Code ran when host selects it in the server creation screen, or the server settings. </summary>
        public abstract void LoadSettings(Transform parent);

        /// <summary> Code ran when a client joins a lobby with that gamemode. </summary>
        public abstract void JoinStart(); 
    }

    // Put this into seperate files later

    // P A (i) N is already in its own file

    public class NormalCampain : Gamemode
    {
        public override string Name => "Normal Campain";
        public override GamemodeTypes Type => GamemodeTypes.NormalCampain;

        public override void Start() => Log.Info($"Starting {Name}");

        public override void LoadSettings(Transform parent) => Log.Info($"Loading gamemode settings for {Name}");

        public override void JoinStart() => Log.Info($"Joining lobby with gamemode {Name}");
    }

    public class UltraCompany : Gamemode
    {
        public override string Name => "Ultra Company";
        public override GamemodeTypes Type => GamemodeTypes.UltraCompany;

        public override void Start() => Log.Info($"Starting {Name}");

        public override void LoadSettings(Transform parent) => Log.Info($"Loading gamemode settings for {Name}");

        public override void JoinStart() => Log.Info($"Joining lobby with gamemode {Name}");
    }

    public class TeamKill2 : Gamemode
    {
        public override string Name => "Team Kill 2";
        public override GamemodeTypes Type => GamemodeTypes.TeamKill2;

        public override void Start() => Log.Info($"Starting {Name}");

        public override void LoadSettings(Transform parent) => Log.Info($"Loading gamemode settings for {Name}");

        public override void JoinStart() => Log.Info($"Joining lobby with gamemode {Name}");
    }

    public class HideAndSeek : Gamemode
    {
        public override string Name => "Hide and Seek";
        public override GamemodeTypes Type => GamemodeTypes.HideAndSeek;

        public override void Start() => Log.Info($"Starting {Name}");

        public override void LoadSettings(Transform parent) => Log.Info($"Loading gamemode settings for {Name}");

        public override void JoinStart() => Log.Info($"Joining lobby with gamemode {Name}");
    }

    public class TheArena : Gamemode
    {
        public override string Name => "The Arena";
        public override GamemodeTypes Type => GamemodeTypes.TheArena;

        public override void Start() => Log.Info($"Starting {Name}");

        public override void LoadSettings(Transform parent) => Log.Info($"Loading gamemode settings for {Name}");

        public override void JoinStart() => Log.Info($"Joining lobby with gamemode {Name}");
    }
}
