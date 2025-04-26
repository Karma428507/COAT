namespace COAT.IO;

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class Reader : IO
{
    public Reader(IntPtr memory, int length) : base(memory, length) { }

    public static void Read(IntPtr memory, int length, Action<Reader> cons) => cons(new(memory, length));

    /// <summary> Converts integer to float. </summary>
    public static unsafe float Int2Float(int value) => *(float*)&value;
    /// <summary> Converts int to uint. </summary>
    public static unsafe uint Int2Uint(int value) => *(uint*)&value;

    // Directly reads to the buffer
    #region Types
    public bool Bool() => Marshal.ReadByte(memory, Inc(1)) == 0xFF;
    public void Bools(out bool v0, out bool v1, out bool v2, out bool v3, out bool v4, out bool v5, out bool v6, out bool v7)
    {
        byte value = Byte();
        v0 = (value & 1 << 0) != 0; v1 = (value & 1 << 1) != 0; v2 = (value & 1 << 2) != 0; v3 = (value & 1 << 3) != 0;
        v4 = (value & 1 << 4) != 0; v5 = (value & 1 << 5) != 0; v6 = (value & 1 << 6) != 0; v7 = (value & 1 << 7) != 0;
    }
    public byte Byte() => Marshal.ReadByte(memory, Inc(1));
    public byte[] Bytes(int start, int amount)
    {
        var bytes = new byte[amount];
        Marshal.Copy(memory + Inc(amount), bytes, start, amount);
        return bytes;
    }
    public byte[] Bytes(int amount) => Bytes(0, amount);
    public int Int() => Marshal.ReadInt32(memory, Inc(4));
    public uint Id() => Int2Uint(Marshal.ReadInt32(memory, Inc(4)));
    public float Float() => Int2Float(Marshal.ReadInt32(memory, Inc(4)));
    public long Long() => Marshal.ReadInt64(memory, Inc(8));
    #endregion

    // To read structures into the buffer
    #region Special Types
    public string String() => Encoding.Unicode.GetString(Bytes(Int()));
    public Vector3 Vector() => new(Float(), Float(), Float());
    public Color32 Color() => new(Byte(), Byte(), Byte(), Byte());
    public T Enum<T>() where T : Enum => (T)System.Enum.ToObject(typeof(T), Byte());

    // I'm going to rewrite this when I update the player structure
    /*public void Player(out Team team, out byte weapon, out byte emoji, out byte rps, out bool typing)
    {
        short value = Marshal.ReadInt16(mem, Inc(2));

        weapon = (byte)(value >> 10 & 0b111111);
        team = (Team)(value >> 7 & 0b111);
        emoji = (byte)(value >> 3 & 0b1111);
        rps = (byte)(value >> 1 & 0b11);
        typing = (value & 1) != 0;

        if (weapon == 0b111111) weapon = 0xFF;
        if (emoji == 0b1111) emoji = 0xFF;
    }*/
    #endregion
}
