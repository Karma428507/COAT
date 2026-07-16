namespace COAT.Chat;

using COAT.Assets;
using COAT.UI.Overlay;

using System;
using System.Collections.Generic;

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
    public static List<CommandHandler> ExternalCommands;
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
    }

    public static void Hello(bool force = false)
    {
        void Msg(string message) => ChatUI.Instance.Receive(Darkblue, BOT_PREFIX + "COAT bot", message);

        Msg("Welcome to COAT, for a list of commands, type in /help");
        Msg("If you want to talk without typing /tts for each message, type in /tts-auto");
    }

    /// <summary> Parses the recieved message for the user. </summary>
    /// <returns> The parsed message. </returns>
    public static string Parse(string message)
    {
        return Localization.CutColors(message);
    }

    /// <summary> Handles the message and runs the corresponding command. </summary>
    /// <returns> True if the command is found and run, or false if the command is not found or the message is not a command. </returns>
    public static bool IsCommand(string message)
    {
        // the message is not a command, because they start with /
        if (!message.StartsWith("/")) return false;
        message = message.Substring(1).Trim();

        // find a command by name and run it
        string name = (message.Contains(" ") ? message[..message.IndexOf(' ')] : message).ToLower();

        // Replace false with the game setting
        if (name == "experimental" && true)
        {
            string newName = message[name.Length..];
            name = (message.Contains(" ") ? message[..message.IndexOf(' ')] : message).ToLower();

            Log.Debug($"Name: [{newName}], [{name}]");

            ExperimentalCommands.RunCommand(name, message[name.Length..]);
        }

        foreach (var ext in ExternalCommands)
            ext.RunCommand(name, message[name.Length..]);

        // the command was not found
        return false;
    }
}
