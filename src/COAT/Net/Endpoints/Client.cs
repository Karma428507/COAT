namespace COAT.Net.Endpoints;

using Steamworks;
using Steamworks.Data;
using System;

using COAT.Content;
//using COAT.IO;
//using COAT.Net.Types;
//using COAT.Sprays;
using COAT.World;
using System.Net;
using COAT.IO;
using COAT.UI.Overlays;

// Handler for players joining the server
public class Client : Endpoint, IConnectionManager
{
    public ConnectionManager Manager { get; protected set; }

    public override void Load()
    {
        // Loads all of the listener functions
        Listen(PacketType.Level, World.ReadData);
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

    public void OnDisconnected(ConnectionInfo info) =>Log.Info("Player Disconnected");

    public void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel) => Handle(Manager.Connection, LobbyController.LastOwner.AccountId, data, size);
}
