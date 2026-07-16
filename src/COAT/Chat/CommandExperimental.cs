namespace COAT.Chat;

using COAT.Assets;
using COAT.Content;
using COAT.Entities;
using COAT.Net;
using COAT.Utils;

using System;
using System.Collections.Generic;

/// <summary> The normal commands. </summary>
public class CommandExperimental : CommandHandler
{
    public override void Load()
    {
        Register("getname", "Gets username", args =>
        {
            string us = PrefsManager.Instance.GetString("COAT.username");

            Chat.Receive($"Username: {us}");
            Chat.Receive($"IS NULL? {us == null}");
        });

        Register("setname", "Sets username", args =>
        {
            if (args.Length == 0)
            {
                PrefsManager.Instance.SetString("COAT.username", null);
            }
            else
            {
                string name = "";

                for (int i = 0; i < args.Length; i++)
                    name += i == 0 ? args[i] : " " + args[i];

                PrefsManager.Instance.SetString("COAT.username", name);
            }

            string us = PrefsManager.Instance.GetString("COAT.username");

            Chat.Receive($"Set the username to: {us}");
        });

        Register("cohost", "<player>", "Gives another player host abilities", args =>
        {
            Log.Debug("COHOST command found");
        });
    }
}
