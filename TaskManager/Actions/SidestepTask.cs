/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace DeepCombined.TaskManager.Actions
{
    public class SideStepTask : ITask
    {
        private static FieldInfo MoveTo;

        static SideStepTask()
        {
            MoveTo = typeof(AvoidanceManager).GetFields(BindingFlags.Static | BindingFlags.NonPublic).First(i => !i.IsInitOnly && i.FieldType == typeof(Vector3));
        }

        public string Name => nameof(SideStepTask);


        public async Task<bool> Run()
        {
            bool supportsCapabilities = RoutineManager.Current.SupportedCapabilities != CapabilityFlags.None;

            if (AvoidanceManager.IsRunningOutOfAvoid && Core.Me.IsCasting)
            {
                ActionManager.StopCasting();
                return true;
            }

            if (AvoidanceManager.IsRunningOutOfAvoid && !supportsCapabilities)
            {
                return true;
            }

            PoiType poiType = Poi.Current.Type;

            // taken from HB
            // Special case: Bot will do a lot of fast stop n go when avoiding a mob that moves slowly and trying to
            // do something near the mob. To fix, a delay is added to slow down the 'Stop n go' behavior
            if (poiType == PoiType.Collect || poiType == PoiType.Gather || poiType == PoiType.Hotspot)
            {
                if (Core.Me.InCombat && AvoidanceManager.Avoids.Any(o => o.IsPointInAvoid(Poi.Current.Location)))
                {
                    TreeRoot.StatusText = "Waiting for 'avoid' to move before attempting to interact " +
                                          Poi.Current.Name;
                    int randomWaitTime = new Random().Next(3000, 8000);
                    await Coroutine.Wait(randomWaitTime,
                        () => Core.Me.InCombat ||
                              !AvoidanceManager.Avoids.Any(o => o.IsPointInAvoid(Poi.Current.Location)));
                }
            }

            return false;
        }

        public void Tick()
        {
        }


        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}