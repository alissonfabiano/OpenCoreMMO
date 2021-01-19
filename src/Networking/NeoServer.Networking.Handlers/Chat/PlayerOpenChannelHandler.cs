﻿using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerOpenChannelHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerOpenChannelHandler(Game game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var channel = new OpenPrivateChannelPacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player)) return;


            //connection.OutgoingPackets.Enqueue(new PlayerOpenPrivateChannelPacket(receiver.Name));
            //connection.Send();
        }
    }
}
