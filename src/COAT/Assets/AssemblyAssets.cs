namespace COAT.Assets;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

public class AssemblyAssets
{
    /// <summary> Quick way to access the DLL assembly. </summary>
    private static Assembly assembly;
    /// <summary> List of assets imbedded in the DLL. </summary>
    private static List<string> assetNames = new List<string>();

    /// <summary> Goes through the resources within the DLL and loads it into an array. </summary>
    public static void Load()
    {
        string[] res;

        assembly = Assembly.GetExecutingAssembly();
        res = assembly.GetManifestResourceNames();

        foreach (string name in res)
        {
            string cut = name.Substring("COAT.assets.".Length);
            assetNames.Add(cut);
        }
    }

    private static string CheckIfExists(string path)
    {
        foreach (string name in assetNames)
            if (name == path)
                return "COAT.assets." + path;

        return "";
    }

    public static string GetTextFromEmbedded(string path)
    {
        string resourceName = CheckIfExists(path);

        if (resourceName == "")
            return "";
        
        Stream stream = assembly.GetManifestResourceStream(resourceName);
        StreamReader source = new StreamReader(stream);
        string fileContent = source.ReadToEnd();
        source.Dispose();
        stream.Dispose();
        return fileContent;
    }

    public static string[] GetLinedTextFromEmbedded(string path)
    {
        string resourceName = CheckIfExists(path), line;
        List<string> fileContent = new List<string>();

        if (resourceName == "")
            return null;

        Stream stream = assembly.GetManifestResourceStream(resourceName);
        StreamReader source = new StreamReader(stream);

        while ((line = source.ReadLine()) != null)
            fileContent.Add(line);

        source.Dispose();
        stream.Dispose();
        return fileContent.ToArray();
    }

    public static byte[] GetDataFromEmbedded(string path)
    {
        string resourceName = CheckIfExists(path);

        if (resourceName == "")
            return null; 
        
        Stream stream = assembly.GetManifestResourceStream(resourceName);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        stream.Dispose();
        return data;
    }
}
