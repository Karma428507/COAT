namespace COAT.Net.Types.Players;

using COAT.Content;
using COAT.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

public class RemotePlayer : Entity
{
    // Variables for coords are needed

    public Team Team;



    public override void Read(Reader r)
    {
        // Reads info about the player related to this
    }

    public override void Write(Writer w)
    {
        // Idk why there's a need to write
    }
}
