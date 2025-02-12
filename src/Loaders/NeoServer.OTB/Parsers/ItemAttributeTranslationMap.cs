﻿using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Item;

namespace NeoServer.OTB.Parsers;

internal class ItemAttributeTranslationMap
{
    public static Dictionary<string, ItemAttribute> Map = new(StringComparer.InvariantCultureIgnoreCase)
    {
        ["type"] = ItemAttribute.Type,
        ["description"] = ItemAttribute.Description,
        ["runespellname"] = ItemAttribute.RuneSpellName,
        ["weight"] = ItemAttribute.Weight,
        ["showcount"] = ItemAttribute.ShowCount,
        ["armor"] = ItemAttribute.Armor,
        ["defense"] = ItemAttribute.Defense,
        ["extradef"] = ItemAttribute.ExtraDefense,
        ["attack"] = ItemAttribute.Attack,
        ["rotateto"] = ItemAttribute.RotateTarget,
        ["moveable"] = ItemAttribute.Moveable,
        ["movable"] = ItemAttribute.Moveable,
        ["blockprojectile"] = ItemAttribute.BlockProjectTile,
        ["allowpickupable"] = ItemAttribute.Pickupable,
        ["pickupable"] = ItemAttribute.Pickupable,
        ["forceserialize"] = ItemAttribute.ForceSerialize,
        ["floorchange"] = ItemAttribute.FloorChange,
        ["corpsetype"] = ItemAttribute.CorpseType,
        ["containersize"] = ItemAttribute.Capacity,
        ["fluidsource"] = ItemAttribute.SourceLiquidType,
        ["readable"] = ItemAttribute.Readable,
        ["writeable"] = ItemAttribute.Writeable,
        ["maxtextlen"] = ItemAttribute.MaxLength,
        ["writeonceitemid"] = ItemAttribute.WriteOnceItemId,
        ["weapontype"] = ItemAttribute.WeaponType,
        ["slottype"] = ItemAttribute.BodyPosition,
        ["ammotype"] = ItemAttribute.AmmoType,
        ["shoottype"] = ItemAttribute.ShootType,
        ["effect"] = ItemAttribute.Effect,
        ["range"] = ItemAttribute.Range,
        ["stopduration"] = ItemAttribute.StopDecaying,
        ["decayto"] = ItemAttribute.ExpireTarget,
        ["transformequipto"] = ItemAttribute.TransformEquipTo,
        ["transformdeequipto"] = ItemAttribute.TransformDequipTo,
        ["duration"] = ItemAttribute.Duration,
        ["showduration"] = ItemAttribute.ShowDuration,
        ["charges"] = ItemAttribute.Charges,
        ["showcharges"] = ItemAttribute.ShowCharges,
        ["showattributes"] = ItemAttribute.ShowAttributes,
        ["hitchance"] = ItemAttribute.HitChance,
        ["maxhitchance"] = ItemAttribute.MaxHitChance,
        ["invisible"] = ItemAttribute.Invisible,
        ["speed"] = ItemAttribute.Speed,
        ["healthgain"] = ItemAttribute.HealthGain,
        ["healthticks"] = ItemAttribute.HealTicks,
        ["managain"] = ItemAttribute.ManaGain,
        ["manaticks"] = ItemAttribute.ManaTicks,
        ["manashield"] = ItemAttribute.ManaShield,
        ["skillsword"] = ItemAttribute.SkillSword,
        ["skillaxe"] = ItemAttribute.SkillAxe,
        ["skillclub"] = ItemAttribute.SkillClub,
        ["skilldist"] = ItemAttribute.SkillDistance,
        ["skillfish"] = ItemAttribute.SkillFishing,
        ["skillshield"] = ItemAttribute.SkillShield,
        ["skillfist"] = ItemAttribute.SkillFist,
        ["maxhitpoints"] = ItemAttribute.MaxHitpoints,
        ["maxhitpointspercent"] = ItemAttribute.MaxHitpointsPercent,
        ["maxmanapoints"] = ItemAttribute.MaxManapoints,
        ["maxmanapointspercent"] = ItemAttribute.MaxManaPointsPercent,
        ["magicpoints"] = ItemAttribute.MagicPoints,
        ["magiclevelpoints"] = ItemAttribute.MagicPoints,
        ["magicpointspercent"] = ItemAttribute.MagicPointsPercent,
        ["criticalhitchance"] = ItemAttribute.CriticalHitChance,
        ["criticalhitamount"] = ItemAttribute.CriticalHitAmount,
        ["lifeleechchance"] = ItemAttribute.LifeLeechChance,
        ["lifeleechamount"] = ItemAttribute.LifeLeechAmount,
        ["manaleechchance"] = ItemAttribute.ManaLeechChange,
        ["manaleechamount"] = ItemAttribute.ManaLeechAmout,
        ["fieldabsorbpercentenergy"] = ItemAttribute.FieldAbsorbPercentEnergy,
        ["fieldabsorbpercentfire"] = ItemAttribute.FieldAbsorbEercentFire,
        ["fieldabsorbpercentpoison"] = ItemAttribute.FieldAbsorbPercentPoison,
        ["fieldabsorbpercentearth"] = ItemAttribute.FieldAbsorbPercentPoison,
        ["absorbpercentall"] = ItemAttribute.AbsorbPercentAll,
        ["absorbpercentallelements"] = ItemAttribute.AbsorbPercentAll,
        ["absorbpercentelements"] = ItemAttribute.AbsorbPercentElements,
        ["absorbpercentmagic"] = ItemAttribute.AbsorbPercentMagic,
        ["absorbpercentenergy"] = ItemAttribute.AbsorbPercentEnergy,
        ["absorbpercentfire"] = ItemAttribute.AbsorbPercentFire,
        ["absorbpercentpoison"] = ItemAttribute.AbsorbPercentPoison,
        ["absorbpercentearth"] = ItemAttribute.AbsorbPercentPoison,
        ["absorbpercentice"] = ItemAttribute.AbsorbPercentIce,
        ["absorbpercentholy"] = ItemAttribute.AbsorbPercentHoly,
        ["absorbpercentdeath"] = ItemAttribute.AbsorbPercentDeath,
        ["absorbpercentlifedrain"] = ItemAttribute.AbsorbPercentLifeDrain,
        ["absorbpercentmanadrain"] = ItemAttribute.AbsorbPercentManaDrain,
        ["absorbpercentdrown"] = ItemAttribute.AbsorbPercentDrown,
        ["absorbpercentphysical"] = ItemAttribute.AbsorbPercentPhysical,
        ["absorbpercenthealing"] = ItemAttribute.AbsorbPercentHealing,
        ["absorbpercentundefined"] = ItemAttribute.AbsorbPercenUndefined, //remove
        ["suppressdrunk"] = ItemAttribute.SuppressDrunk,
        ["suppressenergy"] = ItemAttribute.SuppressEnergy,
        ["suppressfire"] = ItemAttribute.SuppressFire,
        ["suppresspoison"] = ItemAttribute.SuppressPoison,
        ["suppressdrown"] = ItemAttribute.SuppressDrown,
        ["suppressphysical"] = ItemAttribute.SuppressPhysical,
        ["suppressfreeze"] = ItemAttribute.SuppressFreeze,
        ["suppressdazzle"] = ItemAttribute.SuppressDazzle,
        ["suppresscurse"] = ItemAttribute.SuppressCurse,
        ["field"] = ItemAttribute.Field,
        ["replaceable"] = ItemAttribute.Replaceable,
        ["partnerdirection"] = ItemAttribute.PartnerDirection,
        ["leveldoor"] = ItemAttribute.LevelDoor,
        ["maletransformto"] = ItemAttribute.MaleTransformTo,
        ["malesleeper"] = ItemAttribute.MaleTransformTo,
        ["femaletransformto"] = ItemAttribute.FemaleTransformTo,
        ["femalesleeper"] = ItemAttribute.FemaleTransformTo,
        ["transformto"] = ItemAttribute.TransformTo,
        ["destroyto"] = ItemAttribute.DestroyTo,
        ["elementice"] = ItemAttribute.ElementIce,
        ["elementearth"] = ItemAttribute.ElementEarth,
        ["elementfire"] = ItemAttribute.ElementFire,
        ["elementenergy"] = ItemAttribute.ElementEnergy,
        ["walkstack"] = ItemAttribute.WalkStack,
        ["blocking"] = ItemAttribute.Blocking,
        ["allowdistread"] = ItemAttribute.AllowDistRead,
        ["minlevel"] = ItemAttribute.MinimumLevel,
        ["teleport"] = ItemAttribute.TeleportDestination,
        ["useon"] = ItemAttribute.UseOn,
        ["healing"] = ItemAttribute.Healing,
        ["min"] = ItemAttribute.Min,
        ["max"] = ItemAttribute.Max,
        ["sentence"] = ItemAttribute.Sentence,
        ["vocations"] = ItemAttribute.Vocation,
        ["regeneration"] = ItemAttribute.Regeneration,
        ["needtarget"] = ItemAttribute.NeedTarget,
        ["formula"] = ItemAttribute.Formula,
        ["damage"] = ItemAttribute.Damage,
        ["area"] = ItemAttribute.Area,
        ["ticks"] = ItemAttribute.Ticks,
        ["count"] = ItemAttribute.Count,
        ["script"] = ItemAttribute.Script,
        ["worth"] = ItemAttribute.Worth,
        ["manause"] = ItemAttribute.ManaUse,
        ["cooldown"] = ItemAttribute.CooldownTime,
        ["actionid"] = ItemAttribute.ActionId,
        ["decayElapsed"] = ItemAttribute.DecayElapsed,
    };
}