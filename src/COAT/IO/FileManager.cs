namespace COAT.IO;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary> To manage files like assets, sprays and logs </summary>
public static class FileManager
{
    /// <summary> Gets the DLL's path from plugin. </summary>
    /// <returns> The DLL's path. </returns>
    public static string GetDLLRoot() =>
        Path.GetDirectoryName(Plugin.Instance.Location);

    /// <summary> Merges the path with the DLL's location. </summary>
    /// <param name="path"> The path desired to be merged. </param>
    /// <returns> The full path name. </returns>
    public static string MergeDLLPath(string path) =>
        Path.Combine(GetDLLRoot(), path);

    /// <summary> Merges the path with the DLL's location. </summary>
    /// <param name="path1"> The directory desired to be merged. </param>
    /// <param name="path2"> The path desired to be merged. </param>
    /// <returns> The full path name. </returns>
    public static string MergeDLLPath(string path1, string path2) =>
        Path.Combine(GetDLLRoot(), path1, path2);
    
    /// <summary> Writes the data to the directory. </summary>
    /// <param name="path"> The path to the directory. </param>
    /// <param name="data"> The data to be written/appened. </param>
    public static void CreateAppendFile(string path, List<string> data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.AppendAllLines(path, data);
    }
}
