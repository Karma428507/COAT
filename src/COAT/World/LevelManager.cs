namespace COAT.World;

using HarmonyLib;
using UnityEngine;

using COAT.Content;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI;
using COAT.UI.Fragments;

using static COAT.UI.Rect;
using COAT.World.Levels;
using System.Collections.Generic;
using System.Reflection;

/// <summary> List of all interactions with the level needed by the multiplayer. </summary>
public class LevelManager
{
    public static List<LevelModule> Modules = new List<LevelModule>()
    {
        // Prelude
        new Level01(),
        new Level02(),
        new Level03(),
        new Level05(),
        new Level0S(),

        // Limbo
        new Level12(),
        new Level13(),
        new Level14(),

        // Lust
        new Level24(),

        // Gluttony
        new Level31(),
        new Level32(),

        // Greed
        new Level41(),
        new Level42(),
        new Level43(),
        new Level44(),

        // Wrath
        new Level51(),
        new Level52(),
        new Level53(),
        new Level54(),

        // Heresy
        new Level61(),
        new Level62(),

        // Violence
        new Level71(),
        new Level72(),
        new Level73(),
        new Level74(),

        // Encore
        new Level0E(),
        new Level1E(),

        // Perfect
        new LevelP1(),
        new LevelP2(),

        // Endless
        new LevelEndless(),
    };

    // NEVER DO DESTROY IMMEDIATE IN STATIC ACTION
    public static void Load()
    {
        foreach (var module in Modules)
            module.Load();

        // Do something relating to angry and envy later
    }
}