namespace COAT.Net.Pages;

using COAT.IO;
using COAT.Net.Files;
using Steamworks;
using System.Collections.Generic;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class PageFile : NetFile
{
    public PageFile(string path) : base(path)
    {
        Log.Error("Page file cannot from a physical storage media.");
    }

    public PageFile(byte[] data) : base(data)
    {
    }
}
