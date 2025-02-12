﻿using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Items.Factories;

namespace NeoServer.Game.Items.Events;

public class FieldRuneUsedEventHandler : IGameEventHandler
{
    private readonly IAreaEffectStore _areaEffectStore;
    private readonly IItemFactory itemFactory;
    private readonly IMap map;

    public FieldRuneUsedEventHandler(IMap map, IItemFactory itemFactory, IAreaEffectStore areaEffectStore)
    {
        this.map = map;
        this.itemFactory = itemFactory;
        _areaEffectStore = areaEffectStore;
    }

    public void Execute(ICreature usedBy, IDynamicTile onTile, IUsableOnTile item)
    {
        if (item is not IFieldRune rune) return;

        if (!string.IsNullOrWhiteSpace(rune.Area))
        {
            var template = _areaEffectStore.Get(rune.Area);
            foreach (var coordinate in AreaEffect.Create(onTile.Location, template))
            {
                var location = coordinate.Location;
                var field = ItemFactory.Instance.Create(rune.Field, location, null);

                if (map[location] is not IDynamicTile tile) continue;

                tile.AddItem(field);

                CauseDamageToCreaturesOnTile(field, tile);
            }
        }
        else
        {
            var field = ItemFactory.Instance.Create(rune.Field, onTile.Location, null);
            onTile.AddItem(field);

            CauseDamageToCreaturesOnTile(field, onTile);
        }
    }

    public void CauseDamageToCreaturesOnTile(IItem item, ITile tile)
    {
        if (item is not IMagicField field) return;
        if (tile is not IDynamicTile onTile) return;

        if (!onTile.HasCreature) return;

        foreach (var creature in onTile.Creatures) field.CauseDamage(creature);
    }
}