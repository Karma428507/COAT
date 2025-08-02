namespace COAT.Chat.Commands;

using COAT.Chat;
using COAT.Content;
using COAT.Net;
using COAT.UI.Overlays;

/// <summary> Commands for debugging </summary>
public class Debug : ICommandModule
{
    static ChatUI chat => ChatUI.Instance;

    // Always true
    public bool Condition() => Plugin.DebugMode;

    public void Load()
    {
        ChatHandler.Register("list", "list", args =>
        {
            string text = "";
            foreach (uint id in Networking.COATPLAYERS)
            {
                text = text + $"{id}";
            }
            chat.Receive(text);
        });

        ChatHandler.Register("getname", "Gets username", args =>
        {
            string us = PrefsManager.Instance.GetString("COAT.username");

            chat.Receive($"Username: {us}");
            chat.Receive($"IS NULL? {us == null}");
        });

        ChatHandler.Register("setname", "Sets username", args =>
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

        ChatHandler.Register("dumpprov", "Looks at the entities list", args =>
        {
            for (int i = 0; i < (int)EntityType.Ball; i++)
            {
                Log.Debug($"\t{Entities.Providers.ContainsKey((EntityType)i)}\n");
            }
        });
    }
}
