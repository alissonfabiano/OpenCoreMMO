﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories.AttributeFactory;

namespace NeoServer.Game.Items.Bases;

public abstract class BaseItem : IItem
{
    private IThing _owner;

    protected BaseItem(IItemType metadata, Location location)
    {
        Location = location;
        Metadata = metadata;

        Decay = DecayableFactory.CreateIfItemIsDecayable(this);
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        OnDeleted?.Invoke(this);
    }

    public bool IsDeleted { get; private set; }

    public void OnItemRemoved(IThing from) => OnRemoved?.Invoke(this, from);

    public void SetActionId(ushort actionId)
    {
        ActionId = actionId;
    }

    public void SetUniqueId(uint uniqueId)
    {
        UniqueId = uniqueId;
    }

    public ushort ActionId { get; private set; }
    public uint UniqueId { get; private set; }

    public IItemType Metadata { get; private set; }

    public void UpdateMetadata(IItemType newMetadata)
    {
        Metadata = newMetadata;
    }

    public Location Location { get; set; }

    public void SetNewLocation(Location location)
    {
        if (!((IItem)this).CanBeMoved) return;
        Location = location;
    }

    public virtual string GetLookText(IInspectionTextBuilder inspectionTextBuilder, IPlayer player,
        bool isClose = false)
    {
        return inspectionTextBuilder is null
            ? $"You see {Metadata.Article} {Metadata.Name}."
            : inspectionTextBuilder.Build(this, player, isClose);
    }

    public string FullName => Metadata.FullName;
    public byte Amount { get; set; } = 1;

    public virtual void Use(IPlayer usedBy)
    {
        //Checks if there is a function for the type already registered
        if (!IUsable.UseFunctionMap.TryGetValue(Metadata.TypeId, out var useFunc)) return;
        useFunc?.Invoke(this, usedBy);
    }

    public virtual float Weight => Metadata.Weight;

    public IThing Parent { get; private set; }

    public void SetParent(IThing parent)
    {
        Parent = parent;
    }

    public void SetOwner(IThing owner)
    {
        Owner = owner;
    }

    public IThing Owner
    {
        get => _owner is IContainer container ? container.RootParent : _owner;
        private set => _owner = value;
    }

    #region Decay

    public IDecayable Decay { get; protected set; }

    #endregion

    #region Events

    public event ItemDelete OnDeleted;
    public event ItemRemove OnRemoved;

    #endregion

    public override string ToString()
    {
        var plural = Metadata.Plural ?? $"{Metadata.Name}s";
        return Amount > 1 ? $"{Amount} {plural}" : Metadata.FullName;
    }
}