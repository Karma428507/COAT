namespace COAT.Net.Types.Players;

using COAT.Content;
using COAT.IO;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LocalPlayer : Entity
{
    private NewMovement nm => NewMovement.Instance;
    private FistControl fc => FistControl.Instance;
    private GameObject cw => GunControl.Instance.currentWeapon;

    /// <summary> Team can be changed through the players list. </summary>
    public Team Team;
    /// <summary> Component that plays the voice of the local player, not his teammates. </summary>
    public AudioSource Voice;


    private void Awake()
    {
        Owner = Id = Tools.AccId;
        Type = EntityType.Player;

        Voice = gameObject.AddComponent<AudioSource>();
    }

    public override void Read(Reader r)
    {
        // No need to read because LocalPlayer is it's own thing
    }

    public override void Write(Writer w)
    {
        // Only added SOME of the packet data
        UpdatesCount++;

        w.Vector(nm.transform.position);
        w.Float(nm.transform.eulerAngles.y);
        w.Float(135f - Mathf.Clamp(CameraController.Instance.rotationX, -40f, 80f));
    }
}
