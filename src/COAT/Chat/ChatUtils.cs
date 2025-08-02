namespace COAT.Chat;

using COAT.Content;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI.Overlays;
using COAT.World;
using Sam;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ChatUtils
{
    /// <summary> Prefix that will be added to BOT messages. </summary>
    public const string BOT_PREFIX = "[#F75][14]\\[BOT][][]";
    /// <summary> Prefix that will be added to TTS messages. </summary>
    public const string TTS_PREFIX = "[#F75][14]\\[TTS][][]";
    /// <summary> Prefox that will be added to HOST messages. </summary>
    public const string HOST_PREFIX = "[#F75][14]\\[HOST][][]";
    /// <summary> Prefix that will be added to COAT messages. </summary>
    public const string COAT_PREFIX = "[#FE7][14]\\[COAT][][]";

    public static Color[] DevColor = new[]
        { Team.Pink.Color(), Team.Purple.Color() };

    public static uint[] DevID = new uint[]
        { 1811031719u, 1238954961u };

    public static string[] DevFallbackNames = new[]
    { "<color=#0fc>Bryan</color>_-000-", "whyis2plus2" };

    public static string[] FunFacts = new[]
    { "COAT has anti-F-Ban, because 2 of it's devs independently made their own F-Ban's", "Mods, strip him down butt booty naked and slam his ass onto a photocopy-er then take a snapshot of his balls. im saving it for later.", "I came up with the idea of P A (i) N because i was punching KARMA in a lobby", "ombor.", "P A (i) N *will have* custom emotes!", "Ourple team exists because of the  Pro Tips messages!", "this fun fact isnt fun but..\\n/hello Doesnt actually send the message to other players..", "umm.. uhh.. idfk man these are barely fun facts im not creative enough :c", "You can KnuckleBlaster a coin to send it flying!" };

    public static string[] ProTips = new[]
    { $"Press \\[{$"{Keybinds.EmojiWheelKey}".ToUpper()}] to emote!", "Use the \\[ESC] menu to leave a lobby!", "You can add custom colors to your name and lobby names by doing [red]<[1] []color=red[1] []>[]!", "You can blacklist certain mods by typing their names into the \"Modlist\" inside of Settings![8][#bbb](F3)[][]" };

    /// <summary> Sends some useful information to the chat. </summary>
    public static void Hello(bool force = false)
    {
        Func<string[], string> GetRandom = l => l[UnityEngine.Random.Range(0, l.Length)];

        // if the last owner of the lobby is not equal to 0, then the lobby is not created for the first time
        if (LobbyController.LastOwner != 0L && !force) return;

        void Msg(string msg, int dev)
        {
            string devname = Tools.Friend(DevID[dev - 1]).Name;
            if (devname == "[unknown]") devname = DevFallbackNames[dev - 1];

            ChatUI.Instance.Receive(ColorToHex(DevColor[dev - 1]), BOT_PREFIX + devname, msg);
        }
        void Tip(string tip, int dev) => Msg($"[14]* {tip}[]", dev);

        Msg("[24]<b>Hey!</b>[18] Welcome to COAT.", 1);
        Msg("I[6][#bbb](Bryan)[][] respond the quickest out of all the devs,", 1);
        Msg("So if you ever have any questions or confusion about COAT, feel free to ask me InGame or on Discord. [6][#bbb](@fredayddd321ewq)[][]\\n", 1);

        if (UnityEngine.Random.Range(0, 10) == 1)
        {
            Msg("FunFact:", 1);
            Tip(GetRandom(FunFacts), 1);
        }
        else
        {
            Msg("Pro Tip:", 2);
            Tip(GetRandom(ProTips), 2);
        }
    }

    public static string ColorToHex(Color color)
    {
        return $"#{Mathf.RoundToInt(color.r * 255):X2}" +
               $"{Mathf.RoundToInt(color.g * 255):X2}" +
               $"{Mathf.RoundToInt(color.b * 255):X2}";
    }
}
