using COAT.Content;
using COAT.Net;

namespace COAT.World.Levels;

public class Level41 : LevelModule
{
    public override string Level => "Level 4-1";

    public override void Load()
    {
        LevelSyncLimbo(new(-290.25f, 24.5f, 814.75f));
    }
}

public class Level42 : LevelModule
{
    public override string Level => "Level 4-2";

    public override void Load()
    {
        LevelSyncLimbo(new(-150.75f, 33f, 953.1049f));

        LevelEnable("6A - Indoor Garden", new(-19f, 35f, 953.9481f));
        LevelEnable("6B - Outdoor Arena", new(35f, 35f, 954f));

        LevelDestroy("6A Activator", new(-79f, 45f, 954f));
        LevelDestroy("6B Activator", new(116f, 19.5f, 954f));

        LevelSync("DoorOpeners", new(-1.5f, -18f, 774.5f));
        LevelSync("DoorsOpener", new(40f, 5f, 813.5f));
    }
}

public class Level43 : LevelModule
{
    public override string Level => "Level 4-3";

    public override void Load()
    {
        LevelPlaceTorches(new(0f, -10f, 310f), 3f);
        LevelDestroy("Doorblocker", new(-59.5f, -35f, 676f));

        LevelSync("DoorActivator", new(2.5f, -40f, 628f));
        LevelSync("Trigger (Intro)", new(-104f, -20f, 676f)); // boss
        LevelSync("Secret Tablet", new(-116.4f, -39.5f, 675.9f), obj => MusicManager.Instance.StopMusic());
    }
}

public class Level44 : LevelModule
{
    public override string Level => "Level 4-4";

    public override void Load()
    {
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
    }
}