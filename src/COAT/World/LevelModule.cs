namespace COAT.World;

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class LevelModule
{
    public virtual string Level { get; set; }

    public abstract void Load();

    // StaticAction functions
    public void LevelDestroy(string name, Vector3 position) =>
        StaticAction.Destroy(Level, name, position);

    public void LevelEnable(string name, Vector3 position) =>
        StaticAction.Enable(Level, name, position);

    public void LevelFind(string name, Vector3 position, Action<GameObject> action) =>
        StaticAction.Find(Level, name, position, action);

    public void LevelPatch(string name, Vector3 position) =>
        StaticAction.Patch(Level, name, position);

    public void LevelPlaceTorches(Vector3 position, float radius) =>
        StaticAction.PlaceTorches(Level, position, radius);
    
    // NetAction functions
    public void LevelSync(string name, Vector3 position, Action<Transform> action = null) =>
        NetAction.Sync(Level, name, position, action);

    public void LevelSyncButton(string name, Vector3 position, Action<Transform> action = null) =>
        NetAction.SyncButton(Level, name, position, action);

    public void LevelSyncLimbo(Vector3 position) =>
        NetAction.SyncLimbo(Level, position);
}
