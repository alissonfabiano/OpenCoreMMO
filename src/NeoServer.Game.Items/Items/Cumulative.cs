﻿using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items
{
    public class Cumulative : MoveableItem, ICumulative, IItem
    {
        public Cumulative(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location)
        {
            Amount = 1;
            if (attributes != null && attributes.TryGetValue(Common.ItemAttribute.Count, out var amount))
            {
                Amount = Math.Min((byte)100, (byte)amount);
            }
        }
        
        public Cumulative(IItemType type, Location location, byte amount) : base(type, location) => Amount = Math.Min((byte)100, amount);

        public event ItemReduce OnReduced;
        public byte Amount { get; set; }

        public new float Weight => CalculateWeight(Amount);

        public float CalculateWeight(byte amount) => Metadata.Weight * amount;

        public Span<byte> GetRaw()
        {
            Span<byte> cache = stackalloc byte[3];
            var idBytes = BitConverter.GetBytes(Metadata.ClientId);

            cache[0] = idBytes[0];
            cache[1] = idBytes[1];
            cache[2] = Amount;

            return cache.ToArray();
        }
        public static bool IsApplicable(IItemType type) => type.Flags.Contains(Common.ItemFlag.Stackable);

        public ICumulative Clone(byte amount)
        {
            var clone = (ICumulative) MemberwiseClone();
            clone.Amount = amount;
            clone.ClearSubscribers();
            return clone;
        }
        

        public void ClearSubscribers()
        {
            OnReduced = null;
        }

        /// <summary>
        /// Reduce amount from item
        /// </summary>
        /// <param name="amount">Amount to be reduced</param>
        public void Reduce(byte amount)
        {
            amount = (byte)(amount > 100 ? 100 : amount);
            Amount -= amount;
            OnReduced?.Invoke(this, amount);
        }

        /// <summary>
        /// Split item in two parts
        /// </summary>
        /// <param name="amount">Amount to be reduced</param>
        public ICumulative Split(byte amount)
        {
            amount = (byte)(amount > 100 ? 100 : amount);
            Amount -= amount;
            return Clone(amount);
        }

        public void Increase(byte amount) => Amount = (byte)(amount + Amount > 100 ? 100 : amount + Amount);


        public byte AmountToComplete => (byte)(100 - Amount);

        public bool TryJoin(ref ICumulative item)
        {
            if (item?.Metadata?.ClientId is null) return false;
            if (item.Metadata.ClientId != Metadata.ClientId) return false;
            
            var totalAmount = Amount + item.Amount;

            if (totalAmount <= 100)
            {
                Amount = (byte)totalAmount;
                item = null;
                return true;
            }
            Amount = 100;

            item.Amount = (byte)(totalAmount - Amount);

            return true;
        }
        public override string ToString() => $"{Amount} {Metadata.Name}";
    }
}
