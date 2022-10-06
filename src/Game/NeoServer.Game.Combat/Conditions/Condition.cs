﻿using System;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Combat.Conditions;

public class Condition : BaseCondition
{
    /// <param name="duration">Duration in milliseconds</param>
    public Condition(ConditionType type, uint duration) : base(duration)
    {
        Type = type;
    }

    public Condition(ConditionType type, uint duration, Action onEnd) : base(duration, onEnd)
    {
        Type = type;
    }

    public override ConditionType Type { get; }
}