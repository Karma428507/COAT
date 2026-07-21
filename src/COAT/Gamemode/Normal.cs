namespace COAT.Gamemode;

using COAT;

/// <summary> Base multiplayer. </summary>
internal class Normal : Gamemode
{
    public override string Name => "Normal";

    public override void Initialize()
    {
        AddSetting("pvp", false);
        AddSetting("heal", false);

        Log.Debug("Normal multiplayer campaign.");
    }
}
