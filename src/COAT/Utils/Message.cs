namespace COAT.Utils;

using COAT.Assets;
using COAT.Content;
using COAT.IO;
using COAT.Net;
using COAT.Net.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary> Tools for sending messages. </summary>
public class Message
{
    #region receiving message
    #endregion
    #region chat specific
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
