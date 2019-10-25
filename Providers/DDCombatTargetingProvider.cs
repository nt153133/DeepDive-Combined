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
using System.Text;
using System.Threading.Tasks;
using Clio.Utilities;
using Deep.Helpers;
using Deep.Memory;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;

namespace Deep.Providers
{
    // ReSharper disable once InconsistentNaming
    internal class DDCombatTargetingProvider : ITargetingProvider
    {
        private Vector3 _location;
        private int _level;
        private GameObject _targetObject;

        internal IEnumerable<BattleCharacter> Targets { get; private set; }
        public List<BattleCharacter> GetObjectsByWeight()
        {
            //Set some variables here that will get called often so memory reads only need to be performed one time
            var player = Core.Me;
            _location = player.Location;
            _level = player.ClassLevel;
            _targetObject = Core.Target;

            var bossFloor = DeepDungeonManager.BossFloor;
            var combatReach = Constants.ModifiedCombatReach;

            if (combatReach > 1)
                combatReach *= combatReach;

            var portalActive = DeepDungeonManager.PortalActive;


            var units = GameObjectManager.GetObjectsOfType<BattleCharacter>();//.ToArray();

            //using (new PerformanceTimer("targeting " + units.Length))
            //{

            var inDD = Constants.InDeepDungeon;
            Targets = units
                .Where(target =>
                {
                    var targetNpcId = target.NpcId;
                    if (targetNpcId == 5042 || targetNpcId == 0)
                        return false;

                    var targetInCombat = target.InCombat;

                    if (inDD && (Blacklist.Contains(target) && !targetInCombat))
                        return false;

                    if (Constants.TrapIds.Contains(targetNpcId) || Constants.IgnoreEntity.Contains(targetNpcId))
                        return false;

                    if (!target.IsTargetable)
                        return false;

                    if (portalActive && !targetInCombat)
                    {
                        var targetLevel = target.ClassLevel;
                        if (_level - targetLevel >= 3)
                        {
                            return false;
                        }
                    }

                    if (target.IsDead) 
                        return false;

                    if (target.StatusFlags.HasFlag(StatusFlags.Hostile))
                    {
                        if (bossFloor)
                            return true;

                        if (target.Location.Distance2DSqr(_location) < combatReach)
                        {
                            return targetInCombat || target.InLineOfSight();
                        }
                    }

                    return false;
                });




            return Targets.OrderByDescending(Priority).ToList();
            //}


        }

        private double Priority(BattleCharacter battleCharacter)
        {
            var weight = 1000.0;
            var distance2D = battleCharacter.Distance2D(_location);

            weight -= distance2D / 2.25;
            weight += battleCharacter.ClassLevel / 1.25;
            weight += 100 - battleCharacter.CurrentHealthPercent;
            
            if (PartyManager.IsInParty && !PartyManager.IsPartyLeader)
                if (battleCharacter.IsTargetingMyPartyMember())
                    weight += 100;

            if (battleCharacter.HasTarget && battleCharacter.TargetCharacter == Core.Me)
                weight += 50;

            var battleCharacterInCombat = battleCharacter.InCombat;

            if (!battleCharacterInCombat)
                weight -= 5;
            else
                weight += 50;

            if (_targetObject != null && _targetObject.ObjectId == battleCharacter.ObjectId)
                weight += 10;

            if (battleCharacterInCombat && distance2D < 5)
                weight *= 1.5;

            if (distance2D > 25)
                weight /= 2;

            if ((battleCharacter.NpcId == Mobs.PalaceHornet || battleCharacter.NpcId == Mobs.PalaceSlime) && battleCharacterInCombat)
                return weight * 100.0;


            return weight;
        }
    }
}
