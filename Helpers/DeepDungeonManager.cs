/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Runtime.CompilerServices;
using Buddy.Coroutines;
using Deep.Helpers.Logging;
using ff14bot;
using ff14bot.Directors;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.RemoteAgents;

namespace Deep.Helpers
{
    /// <summary>
    ///     helper class for wrapping director calls
    /// </summary>
    public static class DeepDungeonManager
    {
        public static InstanceContentDirector Director => DirectorManager.ActiveDirector as InstanceContentDirector;
        
        //public static event EventHandler OnInventoryChange;

        public static bool BossFloor => Director != null && Director.DeepDungeonLevel % 10 == 0;
        public static bool NextFloorIsBossFloor => Director != null && Director.DeepDungeonLevel % 10 == 9;

        public static bool IsCasting => Core.Me.IsCasting;

        public static int PortalStatus => Director.DeepDungeonPortalStatus;
        public static int Level => Director.DeepDungeonLevel;

        public static bool PortalActive => Director.DeepDungeonPortalStatus == 11;
        public static bool ReturnActive => Director.DeepDungeonReturnStatus == 11;

        private static DDInventoryItem[] _inventory;

        static DeepDungeonManager()
        {
            PomanderChange();
        }

        public static DDInventoryItem GetInventoryItem(Pomander pom)
        {
            using (new PerformanceLogger($"GetInventoryItem", true))
            {
                //return Director.DeepDungeonInventory[(byte) pom - 1];
                return _inventory[(byte) Constants.PomanderInventorySlot(pom)];
            }
        }

        public static void UsePomander(Pomander pom)
        {
            using (new PerformanceLogger($"UsePomander", true))
            {
                AgentModule.GetAgentInterfaceByType<AgentDeepDungeonInformation>().UsePomander(pom);
                Navigator.NavigationProvider.ClearStuckInfo(); // don't trigger antistuck
                PomanderChange();
            }
        }

        public static void PomanderChange()
        {
            using (new PerformanceLogger($"PomanderChange", true))
            {
                _inventory = Director.DeepDungeonInventory;
                
            }
        }
    }
}