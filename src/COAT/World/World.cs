namespace COAT.World;

using COAT;
using COAT.Content;
using COAT.IO;
using COAT.Net;
using System;
using System.Collections.Generic;
using System.Text;

public class World
{
    public static void Load()
    {
        void LoadLevel() {
            if (LobbyController.Online && LobbyController.IsOwner && Tools.Pending != "Main Menu")
            {
                Networking.Send(PacketType.Level, WriteData, size: 256);
            }
        }

        Events.OnLoadingStarted += LoadLevel;
    }

    public static void WriteData(Writer w)
    {
        w.String(Tools.Pending ?? Tools.Scene);
        w.String(COAT.Version.CURRENT);
        w.Byte((byte)PrefsManager.Instance.GetInt("difficulty"));

        // other thing is a byte array of "activated" things
    }

    public static void ReadData(Reader r)
    {
        Tools.Load(r.String());

        // Check version later
        r.String();

        PrefsManager.Instance.SetInt("difficulty", r.Byte());
    }
}
