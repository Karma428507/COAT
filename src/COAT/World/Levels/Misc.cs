namespace COAT.World.Levels;

public class LevelP1 : LevelModule
{
    public override string Level => "Level p-1";

    public override void Load()
    {
        
    }
}

public class LevelP2 : LevelModule
{
    public override string Level => "Level P-2";

    public override void Load()
    {

    }
}

public class LevelEndless : LevelModule
{
    public override string Level => "Endless";

    public override void Load()
    {
        LevelFind("Cube", new(-40f, 0.5f, 102.5f), obj => obj.transform.position = new(-40f, -10f, 102.5f));
    }
}