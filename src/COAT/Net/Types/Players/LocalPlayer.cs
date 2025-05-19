namespace COAT.Net.Types.Players;

using COAT.Content;
using COAT.IO;
using System;
using System.Collections.Generic;
using System.Text;

public class LocalPlayer : Entity
{

    public Team Team;

    // WARNING, MANY THINGS NEED TO BE COMPLETED BEFORE WORKING ON LOCAL/REMOTE
    // - Stats
    // - Improved memory manager
    // - Assets handling

    public override void Read(Reader r)
    {
        // No need to read because LocalPlayer is it's own thing
    }

    public override void Write(Writer w)
    {
        // Sends this player's information
    }
}
