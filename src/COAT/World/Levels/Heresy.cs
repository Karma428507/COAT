namespace COAT.World.Levels;

public class Level61 : LevelModule
{
    public override string Level => "Level 6-1";

    public override void Load()
    {
        LevelFind("Trigger", new(0f, -10f, 590.5f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[0] = null);
        LevelDestroy("Cube (5)", new(-40f, -10f, 548.5f));

        LevelFind("Door", new(168.5f, -36.62495f, 457f), obj => obj.GetComponent<Door>().closedPos = new(0f, 13.3751f, -15f));
        LevelDestroy("Cage", new(168.5f, -130f, 140f));
        LevelDestroy("Cube", new(102f, -165f, -503f));

        LevelSync("EnemyActivatorTrigger", new(168.5f, -125f, -438f));
    }
}

public class Level62 : LevelModule
{
    public override string Level => "Level 6-2";

    public override void Load()
    {
        LevelDestroy("Door", new(-179.5f, 20f, 350f));

        LevelSync("Trigger", new(-290f, 40f, 350f)); // boss
    }
}