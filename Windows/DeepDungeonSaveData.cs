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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Deep.Structure;
using ff14bot.Enums;
using ff14bot.Managers;
using Deep.Helpers;
using Deep.Helpers.Logging;
using ff14bot;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;

namespace Deep.Windows
{
    internal class DeepDungeonSaveData
    {
        internal static AtkAddonControl Window() => RaptureAtkUnitManager.GetWindowByName(WindowNames.DDsave);

        internal static bool IsOpen => RaptureAtkUnitManager.GetWindowByName(WindowNames.DDsave) != null;

        private static AgentDeepDungeonSaveData SD => Constants.GetSaveInterface();
        /// <summary>
        /// clicks a save slot. number should be greater than 0
        /// </summary>
        /// <param name="number"></param>
        internal static async Task ClickSaveSlot(uint number)
        {
            if(number >= 2)
                throw new ArgumentOutOfRangeException();

            if (IsOpen && SD.Reset)
                await Close();

            if (DeepDungeonMenu.IsOpen)
                await DeepDungeonMenu.OpenSaveMenu();

            Logger.Info("Clicking Save slot {0} // {1}", number +1, SD.Reset);


            await Coroutine.Wait(5000, () => IsOpen);

            var window = RaptureAtkUnitManager.GetWindowByName(WindowNames.DDsave);
            window.SendAction(1, 3, number);

            await Coroutine.Yield();
        }

        /// <summary>
        /// Sends a reset command to a window
        /// </summary>
        internal static async Task ClickReset(uint number)
        {
            if (number >= 2)
                throw new ArgumentOutOfRangeException();

            if (IsOpen && !SD.Reset)
                await Close();

            if (DeepDungeonMenu.IsOpen)
                await DeepDungeonMenu.OpenResetMenu();


            Logger.Info("Clicking Reset slot {0} // {1}", number +1, SD.Reset);
            await Coroutine.Wait(5000,() => IsOpen);
            var window = RaptureAtkUnitManager.GetWindowByName(WindowNames.DDsave);
            window.SendAction(2, 3, number, 3, 2);
            await Coroutine.Wait(500, () => SelectYesno.IsOpen);
            //confirm that we want to delete this data.
            if (SelectYesno.IsOpen)
                SelectYesno.ClickYes();
        }


        
        /// <summary>
        /// close the window
        /// </summary>
        public static async Task Close()
        {
            if(IsOpen)
                RaptureAtkUnitManager.GetWindowByName(WindowNames.DDsave).SendAction(1, 3, uint.MaxValue);
            //await Coroutine.Sleep(1000); //these windows take a second
            await Coroutine.Wait(1500, () => DeepDungeonMenu.IsOpen);
        }

    }
}