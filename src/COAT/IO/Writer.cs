namespace COAT.IO;

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

class Writer : IO
{
    public Writer(IntPtr memory, int length) : base(memory, length) { }

    /// <summary> Converts float to integer. </summary>
    public static unsafe int Float2Int(float value) => *(int*)&value;
    /// <summary> Converts uint to int. </summary>
    public static unsafe int Uint2int(uint value) => *(int*)&value;

    // Directly writes to the buffer
    #region Types
    public void Bool(bool value) => Marshal.WriteByte(memory, Inc(1), (byte)(value ? 0xFF : 0x00));
    public void Flags(bool v0 = false, bool v1 = false, bool v2 = false, bool v3 = false, bool v4 = false, bool v5 = false, bool v6 = false, bool v7 = false) =>
        Byte((byte)((v0 ? 1 : 0) | (v1 ? 1 << 1 : 0) | (v2 ? 1 << 2 : 0) | (v3 ? 1 << 3 : 0) | (v4 ? 1 << 4 : 0) | (v5 ? 1 << 5 : 0) | (v6 ? 1 << 6 : 0) | (v7 ? 1 << 7 : 0)));
    public void Byte(byte value) => Marshal.WriteByte(memory, Inc(1), value);
    public void Bytes(byte[] value, int start, int length) => Marshal.Copy(value, start, memory + Inc(length), length);
    public void Bytes(byte[] value) => Bytes(value, 0, value.Length);
    public void Int(int value) => Marshal.WriteInt32(memory, Inc(4), value);
    public void UInt(uint value) => Marshal.WriteInt32(memory, Inc(4), Uint2int(value));
    public void Float(float value) => Marshal.WriteInt32(memory, Inc(4), Float2Int(value));
    public void Long(long value) => Marshal.WriteInt64(memory, Inc(8), value);
    #endregion

    // To write structures into the buffer
    #region Special Types
    public void String(string value)
    {
        value ??= "";
        Int(value.Length * 2);
        Bytes(Encoding.Unicode.GetBytes(value));
    }
    public void Vector(Vector3 value)
    {
        Float(value.x);
        Float(value.y);
        Float(value.z);
    }
    public void Color(Color32 value) => Int(value.rgba);
    public void Enum<T>(T value) where T : Enum => Byte(Convert.ToByte(value));
    // I'm going to rewrite this when I update the player structure
    /*public void Player(Team team, byte weapon, byte emoji, byte rps, bool typing)
    {
        if (weapon == 0xFF) weapon = 0b111111;
        if (emoji == 0xFF) emoji = 0b1111; // null emoji is recorded as 255, but only 4 bits stand out under emoji

        Marshal.WriteInt16(mem, Inc(2), (short)((weapon << 10) | (Convert.ToByte(team) << 7) | (emoji << 3) | (rps << 1) | (typing ? 1 : 0)));
    }*/
    #endregion
}
