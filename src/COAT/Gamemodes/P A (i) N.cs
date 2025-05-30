using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using COAT;

namespace COAT.Gamemodes
{
    public class PAiN : Gamemode
    {
        public override string Name => "P A (i) N";
        public override GamemodeTypes Type => GamemodeTypes.PAiN;

        public override void Start()
        {
            Log.Info($"Starting {Name}");


        }

        public override void LoadSettings(Transform parent)
        {
            Log.Info($"Loading gamemode settings for {Name}");


        }

        public override void JoinStart()
        {
            Log.Info($"Joining lobby with gamemode {Name}");


        }
    }
}