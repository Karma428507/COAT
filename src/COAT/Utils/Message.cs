namespace COAT.Utils;

using COAT.Chat;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI.Overlay;

using Sam;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using static COAT.Assets.Localization;

/// <summary> Tools for sending messages. </summary>
public class Message
{
    #region receiving message

    /// <summary> Sends a localized message to the HUD. </summary>
    public static void HudLocal(string key, bool silent = false) => HudMessageReceiver.Instance?.SendHudMessage(Get(key), silent: silent);

    /// <summary> Sends a localized & formatted message to the HUD. </summary>
    public static void HudLocal(string key, bool silent, params string[] args) => HudMessageReceiver.Instance?.SendHudMessage(Format(key, args), silent: silent);

    /// <summary> Sends a localized message to the HUD after scene loading. </summary>
    public static void Hud2NSLocal(string key) => text2Show = Get(key);

    /// <summary> Sends a localized & formatted message to the HUD after scene loading. </summary>
    public static void Hud2NSLocal(string key, params string[] args) => text2Show = Format(key, args);

    /// <summary> Sends a localized message to the chat. </summary>
    public static void MsgLocal(string key) => ChatUI.Instance.Receive(Get(key), false);

    /// <summary> Sends a localized & formatted message to the chat. </summary>
    public static void MsgLocal(string key, params string[] args) => ChatUI.Instance.Receive(Format(key, args), false);

    #endregion
    #region chat specific

    /// <summary> Writes a message to chat directly. </summary>
    public static void Receive(string msg, bool format = true) => ChatUI.Instance.Receive(msg, format);

    /// <summary> Writes a message for a player. </summary>
    public static void Receive(string msg, Friend author, string color, bool tts = false)
    {
        string username = LobbyController.Lobby?.GetMemberData(author, "username");
        
        string FormattedColor = (color.StartsWith('#') ? color : $"#{color}");
        string FormattedMsg = Censoring.ParseMessage(CutDangerous(msg));
        string FormattedName;

        string FormattedPrefixes = tts ? ChatManager.TTS_PREFIX : "";
        FormattedPrefixes += author.Id == LobbyController.LastOwner ? ChatManager.HOST_PREFIX : "";

        if (username != "")
            FormattedName = $"{username} ({author.Name})".Replace("[", "\\[");
        else
            FormattedName = author.Name.Replace("[", "\\[");

        Receive($"<b>{FormattedPrefixes}[{FormattedColor}]{FormattedName}[][#F75]:[]</b> {FormattedMsg}");
    }

    /// <summary> Speaks the message before writing it. </summary>
    public static void ReceiveTTS(string msg, Friend author, string color)
    {
        // Censor the message
        msg = Censoring.ParseMessage(msg);

        // play the message in the local player's position if he is its author
        if (author.IsMe)
            SamAPI.TryPlay(msg, Networking.LocalPlayer.Voice);

        // or find the author among the other players and play the sound from them
        else if (Networking.Entities.TryGetValue(author.Id.AccountId, out var entity) && entity is RemotePlayer player)
            SamAPI.TryPlay(msg, player.Voice);

        //AudioSource.PlayClipAtPoint(SamAPI.Clip, NewMovement.Instance.transform.position);
        Receive(msg, author, color, true);
    }

    #endregion
    #region parsing utilities

    // <summary> Returns a string without Unity and Jaket formatting. </summary>
    public static string CutColors(string original) => Regex.Replace(original, "<.*?>|\\[.*?\\]", string.Empty);

    // <summary> Returns a string without the tags that can cause lags. </summary>
    public static string CutDangerous(string original) => Regex.Replace(original, "</?size.*?>|</?quad.*?>|</?material.*?>", string.Empty).Replace('\n', ' ');

    /// <summary> Parses the colors in the given string so that Unity could understand them. </summary>
    public static string ParseColors(string original, int maxSize = 64)
    {
        Stack<bool> types = new(); // true - font size, false - color
        StringBuilder builder = new(original.Length);
        int pointer = 0;

        // \n is read as a regular text, so it must be manually replaced with a transfer char
        // space and \ are needed to prevent OutOfBounds
        original = $" {original.Replace("\\n", "\n")}\\";

        while (pointer < original.Length)
        {
            // find the index of the next special char
            int old = pointer;
            pointer = original.IndexOfAny(new[] { '\\', '[' }, pointer);

            // save a piece of the original line without special characters
            builder.Append(original.Substring(old, pointer - old));

            // process the special char
            char c = original[pointer];

            if (c == '\\') pointer++;
            else if (c == '[')
            {
                if (original[pointer - 1] == '\\')
                {
                    builder.Append('[');
                    pointer++;
                }
                else if (original[pointer + 1] == ']')
                {
                    builder.Append(types.Pop() ? "</size>" : "</color>");
                    pointer += 2;
                }
                else
                {
                    old = ++pointer;
                    pointer = original.IndexOf(']', pointer);

                    var content = original.Substring(old, pointer - old);
                    bool isSize = int.TryParse(content, out var size);

                    types.Push(isSize);
                    builder.Append(isSize ? "<size=" : "<color=").Append(isSize ? Math.Min(size, maxSize) : content).Append('>');
                    pointer++;
                }
            }
        }

        // just in case
        foreach (var size in types) builder.Append(size ? "</size>" : "</color>");

        return builder.ToString().Substring(1);
    }

    /// <summary> Reverses the string because Arabic is right-to-left language. </summary>
    public static string ParseArabic(string original) => new(original.Replace("\\n", "\n").Replace('{', '#').Replace('}', '{').Replace('#', '}').Reverse().ToArray());

    #endregion
}
