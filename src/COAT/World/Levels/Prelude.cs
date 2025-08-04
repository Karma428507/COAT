namespace COAT.World.Levels;

using COAT.UI;
using UnityEngine;

public class Level01 : LevelModule
{
    public override string Level => "Level 0-1";

    public override void Load()
    {
        LevelFind("Cube (2)", new(202f, 73f, 421f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects = new GameObject[0]);
        LevelSync("Cube (2)", new(202f, 73f, 421f)); // boss
    }
}

public class Level02 : LevelModule
{
    public override string Level => "Level 0-2";

    public override void Load()
    {
        LevelDestroy("Invisible Wall", new(0f, -7f, 163.5f));
        LevelDestroy("Invisible Wall", new(-45f, -8f, 287.5f));
        LevelDestroy("SwordMachine (1)", new(13f, -10f, 173f));
        LevelDestroy("Activator", new(-44.5f, 0f, 157f));
        LevelDestroy("SwordsMachine", new(-45f, -11f, 268f));
        LevelDestroy("SwordsMachine", new(-55f, -11f, 293f));

        LevelSync("Activator", new(-81f, 9f, 339.5f), obj => obj.parent.parent.gameObject.SetActive(true)); // boss

    }
}

public class Level03 : LevelModule
{
    public override string Level => "Level 0-3";

    public override void Load()
    {
        LevelSync("Cube (1)", new(-89.5f, 9.3f, 413f)); // boss
    }
}

public class Level05 : LevelModule
{
    public override string Level => "Level 0-5";

    public override void Load()
    {
        LevelFind("Cube", new(182f, 4f, 382f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[0] = null); // corridor

        LevelSync("Cube", new(182f, 4f, 382f)); // boss
        LevelSync("DelayedDoorActivation", new(175f, -6f, 382f));
    }
}

public class Level0S : LevelModule
{
    public override string Level => "Level 0-S";

    public override void Load()
    {
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
    }
}