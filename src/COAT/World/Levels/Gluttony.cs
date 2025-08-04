namespace COAT.World.Levels;

using COAT.UI.Fragments;

public class Level31 : LevelModule
{
    public override string Level => "Level 3-1";

    public override void Load()
    {
        LevelSync("Trigger", new(-203f, -72.5f, 563f)); // lightning
        LevelSync("Deactivator", new(-203f, -72.5f, 528f));
        LevelSync("End Lights", new(-203f, -72.5f, 528f));
    }
}

public class Level32 : LevelModule
{
    public override string Level => "Level 3-2";

    public override void Load()
    {
        LevelDestroy("Door", new(-10f, -161f, 955f));

        LevelSync("Cube", new(-5f, -121f, 965f), obj => Teleporter.Teleport(new(-5f, -159.5f, 970f)));
    }
}