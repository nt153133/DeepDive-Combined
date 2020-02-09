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
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using Clio.Utilities.Helpers;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using DeepCombined.Providers;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing;

namespace DeepCombined.TaskManager.Actions
{
    internal class FloorExit : ITask
    {
        public static Vector3 location = Vector3.Zero;
        private readonly WaitTimer _moveTimer = new WaitTimer(TimeSpan.FromMinutes(1));
        public static List<uint> blackList = new List<uint>();

        public static uint ExitObjectId = 0;
        private int Level;

        private Poi Target => Poi.Current;
        public string Name => "Floor Exit";

        public async Task<bool> Run()
        {
            if (Target.Type != PoiType.Wait)
                return false;

            //let the navigator handle movement if we are far away
            if (Target.Location.Distance2D(Core.Me.Location) > 3)
                return false;

            // move closer plz
            if (Target.Location.Distance2D(Core.Me.Location) >= 2)
            {
                await CommonTasks.MoveAndStop(new MoveToParameters(Target.Location, "Floor Exit"), 0.5f, true);
                return true;
            }

            await CommonTasks.StopMoving();
            _moveTimer.Reset();
            var _level = DeepDungeonManager.Level;
            
            if (!GameObjectManager.GameObjects.Any(r => r.NpcId == EntityNames.OfPassage))
                blackList.Clear();
            
            ExitObjectId = GameObjectManager.GameObjects.Where(r => r.NpcId == EntityNames.OfPassage).OrderBy(r => r.Distance()).FirstOrDefault().ObjectId;
            Logger.Info($"Exit object id is {ExitObjectId}");
            await Coroutine.Wait(-1,
                () => Core.Me.InCombat || _level != DeepDungeonManager.Level || CommonBehaviors.IsLoading ||
                      QuestLogManager.InCutscene || GameObjectManager.GetObjectByObjectId(ExitObjectId) == null|| _moveTimer.IsFinished || !GameObjectManager.GetObjectByObjectId(ExitObjectId).IsVisible);
            
            if (_moveTimer.IsFinished || GameObjectManager.GetObjectByObjectId(ExitObjectId) == null || !GameObjectManager.GetObjectByObjectId(ExitObjectId).IsVisible)
            {
                if (Poi.Current.Unit != null)
                {
                    if (!Poi.Current.Unit.IsValid)
                    {
                        Logger.Warn("Waited 2 minutes at exit: Blacklisting current exit for 10min not valid");
                        DDTargetingProvider.Instance.AddToBlackList(Poi.Current.Unit, TimeSpan.FromMinutes(10),
                            "Waited at exit(not valid) for 5 minutes");
                    }
                    else
                    {
                        Logger.Warn("Waited 2 minutes at exit: Blacklisting current exit for 5 min");
                        DDTargetingProvider.Instance.AddToBlackList(Poi.Current.Unit, TimeSpan.FromMinutes(5),
                            "Waited at exit for 5 minutes");
                    }
                }
                else
                {
                    Logger.Warn("Waited 2 minutes at exit but poi is null");
                }
                
                if (PartyManager.IsInParty && PartyManager.AllMembers.All(i=> i.BattleCharacter.Location.Distance2D(GameObjectManager.GetObjectByObjectId(ExitObjectId).Location)<10))
                    blackList.Add(ExitObjectId);
            }
            
            Poi.Clear("Floor has changed or we have entered combat");
            location = Vector3.Zero;
            Navigator.Clear();
            return true;
        }


        public void Tick()
        {
            if (!Constants.InDeepDungeon || CommonBehaviors.IsLoading || QuestLogManager.InCutscene)
                return;

            if (location == Vector3.Zero || Level != DeepDungeonManager.Level)
            {
                //var ret = GameObjectManager.GetObjectByNPCId(EntityNames.FloorExit);
                var ret = GameObjectManager.GameObjects.Where(r => r.NpcId == EntityNames.OfPassage && !blackList.Contains(r.ObjectId))
                    .OrderBy(r => r.Distance() ).FirstOrDefault();
                if (ret != null)
                {
                    Level = DeepDungeonManager.Level;
                    location = ret.Location;
                }
                else if (Level != DeepDungeonManager.Level)
                {
                    Level = DeepDungeonManager.Level;
                    location = Vector3.Zero;
                    blackList.Clear();
                }
            }

            if (location != Vector3.Zero)
            {
                var ret = GameObjectManager.GameObjects.Where(r => r.NpcId == EntityNames.OfPassage && !blackList.Contains(r.ObjectId))
                    .OrderBy(r => r.Distance()).FirstOrDefault();
                if (ret != null)
                    if (Core.Me.Location.Distance2D(ret.Location) < location.Distance2D(ret.Location))
                        location = ret.Location;
            }

            //if we are in combat don't move toward the carn of return
            if (Poi.Current != null && (Poi.Current.Type == PoiType.Kill || Poi.Current.Type == PoiType.Wait ||
                                        Poi.Current.Type == PoiType.Collect))
                return;

            // if (location != Vector3.Zero && Navigator.NavigationProvider.LookupPathInfo(GameObjectManager.GameObjects.Where(r => r.NpcId == EntityNames.FloorExit).OrderBy(r => r.Distance(location)).FirstOrDefault(), 3).Navigability != PathNavigability.Navigable)
            //     return;

            if (DDTargetingProvider.Instance.LevelComplete && !DeepDungeonManager.BossFloor && location != Vector3.Zero)
                Poi.Current = new Poi(location, PoiType.Wait);
        }
    }
}