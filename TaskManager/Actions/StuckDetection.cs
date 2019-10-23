/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */
using Clio.Utilities;
using Clio.Utilities.Helpers;
using Deep.Providers;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deep.Helpers.Logging;

namespace Deep.TaskManager.Actions
{
    class StuckDetection : ITask
    {
        public string Name => "Stuck Detection";

        public async Task<bool> Run()
        {
            if (MoveTimer.IsFinished && (Poi.Current != null && Poi.Current.Type != PoiType.None))
            {
                var path = StraightPathHelper.RealStraightPath();
                Logger.Info($"Dump path:");
                foreach(var x in path)
                {
                    Logger.Info(x.ToString());
                }

                Logger.Warn("No activity was detected for {0} seconds. Adding target to the blacklist and trying again", MoveTimer.WaitTime.TotalSeconds);
                if (Poi.Current.Unit != null)
                {
                    DDTargetingProvider.Instance.AddToBlackList(Poi.Current.Unit, TimeSpan.FromSeconds(3), "Navigation Error");
                }
                if(Poi.Current.Type != PoiType.None)
                    Poi.Clear("No activity detected");

                MoveTimer.Reset();
                return true;
            }
            else if (MoveTimer.IsFinished)
            {
                Logger.Warn("No activity was detected for {0} seconds. Clearing Navigator?", MoveTimer.WaitTime.TotalSeconds);
                await CommonTasks.StopMoving();
                Navigator.Clear();
                MoveTimer.Reset();
                return true;
            }

            return false;
        }

        internal readonly WaitTimer MoveTimer = new WaitTimer(TimeSpan.FromSeconds(15));
        private Vector3 _location = Vector3.Zero;
        private 
        const float DISTANCE = 0.25f;

        public void Tick()
        {

            var location = Core.Me.Location;
            if (location.DistanceSqr(_location) > DISTANCE)
            {
                _location = location;
                MoveTimer.Reset();
            }
        }
    }
}
