namespace COAT.Chat;

using COAT.Assets;
using COAT.UI.Overlay;

using System;
using System.Collections.Generic;
using System.Linq;
using static UI.Utils.Pal;

/// <summary> Handler for chat processing </summary>
public static class ChatManager
{
    /// <summary> Prefix that will be added to BOT messages. </summary>
    public const string BOT_PREFIX = "[#F75][14]\\[BOT][][]";
    /// <summary> Prefix that will be added to TTS messages. </summary>
    public const string TTS_PREFIX = "[#F75][14]\\[TTS][][]";
    /// <summary> Prefox that will be added to HOST messages. </summary>
    public const string HOST_PREFIX = "[#F75][14]\\[HOST][][]";
    /// <summary> Prefix that will be added to COAT messages. </summary>
    public const string COAT_PREFIX = "[#FE7][14]\\[COAT][][]";

    /// <summary> A list of command handlers for other mods to add there own commands to. </summary>
    public static List<CommandHandler> ExternalCommands = new List<CommandHandler>();
    /// <summary> For all of the general purpose commands a normal player would use. </summary>
    public static CommandHandler NormalCommands { get; private set; }
    /// <summary> Commands specific for debugging, only available in debug builds. </summary>
    public static CommandHandler DebugCommands { get; private set; }
    /// <summary> For all of the general purpose commands a normal player would use. </summary>
    public static CommandHandler ExperimentalCommands { get; private set; }

    /// <summary> The function where the chat services and commands are initialized. </summary>
    public static void Load()
    {
        // Load all the externals first
        foreach (var ext in ExternalCommands) ext.Load();

        // Load the internal commands
        NormalCommands = new CommandNormal();
        DebugCommands = new CommandDebug();
        ExperimentalCommands = new CommandExperimental();

        NormalCommands.Load();
        DebugCommands.Load();
        ExperimentalCommands.Load();
    }

    public static void Hello(bool force = false)
    {
        void Msg(string message) => ChatUI.Instance.Receive(Darkblue, BOT_PREFIX + "COAT bot", message);

        Msg("Welcome to COAT, for a list of commands, type in /help");
        Msg("If you want to talk without typing /tts for each message, type in /tts-auto");
    }

    /// <summary> Parses the recieved message for the user. </summary>
    public static string Parse(string message)
    {
        return Localization.CutColors(message);
    }



    /// <summary> Handles the message and runs the corresponding command. </summary>
    /// <returns> True if the command is found and run, or false if the command is not found or the message is not a command. </returns>
    public static bool IsCommand(string message)
    {
        bool result = false;

        // the message is not a command, because they start with /
        if (!message.StartsWith("/")) return false;
        message = message.Substring(1).Trim();

        string[] command = message.Split(' ');

        // find a command by name and run it
        string name = (message.Contains(" ") ? message[..message.IndexOf(' ')] : message).ToLower();

        // Replace false with the game setting
        if (command[0] == "experimental" && true)
        {
            string newName = message[(name.Length + 1)..];

            result = ExperimentalCommands.RunCommand(command[1], command.Skip(2).ToArray());
            if (result) return true;
        }

#if DEBUG
        if (name == "debug")
        {
            string newName = message[(name.Length + 1)..];

            result = DebugCommands.RunCommand(newName, command.Skip(2).ToArray());
            if (result) return true;
        }
#endif

        result = NormalCommands.RunCommand(name, command.Skip(1).ToArray());
        if (result) return true;

        foreach (var ext in ExternalCommands)
        {
            result = ext.RunCommand(name, command.Skip(1).ToArray());
            if (result) return true;
        }

        return false;
    }
}
