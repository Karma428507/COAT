namespace COAT.World.Levels;

using COAT.Content;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI;
using COAT.UI.Fragments;
using HarmonyLib;
using UnityEngine;

using static COAT.UI.Rect;

public class Level71 : LevelModule
{
    public override string Level => "Level 7-1";

    public override void Load()
    {
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
    }
}

public class Level72 : LevelModule
{
    public override string Level => "Level 7-2";

    public const string BASEMENT_TERMILA_TEXT =
@"MACHINE ID: V3#{0}
LOCATION: TERMINAL
CURRENT OBJECTIVE: FUN

AUTHORIZATION... <color=#32CD32>DONE</color>

> Hello?
<color=#FF341C>UNKNOWN COMMAND</color>

> Please let me in :3
OPENING ALL DOORS... <color=#32CD32>DONE</color>";

    public override void Load()
    {
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
    }

    private void Fill(string text, int size, TextAnchor align, Transform canvas)
    {
        for (int i = 3; i < canvas.childCount; i++) Tools.Destroy(canvas.GetChild(i).gameObject);
        UIB.Text(text, canvas, Size(964f, 964f), align: align).transform.localScale /= 8f;
    }
}

public class Level73 : LevelModule
{
    public override string Level => "Level 7-3";

    public override void Load()
    {
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
    }
}

public class Level74 : LevelModule
{
    public override string Level => "Level 7-4";

    public override void Load()
    {
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
    }
}