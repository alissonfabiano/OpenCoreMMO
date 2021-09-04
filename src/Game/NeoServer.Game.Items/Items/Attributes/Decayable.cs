﻿using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.DataStore;

namespace NeoServer.Game.Items.Items.Attributes
{
    public class Decayable: IDecayable
    {
        public event DecayDelegate OnDecayed;
        public Decayable(IDecayable item, int decaysTo, int duration)
        {
            Item = item;
            DecaysTo = decaysTo;
            Duration = duration;
        }

        public IDecayable Item { get; }
        public int DecaysTo { get; }
        public int Duration { get; }
        private long _startedToDecayTime;
        public bool StartedToDecay => _startedToDecayTime != default;

        public int Elapsed { get; private set; }
        public bool Expired => StartedToDecay && Elapsed >= Duration;
        public bool ShouldDisappear => DecaysTo == 0;

        public void Start()
        {
            _startedToDecayTime = DateTime.Now.Ticks;
        }

        public void Pause() => Elapsed += (int)((DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond);

        public bool Decay()
        {
            OnDecayed?.Invoke(Item);
            
            if (DecaysTo <= 0) return false;
            if (!ItemTypeStore.Data.TryGetValue((ushort)DecaysTo, out var newItem)) return false;

            //Metadata = newItem;
            _startedToDecayTime = DateTime.Now.Ticks;
            
            return true;
        }
    }
}