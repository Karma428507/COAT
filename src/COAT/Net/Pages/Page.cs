namespace COAT.Pages;

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
    public int GetPropertyID(string name) => EntryIDs.IndexOf(name);
    /// <summary> Converts the index into it's name. </summary>
    public string GetPropertyName(int index) => EntryIDs[index];
    
    /// <summary> Adds a property to the page. </summary>
    protected void AddProperty(string name, object obj)
    {
        EntryIDs.Add(name);
        Properties[name] = obj;
    }

    /// <summary> Adds a property to the page. </summary>
    protected void ChangeProperty(string name, object obj)
    {
        // Check if the player is allowed to change the property (change later)
        if (Index != PageManager.PAGE_INDEX_PLAYER)
            return;

        // If the player is editing a player page, make sure it's theres

        Properties[name] = obj;
    }

    /// <summary> A function for the initial page loading logic. </summary>
    private void Initialize()
    {

    }
}
