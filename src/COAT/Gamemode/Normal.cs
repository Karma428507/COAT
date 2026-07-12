namespace COAT.Gamemode;

using COAT;

/// <summary> Base multiplayer. </summary>
internal class Normal : Gamemode
{
    public override string Name => "Normal";

    public override void Initialize()
    {
        Log.Debug("Normal multiplayer campaign.");
    }
}
