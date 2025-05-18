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
        // IDK what to put here
        // Doesn't look related to chat so ignore :3
        Manager.Receive(256);
        Manager.Connection.Flush();
        Pointers.Reset();
    }

    public override void Close() => Manager?.Close();

    public void Connect(SteamId id)
    {
        Manager = SteamNetworkingSockets.ConnectRelay<ConnectionManager>(id, 4242);
        Manager.Interface = this;
    }

    public void OnConnecting(ConnectionInfo info)
    {
        Log.Info("Player is Connecting");
        // Checks if the player is already or banned (connection.close())
        // Sets the connection ID to the Account ID
        // Checks if steam user and uses connection.accept() if one
        // If not, then use connection.close()
    }

    public void OnConnected(ConnectionInfo info)
    {
        Log.Info("Player Connecting");
        // Not sure what jaket does but it looks like it sends level info
    }

    public void OnDisconnected(ConnectionInfo info)
    {
        Log.Info("Player Disconnected");
        // Kills the player entity from the server
    }

    public void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        Handle(Manager.Connection, LobbyController.LastOwner.AccountId, data, size);
    }
}
