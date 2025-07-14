namespace COAT.Commands;

using COAT.Assets;
using COAT.Net;
using COAT.UI.Menus;
using COAT.UI.Overlays;
using System;
using UnityEngine;
using static InputActions;

/// <summary> List of chat commands used by the mod. </summary>
public class Commands
{
    static Chat chat => Chat.Instance;

    /// <summary> Chat command handler. </summary>
    public static CommandHandler Handler = new();

    /// <summary> Registers all default mod commands. </summary>
    public static void Load()
    {
        Handler.Register("help", "Display the list of all commands", args =>
        {
            Handler.Commands.ForEach(command =>
            {
                chat.Receive($"[14]* /{command.Name}{(command.Args == null ? "" : $" [#BBBBBB]{command.Args}[]")} - {command.Desc}[]");
            });
        });

        Handler.Register("list", "list", args =>
        {
            string text = "";
            foreach (uint id in Networking.COATPLAYERS)
            {
                text = text + $"{id}";
            }
            chat.Receive(text);
        });

        Handler.Register("hello", "Resend the tips for new players", args => chat.Hello(true));

        Handler.Register("tts-volume", "\\[0-100]", "Set Sam's volume to keep your ears comfortable", args =>
        {
            if (args.Length == 0)
                chat.Receive($"[#FFA500]TTS volume is {Settings.TTSVolume}.");
            else if (int.TryParse(args[0], out int value))
            {
                int clamped = Mathf.Clamp(value, 0, 100);
                Settings.TTSVolume = clamped;

                chat.Receive($"[#32CD32]TTS volume is set to {clamped}.");
            }
            else
                chat.Receive("[#FF341C]Failed to parse value. It must be an integer in the range from 0 to 100.");
        });
        Handler.Register("tts-auto", "\\[on/off]", "Turn auto reading of all messages", args =>
        {
            bool enable = args.Length == 0 ? !chat.AutoTTS : (args[0] == "on" || (args[0] == "off" ? false : !chat.AutoTTS));
            if (enable)
            {
                Settings.AutoTTS = chat.AutoTTS = true;
                chat.Receive("[#32CD32]Auto TTS enabled.");
            }
            else
            {
                Settings.AutoTTS = chat.AutoTTS = false;
                chat.Receive("[#FF341C]Auto TTS disabled.");
            }
        });

        Handler.Register("plushies", "Display the list of all dev plushies", args =>
        {
            string[] plushies = (string[])GameAssets.PlushiesButReadable.Clone();
            Array.Sort(plushies); // sort alphabetically for a more presentable look

            chat.Receive(string.Join(", ", plushies));
        });
        Handler.Register("plushy", "<name>", "Spawn a plushy by name", args =>
        {
            string name = args.Length == 0 ? null : args[0].ToLower();
            int index = Array.FindIndex(GameAssets.PlushiesButReadable, plushy => plushy.ToLower() == name);

            //if (index == -1)
            //    chat.Receive($"[#FF341C]Plushy named {name} not found.");
            //else
            //    Tools.Instantiate(Items.Prefabs[EntityType.PlushyOffset + index - EntityType.ItemOffset].gameObject, NewMovement.Instance.transform.position);
        });

        Handler.Register("level", "<layer> <level> / sandbox / cyber grind / credits museum", "Load the given level", args =>
        {
            if (args.Length == 1 && args[0].Contains("-")) args = args[0].Split('-');

            if (!LobbyController.IsOwner)
                chat.Receive($"[#FF341C]Only the lobby owner can load levels.");

            else if (args.Length >= 1 && (args[0].ToLower() == "sandbox" || args[0].ToLower() == "sand"))
            {
                Tools.Load("uk_construct");
                chat.Receive("[#32CD32]Sandbox is loading.");
            }
            else if (args.Length >= 1 && (args[0].ToLower().Contains("cyber") || args[0].ToLower().Contains("grind") || args[0].ToLower() == "cg"))
            {
                Tools.Load("Endless");
                chat.Receive("[#32CD32]The Cyber Grind is loading.");
            }
            else if (args.Length >= 1 && (args[0].ToLower().Contains("credits") || args[0].ToLower() == "museum"))
            {
                Tools.Load("CreditsMuseum2");
                chat.Receive("[#32CD32]The Credits Museum is loading.");
            }
            else if (args.Length < 2)
                chat.Receive($"[#FF341C]Insufficient number of arguments.");
            else if
            (
                int.TryParse(args[0], out int layer) && layer >= 0 && layer <= 7 &&
                int.TryParse(args[1], out int level) && level >= 1 && level <= 5 &&
                (level == 5 ? layer == 0 : true) && (layer == 3 || layer == 6 ? level <= 2 : true)
            )
            {
                Tools.Load($"Level {layer}-{level}");
                chat.Receive($"[#32CD32]Level {layer}-{level} is loading.");
            }
            else if (args[1].ToUpper() == "S" && int.TryParse(args[0], out level) && level >= 0 && level <= 7 && level != 3 && level != 6)
            {
                Tools.Load($"Level {level}-S");
                chat.Receive($"[#32CD32]Secret level {level}-S is loading.");
            }
            else if (args[1].ToUpper() == "E" && int.TryParse(args[0], out level) && level >= 0 && level <= 1)
            {
                Tools.Load($"Level {level}-E");
                chat.Receive($"[#32CD32]Encore level {level}-E is loading.");
            }
            else if (args[0].ToUpper() == "P" && int.TryParse(args[1], out level) && level >= 1 && level <= 2)
            {
                Tools.Load($"Level P-{level}");
                chat.Receive($"[#32CD32]Prime level P-{level} is loading.");
            }
            else
                chat.Receive("[#FF341C]Layer must be an integer from 0 to 7. Level must be an integer from 1 to 5.");
        });

        Handler.Register("authors", "Display the list of the mod developers", args =>
        {
            void Msg(string msg) => chat.Receive($"[14]{msg}[]");

            // man, I love how GITHUB reverted this
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
            Msg("* [#0096FF]bryan[] - UI");
            Msg("* [#0096FF]archangel[] - UI");
            Msg("I'm going to add more things to this list later...");
        });

        Handler.Register("getname", "Gets username", args =>
        {
            string us = PrefsManager.Instance.GetString("COAT.username");

            chat.Receive($"Username: {us}");
            chat.Receive($"IS NULL? {us == null}");
        });

        Handler.Register("setname", "Sets username", args =>
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

            chat.Receive($"Set the username to: {us}");
        });
    }
}