namespace COAT.Assets;

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary> Class that works with the assets of the game. </summary>
public class GameAssets
{
    /// <summary> List of items that mustn't be synchronized, because they are not items at all. </summary>
    public static readonly string[] ItemExceptions = new[]
    { "Minotaur", "Tram (3)", "BombTrigger", "BombStationTramTeleporterKey", "Checker" };

    /// <summary> List of internal names of all enemies. </summary>
    public static readonly string[] Enemies = new[]
    {
        "Zombie", "Projectile Zombie", "Super Projectile Zombie", "ShotgunHusk", "MinosBoss", "Stalker", "Sisyphus", "Ferryman",
        "SwordsMachineNonboss", "Drone", "Streetcleaner", "Mindflayer", "V2", "V2 Green Arm Variant", "Turret", "Gutterman",
        "Guttertank", "Spider", "StatueEnemy", "Mass", "Idol", "Mannequin", "Minotaur", "Virtue",
        "Gabriel", "Gabriel 2nd Variant", "Wicked", "Flesh Prison", "DroneFlesh", "Flesh Prison 2", "DroneFleshCamera Variant", "DroneSkull Variant",
        "MinosPrime", "SisyphusPrime", "Cancerous Rodent", "Very Cancerous Rodent", "Mandalore", "Big Johninator", "Puppet"
    };

    /// <summary> List of internal names of all items. </summary>
    public static readonly string[] Items = new[]
    { "Apple Bait", "Maurice Bait", "SkullBlue", "SkullRed", "Soap", "Torch", "Florp/Florp_fbx" };

    /// <summary> List of internal names of all dev plushies. </summary>
    public static readonly string[] Plushies = new[]
    {
        "Jacob", "Mako", "HEALTH - Jake", "Dalia", "Jericho", "Meganeko", "Imp", "FlyingDog", "Dawg", "Sam",
        "Cameron", "Gianni", "Salad", "Mandalore", "Joy", "Weyte", "Heck", "Hakita", "Lenval", "CabalCrow",
        "Quetzal", "HEALTH - John", "PITR", "HEALTH - BJ", "Francis", "Vvizard", "Lucas", "Scott", "KeygenChurch", "V1Plush"
    };

    /// <summary> List of readable names of all dev plushies needed for the /plushy command. </summary>
    public static readonly string[] PlushiesButReadable = new[]
    {
        "Jacob", "Maximilian", "Jake", "Dalia", "Jericho", "Meganeko", "Tucker", "BigRock", "Victoria", "Samuel",
        "Cameron", "Gianni", "Salad", "Mandalore", "Joy", "Weyte", "Heckteck", "Hakita", "Lenval", "CabalCrow",
        "Quetzal", "John", "Pitr", "BJ", "Francis", "Vvizard", "Lucas", "Scott", "KGC", "V1"
    };
    #region tools

    private static GameObject Prefab(string name) => AssetHelper.LoadPrefab($"Assets/ULTRAKILL Assets/{name}.prefab");

    //private static void Material(string name, Cons<Material> cons) => Addressables.LoadAssetAsync<Material>($"Assets/Models/{name}.mat").Task.ContinueWith(t => cons(t.Result));

    #endregion
    #region loading

    public static GameObject Enemy(string name) => Prefab($"Enemies/{name}");

    public static GameObject Item(string name) => Prefab($"Models/Objects/{name}");

    public static GameObject Bait(string name) => Prefab($"Fishing/{name}");

    public static GameObject Fish(string name) => Prefab($"Fishing/Fishes/{name}");

    public static GameObject Plushie(string name) => Prefab($"Credits Museum/Developers/{(name.StartsWith("!") ? name : $"{name}_fbx")}");

    /// <summary> Loads the torch prefab. </summary>
    public static GameObject Torch() => Prefab("Levels/Interactive/Altar (Torch) Variant");

    /// <summary> Loads the blast explosion prefab. </summary>
    public static GameObject Blast() => Prefab("Attacks and Projectiles/Explosions/Explosion Wave");

    /// <summary> Loads the harmless explosion prefab. </summary>
    public static GameObject Harmless() => Prefab("Attacks and Projectiles/Explosions/Explosion Harmless");

    /// <summary> Loads the shotgun pickup prefab. </summary>
    public static GameObject Shotgun() => Prefab("Weapons/Pickups/ShotgunPickUp");

    /// <summary> Loads the squeaky toy sound prefab. </summary>
    public static GameObject Squeaky() => AssetHelper.LoadPrefab("Assets/Particles/SoundBubbles/SqueakyToy.prefab");

    /// <summary> Loads the fish pickup prefab. </summary>
    public static GameObject FishTemplate() => Prefab("Fishing/Fish Pickup Template");

    /// <summary> Loads a swordsmachine material by name. </summary>
    //public static void SwordsMaterial(string name, Renderer output) => Material($"Enemies/SwordsMachine/{name}", mat => output.material = mat);

    /// <summary> Loads an insurrectionist material by name. </summary>
    //public static void SisyMaterial(string name, Renderer[] output) => Material($"Enemies/Sisyphus/{name}", mat => output[0].material = output[1].material = mat);

    /// <summary> Loads a Gabriel voice line by name. </summary>
    //public static void GabLine(string name, Cons<AudioClip> output) => Sound($"Voices/Gabriel/{name}.ogg", output);

    #endregion
    #region Debug
    public static List<GameObject> gameObjects = new List<GameObject>();

    /// <summary> A debug function to fill a list of asset addresses </summary>
    public static void LoadAddresses()
    {
        string path = Path.Combine(Path.GetDirectoryName(Plugin.Instance.Location), "logs", "Assets.txt");
        List<string> ToWrite = new();

        ToWrite.Add("Game Objects");

        Addressables.InitializeAsync().Completed += (operationHandle) =>
        {
            HashSet<string> primaryKeys = new HashSet<string>();

            var resourceLocators = Addressables.ResourceLocators;
            foreach (var locator in resourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    locator.Locate(key, typeof(GameObject), out var gameObjectLocations);
                    if (gameObjectLocations != null)
                        foreach (var location in gameObjectLocations)
                            if (location.ToString().Contains('/'))
                                ToWrite.Add(location.ToString());
                }
            }

            ToWrite.Add("Textures");

            foreach (var locator in resourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    locator.Locate(key, typeof(Texture), out var textureLocations);
                    if (textureLocations != null)
                        foreach (var location in textureLocations)
                            if (location.ToString().Contains('/'))
                                ToWrite.Add(location.ToString());
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.AppendAllLines(path, ToWrite);
        };
    }
    #endregion
}
