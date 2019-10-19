/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Clio.Utilities;
using Deep.Helpers;
using Deep.Logging;
using Deep.Memory;
using Deep.Tasks;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Deep.Providers
{
    internal class DDTargetingProvider
    {
        private static DDTargetingProvider _instance;

        internal static DDTargetingProvider Instance => _instance ?? (_instance = new DDTargetingProvider());

        public DDTargetingProvider()
        {
            LastEntities = new ReadOnlyCollection<GameObject>(new List<GameObject>());
        }

      
        public ReadOnlyCollection<GameObject> LastEntities { get; set; }

        internal bool LevelComplete
        {
            get
            {
                if (!DeepDungeonManager.PortalActive)
                    return false;

                if (Settings.Instance.GoExit && PartyManager.IsInParty)
                {
                    if (PartyManager.AllMembers.Any(i => i.CurrentHealth == 0))
                        return false;

                    if (Settings.Instance.GoForTheHoard)
                    {
                        return !LastEntities.Any(i => (i.NpcId == EntityNames.Hidden || i.NpcId == EntityNames.BandedCoffer) && !Blacklist.Contains(i.ObjectId, (BlacklistFlags)DeepDungeonManager.Level));
                    }

                    //Logger.Instance.Verbose("Full Explore : {0} {1}", _levelComplete, !NotMobs().Any());
                    return true;
                }
                return !LastEntities.Any();
            }
        }

        internal void Reset()
        {
            Blacklist.Clear(i => true);
        }

        private int _floor;
        private DateTime _lastPulse = DateTime.MinValue;

        internal void Pulse()
        {
            if (CommonBehaviors.IsLoading)
                return;

            if (!Constants.InDeepDungeon)
                return;

            if (_floor != DeepDungeonManager.Level)
            {
                Logger.Info("Level has Changed. Clearing Targets");
                _floor = DeepDungeonManager.Level;
                Blacklist.Clear(i => i.Flags == (BlacklistFlags)DeepDungeonManager.Level);
            }
            
            //if (CairnOfReturn != null && !CairnOfReturn.IsValid)
            //    CairnOfReturn = null;

            //if (Portal != null && !Portal.IsValid)
            //    Portal = null;

            using (new PerformanceLogger("Targeting Pulse"))
            {
                LastEntities = new ReadOnlyCollection<GameObject>(GetObjectsByWeight());

                if (_lastPulse + TimeSpan.FromSeconds(5) < DateTime.Now)
                {
                    Logger.Verbose($"Found {LastEntities.Count} Targets");
                    _lastPulse = DateTime.Now;
                }
                
            }
        }

        /// <summary>
        /// decide what we need to do
        /// </summary>
        public GameObject FirstEntity => LastEntities.FirstOrDefault();
        //{
        //    get
        //    {
        //        var badGuys = (CombatTargeting.Instance.Provider as DDCombatTargetingProvider)?.GetObjectsByWeight();

        //        var anyBadGuysAround = badGuys != null && badGuys.Any();

        //        //if (Beta.Target != null && Beta.Target.IsValid && !Blacklist.Contains(Beta.Target.ObjectId, (BlacklistFlags)DeepDungeonManager.Level) && Beta.Target.Type != GameObjectType.GatheringPoint)
        //        //    return null;

        //        // Party member is dead
        //        if (PartyManager.AllMembers.Any(member => member.CurrentHealth == 0))
        //        {
        //            // Select Cairn of Return as highest priority if it is known and can be used.
        //            if (CairnOfReturn != null && DeepDungeonManager.ReturnActive)
        //                return CairnOfReturn;

        //            // If the Cairn of Return is not yet active and there are any mobs around: Kill the mobs.
        //            if (anyBadGuysAround)
        //                return new Poi(badGuys.First(), PoiType.Kill);
        //        }

        //        // Cairn of Passage
        //        if (LevelComplete && Portal != null)
        //            return Portal;

        //        // Bosses or Pomander of Rage / Pomander of Lust
        //        if ((DeepDungeonManager.BossFloor || Core.Me.HasAura(Auras.Lust)) && anyBadGuysAround)
        //            return new Poi(badGuys.First(), PoiType.Kill);

        //        // Chests
        //        if (LastEntities != null && LastEntities.Any())
        //            return LastEntities.First();

        //        // Kill something
        //        if (anyBadGuysAround)
        //            return new Poi(badGuys.First(), PoiType.Kill);

        //        return new Poi(
        //            SafeSpots.OrderByDescending(i => i.Distance2D(Core.Me.Location)).First(),
        //            PoiType.Hotspot
        //        );


        //    }
        //}

        internal void AddToBlackList(GameObject obj, string reason)
        {
            AddToBlackList(obj, TimeSpan.FromMinutes(3), reason);
        }

        internal void AddToBlackList(GameObject obj, TimeSpan time, string reason)
        {
            Blacklist.Add(obj, (BlacklistFlags)_floor, time, reason);
            Poi.Clear(reason);
        }

        private List<GameObject> GetObjectsByWeight()
        {
            return GameObjectManager.GameObjects
                .Where(Filter)
                .OrderByDescending(Sort)
                .ToList();
        }

        private float Sort(GameObject obj)
        {
            var weight = 100f;

            weight -= obj.Distance2D();

            if (obj.Type == GameObjectType.BattleNpc)
            {
                return weight / 2;
            }

            if (obj.NpcId == EntityNames.BandedCoffer)
                weight += 500;

            if (DeepDungeonManager.PortalActive && Settings.Instance.GoForTheHoard && (obj.NpcId == EntityNames.Hidden))
                weight += 5;
            else if (DeepDungeonManager.PortalActive && Settings.Instance.GoExit && obj.NpcId != EntityNames.OfPassage && PartyManager.IsInParty)
                weight -= 10;

            return weight;
        }

        private bool Filter(GameObject obj)
        {
            if (obj.NpcId == 5042) //script object
                return false;

            if (obj.Location == Vector3.Zero)
                return false;

            
            if (Blacklist.Contains(obj) || Constants.TrapIds.Contains(obj.NpcId) || Constants.IgnoreEntity.Contains(obj.NpcId))
                return false;


            if (obj.Type == GameObjectType.BattleNpc)
            {
                if (DeepDungeonManager.PortalActive)
                    return false;

                var battleCharacter = (BattleCharacter) obj;
                return !battleCharacter.IsDead;
            }

            return obj.Type == GameObjectType.EventObject || obj.Type == GameObjectType.Treasure || obj.Type == GameObjectType.BattleNpc;
        }
    }
}