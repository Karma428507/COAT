namespace COAT.Net;

using COAT.Assets;
using COAT.Content;
using COAT.Net.Types;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// A class that provides a list on entities
/// </summary>
public class Entities
{
    /// <summary> Dictionary of entity types to their providers. </summary>
    public static Dictionary<EntityType, Prov> Providers = new();
    /// <summary> Last used id, next id's are guaranteed to be greater than it. </summary>
    public static uint LastId;

    public static void Load()
    {
        // Add the thing for players ONCE DollAssets.cs is fixed
        Providers.Add(EntityType.Player, DollAssets.CreateDoll);
    }

    /// <summary> Instantiates the given prefab and marks it with the Net tag. </summary>
    public static GameObject Mark(GameObject prefab)
    {
        // the instance is created on these coordinates so as not to collide with anything after the spawn
        var instance = Tools.Instantiate(prefab, Vector3.zero);

        instance.name = "Net";
        return instance;
    }

    /// <summary> Returns an entity of the given type. </summary>
    public static Entity Get(uint id, EntityType type)
    {
        var entity = Providers[type]();
        if (entity == null) return null;

        entity.Id = id;
        entity.Type = type;

        return entity;
    }

    /// <summary> Returns the next available id, skips ids of all existing entities. </summary>
    public static uint NextId()
    {
        if (LastId < Tools.AccId) LastId = Tools.AccId;

        LastId++;
        while (Networking.Entities.ContainsKey(LastId)) LastId += 8192;

        return LastId;
    }

    /// <summary> Entity provider. </summary>
    public delegate Entity Prov();
}
