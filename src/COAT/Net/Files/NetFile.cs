namespace COAT.Net.Files;

using COAT.Net.Files;
using System.IO;
using UnityEngine;

/// <summary> Holds the virtual file being passed through packets when recieved. </summary>
public class NetFile
{
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
