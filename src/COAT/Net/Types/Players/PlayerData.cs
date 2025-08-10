namespace COAT.Net.Types;

using COAT.Content;
using COAT.IO;
using COAT.Net;
using Steamworks.Data;
using System.Collections.Generic;

/* Packet structure
 *
 *  1  B    | Packet type
 *  1  B    | Color R
 *  1  B    | Color G
 *  1  B    | Color B
 *  20 B    | Reserved
 *
 */
public class PlayerData
{
    public static Dictionary<uint, PlayerData> PlayerList = new Dictionary<uint, PlayerData>();
    public static PlayerData LocalData;

    // Data related to COAT players
    public UnityEngine.Color Color;

    /// <summary> Mass sends there info and a formal request letter. </summary>
    public static void OnEnter()
    {
        LocalData = new PlayerData();
        LocalData.Color = Networking.LocalPlayer.Team.Color();

        Networking.Send(PacketType.COAT_PlayerPacketSend, Write, size: 23);
        Networking.Send(PacketType.COAT_PlayerPacketRequest, w => w.Id(Tools.AccId), size: 4);
    }

    /// <summary> Writes the data to send. </summary>
    public static void Write(Writer w)
    {
        w.Byte((byte)(LocalData.Color.r * 255));
        w.Byte((byte)(LocalData.Color.g * 255));
        w.Byte((byte)(LocalData.Color.b * 255));
        w.Int(0);
    }

    /// <summary> Reads the data and adds it to the list. </summary>
    public static void Read(Connection con, uint sender, Reader r)
    {
        PlayerData data = new PlayerData();
        data.Color = new UnityEngine.Color(r.Byte() / 255, r.Byte() / 255, r.Byte() / 255);
        r.Int();

        PlayerList.Add(con, data);
    }

    /// <summary> Sends the packet after being asked. </summary>
    public static void WriteRequest(Reader r) =>
        Writer.Write(w => { w.Enum(PacketType.COAT_PlayerPacketSend); Write(w); },
            (data, size) => Tools.Send(r.Id(), data, size), 24);
}