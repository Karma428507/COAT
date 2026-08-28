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

    /// <summary> Null page file. </summary>
    public const byte NET_FILE_TYPE_PAGE_NULL =           0x10;
    /// <summary> Net file type of the world page. </summary>
    public const byte NET_FILE_TYPE_PAGE_WORLD =            0x11;
    /// <summary> Net file type of the special page. </summary>
    public const byte NET_FILE_TYPE_PAGE_SPECIAL =          0x12;
    /// <summary> Net file type of the enemies page. </summary>
    public const byte NET_FILE_TYPE_PAGE_ENEMIES =          0x13;
    /// <summary> Net file type of the sandbox page. </summary>
    public const byte NET_FILE_TYPE_PAGE_SANDBOX =          0x14;
    /// <summary> Net file type of the sandbox enemies page. </summary>
    public const byte NET_FILE_TYPE_PAGE_SANDBOX_ENEMIES =  0x15;

    /// <summary> Just another null page but specifically to denote the last net file index for pages. </summary>
    public const byte NET_FILE_TYPE_PAGE_END = 0x7F;

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

    public static bool IsPageRange(byte type) => type >= NET_FILE_TYPE_PAGE_NULL && type <= NET_FILE_TYPE_PAGE_END;
}
