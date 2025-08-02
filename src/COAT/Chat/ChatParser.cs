namespace COAT.Chat;

using COAT.Assets;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary> A class used as an easier method to parse chat. WIP! </summary>
public static class ChatParser
{
    /// <summary> Parses the recieved message for the user. </summary>
    /// <returns> The parsed message. </returns>
    public static string Parse(string message)
    {
        return Bundle.CutColors(message);
    }

    /// <summary> Handles the message and runs the corresponding command. </summary>
    /// <returns> True if the command is found and run, or false if the command is not found or the message is not a command. </returns>
    public static bool IsCommand(string message)
    {
        // the message is not a command, because they start with /
        if (!message.StartsWith("/")) return false;
        message = message.Substring(1).Trim();

        // find a command by name and run it
        string name = (message.Contains(" ") ? message.Substring(0, message.IndexOf(' ')) : message).ToLower();
        foreach (var command in ChatHandler.Commands)
            if (command.Name == name)
            {
                command.Handle(message.Substring(name.Length));
                return true;
            }

        // the command was not found
        return false;
    }
}
