namespace COAT.Chat;

using COAT.Chat.Commands;
using System;
using System.Collections.Generic;

/// <summary> Handler for chat processing </summary>
public static class ChatHandler
{
    public static List<Command> Commands = new List<Command>();

    public static void Load()
    {
        List<ICommandModule> modules = new List<ICommandModule>()
        {
            new Debug(),
            new Info(),
            new Settings(),
            new Utils(),
        };

        foreach (var module in modules)
            if (module != null && module.Condition())
                module.Load();
    }

    /// <summary> Registers a new command. </summary>
    public static void Register(string name, string args, string desc, Action<string[]> handler) =>
        Commands.Add(new(name, args, desc, handler));


    /// <summary> Registers a new command with no arguments. </summary>
    public static void Register(string name, string desc, Action<string[]> handler) =>
        Commands.Add(new(name, null, desc, handler));
}
