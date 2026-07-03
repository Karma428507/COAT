namespace COAT.Optimizations;

using System;
using System.Collections.Generic;
using System.Text;

/// <summary> Defines the limits for server optimization. </summary>
public class Limits
{
    /// <summary> Max amount of bytes a player can send per second. </summary>
    public const int SPAM_RATE = 32 * 1024;

    /// <summary> Max amount of entity bullets per player and common bullets per second. </summary>
    public const int MAX_BULLETS = 10;
    /// <summary> Max amount of entities per player. </summary>
    public const int MAX_ENTITIES = 16;
    /// <summary> Max amount of plushies per player. </summary>
    public const int MAX_PLUSHIES = 6;
}
