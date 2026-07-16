namespace COAT.Chat;

using COAT.Assets;
using COAT.Content;
using COAT.Entities;
using COAT.Net;
using COAT.Utils;

using System;
using System.Collections.Generic;

/// <summary> The normal commands. </summary>
[Obsolete("Fix the credits")]
public class CommandNormal : CommandHandler
{
    public override void Load()
    {
        Register("help", "Display the list of all commands", args =>
        {
            ChatManager.NormalCommands.GetCommands().ForEach(command =>
            {
                Chat.Receive($"[14]* /{command.Name}{(command.Args == null ? "" : $" [#BBBBBB]{command.Args}[]")} - {command.Desc}[]");
            });
        });

        Register("hello", "Resend the tips for new players", args => ChatManager.Hello(true));

        Register("plushies", "Display the list of all dev plushies", args =>
        {
            string[] plushies = (string[])GameAssets.PlushiesButReadable.Clone();
            Array.Sort(plushies); // sort alphabetically for a more presentable look

            Chat.Receive(string.Join(", ", plushies));
        });

        Register("plushy", "<name>", "Spawn a plushy by name", args =>
        {
            string name = args.Length == 0 ? null : args[0].ToLower();
            int index = Array.FindIndex(GameAssets.PlushiesButReadable, plushy => plushy.ToLower() == name);

            if (index == -1)
                Chat.Receive($"[#FF341C]Plushy named {name} not found.");
            else
                Tools.Instantiate(Items.Prefabs[EntityType.PlushyOffset + index - EntityType.ItemOffset].gameObject, NewMovement.Instance.transform.position);
        });

        Register("level", "<layer> <level> / sandbox / cyber grind / credits museum", "Load the given level", args =>
        {
            if (args.Length == 1 && args[0].Contains("-")) args = args[0].Split('-');

            if (!LobbyController.IsOwner)
                Chat.Receive($"[#FF341C]Only the lobby owner can load levels.");

            else if (args.Length >= 1 && (args[0].ToLower() == "sandbox" || args[0].ToLower() == "sand"))
            {
                Mapping.Load("uk_construct");
                Chat.Receive("[#32CD32]Sandbox is loading.");
            }
            else if (args.Length >= 1 && (args[0].ToLower().Contains("cyber") || args[0].ToLower().Contains("grind") || args[0].ToLower() == "cg"))
            {
                Mapping.Load("Endless");
                Chat.Receive("[#32CD32]The Cyber Grind is loading.");
            }
            else if (args.Length >= 1 && (args[0].ToLower().Contains("credits") || args[0].ToLower() == "museum"))
            {
                Mapping.Load("CreditsMuseum2");
                Chat.Receive("[#32CD32]The Credits Museum is loading.");
            }
            else if (args.Length < 2)
                Chat.Receive($"[#FF341C]Insufficient number of arguments.");
            else if
            (
                int.TryParse(args[0], out int layer) && layer >= 0 && layer <= 7 &&
                int.TryParse(args[1], out int level) && level >= 1 && level <= 5 &&
                (level == 5 ? layer == 0 : true) && (layer == 3 || layer == 6 ? level <= 2 : true)
            )
            {
                Mapping.Load($"Level {layer}-{level}");
                Chat.Receive($"[#32CD32]Level {layer}-{level} is loading.");
            }
            else if (args[1].ToUpper() == "S" && int.TryParse(args[0], out level) && level >= 0 && level <= 7 && level != 3 && level != 6)
            {
                Mapping.Load($"Level {level}-S");
                Chat.Receive($"[#32CD32]Secret level {level}-S is loading.");
            }
            else if (args[1].ToUpper() == "E" && int.TryParse(args[0], out level) && level >= 0 && level <= 1)
            {
                Mapping.Load($"Level {level}-E");
                Chat.Receive($"[#32CD32]Encore level {level}-E is loading.");
            }
            else if (args[0].ToUpper() == "P" && int.TryParse(args[1], out level) && level >= 1 && level <= 2)
            {
                Mapping.Load($"Level P-{level}");
                Chat.Receive($"[#32CD32]Prime level P-{level} is loading.");
            }
            else
                Chat.Receive("[#FF341C]Layer must be an integer from 0 to 7. Level must be an integer from 1 to 5.");
        });

        Register("authors", "Display the list of the mod developers", args =>
        {
            void Msg(string msg) => Chat.Receive($"[14]{msg}[]");

            Msg("JAKET Leading developers:");
            Msg("* [#0096FF]xzxADIxzx[] - the main developer of JAKET");
            Msg("* [#8A2BE2]Sowler[] - owner of the JAKET Discord server");
            Msg("* [#FFA000]Fumboy[] - textures and a part of animations");

            Msg("JAKET Contributors:");
            Msg("* [#00E666]Rey Hunter[] - really cool icons for emotes");
            Msg("* [#00E666]Ardub[] - invaluable help with The Cyber Grind [12][#cccccc](he did 90% of the work)");
            Msg("* [#00E666]Kekson1a[] - Steam Rich Presence support");

            Msg("JAKET Translators:");
            Msg("[#cccccc]NotPhobos - Spanish, sSAR - Italian, Theoyeah - French, Sowler - Polish,");
            Msg("[#cccccc]Ukrainian, Poyozit - Portuguese, Fraku - Filipino, Iyad - Arabic");

            Msg("COAT Leading developers:");
            Msg("Leading developers:");
            Msg("* [#0096FF]Karma[] - the main developer of this fork :3");
            Msg("* [#0096FF]whyis2+2[] - UI");
            Msg("* [#0096FF]bryan[] - UI, P A (I) N, emotes");
            Msg("* [#0096FF]archangel[] - UI");
            Msg("I'm going to add more things to this list later...");
        });
    }
}
