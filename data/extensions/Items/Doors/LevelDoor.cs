﻿using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Extensions.Items.Doors
{
    public class LevelDoor : Door
    {
        public LevelDoor(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes) :
            base(metadata, location, attributes)
        {
        }

        public override void Use(IPlayer player)
        {
            Metadata.Attributes.TryGetAttribute(ItemAttribute.LevelDoor, out _);

            Metadata.Attributes.TryGetAttribute(ItemAttribute.ActionId, out int actionId);

            if (player.Level < actionId - 1000)
            {
                OperationFailService.Display(player.CreatureId, "Only the worthy may pass.");
                return;
            }

            var directionTo = Location.DirectionTo(player.Location, true);

            if (!Metadata.Attributes.TryGetAttribute<string>("orientation", out var doorOrientation)) return;

            Teleport(player, doorOrientation, directionTo);
        }

        private void Teleport(IPlayer player, string doorOrientation, Direction directionTo)
        {
            if (doorOrientation is "top" or "bottom")
            {
                TeleportNorthOrSouth(player, directionTo);
            }

            if (doorOrientation is "left" or "right")
            {
                TeleportEastOrWest(player, directionTo);
            }
        }

        private void TeleportEastOrWest(IPlayer player, Direction directionTo)
        {
            Console.WriteLine(directionTo);
            if (directionTo is Direction.East or Direction.SouthEast or Direction.NorthEast)
            {
                player.TeleportTo((ushort)(Location.X - 1), Location.Y, Location.Z);
            }

            if (directionTo is Direction.West or Direction.NorthWest or Direction.SouthWest)
            {
                player.TeleportTo((ushort)(Location.X + 1), Location.Y, Location.Z);
            }
        }

        private void TeleportNorthOrSouth(IPlayer player, Direction directionTo)
        {
            if (directionTo is Direction.South or Direction.SouthEast or Direction.SouthWest)
            {
                player.TeleportTo(Location.X, (ushort)(Location.Y - 1), Location.Z);
            }

            if (directionTo is Direction.North or Direction.NorthEast or Direction.NorthWest)
            {
                player.TeleportTo(Location.X, (ushort)(Location.Y + 1), Location.Z);
            }
        }

        public static bool IsApplicable(IItemType type) =>
            Door.IsApplicable(type) && type.Attributes.HasAttribute(ItemAttribute.LevelDoor);
    }
}