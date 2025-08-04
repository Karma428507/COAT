namespace COAT.World.Levels;

using COAT.UI.Fragments;

public class Level24 : LevelModule
{
    public override string Level => "Level 2-4";

    public override void Load()
    {
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
    }
}