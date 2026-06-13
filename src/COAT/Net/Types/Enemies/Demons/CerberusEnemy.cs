namespace COAT.Net.Types;

using UnityEngine;
using UnityEngine.AI;

using COAT.Entities;
using COAT.IO;

public class CerberusEnemy : Enemy
{
    // When the cerberus is going to throw the ball
    private bool prepareThrow;
    
    private void Awake()
    {
        Init(_ => Enemies.Type(EnemyId));
    }

    private void Start()
    {
        SpawnEffect();
        Boss(Tools.Scene == "Level 0-5", 80f);
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
