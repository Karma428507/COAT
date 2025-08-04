namespace COAT.World.Levels;

using COAT.UI.Fragments;
using HarmonyLib;

public class Level51 : LevelModule
{
    public override string Level => "Level 5-1";

    public override void Load()
    {
        LevelFind("1 - Main Cave", new(0f, -50f, 350f), obj => obj.GetComponent<ObjectActivator>().events.toDisActivateObjects[0] = null);
        LevelDestroy("HudMessage", new(0f, -100f, 295.5f));
        LevelDestroy("Door", new(218.5f, -41f, 234.5f));

        // there is a checkpoint deactivator, the deactivation of which needs to be synchronized, and some metro doors
        LevelSync("CheckPointsUndisabler", new(0f, -50f, 350f));
        LevelSync("DelayedActivator", new(-15f, 36f, 698f));
        LevelSync("DelayedActivator", new(-15f, 38f, 778f));
    }
}

public class Level52 : LevelModule
{
    public override string Level => "Level 5-2";

    public override void Load()
    {
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
    }
}

public class Level53 : LevelModule
{
    public override string Level => "Level 5-3";

    public override void Load()
    {
        // there are altars that activate skulls in the mirror part of the levebut the client has these skulls destroyed
        LevelFind("Cube", new(-64.5f, 17.4f, 390.5f), obj =>
        {
            if (obj.TryGetComponent(out ItemPlaceZone zone)) zone.activateOnSuccess = new[] { zone.activateOnSuccess[1] };
        });
        LevelFind("Cube", new(-64.5f, 17.4f, 398.5f), obj =>
        {
            if (obj.TryGetComponent(out ItemPlaceZone zone)) zone.activateOnSuccess = new[] { zone.activateOnSuccess[1] };
        });
    }
}

public class Level54 : LevelModule
{
    public override string Level => "Level 5-4";

    public override void Load()
    {
        LevelSync("Activator", new(641.2f, 690f, 521.7f), obj => // boss
        {
            obj.gameObject.scene.GetRootGameObjects().Do(o =>
            {
                if (o.name == "Underwater") o.SetActive(false);
                if (o.name == "Surface") o.SetActive(true);
            });
            Teleporter.Teleport(new(641.25f, 691.5f, 522f));
        });
    }
}