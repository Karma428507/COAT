namespace COAT.Net.Types;

using COAT.Content;
using COAT.IO;
using COAT.UI.Overlays;
using COAT.World;
using Steamworks.Ugc;
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

    /// <summary> Whether the player parried a projectile or just punched. </summary>
    public bool Parried;
    /// <summary> Hook position. Will be zero if the hook is not currently in use. </summary>
    public Vector3 Hook;
    /// <summary> Entity of the item the player is currently holding in their hands. </summary>
    public Item HeldItem;

    /// <summary> Index of the current weapon in the global list. </summary>
    private byte weapon;
    /// <summary> Whether the next packet of drill damage will be skipped. </summary>
    private bool skip;
    /// <summary> Whether the current level is 4-4. Needed to sync fake slide animation. </summary>
    private bool is44;

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
        w.Vector(Hook);

        w.Byte((byte)nm.hp);
        w.Byte((byte)Mathf.Floor(WeaponCharges.Instance.raicharge * 2.5f));
        w.Player(Team, weapon, Movement.Instance.Emoji, Movement.Instance.Rps, Chat.Shown);
        w.Flags(
            nm.walking,
            nm.sliding || (is44 && nm.transform.position.y > 610f && nm.transform.position.y < 611f),
            nm.gc.heavyFall,
            !nm.gc.onGround,
            nm.boost && !nm.sliding,
            nm.ridingRocket != null,
            Hook != Vector3.zero,
            fc.shopping);
    }
}
