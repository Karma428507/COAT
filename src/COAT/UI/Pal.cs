namespace COAT.UI;

using UnityEngine;

/// <summary> Palette of all colors used by the mod. </summary>
public static class Pal
{
    public static string Green = "#32CD32";
    public static string Orange = "#FFA500";
    public static string Red = "#FF341C";
    public static string Darkblue = "#20AAFF"; // this blue is the offical COAT color
    public static string Blue = "#0080FF";
    public static string Pink = "#FF66CC";
    public static string Purple = "#B300FF";
    public static string Grey = "#BBBBBB";
    public static string DarkGrey = "#777777";
    public static string Coral = "#FF7F50";
    public static string Discord = "#5865F2";

    public static Color white = Color.white;
    public static Color black = Color.black;
    public static Color clear = Color.clear;

    public static Color green = new(.2f, .8f, .2f);
    public static Color yellow = new(1f, 1f, 0f);
    public static Color orange = new(1f, .35f, 0f);
    public static Color red = new(1f, .2f, .11f);
    public static Color darkblue = new(.125f, .67f, 1f); // this blue is the offical COAT color
    public static Color blue = new(0f, .5f, 1f);
    public static Color pink = new(1f, .4f, .8f);
    public static Color purple = new(.7f, 0f, 1f);
    public static Color grey = new(.73f, .73f, .73f);
    public static Color darkgrey = new(.467f, .467f, .467f);
    public static Color coral = new(1f, .5f, .31f);
    public static Color discord = new(.345f, .396f, .949f);

    public static Color Dark(Color original) => Color.Lerp(original, black, .38f);
}
