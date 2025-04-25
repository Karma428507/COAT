namespace COAT.IO;

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;

/// <summary> Used as the parent for the Reader/Writer class </summary>
public abstract class IO
{
    /// <summary> Current position in the allocated buffer </summary>
    public int Position;
    /// <summary> Pointer to the allocated memory. </summary>
    public readonly IntPtr memory;
    /// <summary> Allocated memory length. </summary>
    public readonly int length;

    /// <summary> Sets up an IO service instance with the given memory </summary>
    public IO(IntPtr memory, int length) { this.memory = memory; this.length = length; }

    /// <summary> Increases the position of the cursor </summary>
    public int Inc(int amount)
    {
        if (Position < 0) throw new IndexOutOfRangeException("Attempt to write data at a negative index.");
        Position += amount;

        if (Position > length) throw new IndexOutOfRangeException("Attempt to write more bytes than were allocated in memory.");
        return Position - amount;
    }
}