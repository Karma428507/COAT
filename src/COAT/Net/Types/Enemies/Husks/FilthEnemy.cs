namespace COAT.Net.Types;

using UnityEngine;
using UnityEngine.AI;

using COAT.Entities;
using COAT.IO;

public class FilthEnemy : Enemy
{
    /// <summary> Whether the filth is about to jump (brutal). </summary>
    private bool crouching;

    private void Awake()
    {
        Init(_ => Enemies.Type(EnemyId));
    }

    private void Start()
    {
        SpawnEffect();
        
        transform.parent.position = transform.position + Vector3.down * 10f; // teleport the spawn effect
    }

    private void Update() => Stats.MTE(() =>
    {
        if (IsOwner || Dead) return;

        transform.position = new(x.Get(LastUpdate), y.Get(LastUpdate), z.Get(LastUpdate));
    });

    #region entity

    public override void Write(Writer w)
    {
        base.Write(w);
    }

    public override void Read(Reader r)
    {
        base.Read(r);
        if (IsOwner) return;
    }

    public override void Kill()
    {
    }

    #endregion
}
