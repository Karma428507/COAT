namespace COAT.Net.Pages;

using COAT.IO;
using COAT.Net;

using System.Collections.Generic;

/// <summary> The container class for all of the pages (long term server data). </summary>
public abstract class Page
{
    /// <summary> The index number to determine what page it is. </summary>
    private int Index;

    /// <summary> The entries for the page with a name and object defining each property. </summary>
    private Dictionary<string, object> Properties = new Dictionary<string, object>();
    /// <summary> List of names used in the page to be converted into a number by it's index. </summary>
    private List<string> EntryIDs = new List<string>();

    protected Page(int index)
    {
        Index = index;
        Initialize();
    }

    /// <summary> Converts the name used for organizing data into a int. </summary>
    public int GetPropertyID(string key) => EntryIDs.IndexOf(key);
    /// <summary> Converts the index into it's name. </summary>
    public string GetPropertyName(int index) => EntryIDs[index];
    
    /// <summary> Adds a property to the page. </summary>
    protected void AddProperty(string key, object obj)
    {
        EntryIDs.Add(key);
        Properties[key] = obj;
    }

    /// <summary> Sets a property to the page (host only). </summary>
    protected void SetProperty(string key, object obj) => Properties[key] = obj;

    /// <summary> Adds a property to the page and submits it to the main page. </summary>
    public void ChangeProperty(string key, object obj)
    {
        Properties[key] = obj;

        if (LobbyController.IsOwner)
        {

        }
        else
        {

        }
    }

    /// <summary> Gets a property from the page. </summary>
    public object GetProperty(string key) => Properties[key];

    /// <summary> A function for the initial page loading logic. </summary>
    private void Initialize()
    {

    }

    public abstract void Reload();
}
