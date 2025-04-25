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

// Handler for the host of a server
class Server : Endpoint, ISocketManager
{
    public SocketManager Manager { get; protected set; }

    public override void Load()
    {
        // Loads all of the listener functions
    }

    public override void Update()
    {
        // IDK what to put here
        // Doesn't look related to chat so ignore :3
    }

    public override void Close() => Manager?.Close();

    public void Open()
    {
        // Find out if 4242 is important or if I can put in any value
        // All I know is that 4242 is the virtual port value
        Manager = SteamNetworkingSockets.CreateRelaySocket<SocketManager>(4242);

        // This set the interface to this class which uses the On functions below
        Manager.Interface = this;
    }

    public void OnConnecting(Connection connection, ConnectionInfo info)
    {
        Log.Info("Player is Connecting");
        // Checks if the player is already or banned (connection.close())
        // Sets the connection ID to the Account ID
        // Checks if steam user and uses connection.accept() if one
        // If not, then use connection.close()
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        Log.Info("Player Connecting");
        // Not sure what jaket does but it looks like it sends level info
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Log.Info("Player Disconnected");
        // Kills the player entity from the server
    }

    public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        Log.Info("Message");
        // Chat system
        // No listeners are needed to get chat running, only this
    }
}
