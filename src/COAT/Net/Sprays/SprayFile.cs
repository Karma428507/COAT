namespace COAT.Net.Sprays;

using COAT.Net.Files;
using System.IO;
using UnityEngine;

/// <summary> File containing an image data aka spray, or container for a remote spray. </summary>
public class SprayFile : NetFile
{
    /// <summary> Max size of the image in bytes. If the image is bigger, it won't be loaded. </summary>
    new public const int MAX_FILE_SIZE = 256 * 1024;

    /// <summary> List of supported image extensions. </summary>
    public const string SUPPORTED = ".png.jpg.jpeg";

    private Sprite _sprite;
    public Sprite Sprite => _sprite ??= LoadSprite(Data);

    /// <summary> Converts the data to a sprite. </summary>
    public static Sprite LoadSprite(byte[] data)
    {
        Texture2D tex = new(2, 2) { filterMode = FilterMode.Point };
        tex.LoadImage(data);

        return Sprite.Create(tex, new(0f, 0f, tex.width, tex.height), Vector2.zero, 256);
    }

    public SprayFile(string path) : base(path) { }

    public SprayFile(byte[] data) : base(data) { }

    /// <summary> Whether the image fits the max size. </summary>
    public bool IsValid() => File.Exists(Path) && new FileInfo(Path).Length < MAX_FILE_SIZE;

    /// <summary> Shortens the name of the file. </summary>
    public string ShortName(int max = 21) => Name.Length > max ? Name.Substring(0, max - 3) + "..." : Name;
}
