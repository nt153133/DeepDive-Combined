/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using ff14bot.Managers;
using ff14bot.Objects;

namespace DeepCombined.Helpers
{
    internal static class LocalPlayerExtension
    {
        /// <summary>
        ///     are we really in combat?
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal static bool InRealCombat(this LocalPlayer player)
        {
            return player.InCombat || GameObjectManager.NumberOfAttackers > 0;
        }
    }
}