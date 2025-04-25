namespace COAT.IO;

using System;
using System.Reflection;
using System.Runtime.InteropServices;

class Pointers
{
    public static int RESERVED { get; private set; }
    /// <summary> The pointer for the global buffer </summary>
    public static IntPtr Pointer;
    /// <summary> An offset for the buffer </summary>
    public static int Offset;

    /// <summary> Allocates the main server's buffer </summary>
    public static void AllocateGlobal()
    {
        // Set num to player count later
        int num = 8;
        RESERVED = num * 32 * 1024;
        Pointer = Marshal.AllocHGlobal(num * 8);
    }

    /// <summary> Allocates </summary>
    public static IntPtr Allocate(int Bytes)
    {
        var alloc = Pointer + Offset;

        if ((Offset += Bytes) >= RESERVED) throw new OutOfMemoryException("Attempt to allocate more bytes than were reserved in memory.");
        return alloc;
    }

    /// <summary> Frees the memory allocated for writers </summary>
    public static void Reset() => Offset = 0;

    /// <summary> Frees all of the global memory </summary>
    public static void Free() => Marshal.FreeHGlobal(Pointer);
}
