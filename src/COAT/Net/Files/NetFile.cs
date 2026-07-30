namespace COAT.Net.Files;

using COAT.Net.Files;
using System.IO;
using UnityEngine;

/// <summary> Holds the virtual file being passed through packets when recieved. </summary>
public class NetFile
{
    /// <summary> Net file type for no files, throw error if this is null. </summary>
    public const byte NET_FILE_TYPE_NULL =                  0x00;
    /// <summary> Net file type for pngs, specifcally for sprays. </summary>
    public const byte NET_FILE_TYPE_SPRAY =                 0x01;

    /// <summary> Net file type the player page. </summary>
    public const byte NET_FILE_TYPE_PAGE_PLAYER =           0x10;
    /// <summary> Net file type the world page. </summary>
    public const byte NET_FILE_TYPE_PAGE_WORLD =            0x11;
    /// <summary> Net file type the special page. </summary>
    public const byte NET_FILE_TYPE_PAGE_SPECIAL =          0x12;
    /// <summary> Net file type the enemies page. </summary>
    public const byte NET_FILE_TYPE_PAGE_ENEMIES =          0x13;
    /// <summary> Net file type the sandbox page. </summary>
    public const byte NET_FILE_TYPE_PAGE_SANDBOX =          0x14;
    /// <summary> Net file type the sandbox enemies page. </summary>
    public const byte NET_FILE_TYPE_PAGE_SANDBOX_ENEMIES =  0x15;

    /// <summary> Max file size for the net file. </summary>
    public const int MAX_FILE_SIZE = 0;

    /// <summary> Name of the file and path to it. </summary>
    public readonly string Name, Path;

    private byte[] data;
    public byte[] Data => data ??= File.ReadAllBytes(Path);

    public NetFile(string path)
    {
        Name = System.IO.Path.GetFileNameWithoutExtension(path);
        Path = path;
    }

    public NetFile(byte[] data)
    {
        Name = Path = "Net";
        this.data = data;
    }
}
