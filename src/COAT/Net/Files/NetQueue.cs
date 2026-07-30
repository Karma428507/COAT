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

    public override bool Equals(object obj) => Equals(obj as NetQueue);

    public bool Equals(NetQueue queue) => queue.ID == ID && queue.Type == Type;
}
