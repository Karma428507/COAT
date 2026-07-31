namespace COAT.Net.Files;

using COAT.Net.Files;
using System.IO;
using UnityEngine;

/// <summary> Holds the virtual file being passed through packets when recieved. </summary>
public class NetQueue
{
    public uint ID;

    public byte Type;

    public NetQueue(uint ID, byte Type)
    {
        this.ID = ID;
        this.Type = Type;
    }

    public bool Compare(uint ID, byte Type) => this.ID == ID && this.Type == Type;

    public NetQueue GetSelf(uint ID, byte Type) => Compare(ID, Type) ? this : null;

    public static bool operator ==(NetQueue a, NetQueue b) {
        if (ReferenceEquals(a, b))
            return true;

        if (ReferenceEquals(a, null))
            return false;

        if (ReferenceEquals(b, null))
            return false;

        Log.Debug($"ID: {a.ID} == {b.ID}");
        Log.Debug($"Type: {a.Type} == {b.Type}");

        return a.ID == b.ID && a.Type == b.Type;
    }

    public static bool operator !=(NetQueue a, NetQueue b) => !(a == b);
}
