namespace COAT.Content;

/// <summary> Sub-packets relating to world actions. </summary>
public enum SyncType
{
    /// <summary> Dealing with actions within the level (not really sure how it works). </summary>
    NetAction = 0,

    /// <summary> Tells what door should be unlocked. </summary>
    DoorUnlock,
    /// <summary> Unlocks the last door. </summary>
    FinalDoorUnlock,

    /// <summary> Breaks an object. </summary>
    BreakObject,
    /// <summary> Burns an object. </summary>
    BurnObject,

    /// <summary> Activates a cerberus. </summary>
    ActivateStatue,

    // To be added later
    FillTree,
    Tram,

    /// <summary> These are for the literal hooks within ultrakill, not a name for networking nonsense (totally didn't spend an hour trying to figure that out). </summary>
    HookPoint,
}
