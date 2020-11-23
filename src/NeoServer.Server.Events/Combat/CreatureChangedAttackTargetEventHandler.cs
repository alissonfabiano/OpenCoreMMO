﻿using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Tasks;
using System;

namespace NeoServer.Server.Events.Combat
{
    public class CreatureChangedAttackTargetEventHandler
    {
        private readonly Game game;

        public CreatureChangedAttackTargetEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICombatActor actor, uint oldTarget, uint newTarget)
        {
            
            if (actor.AttackEvent != 0)
            {
                return;
            }

            var result = Attack(actor);
            var attackSpeed = result ? actor.BaseAttackSpeed : 300;
            actor.AttackEvent = game.Scheduler.AddEvent(new SchedulerEvent((int)actor.BaseAttackSpeed, () => Attack(actor)));
        }
        private bool Attack(ICombatActor actor)
        {
            var result = false;
            if (actor.Attacking)
            {
                if (game.CreatureManager.TryGetCreature(actor.AutoAttackTargetId, out var creature) && creature is ICombatActor enemy)
                {
                    result = actor.Attack(enemy);
                }
            }
            else
            {
                if (actor.AttackEvent != 0)
                {
                    game.Scheduler.CancelEvent(actor.AttackEvent);
                    actor.AttackEvent = 0;
                }
            }

            if (actor.AttackEvent != 0)
            {
                actor.AttackEvent = 0;
                Execute(actor, 0, 0);
            }
            return result;
        }

        private void MoveAroundEnemy(ICombatActor actor)
        {
            if (!(actor is IMonster monster)) return;

            monster.MoveAroundEnemy();
        }
    }
}