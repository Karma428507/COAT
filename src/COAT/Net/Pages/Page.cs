namespace COAT.Net.Pages;

using COAT.IO;
using COAT.Net;
using System;
using System.Collections.Generic;

/// <summary> The container class for all of the pages (long term server data). </summary>
public abstract class Page
{
    /// <summary> The index number to determine what page it is. </summary>
    private static int Index;
    /// <summary> Checked if the constructor was ran before. </summary>
    private static bool RanOnce = false;

    /// <summary> The entries for the page with a name and object defining each property. </summary>
    private Dictionary<string, object> Properties = new Dictionary<string, object>();
    /// <summary> References of the objects of the properties, used when reading the data. </summary>
    private static Dictionary<string, object> PropertiesObjects = new Dictionary<string, object>();
    /// <summary> List of names used in the page to be converted into a number by it's index. </summary>
    private List<string> EntryIDs = new List<string>();

    protected Page(int index)
    {
        if (!RanOnce)
        {
            // Initial loading
            Index = index;
            LoadProperties();

            // Setup the Property Objects
            Initialize();
            RanOnce = true;
        }
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
        PropertiesObjects.Add(key, obj.GetType());
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
        foreach (KeyValuePair<string, object> kvp in PropertiesObjects)
            Log.Debug($"\t- [{kvp.Key}]: {kvp.Value}");
    }

    /// <summary> Loads the properties. </summary>
    public abstract void LoadProperties();

    /// <summary> When going into a new scene, this method runs code related to the specific page. </summary>
    public abstract void Reload();

    public byte[] GetFile()
    {
        List<byte> data = new();

        for (int i = 0; i < Properties.Count; i++)
        {
            object value = Properties[EntryIDs[i]];

            Log.Debug($"i = {i}; Obj: {value}, Type: {value.GetType()}");

            switch (value)
            {
                case string s:
                    data.Add((byte)(s.Length & 0xFF));
                    data.Add((byte)((s.Length >> 8) & 0xFF));
                    data.Add((byte)((s.Length >> 16) & 0xFF));
                    data.Add((byte)((s.Length >> 24) & 0xFF));

                    foreach (char c in s)
                        data.Add((byte)c);
                    
                    break;

                case char c:
                    data.Add((byte)c);
                    break;

                case byte b:
                    data.Add(b);
                    break;

                case short s:
                    data.Add((byte)(s & 0xFF));
                    data.Add((byte)((s >> 8) & 0xFF));
                    break;

                case ushort s:
                    data.Add((byte)(s & 0xFF));
                    data.Add((byte)((s >> 8) & 0xFF));
                    break;

                case int I:
                    data.Add((byte)(I & 0xFF));
                    data.Add((byte)((I >> 8) & 0xFF));
                    data.Add((byte)((I >> 16) & 0xFF));
                    data.Add((byte)((I >> 24) & 0xFF));
                    break;

                case uint I:
                    data.Add((byte)(I & 0xFF));
                    data.Add((byte)((I >> 8) & 0xFF));
                    data.Add((byte)((I >> 16) & 0xFF));
                    data.Add((byte)((I >> 24) & 0xFF));
                    break;

                case long l:
                    data.Add((byte)(l & 0xFF));
                    data.Add((byte)((l >> 8) & 0xFF));
                    data.Add((byte)((l >> 16) & 0xFF));
                    data.Add((byte)((l >> 24) & 0xFF));
                    data.Add((byte)((l >> 32) & 0xFF));
                    data.Add((byte)((l >> 40) & 0xFF));
                    data.Add((byte)((l >> 48) & 0xFF));
                    data.Add((byte)((l >> 56) & 0xFF));
                    break;

                case ulong l:
                    data.Add((byte)(l & 0xFF));
                    data.Add((byte)((l >> 8) & 0xFF));
                    data.Add((byte)((l >> 16) & 0xFF));
                    data.Add((byte)((l >> 24) & 0xFF));
                    data.Add((byte)((l >> 32) & 0xFF));
                    data.Add((byte)((l >> 40) & 0xFF));
                    data.Add((byte)((l >> 48) & 0xFF));
                    data.Add((byte)((l >> 56) & 0xFF));
                    break;

                default:
                    Log.Error("Unknown type");
                    return null;
            }

        }

        return data.ToArray();
    }

    public void SetPage(byte[] data)
    {
        int index = 0;

        for (int i = 0; i < Properties.Count; i++)
        {
            Type type = PropertiesObjects[EntryIDs[i]].GetType();

            if (type == typeof(string))
            {
                int len = data[index++] | (data[index++] << 8)
                         | (data[index++] << 16) | (data[index++] << 24);
                Log.Debug($"String length: {len}");
                Properties[EntryIDs[i]] = data[index++];
            }
            else if (type == typeof(char))
                Properties[EntryIDs[i]] = data[index++];
            else if (type == typeof(byte))
                Properties[EntryIDs[i]] = data[index++];
            else if (type == typeof(short))
            {
                short val1 = (short)(data[index++] | (data[index++] << 8));
                Properties[EntryIDs[i]] = val1;
            }
            else if (type == typeof(ushort))
            {
                ushort val2 = (ushort)(data[index++] | (data[index++] << 8));
                Properties[EntryIDs[i]] = val2;
            }
            else if (type == typeof(int))
            {
                int val3 = (data[index++] | (data[index++] << 8)
                                | (data[index++] << 16) | (data[index++] << 24));
                Properties[EntryIDs[i]] = val3;
            }
            else if (type == typeof(uint))
            {
                int val4 = (data[index++] | (data[index++] << 8)
                                | (data[index++] << 16) | (data[index++] << 24));
                Properties[EntryIDs[i]] = val4;
            }
            else if (type == typeof(uint))
            {
                int val5 = (data[index++] | (data[index++] << 8)
                                | (data[index++] << 16) | (data[index++] << 24)
                                | (data[index++] << 32) | (data[index++] << 40)
                                | (data[index++] << 48) | (data[index++] << 56));
                Properties[EntryIDs[i]] = val5;
            }
            else if (type == typeof(uint))
            {
                int val6 = (data[index++] | (data[index++] << 8)
                                | (data[index++] << 16) | (data[index++] << 24)
                                | (data[index++] << 32) | (data[index++] << 40)
                                | (data[index++] << 48) | (data[index++] << 56));
                Properties[EntryIDs[i]] = val6;
            }
            else
            {
                Log.Error("Unknown type");
                return;
            }

            Log.Debug($"i = {i}; Obj: {Properties[EntryIDs[i]]}, Type: {Properties[EntryIDs[i]].GetType()}");
        }

        for (int i = 0; i < Properties.Count; i++)
        {
            object value = Properties[EntryIDs[i]];

            Log.Debug($"i = {i}; Obj: {value}, Type: {value.GetType()}");
        }
    }
}
