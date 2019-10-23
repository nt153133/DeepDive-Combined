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
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.RemoteWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deep.Helpers.Logging;

namespace Deep.TaskManager.Actions
{
    class LobbyHandler : ITask
    {
        private GameObject _target;

        public string Name => "Lobby";

        public async Task<bool> Run()
        {
            if (WorldManager.ZoneId != 570) return false;
            TreeRoot.StatusText = "Lobby Room";
            if (_target == null || !_target.IsValid)
            {
                Logger.Warn($"Unable to find Lobby Target");
                return false;
            }
            if(!Navigator.InPosition(_target.Location, Core.Me.Location, 3))
            {
                if (!await CommonTasks.MoveAndStop(new MoveToParameters(_target.Location, "Moving to Lobby Exit"), 3))
                {
                    Logger.Warn("Failed to move toward the exit?");
                }
                return true;
            }
            _target.Interact();
            await Coroutine.Wait(250, () => SelectYesno.IsOpen);
            SelectYesno.ClickYes();
            return true;
        }

        public void Tick()
        {
            if(_target != null && !_target.IsValid)
            {
                _target = null;
            }
            if (WorldManager.ZoneId != 570) return;
            _target = GameObjectManager.GameObjects.Where(i => i.NpcId == EntityNames.LobbyExit)
                       .OrderBy(i => i.Distance2D(Core.Me.Location)).FirstOrDefault();
        }
    }
}
