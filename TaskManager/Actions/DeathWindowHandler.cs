/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System.Threading.Tasks;
using Buddy.Coroutines;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace DeepCombined.TaskManager.Actions
{
    internal class DeathWindowHandler : ITask
    {
        public string Name => "Death Window";

        public async Task<bool> Run()
        {
            if (RaptureAtkUnitManager.GetWindowByName("DeepDungeonResult") != null && Core.Me.IsDead)
            {
                GameStatsManager.Died();
                Logger.Warn("We have died...");
                
                DeepTracker.Died();
                DeepTracker.EndRun(true);
                RaptureAtkUnitManager.GetWindowByName("DeepDungeonResult").SendAction(1, 3, uint.MaxValue);
                await Coroutine.Sleep(250);
                return true;
            }
            
            if (RaptureAtkUnitManager.GetWindowByName("DeepDungeonResult") != null)
            {
                //GameStatsManager.Died();
                //Logger.Warn("We have died...");
                
                //DeepTracker.Died();
                DeepTracker.EndRun(false);
                RaptureAtkUnitManager.GetWindowByName("DeepDungeonResult").SendAction(1, 3, uint.MaxValue);
                await Coroutine.Sleep(250);
                return true;
            }

            if (NotificationRevive.IsOpen)
            {
                NotificationRevive.Click();
                await Coroutine.Wait(250, () => SelectYesno.IsOpen);
                SelectYesno.ClickYes();
                return true;
            }

            if (ClientGameUiRevive.ReviveState == ReviveState.Dead && SelectYesno.IsOpen)
            {
                SelectYesno.ClickYes();
                return true;
            }

            if (Core.Me.IsDead)
            {
                TreeRoot.StatusText = "I am dead. No window to use...";
                await Coroutine.Sleep(250);
                return true;
            }

            return false;
        }

        public void Tick()
        {
        }
    }
}