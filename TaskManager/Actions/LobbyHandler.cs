/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.RemoteWindows;

namespace DeepCombined.TaskManager.Actions
{
    internal class LobbyHandler : ITask
    {
        private GameObject _target;

        public string Name => "Lobby";

        public async Task<bool> Run()
        {
            if (WorldManager.ZoneId != Constants.SelectedDungeon.LobbyId) return false;

            await Coroutine.Sleep(5000);

            _target = GameObjectManager.GameObjects.Where(r => r.NpcId == EntityNames.LobbyExit).OrderBy(r => r.Distance()).FirstOrDefault();

            Navigator.Stop();
            Navigator.Clear();

            TreeRoot.StatusText = "Lobby Room";

            if (_target == null || !_target.IsValid)
            {
                Logger.Warn("Unable to find Lobby Target");
                return false;
            }

            // move closer plz
            if (_target.Location.Distance2D(Core.Me.Location) >= 4.4)
            {
                Logger.Verbose("target range" + _target.Location.Distance2D(Core.Me.Location));

                Navigator.Stop();

                Navigator.PlayerMover.MoveTowards(_target.Location);
                while (_target.Location.Distance2D(Core.Me.Location) >= 4.4)
                {
                    Navigator.PlayerMover.MoveTowards(_target.Location);
                    await Coroutine.Sleep(100);
                }

                //await Buddy.Coroutines.Coroutine.Sleep(1500); // (again, probably better to just wait until distance to destination is < 2.0f or something)
                Navigator.PlayerMover.MoveStop();
            }

            _target.Interact();
            await Coroutine.Wait(500, () => SelectYesno.IsOpen);
            await Coroutine.Sleep(1000);
            SelectYesno.ClickYes();
            DeepTracker.EndRun(false);
            return true;
        }

        public void Tick()
        {
            if (_target != null && !_target.IsValid) _target = null;
            if (WorldManager.ZoneId != Constants.SelectedDungeon.LobbyId) return;

            _target = GameObjectManager.GetObjectByNPCId(EntityNames.LobbyExit);
        }
    }
}