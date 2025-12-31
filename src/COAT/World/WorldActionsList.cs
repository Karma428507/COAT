namespace COAT.World;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using COAT.Content;
using COAT.Input;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI;
using COAT.UI.Screen;

using static UI.Elements.Rect;

/// <summary> List of all interactions with the level needed by the multiplayer. </summary>
public class WorldActionsList
{
    public const string BASEMENT_TERMILA_TEXT =
@"MACHINE ID: V3#{0}
LOCATION: TERMINAL
CURRENT OBJECTIVE: FUN

AUTHORIZATION... <color=#32CD32>DONE</color>

> Hello?
<color=#FF341C>UNKNOWN COMMAND</color>

> Please let me in :3
OPENING ALL DOORS... <color=#32CD32>DONE</color>";

    private static string Level = "";

    // NEVER DO DESTROY IMMEDIATE IN STATIC ACTION
    public static void Load()
    {
        #region Prelude
        Level = "Level 0-1";
        LevelFind("Cube (2)", new(202f, 73f, 421f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects = new GameObject[0]);
        LevelSync("Cube (2)", new(202f, 73f, 421f)); // boss

        Level = "Level 0-2";
        LevelDestroy("Invisible Wall", new(0f, -7f, 163.5f));
        LevelDestroy("Invisible Wall", new(-45f, -8f, 287.5f));
        LevelDestroy("SwordMachine (1)", new(13f, -10f, 173f));
        LevelDestroy("Activator", new(-44.5f, 0f, 157f));
        LevelDestroy("SwordsMachine", new(-45f, -11f, 268f));
        LevelDestroy("SwordsMachine", new(-55f, -11f, 293f));

        LevelSync("Activator", new(-81f, 9f, 339.5f), obj => obj.parent.parent.gameObject.SetActive(true)); // boss
        
        Level = "Level 0-3";
        LevelSync("Cube (1)", new(-89.5f, 9.3f, 413f)); // boss
        
        Level = "Level 0-4";
        Level = "Level 0-5";
        LevelFind("Cube", new(182f, 4f, 382f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[0] = null); // corridor

        LevelSync("Cube", new(182f, 4f, 382f)); // boss
        LevelSync("DelayedDoorActivation", new(175f, -6f, 382f)); 
        
        Level = "Level 0-S";
        LevelFind("Cube", new(0f, -7.6f, 30f), obj => // blue altar
        {
            if (obj.TryGetComponent(out ItemPlaceZone zone)) zone.deactivateOnSuccess = new[] { zone.deactivateOnSuccess[0] };
        });
        LevelFind("Cube", new(-60f, -7.6f, 17.5f), obj => // red altar
        {
            if (obj.TryGetComponent(out ItemPlaceZone zone)) zone.deactivateOnSuccess = new GameObject[0];
        });

        LevelEnable("Wicked", new(-60f, -10f, 30f));

        LevelFind("Cube", new(-65f, -30.5f, 180f), obj => UIB.Component<HurtZone>(obj, zone =>
        {
            zone.bounceForce = 100f; // allow players to get out of acid
            zone.damageType = EnviroDamageType.Acid;
            zone.setDamage = 20f;
            zone.trigger = true;
        }));

        LevelSync("Cube", new(-56.6f, -21.4f, -5.9f), obj =>
        {
            if (NewMovement.Instance.dead) Movement.Instance.Respawn(new(-60f, -8.5f, 30f), 180f);
        });
        #endregion
        #region Limbo
        Level = "Level 1-1";
        Level = "Level 1-2";
        // secret statue
        LevelSync("Cube", new(0f, -19f, 442f));
        LevelSync("Cube", new(15f, -15f, 417f));

        // Very Cancerous Rodent™
        LevelSync("Cube", new(-61f, -16.5f, 388.5f));
        LevelSync("Cube (1)", new(-61f, -21.5f, 400.5f));

        Level = "Level 1-3";
        LevelFind("Trigger", new(0f, 9.5f, 412f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[0] = null); // corridor

        Level = "Level 1-4";
        LevelFind("Cube", new(0f, 11f, 612f), obj => Tools.Destroy(obj.GetComponent<DoorController>()));

        LevelSync("Cube", new(0f, -19f, 612f));
        Level = "Level 1-S";
        #endregion
        #region Lust
        Level = "Level 2-1";
        Level = "Level 2-2";
        Level = "Level 2-3";
        Level = "Level 2-4";
        LevelFind("DoorsActivator", new(425f, -10f, 650f), obj =>
        {
            obj.GetComponent<ObjectActivator>().events.onActivate.AddListener(() =>
            {
                Teleporter.Teleport(new(500f, 14f, 570f));
            });
        });
        LevelDestroy("Cube", new(130f, 13f, 702f));

        LevelSync("BossActivator", new(142.5f, 13f, 702.5f));
        LevelSync("DeadMinos", new(279.5f, -599f, 575f), obj =>
        {
            obj.parent.Find("GlobalLights (2)/MetroWall (10)").gameObject.SetActive(false);
            obj.parent.Find("BossMusic").gameObject.SetActive(false);
        });

        Level = "Level 2-S";
        #endregion
        #region Gluttony
        Level = "Level 3-1";
        LevelSync("Trigger", new(-203f, -72.5f, 563f)); // lightning
        LevelSync("Deactivator", new(-203f, -72.5f, 528f));
        LevelSync("End Lights", new(-203f, -72.5f, 528f));

        Level = "Level 3-2";
        LevelDestroy("Door", new(-10f, -161f, 955f));

        LevelSync("Cube", new(-5f, -121f, 965f), obj => Teleporter.Teleport(new(-5f, -159.5f, 970f)));

        #endregion
        #region Greed
        Level = "Level 4-1";
        LevelSyncLimbo(new(-290.25f, 24.5f, 814.75f));
        
        Level = "Level 4-2";
        LevelSyncLimbo(new(-150.75f, 33f, 953.1049f));

        LevelEnable("6A - Indoor Garden", new(-19f, 35f, 953.9481f));
        LevelEnable("6B - Outdoor Arena", new(35f, 35f, 954f));

        LevelDestroy("6A Activator", new(-79f, 45f, 954f));
        LevelDestroy("6B Activator", new(116f, 19.5f, 954f));

        LevelSync("DoorOpeners", new(-1.5f, -18f, 774.5f));
        LevelSync("DoorsOpener", new(40f, 5f, 813.5f));

        Level = "Level 4-3";
        LevelPlaceTorches(new(0f, -10f, 310f), 3f);
        LevelDestroy("Doorblocker", new(-59.5f, -35f, 676f));

        LevelSync("DoorActivator", new(2.5f, -40f, 628f));
        LevelSync("Trigger (Intro)", new(-104f, -20f, 676f)); // boss
        LevelSync("Secret Tablet", new(-116.4f, -39.5f, 675.9f), obj => MusicManager.Instance.StopMusic());

        Level = "Level 4-4";
        LevelFind("SecondVersionActivator", new(117.5f, 663.5f, 323f), obj => obj.GetComponent<ObjectActivator>().events.onActivate.AddListener(() =>
        {
            Networking.EachEntity(e => e.Type == EntityType.V2_GreenArm, e => e.gameObject.SetActive(true));
        }));

        LevelSync("Trigger", new(117.5f, 678.5f, 273f)); // boss
        LevelSync("ExitTrigger", new(172.5f, 668.5f, 263f), obj =>
        {
            Networking.EachEntity(e => e.Type == EntityType.V2_GreenArm, e => e.gameObject.SetActive(false));
        });
        LevelSync("BossOutro", new(117.5f, 663.5f, 323f));
        LevelSync("ExitBuilding Raise", new(1027f, 261f, 202.5f), obj =>
        {
            var next = Tools.ObjFind("TutorialMessage").transform.Find("DeactivateMessage").gameObject;
            if (next.activeSelf) return;
            next.SetActive(true);

            var exit = obj.parent.Find("ExitBuilding");
            exit.GetComponent<Door>().Close();
            exit.Find("GrapplePoint (2)").gameObject.SetActive(true);
        });

        Level = "Level 4-S";
        #endregion
        #region Wrath
        Level = "Level 5-1";
        LevelFind("1 - Main Cave", new(0f, -50f, 350f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[0] = null);
        LevelDestroy("HudMessage", new(0f, -100f, 295.5f));
        LevelDestroy("Door", new(218.5f, -41f, 234.5f));

        // there is a checkpoint deactivator, the deactivation of which needs to be synchronized, and some metro doors
        LevelSync("CheckPointsUndisabler", new(0f, -50f, 350f));
        LevelSync("DelayedActivator", new(-15f, 36f, 698f));
        LevelSync("DelayedActivator", new(-15f, 38f, 778f));

        Level = "Level 5-2";
        LevelFind("Panel", new(960f, 540f, 0f), obj =>
        {
            var uwu = obj.GetComponent<ImageFadeIn>();
            if (uwu == null) return;

            uwu.onFull = new();
            uwu.onFull.AddListener(() =>
            {
                Tools.Destroy(obj);
                Tools.Destroy(Tools.ObjFind("Jakito Huge"));

                var sea = Tools.ObjFind("Sea").transform;
                sea.Find("SeaAmbiance").gameObject.SetActive(true);
                sea.Find("SeaAmbiance (Waves)").gameObject.SetActive(true);

                HudMessageReceiver.Instance?.SendHudMessage("<size=48>Haha</size>", silent: true);
            });
        });

        LevelDestroy("SkullBlue", new(-3.700458f, -1.589029f, 950.6616f));
        LevelDestroy("Arena 1", new(87.5f, -53f, 1240f));
        LevelDestroy("Arena 2", new(87.5f, -53f, 1240f));

        LevelSync("Trigger 1", new(103.4f, 2.61f, 914.7f));
        LevelSync("Trigger 2", new(103.8f, -7.8f, 930.1f));

        LevelSync("FightActivator", new(-77.7f, 52.5f, 1238.9f)); // boss

        Level = "Level 5-3";
        LevelFind("Cube", new(-64.5f, 17.4f, 390.5f), obj =>
        {
            if (obj.TryGetComponent(out ItemPlaceZone zone)) zone.activateOnSuccess = new[] { zone.activateOnSuccess[1] };
        });
        LevelFind("Cube", new(-64.5f, 17.4f, 398.5f), obj =>
        {
            if (obj.TryGetComponent(out ItemPlaceZone zone)) zone.activateOnSuccess = new[] { zone.activateOnSuccess[1] };
        });

        Level = "Level 5-4";
        LevelSync("Activator", new(641.2f, 690f, 521.7f), obj => // boss
        {
            obj.gameObject.scene.GetRootGameObjects().Do(o =>
            {
                if (o.name == "Underwater") o.SetActive(false);
                if (o.name == "Surface") o.SetActive(true);
            });
            Teleporter.Teleport(new(641.25f, 691.5f, 522f));
        });

        Level = "Level 5-S";
        #endregion
        #region Heresy
        Level = "Level 6-1";
        LevelFind("Trigger", new(0f, -10f, 590.5f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[0] = null);
        LevelDestroy("Cube (5)", new(-40f, -10f, 548.5f));

        LevelFind("Door", new(168.5f, -36.62495f, 457f), obj => obj.GetComponent<Door>().closedPos = new(0f, 13.3751f, -15f));
        LevelDestroy("Cage", new(168.5f, -130f, 140f));
        LevelDestroy("Cube", new(102f, -165f, -503f));

        LevelSync("EnemyActivatorTrigger", new(168.5f, -125f, -438f));

        Level = "Level 6-2";
        LevelDestroy("Door", new(-179.5f, 20f, 350f));

        LevelSync("Trigger", new(-290f, 40f, 350f)); // boss

        #endregion
        #region Violence
        Level = "Level 7-1";
        // secret boss
        LevelDestroy("ViolenceArenaDoor", new(-120f, 0f, 530.5f));
        LevelSync("Trigger", new(-120f, 5f, 591f));

        // garden
        LevelFind("Cube", new(0f, 3.4f, 582.5f), obj => obj.transform.position = new(0f, 7.4f, 582.5f));
        LevelFind("MannequinAltar", new(-20f, 5f, 485f), obj =>
        {
            if (!LobbyController.IsOwner) obj.GetComponentInChildren<ItemPlaceZone>().deactivateOnSuccess = new GameObject[0];
        });
        LevelDestroy("Cube", new(-66.25f, 9.9f, 485f));

        LevelDestroy("ViolenceArenaDoor", new(0f, 12.5f, 589.5f));
        LevelDestroy("Walkway Arena -> Stairway Up", new(80f, -25f, 590f));

        LevelSync("Closer", new(0f, 20f, 579f), obj =>
        {
            if (NewMovement.Instance.transform.position.z < 470f) Teleporter.Teleport(new(0f, 5.5f, 485f));
        });
        LevelSyncLimbo(new(96.75f, 26f, 545f));
        LevelSync("RedSkullPickedUp", new(-23.4f, -4.7f, 581.6f));

        // tunnel
        LevelPatch("Wave 2", new(-242.5f, 0f, 0f));

        LevelSyncButton("Forward Button", new(-242.5f, -112.675f, 310.2799f), obj =>
        {
            obj.parent.parent.parent.parent.parent.parent.parent.parent.gameObject.SetActive(true);
            Teleporter.Teleport(new(-242.5f, -112.5f, 314f));
        });
        LevelSync("Wave 2", new(-242.5f, 0f, 0f));
        LevelSync("Wave 3", new(-242.5f, 0f, 0f));
        LevelSync("PlayerTeleportActivator", new(-242.5f, 0f, 0f));

        // outro
        LevelFind("FightStart", new(-242.5f, 120f, -399.75f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[2] = null);
        LevelSync("FightStart", new(-242.5f, 120f, -399.75f)); // boss

        Level = "Level 7-2";
        LevelFind("Intro -> Outdoors", new(-115f, 55f, 419.5f), obj =>
        {
            var door = obj.GetComponent<Door>();
            door?.onFullyOpened.AddListener(() =>
            {
                door.onFullyOpened = null;
                HudMessageReceiver.Instance?.SendHudMessage("<size=48>What?</size>", silent: true);
            });
        });
        LevelFind("9A", new(-23.5f, 37.75f, 806.25f), obj =>
        {
            // welactions aren't perfect
            if (obj.transform.parent.name == "9 Nonstuff") return;

            // open all of the doors
            for (int i = 1; i < obj.transform.childCount; i++) Tools.Destroy(obj.transform.GetChild(i).gameObject);

            // disable the Gate Control Terminal™
            Fill(string.Format(BASEMENT_TERMILA_TEXT, Tools.AccId), 64, TextAnchor.UpperLeft, obj.transform.Find("PuzzleScreen/Canvas"));
        });
        LevelFind("PuzzleScreen (1)", new(-230.5f, 31.75f, 813.5f), obj => Fill("UwU", 256, TextAnchor.MiddleCenter, obj.transform.Find("Canvas")));

        LevelFind("Trigger", new(-218.5f, 65f, 836.5f), obj => Tools.Destroy(obj.GetComponent<ObjectActivator>()));
        LevelFind("BayDoor", new(-305.75f, 49.75f, 600.5f), obj =>
        {
            ObjectActivator trigger;
            obj.GetComponent<Door>().activatedRooms = new[] { (trigger = Tools.Create<ObjectActivator>("Trigger", obj.transform)).gameObject };

            trigger.gameObject.SetActive(false);
            trigger.reactivateOnEnable = true;

            trigger.events = new() { onActivate = new() };
            trigger.events.onActivate.AddListener(() =>
            {
                var root = obj.transform.parent.Find("UsableScreen (1)/PuzzleScreen (1)/Canvas/UsableButtons/");
                root.Find("Button (Closed)").gameObject.SetActive(false);
                root.Find("Button (Open)").gameObject.SetActive(true);
            });
            trigger.events.toDisActivateObjects = new[] { trigger.gameObject };
        });

        LevelDestroy("15 Activator (Station)", new(46.5001f, 24.5f, 701.25f));

        // enable the track points at the level
        LevelEnable("0 - Door 1", new(46.5f, 26.75f, 753.75f));
        LevelEnable("1.25 - Door 2", new(46.5f, 26.75f, 788.75f));
        LevelEnable("2.25 - Door 3", new(46.5f, 26.75f, 823.75f));
        LevelEnable("3.5 - Door 4", new(46.5f, 26.75f, 858.75f));

        LevelSync("Trigger", new(-115f, 50f, 348.5f)); // boss
        LevelSync("TowerDestruction", new(-119.75f, 34f, 552.25f));

        // library
        LevelFind("Enemies", new(88.5f, 5.75f, 701.25f), obj =>
        {
            if (!LobbyController.IsOwner) obj.GetComponents<MonoBehaviour>().Do(Tools.Destroy);
        });
        LevelSync("Arena Start", new(133.5f, 45.75f, 701.25f));

        Level = "Level 7-3";
        // why is there a torch???
        LevelFind("1 - Dark Path", new(0f, -10f, 300f), obj => Tools.Destroy(obj.transform.Find("Altar (Torch) Variant/Cube").gameObject));

        LevelFind("Door 1", new(-55.5f, -2.5f, 618.5f), obj => obj.GetComponent<Door>().Unlock());
        LevelFind("Door 2", new(-75.5f, -12.5f, 568.5f), obj => obj.GetComponent<Door>().Unlock());
        LevelFind("Door 1", new(-75.5f, -12.5f, 578.5f), obj => obj.GetComponent<Door>().Unlock());
        LevelFind("12 - Grand Hall", new(-212.5f, -35f, 483.75f), obj =>
        {
            // teleport players to the final room once the door is opened
            obj.GetComponent<ObjectActivator>().events.onActivate.AddListener(() => Teleporter.Teleport(new(-189f, -33.5f, 483.75f)));
        });
        LevelFind("ViolenceHallDoor", new(-148f, 7.5f, 276.25f), obj => Tools.Destroy(obj.GetComponent<Collider>()));

        LevelDestroy("Door 2", new(-95.5f, 7.5f, 298.75f));
        LevelDestroy("ViolenceHallDoor (1)", new(-188f, 7.5f, 316.25f));

        LevelSync("Trigger", new(-145.5f, 5f, 483.75f), obj => Teleporter.Teleport(new(-131f, -14.5f, 483.75f)));
        LevelSync("Opener", new(-170.5f, 0.5f, 480.75f));
        LevelSync("Opener", new(-170.5f, 0.5f, 490.75f), obj => Tools.ObjFind("Outdoors Areas/6 - Interior Garden/NightSkyActivator").SetActive(true));
        LevelSync("BigDoorOpener", new(-145.5f, -10f, 483.75f), obj => obj.transform.parent.gameObject.SetActive(true));

        Level = "Level 7-4";
        // security system fight
        LevelFind("Trigger", new(0f, 495.25f, 713.25f), obj => obj.GetComponent<ObjectActivator>()?.events.onActivate.AddListener(() =>
        {
            var b = obj.transform.parent.GetComponentInChildren<CombinedBossBar>(true);
            for (int i = 0; i < b.enemies.Length; i++)
                (World.SecuritySystem[i] = b.enemies[i].gameObject.AddComponent<SecuritySystem>()).Type = EntityType.SecuritySystemOffset + i;
        }));
        LevelSync("Trigger", new(0f, 495.25f, 713.25f), obj => Teleporter.Teleport(new(0f, 472f, 745f), false));
        LevelSync("ShieldDeactivator", new(0f, 477.5f, 724.25f));
        LevelSync("DeathSequence", new(-2.5f, 472.5f, 724.25f));
        LevelSyncButton("Button", new(0f, 476.5f, 717.15f));

        // insides
        LevelFind("BrainFightTrigger", new(6.999941f, 841.5f, 610.7503f), obj => obj.GetComponent<ObjectActivator>()?.events.onActivate.AddListener(() =>
        {
            obj.transform.parent.GetComponentsInChildren<DestroyOnCheckpointRestart>(true).Do(Tools.Destroy);
            if (World.Brain) World.Brain.IsFightActive = true;
        }));
        LevelSync("EntryTrigger", new(0f, 458.5f, 649.75f), obj => Teleporter.Teleport(new(0f, 460f, 650f)));
        LevelSync("Deactivator", new(0.75f, 550.5f, 622.75f));
        LevelSync("BrainFightTrigger", new(6.999941f, 841.5f, 610.7503f), obj => Teleporter.Teleport(new(0f, 826.5f, 610f)));
        LevelSync("DelayedIdolSpawner", new(14.49993f, 914.25f, 639.7503f), obj =>
        {
            obj.transform.parent.gameObject.SetActive(true);
            obj.transform.parent.parent.gameObject.SetActive(true);
        });

        Level = "Level 7-S";
        #endregion
        #region Encore
        Level = "Level 0-E";
        Level = "Level 1-E";
        #endregion
        #region Prime
        Level = "Level P-1";
        Level = "Level P-2";
        #endregion
        #region Misc
        Level = "Endless";
        LevelFind("Cube", new(-40f, 0.5f, 102.5f), obj => obj.transform.position = new(-40f, -10f, 102.5f));
        #endregion

        // Do something relating to angry and envy later
    }

    // StaticAction functions
    private static void LevelDestroy(string name, Vector3 position) =>
        StaticAction.Destroy(Level, name, position);

    private static void LevelEnable(string name, Vector3 position) =>
        StaticAction.Enable(Level, name, position);

    private static void LevelFind(string name, Vector3 position, Action<GameObject> action) =>
        StaticAction.Find(Level, name, position, action);

    private static void LevelPatch(string name, Vector3 position) =>
        StaticAction.Patch(Level, name, position);

    private static void LevelPlaceTorches(Vector3 position, float radius) =>
        StaticAction.PlaceTorches(Level, position, radius);

    // NetAction functions
    private static void LevelSync(string name, Vector3 position, Action<Transform> action = null) =>
        NetAction.Sync(Level, name, position, action);

    private static void LevelSyncButton(string name, Vector3 position, Action<Transform> action = null) =>
        NetAction.SyncButton(Level, name, position, action);

    private static void LevelSyncLimbo(Vector3 position) =>
        NetAction.SyncLimbo(Level, position);

    // For 7-2
    private static void Fill(string text, int size, TextAnchor align, Transform canvas)
    {
        for (int i = 3; i < canvas.childCount; i++) Tools.Destroy(canvas.GetChild(i).gameObject);
        UIB.Text(text, canvas, Size(964f, 964f), align: align).transform.localScale /= 8f;
    }
}