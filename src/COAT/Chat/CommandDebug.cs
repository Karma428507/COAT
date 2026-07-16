namespace COAT.Chat;

using COAT.Content;
using COAT.Net;

/// <summary> The normal commands. </summary>
public class CommandDebug : CommandHandler
{
    public override void Load()
    {
        Register("list", "Lists each player", args =>
        {
            string text = "";
            Networking.EachPlayer(c => text = text + $"{c.Id}");
            Chat.Receive(text);
        });

        Register("dumpprov", "Looks at the entities list", args =>
        {
            for (int i = 0; i < (int)EntityType.Ball; i++)
            {
                Chat.Receive($"\t{Entities.Providers.ContainsKey((EntityType)i)}\n");
            }
        });
    }
}
