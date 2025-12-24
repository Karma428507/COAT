namespace COAT.Net.Types;

using COAT.Assets;
using COAT.Content;
using COAT.Entities;
using COAT.Input;
using COAT.IO;
using COAT.UI;
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
    public bool is44;

    private void Awake()
    {
        Owner = Id = Tools.AccId;
        Type = EntityType.Player;

        Voice = gameObject.AddComponent<AudioSource>();
    }

    private void Update() => Stats.MTE(() =>
    {
        if (HeldItem == null || HeldItem.IsOwner) return;
        HeldItem = null;

        fc.currentPunch.ForceThrow();
        fc.currentPunch.PlaceHeldObject(new ItemPlaceZone[0], null);
    });

    #region special

    /// <summary> Synchronizes the suit of the local player. </summary>
    public void SyncSuit() => Networking.Send(PacketType.Style, w =>
    {
        w.Id(Id);
        if (cw?.GetComponentInChildren<GunColorGetter>()?.TryGetComponent<Renderer>(out var renderer) ?? false)
        {
            bool custom = renderer.material.name.Contains("Custom");
            w.Bool(custom);

            if (custom) UIB.Properties(renderer, block =>
            {
                w.Color(block.GetColor("_CustomColor1"));
                w.Color(block.GetColor("_CustomColor2"));
                w.Color(block.GetColor("_CustomColor3"));
            });
        }
        else w.Bool(false);
    }, size: 17);

    /// <summary> Caches the id of the current weapon and paints the hands of the local player. </summary>
    public void UpdateWeapons()
    {
        weapon = Weapons.Type();
        is44 = Tools.Scene == "Level 4-4";

        if (LobbyController.Online) SyncSuit();

        // according to the lore, the player plays for V3, so we need to paint the hands
        var punch = fc.transform.Find("Arm Blue(Clone)");
        if (punch) punch.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = DollAssets.HandTexture();

        var right = cw?.transform.GetChild(0).Find("RightArm");
        if (right) right.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = DollAssets.HandTexture();

        var knuckle = fc.transform.Find("Arm Red(Clone)");
        if (knuckle) knuckle.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = DollAssets.HandTexture(false);
    }

    #endregion

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
        w.Player(Team, weapon, Emotes.Instance.Emoji, Emotes.Instance.Rps, ChatUI.Shown);
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
    public override void GoofyWrite(Writer w, Vector3 position) { }

    public void DumWrite(Writer w)
    {
        // Only added SOME of the packet data
        UpdatesCount++;

        w.Vector(nm.transform.position);
        w.Float(nm.transform.eulerAngles.y);
        w.Float(135f - Mathf.Clamp(CameraController.Instance.rotationX, -40f, 80f));
        w.Vector(Hook);

        w.Byte((byte)nm.hp);
        w.Byte((byte)Mathf.Floor(WeaponCharges.Instance.raicharge * 2.5f));
        w.Player(Team.Pink, (byte)6, Emotes.Instance.Emoji, Emotes.Instance.Rps, ChatUI.Shown);
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

    public override void Damage(Reader r)
    {
        var team = r.Enum<Team>();
        if (!nm.dead && !team.Ally()) // no need to deal damage if an ally hits you
        {
            float mul = Bullets.Types[r.Byte()] == "drill" ? ((skip = !skip) ? 0f : 1f) : 4f;

            nm.GetHurt(Mathf.CeilToInt(r.Float() * mul), false, 0f);
            if (nm.dead) LobbyController.Lobby?.SendChatString("#/s" + (byte)team);
        }
    }
}
