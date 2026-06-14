namespace COAT.Assets;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

public class AssemblyAssets
{
    /// <summary> Goes through the resources within the DLL and loads it into an array. </summary>
    public static void Load()
    {
        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("embedded.txt");

        foreach (string name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            Log.Debug($"Resource: {name}");

        //Assembly.GetExecutingAssembly().
        //Log.Debug($"Text: {stream.ToString()}");
    }
}
