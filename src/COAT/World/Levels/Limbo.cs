namespace COAT.World.Levels;

public class Level12 : LevelModule
{
    public override string Level => "Level 1-2";

    public override void Load()
    {
        // secret statue
        LevelSync("Cube", new(0f, -19f, 442f));
        LevelSync("Cube", new(15f, -15f, 417f));

        // Very Cancerous Rodent™
        LevelSync("Cube", new(-61f, -16.5f, 388.5f));
        LevelSync("Cube (1)", new(-61f, -21.5f, 400.5f));
    }
}

public class Level13 : LevelModule
{
    public override string Level => "Level 1-3";

    public override void Load()
    {
        LevelFind("Trigger", new(0f, 9.5f, 412f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[0] = null); // corridor
    }
}

public class Level14 : LevelModule
{
    public override string Level => "Level 1-4";

    public override void Load()
    {
        LevelFind("Cube", new(0f, 11f, 612f), obj => Tools.Destroy(obj.GetComponent<DoorController>()));

        LevelSync("Cube", new(0f, -19f, 612f)); // boss
    }
}