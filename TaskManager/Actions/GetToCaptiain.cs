/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.Pathing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deep.Helpers.Logging;
using ff14bot.NeoProfiles;

namespace Deep.TaskManager.Actions
{
    class GetToCaptiain : ITask
    {
        public string Name => "GetToCaptain";

        public async Task<bool> Run()
        {
            //we are inside POTD
            if (Constants.InDeepDungeon || Constants.InExitLevel) return false;

            if (WorldManager.ZoneId != Constants.EntranceZoneId ||
               GameObjectManager.GetObjectByNPCId(Constants.EntranceNpcId) == null ||
               GameObjectManager.GetObjectByNPCId(Constants.EntranceNpcId).Distance2D(Core.Me.Location) > 110)
            {
                if (Core.Me.IsCasting)
                {
                    await Coroutine.Sleep(1000);
                    return true;
                }

                if (!ConditionParser.HasAetheryte(Constants.EntranceZone.Id))
                {
                    Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. You don't have that Aetheryte so do something about it...");
                    TreeRoot.Stop();
                    return false;
                }
                
                if (!WorldManager.TeleportById(Constants.EntranceZone.Id))
                {
                    Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. something is very wrong...");
                    TreeRoot.Stop();
                    return false;
                }
                await Coroutine.Sleep(5000);
                return true;

            }

            if (GameObjectManager.GetObjectByNPCId(Constants.EntranceNpcId) != null &&
                !(Constants.EntranceNpcPosition.Distance2D(Core.Me.Location) > 5f)) return false;
            Logger.Verbose("at Move");

            if (GameObjectManager.GetObjectByNPCId(Constants.EntranceNpcId) != null)
                return await CommonTasks.MoveAndStop(
                    new MoveToParameters(GameObjectManager.GetObjectByNPCId(Constants.EntranceNpcId).Location,
                        "Moving toward NPC"), 5f, true);

            return await CommonTasks.MoveAndStop(
                new MoveToParameters(Constants.EntranceNpcPosition, "Moving toward NPC"), 5f, true);
        }

        public void Tick()
        {
            
        }
    }
}
