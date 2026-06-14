namespace COAT.IO;

using COAT.Net;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

public class Pointers
{
    /// <summary> The temporary buffer that manages most of the networking data. </summary>
    public static Pointers SoftBuffer;
    /// <summary> The static buffer that manages syncing level data and static settings. </summary>
    public static Pointers HardBuffer;
    
    /// <summary> The pointer for the global buffer. </summary>
    public IntPtr Pointer;
    /// <summary> An offset for the buffer. </summary>
    public int Offset;
    /// <summary> The size for the buffer. </summary>
    public int Size;

    /// <summary> Allocates the main server's buffer </summary>
    public Pointers(int Size) => Pointer = Marshal.AllocHGlobal(this.Size = Size);

    /// <summary> Loads the hard and soft buffers </summary>
    public static void Load()
    {
        SoftBuffer = new Pointers(256 * 1024);
        HardBuffer = new Pointers(4 * 1024);

        // Since the hardbuffer is meant for level syncs
        Events.OnLoaded += () =>
        {
            if (LobbyController.Online)
                HardBuffer.Reset();
        };
    }

    /// <summary> Allocates a portion of the memory within the memory </summary>
    public IntPtr Allocate(int Bytes)
    {
        var alloc = Pointer + Offset;

        if ((Offset += Bytes) >= Size) throw new OutOfMemoryException("Attempt to allocate more bytes than were reserved in memory.");
        return alloc;
    }

    /// <summary> Frees the memory allocated for writers </summary>
    public void Reset() => Offset = 0;
}
