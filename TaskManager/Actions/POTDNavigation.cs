/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using Clio.Utilities;
using Deep.Helpers;
using Deep.Memory;
using Deep.Providers;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deep.TaskManager.Actions
{
    internal class POTDNavigation : ITask
    {
        private int level = 0;

        private List<Vector3> SafeSpots;

        private int PortalPercent => Constants.Percent[DeepDungeonManager.PortalStatus];

        private Poi Target => Poi.Current;
        public string Name => "PotdNavigator";

        public async Task<bool> Run()
        {
            if (!Constants.InDeepDungeon)
                return false;


            if (Target == null)
                return false;


            if (Target.Location == Vector3.Zero)
                return true;

            if (Navigator.InPosition(Core.Me.Location, Target.Location, 3f) &&
                Target.Type == (PoiType) PoiTypes.ExplorePOI)
            {
                Poi.Clear("We have reached our destination");
                return true;
            }

            var status =
                $"Current Level {DeepDungeonManager.Level}. Level Status: {PortalPercent}% \"Done\": {DDTargetingProvider.Instance.LevelComplete}";
            TreeRoot.StatusText = status;

            if (ActionManager.IsSprintReady && Target.Location.Distance2D(Core.Me.Location) > 5 &&
                MovementManager.IsMoving)
            {
                ActionManager.Sprint();
                return true;
            }

            var res = await CommonTasks.MoveAndStop(
                new MoveToParameters(Target.Location, "Moving toward POTD Objective:" + Target.Name), 1.5f);

            //if (Target.Unit != null)
            //{
            //    Logger.Verbose($"[PotdNavigator] Move Results: {res} Moving To: \"{Target.Unit.Name}\" LOS: {Target.Unit.InLineOfSight()}");
            //}
            //else
            //{
            //    Logger.Verbose($"[PotdNavigator] Move Results: {res} Moving To: \"{Target.Name}\" ");
            //}


            return res;
        }


        public void Tick()
        {
            if (!Constants.InDeepDungeon || CommonBehaviors.IsLoading || QuestLogManager.InCutscene)
                return;

            if (level != DeepDungeonManager.Level)
            {
                level = DeepDungeonManager.Level;
                SafeSpots = new List<Vector3>();
                SafeSpots.AddRange(GameObjectManager.GameObjects.Where(DDTargetingProvider.FilterKnown)
                    .Select(i => i.Location));
            }

            if (!SafeSpots.Any(i => i.Distance2D(Core.Me.Location) < 5))
                SafeSpots.Add(Core.Me.Location);


            if ((Poi.Current == null || Poi.Current.Type == PoiType.None) && !DeepDungeonManager.BossFloor)
                Poi.Current = new Poi(SafeSpots.OrderByDescending(i => i.Distance2D(Core.Me.Location)).First(),
                    (PoiType) PoiTypes.ExplorePOI);
        }
    }
}