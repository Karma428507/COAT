namespace COAT.Net;

using COAT.Content;
using COAT.IO;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// A layer for normal entities to be compatible with networking
/// </summary>
public abstract class Entity : MonoBehaviour
{
    /// <summary> Index for normal entites, player IDs for players </summary>
    public uint Id;
    /// <summary> The entity type </summary>
    public EntityType Type;

    /// <summary> ID of the entity's owner </summary>
    public uint Owner;
    /// <summary> Checks if the entity belongs to the player </summary>
    public bool IsOwner => Owner == Tools.AccId;

    /// <summary> Last update time from the last snapshot </summary>
    public float LastUpdate;
    /// <summary> Amount of updates written </summary>
    public uint UpdatesCount;
    /// <summary> The heavy will not be sycned </summary>
    public bool Dead;

    // Components to add later

    protected void Init(Func<Entity, EntityType> prov, bool getGeneralComponents = false)
    {
        // idk what this does but it adds a list if the player owns it or smth
    }

    /// <summary> Writes the entity data to the writer. </summary>
    public abstract void Write(Writer w);
    /// <summary> Reads the entity data from the reader. </summary>
    public abstract void Read(Reader r);

    /// <summary> Deals damage to the entity. </summary>
    //public virtual void Damage(Reader r) => Bullets.DealDamage(EnemyId, r);
    /// <summary> Kills the entity. </summary>
    public virtual void Kill(Reader r) => Dead = true;

    /// <summary> Kills the entity and informs all the network members about it. </summary>
    public void NetKill()
    {
        Kill(null);
        // Uncomment when working on the kill entity packet
        //Networking.Send(PacketType.KillEntity, w => w.Id(Id), size: 4);
    }

    // Research later
    /// <summary> Class for interpolating floating point values. </summary>
    public class FloatLerp
    {
        /// <summary> Interpolation values. </summary>
        public float Last, Target;

        /// <summary> Updates interpolation values. </summary>
        public void Set(float value)
        {
            Last = Target;
            Target = value;
        }

        /// <summary> Reads values to be interpolated from the reader. </summary>
        public void Read(Reader r) => Set(r.Float());

        /// <summary> Returns an intermediate value. </summary>
        public float Get(float lastUpdate) => Mathf.Lerp(Last, Target, (Time.time - lastUpdate) / Networking.SNAPSHOTS_SPACING);

        /// <summary> Returns the intermediate value of the angle. </summary>
        public float GetAngel(float lastUpdate) => Mathf.LerpAngle(Last, Target, (Time.time - lastUpdate) / Networking.SNAPSHOTS_SPACING);
    }

    /// <summary> Class for finding entities according to their ID. </summary>
    public class EntityProv<T> where T : Entity
    {
        /// <summary> Id of the entity that needs to be found. </summary>
        public uint Id;

        private T value;
        public T Value => value?.Id == Id ? value : Networking.Entities.TryGetValue(Id, out var e) && e is T t ? value = t : null;
    }
}
