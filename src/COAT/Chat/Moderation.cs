namespace COAT.Chat;

using COAT.UI.Menus;
using System.Collections.Generic;
using System.Linq;

// I never been so uncomfortable coding something before
public static class Moderation
{
    // I really don't want to have a list of slurs in plain text
    private static readonly ulong[] EncryptedList =
    {
        0xF30000000007090E,
        0xF300000000070106,
        0xF400000003091013,
        0xF500000B0E090803,
        0xF30000000010010A,
        0xF600041201140512,
    };

    private static List<string> UnencryptedList = new List<string>();

    private static readonly string[] SuffixList =
    {
        "s",
        "ed",
        "er",
        "ful",
        "id",
        "il",
        "ing",
        "ot"
    };

    private static readonly char[] Vowels =
    {
        'a',
        'e',
        'i',
        'o',
        'u',
        'y'
    };

    private static string CensorWord = "";

    public static void Load()
    {
        for (int i = 0; i < 1024; i++) CensorWord += '*';

        // Decrypts the list
        foreach (ulong item in EncryptedList)
        {
            if ((item & 0xF000000000000000) != 0)
            {
                int count = (int)((item & 0xF00000000000000) >> 56);
                string word = "";

                // The i << 3 is there because it looks cool
                for (int i = 0; i < count; i++)
                    word += (char)(((item >> (i << 3)) & 0xFF) + 'a' - 1);

                UnencryptedList.Add(word);
            }
        }
    }

    public static string ParseMessage(string message)
    {
        if (!Settings.EnableModeration)
            return message;

        string[] returnWords = message.Split(' ');
        string[] words = returnWords.Select(s => s.ToLower()).ToArray();

        for (int i = 0; i < words.Length; i++)
        {
            string combinator = string.Concat(words[i].Select((c, j) => j == 0 ||
                        c != words[i][j - 1] ? c : '\0').Where(c => c != '\0'));

            // Check the length
            if (combinator.Length < 3)
                continue;

            // Strips the vowels
            foreach (char c in Vowels) if (combinator[0] == c)
                { combinator = combinator.Substring(1); break; }

            foreach (char c in Vowels) if (combinator[^1] == c)
                { combinator = combinator.Substring(0, combinator.Length - 1); break; }

            // Remove the suffixes
            foreach (string suffix in SuffixList) if (combinator.EndsWith(suffix))
                combinator = combinator.Substring(0, combinator.Length - suffix.Length);

            // Check for any banned words
            foreach (string word in UnencryptedList)
            {
                if (combinator == word)
                {
                    returnWords.SetValue("{" + CensorWord.Substring(0, returnWords[i].Length) + "}", i);
                    break;
                }
            }
        }

        return string.Join(" ", returnWords);
    }
}
