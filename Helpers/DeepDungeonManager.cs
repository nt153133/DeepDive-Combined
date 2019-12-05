/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using DeepCombined.Helpers.Logging;
using DeepCombined.Windows;
using ff14bot;
using ff14bot.Directors;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.RemoteAgents;

namespace DeepCombined.Helpers
{
    /// <summary>
    ///     helper class for wrapping director calls
    /// </summary>
    public static class DeepDungeonManager
    {
        private static DDInventoryItem[] _inventory;

        static DeepDungeonManager()
        {
            PomanderChange();
        }

        public static InstanceContentDirector Director => DirectorManager.ActiveDirector as InstanceContentDirector;

        //public static event EventHandler OnInventoryChange;

        public static bool BossFloor => Director != null && Director.DeepDungeonLevel % 10 == 0;
        public static bool NextFloorIsBossFloor => Director != null && Director.DeepDungeonLevel % 10 == 9;

        public static bool IsCasting => Core.Me.IsCasting;

        public static int PortalStatus => Director.DeepDungeonPortalStatus;
        public static int Level => Director.DeepDungeonLevel;

        public static bool PortalActive => Director.DeepDungeonPortalStatus == 11;
        public static bool ReturnActive => Director.DeepDungeonReturnStatus == 11;

        public static DDInventoryItem GetInventoryItem(Pomander pom)
        {
            //return Director.DeepDungeonInventory[(byte) pom - 1];
            return _inventory[(byte) Constants.PomanderInventorySlot(pom)];
        }

        public static void UsePomander(Pomander pom)
        {
            AgentModule.GetAgentInterfaceByType<AgentDeepDungeonInformation>().UsePomander(pom);
            Navigator.NavigationProvider.ClearStuckInfo(); // don't trigger antistuck
            PomanderChange();
        }

        public static void PomanderChange()
        {
            _inventory = Director.DeepDungeonInventory;
        }

        public static int GetMagiciteCount()
        {
            return Core.Memory.Read<byte>(Director.Pointer + 5160 + 3);
        }

        public static void CastMagicite()
        {
            if (GetMagiciteCount() <= 1) return;
            
            Logger.Warn("Casting Magicite");
            DeepDungeonStatus.Instance.CastMagicite();
        }
    }
}