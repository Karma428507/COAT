using Jaket.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace COAT
{
    public class COAT
    {
        #region EXTERNAL APPS
        public static string DiscordDisplay()
        {
            if (LobbyController.IsCOATLobby)
                return "Playing multiplayer via COAT :3";

            return "Playing multiplayer via Jaket on COAT";
        }

        public static string SteamDisplay()
        {
            if (LobbyController.IsCOATLobby)
                return " | Multiplayer via COAT :3";

            return " | Multiplayer via Jaket on COAT";
        }

        #endregion
    }
}
