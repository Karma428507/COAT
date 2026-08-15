namespace COAT.Chat;

using COAT.Assets;
using COAT.Content;
using COAT.Entities;
using COAT.IO;
using COAT.Net;
using COAT.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary> The normal commands. </summary>
public class CommandExperimental : CommandHandler
{
    public override void Load()
    {
        Register("getname", "Gets username", args =>
        {
            if (LobbyController.Self == null)
                return;

            string user = LobbyController.Lobby?.GetMemberData((Friend)LobbyController.Self, "username");
            Chat.Receive($"Username: {user}");
        });

        Register("setname", "Sets username", args =>
        {
            string name = "";
            
            if (args.Length == 0)
            {
                SaveManager.SetPlayerData("username", "");
                return;
            }

            for (int i = 0; i < args.Length; i++)
                name += i == 0 ? args[i] : " " + args[i];

            SaveManager.SetPlayerData("username", name);
        });

        Register("cohost", "<player>", "Gives another player host abilities", args =>
        {
            Log.Debug("COHOST command found");
        });
    }
}
