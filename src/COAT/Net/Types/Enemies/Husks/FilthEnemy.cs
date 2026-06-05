namespace COAT.Net.Types;

using UnityEngine;
using UnityEngine.AI;

using COAT.Entities;
using COAT.IO;

public class FilthEnemy : SimpleEnemy
{
    /// <summary> Whether the filth is about to jump (brutal). </summary>
    private bool crouching;

    protected override void Awake()
    {
        Init(_ => Enemies.Type(EnemyId));
        InitTransfer();
    }

    protected override void Start()
    {
        SpawnEffect();
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
    }

    public override void Kill()
    {
    }

    #endregion
}
