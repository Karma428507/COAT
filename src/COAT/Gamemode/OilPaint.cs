namespace COAT.Gamemode;

using COAT;
using System.Collections.Generic;

// WIP DEBUG GAMEMODE, MAY OR MAY NOT HAPPEN BEFORE RELEASE
/// <summary> To have different colored non flamable oil for drawing. </summary>
internal class OilPaint : Gamemode
{
    public override string Name => "Oil Paint";

    public override void Initialize()
    {
        AddSetting("flamable", false);

        Log.Debug("DEBUG GAMEMODE");
    }
}
