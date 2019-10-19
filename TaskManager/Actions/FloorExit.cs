/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */
using Buddy.Coroutines;
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
using System.Linq;
using System.Threading.Tasks;

namespace Deep.TaskManager.Actions
{
    class FloorExit : ITask
    {
        public string Name => "Floor Exit";

        private Poi Target => Poi.Current;

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
            } else
            {
                await CommonTasks.StopMoving();
            }
            
            var _level = DeepDungeonManager.Level;
            await Coroutine.Wait(-1, () => Core.Me.InCombat || _level != DeepDungeonManager.Level || CommonBehaviors.IsLoading || QuestLogManager.InCutscene);
            Poi.Clear("Floor has changed or we have entered combat");
            Navigator.Clear();
            return true;
        }

        private int Level = 0;
        private Vector3 location = Vector3.Zero;

        

        public void Tick()
        {
            if (!Constants.InDeepDungeon || CommonBehaviors.IsLoading || QuestLogManager.InCutscene)
                return;

            if (location == Vector3.Zero || Level != DeepDungeonManager.Level)
            {
               
                var ret = GameObjectManager.GetObjectByNPCId(EntityNames.OfPassage);
                if (ret != null)
                {
                    Level = DeepDungeonManager.Level;
                    location = ret.Location;
                    
                }
                else if (Level != DeepDungeonManager.Level)
                {
                    Level = DeepDungeonManager.Level;
                    location = Vector3.Zero;
                }
            }

            //if we are in combat don't move toward the carn of return
            if (Poi.Current != null && (Poi.Current.Type == PoiType.Kill || Poi.Current.Type == PoiType.Wait || Poi.Current.Type == PoiType.Collect))
                return;
            
            if (DDTargetingProvider.Instance.LevelComplete && !DeepDungeonManager.BossFloor && location != Vector3.Zero)
            {
                Poi.Current = new Poi(location, PoiType.Wait);
            }
        }
    }
}
