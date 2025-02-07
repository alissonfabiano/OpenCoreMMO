﻿using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.Common.Contracts.Items.Types.Usable;

public delegate void UseOnTile(ICreature usedBy, IDynamicTile tile, IUsableOnTile item);

public interface IUsableOnItem : IUsableOn
{
    public static readonly Dictionary<ushort, Func<IItem, ICreature, IItem, bool>> UseFunctionMap = new ();

    public bool Use(ICreature usedBy, IItem onItem)
    {
        if (UseFunctionMap.TryGetValue(Metadata.TypeId, out var useFunc)) return useFunc.Invoke(this, usedBy, onItem);
        return true;
    }

    bool CanUseOn(IItem onItem);
    bool CanUseOn(ushort[] items, IItem onItem);
}

public interface IUsableOnTile : IUsableOn
{
    event UseOnTile OnUsedOnTile;

    /// <summary>
    ///     Usable by creatures on items (ground, weapon, stairs..)
    /// </summary>
    /// <param name="usedBy">player whose item is being used</param>
    /// <param name="tile">tile which will receive action</param>
    public bool Use(ICreature usedBy, ITile tile);
}

public interface IUsableAttackOnTile : IUsableOn
{
    /// <summary>
    ///     Usable by creatures on items (ground, weapon, stairs..)
    /// </summary>
    /// <param name="usedBy">player whose item is being used</param>
    /// <param name="tile">tile which will receive action</param>
    /// <param name="combat"></param>
    public bool Use(ICreature usedBy, ITile tile, out CombatAttackResult combat);
}