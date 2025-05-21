namespace COAT.Net.Endpoints;

using Steamworks;
using Steamworks.Data;
using System;

using COAT.Content;
using COAT.IO;
using COAT.Net.Types;
//using COAT.Sprays;
using COAT.World;
using System.Net;
using COAT.UI.Overlays;
using System.Net.Mail;

// Handler for players joining the server
public class Client : Endpoint, IConnectionManager
{
    public ConnectionManager Manager { get; protected set; }

    public override void Load()
    {
        // Loads all of the listener functions
        Listen(PacketType.Level, World.ReadData);
        Listen(PacketType.Ban, (con, sender, r) =>
        {
            if (sender != LobbyController.LastOwner.AccountId)
            Chat.StaticReceive("you were banned...");
            LobbyController.LeaveLobby();
        });

        // PUT ALL COAT PACKETS BELOW THIS. JUST SO I DONT HAVE TO SEARCH THE MILKYWAY TO FIND A SINGLE FUCKING LIL GUY!!!
        // write down coat client id, then send urs
        Listen(PacketType.COAT_Request, r =>
        {
            Networking.COATPLAYERS.Clear(); // clear list so then u can update it
            Networking.COATPLAYERS.Add(Tools.AccId); // add yourself to the list
            Networking.COATPLAYERS.Add(r.Id()); // add the person requesting to the list
            Chat.StaticReceive($"\\nReceived \"COAT_Request\"\\nCleared the \"COATPLAYERS\" list, Added {Tools.Name(Tools.AccId)} to the \"COATPLAYERS\" list, Sent \"COAT_ClientId\".");
            Networking.Send(PacketType.COAT_ClientId, w => { w.Id(Tools.AccId); }); // send out ur id for others to write down
        });

        // write down coat client id.
        Listen(PacketType.COAT_ClientId, r => { Chat.StaticReceive($"Received \"COAT_ClientId\", Added {Tools.Name(r.Id())} to the \"COATPLAYERS\" list."); Networking.COATPLAYERS.Add(r.Id()); });

        Listen(PacketType.COAT_Kick, r => 
        {
            Chat.StaticReceive("you were kicked...");
            LobbyController.LeaveLobby();
        });

        Listen(PacketType.COAT_Mute, r =>
        {
            if (r.Bool()) Networking.MUTEDPLAYERS.Add(r.Id());
            else Networking.MUTEDPLAYERS.Remove(r.Id());
        });
    }

    public override void Update()
    {
        Stats.MeasureTime(ref Stats.ReadTime, () => Manager.Receive(256));

        Manager.Connection.Flush();
        Pointers.Reset();
    }

    public override void Close() => Manager?.Close();

    public void Connect(SteamId id)
    {
        Manager = SteamNetworkingSockets.ConnectRelay<ConnectionManager>(id, 4242);
        Manager.Interface = this;
    }

    public void OnConnecting(ConnectionInfo info) => Log.Info("Player is Connecting");

    public void OnConnected(ConnectionInfo info) => Log.Info("Player Connecting");

    public void OnDisconnected(ConnectionInfo info) => Log.Info("Player Disconnected");

    public void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel) => Handle(Manager.Connection, LobbyController.LastOwner.AccountId, data, size);
}
