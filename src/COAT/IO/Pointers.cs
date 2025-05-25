namespace COAT.IO;

using System;
using System.Reflection;
using System.Runtime.InteropServices;

class Pointers
{
    public static int RESERVED = 256 * 1024;
    /// <summary> The pointer for the global buffer </summary>
    public static IntPtr Pointer;
    /// <summary> An offset for the buffer </summary>
    public static int Offset;

    /// <summary> Allocates the main server's buffer </summary>
    public static void Allocate() => Pointer = Marshal.AllocHGlobal(RESERVED);

    /// <summary> Allocates </summary>
    public static IntPtr Allocate(int Bytes)
    {
        var alloc = Pointer + Offset;

        if ((Offset += Bytes) >= RESERVED) throw new OutOfMemoryException("Attempt to allocate more bytes than were reserved in memory.");
        return alloc;
    }

    /// <summary> Frees the memory allocated for writers </summary>
    public static void Reset() => Offset = 0;
}
