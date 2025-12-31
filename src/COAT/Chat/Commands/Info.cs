namespace COAT.Chat.Commands;

using COAT.Chat;
using COAT.UI.Screen;

/// <summary> Information related commands </summary>
public class Info : ICommandModule
{
    static ChatUI chat => ChatUI.Instance;

    // Always true
    public bool Condition() => true;

    public void Load()
    {
        ChatHandler.Register("help", "Display the list of all commands", args =>
        {
            ChatHandler.Commands.ForEach(command =>
            {
                chat.Receive($"[14]* /{command.Name}{(command.Args == null ? "" : $" [#BBBBBB]{command.Args}[]")} - {command.Desc}[]");
            });
        });

        ChatHandler.Register("hello", "Resend the tips for new players", args => ChatUtils.Hello(true));

        ChatHandler.Register("authors", "Display the list of the mod developers", args =>
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
            Msg("* [#0096FF]bryan[] - UI, P A (I) N, emotes");
            Msg("* [#0096FF]archangel[] - UI");
            Msg("I'm going to add more things to this list later...");
        });
    }
}
