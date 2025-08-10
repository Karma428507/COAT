namespace COAT.Chat.Commands;

using COAT.Assets;
using COAT.Chat;
using COAT.Content;
using COAT.Entities;
using COAT.Net;
using COAT.UI.Overlays;
using System;

/// <summary> Commands for utilities (levels and plushies rn) </summary>
public class Utils : ICommandModule
{
    static ChatUI chat => ChatUI.Instance;

    // Always true
    public bool Condition() => true;

    public void Load()
    {
        ChatHandler.Register("plushies", "Display the list of all dev plushies", args =>
        {
            string[] plushies = (string[])GameAssets.PlushiesButReadable.Clone();
            Array.Sort(plushies); // sort alphabetically for a more presentable look

            chat.Receive(string.Join(", ", plushies));
        });

        ChatHandler.Register("plushy", "<name>", "Spawn a plushy by name", args =>
        {
            string name = args.Length == 0 ? null : args[0].ToLower();
            int index = Array.FindIndex(GameAssets.PlushiesButReadable, plushy => plushy.ToLower() == name);

            if (index == -1)
                chat.Receive($"[#FF341C]Plushy named {name} not found.");
            else
                Tools.Instantiate(Items.Prefabs[EntityType.PlushyOffset + index - EntityType.ItemOffset].gameObject, NewMovement.Instance.transform.position);
        });

        ChatHandler.Register("level", "<layer> <level> / sandbox / cyber grind / credits museum", "Load the given level", args =>
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
    }
}
