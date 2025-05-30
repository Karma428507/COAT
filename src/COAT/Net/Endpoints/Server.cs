namespace COAT.Net.Endpoints;

using Steamworks;
using Steamworks.Data;
using System;

using COAT.Content;
using COAT.IO;
using COAT.Net.Types;
//using COAT.Sprays;
using COAT.UI.Overlays;
using COAT.World;
using System.Net;
using Unity.Audio;
using GameConsole.Commands;

// Handler for the host of a server
public class Server : Endpoint, ISocketManager
{
    public SocketManager Manager { get; protected set; }

    public override void Load()
    {
        // Loads all of the listener functions
        Listen(PacketType.Snapshot, (con, sender, r) =>
        {
            var id = r.Id();
            var type = r.Enum<EntityType>();

            // player can only have one doll and its id should match the player's id
            if ((id == sender && type != EntityType.Player) || (id != sender && type == EntityType.Player)) return;

            if (!ents.ContainsKey(id) || ents[id] == null)
            {
                // double-check on cheats just in case of any custom multiplayer clients existence
                if (!LobbyController.CheatsAllowed && (type.IsEnemy() || type.IsItem())) return;

                // client cannot create special enemies
                if (type.IsEnemy() && !type.IsCommonEnemy()) return;

                Administration.Handle(sender, ents[id] = Entities.Get(id, type));
            }
            ents[id]?.Read(r);
        });
        Listen(PacketType.SpawnBullet, (con, sender, r) =>
        {
            var type = r.Byte(); r.Position = 1; // extract the bullet type
            int cost = type >= 18 && type <= 20 ? 6 : 1; // rail costs more than the rest of the bullets

            if (type == 23 || Administration.CanSpawnBullet(sender, cost))
            {
                Bullets.CInstantiate(r);
                Redirect(r, con);
            }
        });
        Listen(PacketType.DamageEntity, r =>
        {
            if (ents.TryGetValue(r.Id(), out var entity)) entity?.Damage(r);
        });
        Listen(PacketType.KillEntity, (con, sender, r) =>
        {
            if (ents.TryGetValue(r.Id(), out var entity) && entity && (entity is Enemy || entity is Bullet || entity is TeamCoin))
            {
                entity.Kill(r);
                Redirect(r, con);
            }
        });
        ListenAndRedirect(PacketType.Punch, r =>
        {
            if (ents[r.Id()] is RemotePlayer player) player.Punch(r);
        });
        ListenAndRedirect(PacketType.Point, r =>
        {
            if (ents[r.Id()] is RemotePlayer player) player.Point(r);
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
    }

    public override void Update()
    {
        Stats.MeasureTime(ref Stats.ReadTime, () => Manager.Receive(512));
        Stats.MeasureTime(ref Stats.WriteTime, () =>
        {
            if (Networking.Loading) return;
            Networking.EachEntity(entity => Networking.Send(PacketType.Snapshot, w =>
            {
                w.Id(entity.Id);
                w.Enum(entity.Type);
                entity.Write(w);
            }));
        });

        foreach (var con in Manager.Connected) con.Flush();
        Pointers.Reset();
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
        bool AlreadyInLobby = false;
        foreach (var con in Networking.Server.Manager?.Connected) if (con == connection) AlreadyInLobby = true;
        if (Administration.Banned.Contains(connection.Id) || AlreadyInLobby) connection.Close();
        // Sets the connection ID to the Account ID
        // Checks if steam user and uses connection.accept() if one
        // If not, then use connection.close()

        Log.Info("[Server] Someone is connecting...");
        var identity = info.Identity;
        var accId = identity.SteamId.AccountId;

        // multiple connections are prohibited
        if (identity.IsSteamId && Networking.FindCon(accId).HasValue)
        {
            Log.Debug("[Server] Connection is rejected: already connected");
            connection.Close();
            return;
        }

        // check if the player is banned
        if (identity.IsSteamId && Administration.Banned.Contains(accId))
        {
            Log.Debug("[Server] Connection is rejected: banned");
            connection.Close();
            return;
        }

        // this will be used later to find the connection by its id
        connection.ConnectionName = accId.ToString();

        // only steam users in the lobby can connect to the server
        if (identity.IsSteamId && LobbyController.Contains(accId))
            connection.Accept();
        else
        {
            Log.Debug("[Server] Connection rejected: either a non-steam user or not in the lobby");
            connection.Close();
        }
    }

    public void OnConnected(Connection connection, ConnectionInfo info)
    {
        Log.Info("Player Connecting");
        Networking.Send(PacketType.Level, World.WriteData, (data, size) => Tools.Send(connection, data, size), size: 256);
    }

    public void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        Log.Info("Player Disconnected");
        // Kills the player entity from the server
    }

    public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        //var accId = ;
        // Packet handler
        // No listeners are needed to get chat running, only this
        Handle(connection, identity.SteamId.AccountId, data, size);
    }
}
