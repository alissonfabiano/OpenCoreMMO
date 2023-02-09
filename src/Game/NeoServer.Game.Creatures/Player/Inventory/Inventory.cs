﻿using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Creatures.Player.Inventory.Calculations;
using NeoServer.Game.Creatures.Player.Inventory.Operations;
using NeoServer.Game.Creatures.Player.Inventory.Rules;

namespace NeoServer.Game.Creatures.Player.Inventory;

public class Inventory : IInventory
{
    public Inventory(IPlayer player, IDictionary<Slot, (IPickupable Item, ushort Id)> items)
    {
        InventoryMap = new InventoryMap(this);
        Owner = player;

        OnItemAddedToSlot += OnItemAddedToInventorySlot;

        AddItemsToInventory(items);
    }

    internal InventoryMap InventoryMap { get; }

    internal IAmmoEquipment Ammo => InventoryMap.GetItem<IAmmoEquipment>(Slot.Ammo);
    internal IDefenseEquipment Shield => InventoryMap.GetItem<IDefenseEquipment>(Slot.Right);
    public IWeapon Weapon => InventoryMap.GetItem<IWeapon>(Slot.Left);
    public bool IsUsingWeapon => InventoryMap.HasItemOnSlot(Slot.Left);
    public bool HasShield => InventoryMap.HasItemOnSlot(Slot.Right);
    public ushort TotalAttack => this.CalculateTotalAttack();
    public ushort TotalDefense => InventoryMap.CalculateTotalDefense();
    public ushort TotalArmor => InventoryMap.CalculateTotalArmor();
    public byte AttackRange => InventoryMap.CalculateAttackRange();

    public ulong GetTotalMoney(ICoinTypeStore coinTypeStore)
    {
        return this.CalculateTotalMoney(coinTypeStore);
    }

    /// <summary>
    ///     Gets all items that player is wearing except the bag
    /// </summary>
    public IEnumerable<IItem> DressingItems =>
        new List<IItem>
        {
            this[Slot.Head], this[Slot.Necklace], this[Slot.Body], this[Slot.Right], this[Slot.Left], this[Slot.Legs],
            this[Slot.Ring], this[Slot.Ammo], this[Slot.Feet]
        };

    public IDictionary<ushort, uint> Map => InventoryMap.Map;
    public IPlayer Owner { get; }
    public IItem this[Slot slot] => InventoryMap.GetItem<IItem>(slot);

    public T TryGetItem<T>(Slot slot)
    {
        return this[slot] is T item ? item : default;
    }

    public IContainer BackpackSlot => this[Slot.Backpack] as IContainer;
    public float TotalWeight => InventoryMap.CalculateTotalWeight();

    public Result CanAddItem(IItem thing, byte amount = 1, byte? slot = null)
    {
        return this.CanAddItem(slot is null ? Slot.None : (Slot)slot, thing, amount);
    }

    public Result<uint> CanAddItem(IItemType itemType)
    {
        return AddToSlotRule.CanAddItem(this, itemType);
    }

    public uint PossibleAmountToAdd(IItem item, byte? toPosition = null)
    {
        return PossibleAmountToAddCalculation.Calculate(this, item, toPosition);
    }

    public bool CanRemoveItem(IItem item)
    {
        return true;
    }

    private void AddItemsToInventory(IDictionary<Slot, (IPickupable Item, ushort Id)> items)
    {
        foreach (var (slot, (item, _)) in items) TryAddItemToSlot(slot, item);
    }

    #region Operations

    private Result<IPickupable> TryAddItemToSlot(Slot slot, IPickupable item)
    {
        var result = AddToSlotOperation.Add(this, slot, item);

        if (result.Succeeded)
        {
            OnItemAddedToSlot?.Invoke(this, item, slot);
            return result;
        }

        OnFailedToAddToSlot?.Invoke(Owner, result.Error);
        return result;
    }

    public Result<OperationResult<IItem>> AddItem(IItem thing, Slot slot = Slot.None)
    {
        return AddItem(thing, slot is Slot.None ? null : (byte)slot);
    }

    public Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null)
    {
        if (thing is not IPickupable item) return Result<OperationResult<IItem>>.NotPossible;

        position ??= (byte)thing.Metadata.BodyPosition;

        var swappedItem = TryAddItemToSlot((Slot)position, item);

        if (swappedItem.Failed) return new Result<OperationResult<IItem>>(swappedItem.Error);

        if (swappedItem.Value is null) return Result<OperationResult<IItem>>.Success;

        return new Result<OperationResult<IItem>>(new OperationResult<IItem>(Operation.Removed, swappedItem.Value));
    }

    public Result<IPickupable> RemoveItem(Slot slot, byte amount)
    {
        var result = RemoveFromSlotOperation.Remove(this, slot, amount);
        var removedItem = result.Value;

        if (result.Failed) return Result<IPickupable>.Fail(result.Error);

        OnItemRemovedFromSlot?.Invoke(this, removedItem, slot, amount);
        return Result<IPickupable>.Ok(removedItem);
    }

    public Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
        out IItem removedThing)
    {
        removedThing = null;
        var result = RemoveItem((Slot)fromPosition, amount);
        if (result.Failed)
            return Result<OperationResult<IItem>>.Fail(result.Error);

        removedThing = result.Value;
        return Result<OperationResult<IItem>>.Ok(new OperationResult<IItem>(removedThing));
    }

    #endregion

    #region Event Handlers

    private void OnItemAddedToInventorySlot(IInventory inventory, IPickupable item, Slot slot, byte amount)
    {
        if (item is IMovableItem movableItem) movableItem.SetOwner(Owner);
    }

    internal void OnItemReduced(ICumulative item, Slot slot, byte amount)
    {
        if (item.Amount == 0)
        {
            RemoveItem(slot, amount);
            return;
        }

        OnItemRemovedFromSlot?.Invoke(this, item, slot, amount);
    }

    #endregion

    #region Events

    public event AddItemToSlot OnItemAddedToSlot;
    public event RemoveItemFromSlot OnItemRemovedFromSlot;
    public event FailAddItemToSlot OnFailedToAddToSlot;

    #endregion
}