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
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;

namespace DeepCombined.Providers
{
    // ReSharper disable once InconsistentNaming
    internal class DDTargetingProvider
    {
        private static DDTargetingProvider _instance;

        private int _floor;
        private DateTime _lastPulse = DateTime.MinValue;

        public DDTargetingProvider()
        {
            LastEntities = new ReadOnlyCollection<GameObject>(new List<GameObject>());
        }

        internal static DDTargetingProvider Instance => _instance ?? (_instance = new DDTargetingProvider());

        private static GetObjectsByWeightDel GetObjectsByWeight => Constants.SelectedDungeon.GetObjectsByWeight;

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
                        return !LastEntities.Any(i =>
                            (i.NpcId == EntityNames.Hidden || i.NpcId == EntityNames.BandedCoffer) &&
                            !Blacklist.Contains(i.ObjectId, (BlacklistFlags) DeepDungeonManager.Level));

                    //Logger.Instance.Verbose("Full Explore : {0} {1}", _levelComplete, !NotMobs().Any());
                    return true;
                }

                if (Settings.Instance.GoExit)
                {
                    if (Settings.Instance.GoForTheHoard)
                        return !LastEntities.Any(i =>
                            (i.NpcId == EntityNames.Hidden || i.NpcId == EntityNames.BandedCoffer) &&
                            !Blacklist.Contains(i.ObjectId, (BlacklistFlags) DeepDungeonManager.Level));

                    return true;
                }

                return !LastEntities.Any();
            }
        }

        /// <summary>
        ///     decide what we need to do
        /// </summary>
        public GameObject FirstEntity => LastEntities.FirstOrDefault();

        internal void Reset()
        {
            Blacklist.Clear(i => true);
        }

        internal void Pulse()
        {
            if (CommonBehaviors.IsLoading)
                return;
            
            if (!DutyManager.InInstance)
                return;

            if (DeepDungeonManager.Director.TimeLeftInDungeon == TimeSpan.Zero)
                return;

            if (!Constants.InDeepDungeon)
                return;

            if (_floor != DeepDungeonManager.Level)
            {
                Logger.Info("Level has Changed. Clearing Targets");
                _floor = DeepDungeonManager.Level;
                Blacklist.Clear(i => i.Flags == (BlacklistFlags) DeepDungeonManager.Level);
                DeepDungeonManager.PomanderChange();
            }

            using (new PerformanceLogger("Targeting Pulse",true))
            {
                LastEntities = new ReadOnlyCollection<GameObject>(GetObjectsByWeight());
            }
            
            if (_lastPulse + TimeSpan.FromSeconds(5) < DateTime.Now)
            {
                Logger.Verbose($"Found {LastEntities.Count} Targets");
                _lastPulse = DateTime.Now;
            }
            
        }

        internal void AddToBlackList(GameObject obj, string reason)
        {
            AddToBlackList(obj, TimeSpan.FromMinutes(3), reason);
        }

        internal void AddToBlackList(GameObject obj, TimeSpan time, string reason)
        {
            Blacklist.Add(obj, (BlacklistFlags) _floor, time, reason);
            Poi.Clear(reason);
        }

        public static bool FilterKnown(GameObject obj)
        {
            if (obj.Location == Vector3.Zero)
                return false;
            //Blacklists
            if (Blacklist.Contains(obj) || Constants.TrapIds.Contains(obj.NpcId) ||
                Constants.IgnoreEntity.Contains(obj.NpcId))
                return false;

            switch (obj.Type)
            {
                case GameObjectType.Treasure:
                    return true;
                case GameObjectType.EventObject:
                    return true;
                case GameObjectType.BattleNpc:
                    return true;
                default:
                    return false;
            }
        }

        private delegate List<GameObject> GetObjectsByWeightDel();
    }
}