namespace COAT.Chat;

using COAT.UI.Overlay;
using System;
using System.Collections.Generic;

/// <summary> Handles different specific command depending where it's from. </summary>
public abstract class CommandHandler
{
    /// <summary> A simple structure to deal with an individual command. </summary>
    public struct CommandData
    {
        /// <summary> Basic command parameters displayed by the help command. </summary>
        public string Name, Args, Desc;
        /// <summary> Handler for receiving command arguments. </summary>
        public Action<string[]> Handler;

        public CommandData(string name, string args, string desc, Action<string[]> handler)
        {
            this.Name = name; this.Args = args; this.Desc = desc;
            this.Handler = handler;
        }

        /// <summary> Handles the command call and its arguments. </summary>
        public void Handle(string[] args)
        {
            if (args.Length == 0)
                Handler(new string[0]);
            else
                Handler(args);
        }
    }

    /// <summary> List of commands for the hander. </summary>
    private List<CommandData> Commands = new List<CommandData>();
    /// <summary> Makes it easier to write to chat </summary>
    public static ChatUI Chat => ChatUI.Instance;

    /// <summary> Function to put all of the commands in. </summary>
    public abstract void Load();

    /// <summary> Runs a command that shares the same name. </summary>
    public bool RunCommand(string name, string[] arg)
    {
        foreach (var cmd in Commands)
        {
            if (cmd.Name == name)
            {
                cmd.Handle(arg);
                return true;
            }
        }

        return false;
    }

    /// <summary> Returns the command list (might rework later). </summary>
    public List<CommandData> GetCommands() => Commands;

    /// <summary> Registers a new command. </summary>
    public void Register(string name, string args, string desc, Action<string[]> handler) =>
        Commands.Add(new(name, args, desc, handler));

    /// <summary> Registers a new command with no arguments. </summary>
    public void Register(string name, string desc, Action<string[]> handler) =>
        Commands.Add(new(name, null, desc, handler));
}
