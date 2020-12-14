﻿using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureChangedOutfitEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureChangedOutfitEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ICreature creature, IOutfit outfit)
        {
            foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }
                connection.OutgoingPackets.Enqueue(new CreatureOutfitPacket(creature));
                connection.Send();
            }
        }
    }
}
