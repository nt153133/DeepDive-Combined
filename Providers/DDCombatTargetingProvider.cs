/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Collections.Generic;
using System.Linq;
using Clio.Utilities;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;

namespace DeepCombined.Providers
{
    // ReSharper disable once InconsistentNaming
    internal class DDCombatTargetingProvider : ITargetingProvider
    {
        private int _level;
        private Vector3 _location;
        private GameObject _targetObject;

        internal IEnumerable<BattleCharacter> Targets { get; private set; }

        public List<BattleCharacter> GetObjectsByWeight()
        {
            if (!DutyManager.InInstance)
            {
                return new List<BattleCharacter>();
            }

            if (DeepDungeonManager.Director.TimeLeftInDungeon == TimeSpan.Zero)
            {
                return new List<BattleCharacter>();
            }
            //Set some variables here that will get called often so memory reads only need to be performed one time
            LocalPlayer player = Core.Me;
            _location = player.Location;
            _level = player.ClassLevel;
            _targetObject = Core.Target;

            bool bossFloor = DeepDungeonManager.BossFloor;
            float combatReach = Constants.ModifiedCombatReach;

            if (combatReach > 1)
            {
                combatReach *= combatReach;
            }

            bool portalActive = DeepDungeonManager.PortalActive;


            //.ToArray();

            using (new PerformanceLogger($"targeting", true))
            {
                //var units = GameObjectManager.GetObjectsOfType<BattleCharacter>();

                bool inDD = Constants.InDeepDungeon;

                List<BattleCharacter> _Targets = new List<BattleCharacter>();

                foreach (GameObject target1 in GameObjectManager.GameObjects)
                {
                    if (target1.Type != GameObjectType.BattleNpc)
                    {
                        continue;
                    }

                    BattleCharacter target = (BattleCharacter)target1;
                    uint targetNpcId = target.NpcId;

                    if (targetNpcId == 5042 || targetNpcId == 0)
                    {
                        continue;
                    }

                    if (target.IsDead)
                    {
                        continue;
                    }

                    if (Constants.TrapIds.Contains(targetNpcId) || Constants.IgnoreEntity.Contains(targetNpcId))
                    {
                        continue;
                    }

                    bool targetInCombat = target.InCombat;

                    if (inDD && Blacklist.Contains(target) && !targetInCombat)
                    {
                        continue;
                    }

                    if (!target.IsTargetable)
                    {
                        continue;
                    }

                    if (portalActive && !targetInCombat && targetNpcId != Mobs.PalaceHornet)
                    {
                        continue;
                    }

                    if (!target.StatusFlags.HasFlag(StatusFlags.Hostile))
                    {
                        continue;
                    }

                    if (bossFloor)
                    {
                        _Targets.Add(target);
                    }

                    if (!(target.Location.Distance2DSqr(_location) < combatReach))
                    {
                        continue;
                    }

                    if (targetInCombat || target.InLineOfSight())
                    {
                        _Targets.Add((target));
                    }
                }

                return _Targets.OrderByDescending(Priority).ToList();
            }
        }

        private double Priority(BattleCharacter battleCharacter)
        {
            double weight = 1000.0;
            float distance2D = battleCharacter.Distance2D(_location);

            weight -= distance2D / 2.25;
            weight += battleCharacter.ClassLevel / 1.25;
            weight += 100 - battleCharacter.CurrentHealthPercent;

            bool battleCharacterInCombat = battleCharacter.InCombat;

            if ((battleCharacter.NpcId == Mobs.PalaceHornet || battleCharacter.NpcId == Mobs.PalaceSlime) && battleCharacterInCombat)
            {
                return weight * 100.0;
            }

            if (PartyManager.IsInParty && !PartyManager.IsPartyLeader)
            {
                if (battleCharacter.IsTargetingMyPartyMember())
                {
                    weight += 100;
                }
            }

            if (battleCharacter.HasTarget && battleCharacter.TargetCharacter == Core.Me)
            {
                weight += 50;
            }

            if (!battleCharacterInCombat)
            {
                weight -= 5;
            }
            else
            {
                weight += 50;
            }

            if (_targetObject != null && _targetObject.ObjectId == battleCharacter.ObjectId)
            {
                weight += 10;
            }

            if (battleCharacterInCombat && distance2D < 5)
            {
                weight *= 1.5;
            }

            if (distance2D > 25)
            {
                weight /= 2;
            }

            return weight;
        }
    }
}