namespace COAT.Content;

using System;
using System.Collections.Generic;
using System.Text;

/// <summary> All entity types. Will replenish over time. </summary>
public enum EntityType
{
    // NOTE: CHANGING AROUND THE ORDER OF THESE WILL CAUSE DESYNC FROM JAKET AND COAT!!
    // IF YOU HAVE TO ADD A NEW ENTITY, ADD IT TO THE END OR ASK ADI TO ADD IT TO JAKET TOO!!
    None = -1,
    Player,

    // ====================
    // Enemies
    // ====================

    // Angels
    Gabriel,
    Virtue,
    Gabriel_Angry,
    Providence,
    Power,
    PowerWithEffect,

    // Demons
    MaliciousFace,
    Cerberus,
    CerberusStatue,
    CerberusStatueWhite,
    HideousMass,
    Idol,
    Leviathan,
    LeviathanTail,
    Mannequin,
    MannequinPose,
    Minotaur,
    Minotaur_Chase,
    DeathCatcher,
    DeathCatcherCase,
    DeathCatcherCaseEndless,
    DeathCatcherClosed,
    Geryon,
    
    // Husks
    Filth,
    Stray,
    Schism,
    Soldier,
    Hand,
    TheCorpseOfKingMinos,
    Stalker,
    Insurrectionist,
    Ferryman,
    MirrorReaper,
    MirrorReaperCyber,

    // Machines
    Swordsmachine,
    Swordsmachine_Boss,
    Swordsmachine_Tundra,
    Swordsmachine_Agony,
    Drone, // There's flesh, ghost and skull varients but idk what they do rn
    Streetcleaner,
    Mindflayer,
    V2,
    V2_GreenArm,
    Sentry,
    Gutterman,
    Guttertank,
    SecuritySystem_Main,
    SecuritySystem_RocketLauncher, SecuritySystem_RocketLauncher_,
    SecuritySystem_Mortar, SecuritySystem_Mortar_,
    SecuritySystem_Tower, SecuritySystem_Tower_,
    Brain,

    // Misc
    SomethingWicked,
    CancerousRodent,
    VeryCancerousRodent,
    FleshPrison,
    FleshPrison_Eye,
    FleshPanopticon_Eye,
    FleshPanopticon_Face,
    MinosPrime,
    Mandalore,
    FleshPanopticon,
    SisyphusPrime,
    Johninator,
    Puppet,



    // ====================
    // Items
    // ====================

    // General Items
    AppleBait,
    SkullBait,
    BlueSkull,
    RedSkull,
    Soap,
    Torch,
    Florp,

    // Plushies (Organize later)
    Jacob,
    Mako,
    Jake,
    Dalia,
    Jericho,
    Meganeko,
    Tucker,
    BigRock,
    Dawg,
    Sam,
    Cameron,
    Gianni,
    Salad,
    Mandy,
    Joy,
    Weyte,
    Heckteck,
    Hakita,
    Lenval,
    CabalCrow,
    Quetzal,
    John,
    PITR,
    BJ,
    Francis,
    Vvizard,
    Lucas,
    Scott,
    KGC,
    V1,

    /*CabalCrow,
    Cameron,
    Dalia,
    Dawg,
    FlyingDog,
    Francis,
    Gianni,
    Hakita,
    Hank,
    BJ,
    Jake,
    John,
    Heckteck,
    Imp,
    Jacob,
    Jericho,
    Joy,
    Keygen,
    KingGizzard,
    Lenval,
    Lucas,
    Mako,
    MandalorePlush,
    Meganeko,
    PITR,
    Quetzal,
    Salad,
    Sam,
    Scott,
    V1,
    Vvizard,
    Weyte,
    Zombie,*/
    
    // Bullets
    Coin,
    Rocket,
    Ball,

    SandboxEntity = 0x80,
    CustomEntity = 0xFF,

    EnemyOffset = Gabriel,
    SecuritySystemOffset = SecuritySystem_Main,
    ItemOffset = AppleBait,
    PlushyOffset = CabalCrow,
    BulletOffset = Coin
}

/// <summary> Extension class that allows you to get entity class. </summary>
public static class TypeExtensions
{
    /// <summary> Whether the type is an enemy. </summary>
    public static bool IsEnemy(this EntityType type) => type >= EntityType.EnemyOffset && type < EntityType.ItemOffset;

    /// <summary> Whether the type is a common enemy that can be spawned by the sandbox arm. </summary>
    public static bool IsCommonEnemy(this EntityType type) =>
        IsEnemy(type) && type < EntityType.Hand && type != EntityType.TheCorpseOfKingMinos && type != EntityType.SomethingWicked;

    /// <summary> Whether the type is a BIG enemy that can only be spawned in a limited number. </summary>
    public static bool IsBigEnemy(this EntityType type) => type >= EntityType.FleshPrison && type <= EntityType.SisyphusPrime;

    /// <summary> Whether the type is an enemy and can be shot by a coin. </summary>
    public static bool IsTargetable(this EntityType type) => IsEnemy(type) && type != EntityType.Idol && type != EntityType.CancerousRodent;

    /// <summary> Whether the type is an item. </summary>
    public static bool IsItem(this EntityType type) => type >= EntityType.ItemOffset && type < EntityType.PlushyOffset;

    /// <summary> Whether the type is a plushy. </summary>
    public static bool IsPlushy(this EntityType type) => type >= EntityType.PlushyOffset && type < EntityType.BulletOffset;

    /// <summary> Whether the type is a bullet. </summary>
    public static bool IsBullet(this EntityType type) => type >= EntityType.BulletOffset;
}