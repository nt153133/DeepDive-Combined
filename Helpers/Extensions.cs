/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using ff14bot.Enums;
using ff14bot.Objects;

namespace Deep.Helpers
{
    internal static class Extensions
    {
        /// <summary>
        ///     Determines if a player is using a tank role job/class.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns true when the player is using a tank job/class</returns>
        internal static bool IsTank(this ClassJobType type)
        {
            switch (type)
            {
                case ClassJobType.DarkKnight:
                case ClassJobType.Gunbreaker:
                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Determines if a player is using a healer role job/class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsHealer(this ClassJobType type)
        {
            switch (type)
            {
                case ClassJobType.Astrologian:
                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                case ClassJobType.Scholar:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsCaster(this ClassJobType type)
        {
            return type.IsHealer() || type == ClassJobType.Arcanist || type == ClassJobType.BlackMage ||
                   type == ClassJobType.Conjurer || type == ClassJobType.Summoner || type == ClassJobType.Thaumaturge || type == ClassJobType.RedMage;
        }

        /// <summary>
        ///     is the job melee
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsMelee(this ClassJobType type)
        {
            switch (type)
            {
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                case ClassJobType.Pugilist:
                case ClassJobType.Monk:
                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                case ClassJobType.Lancer:
                case ClassJobType.Dragoon:
                case ClassJobType.Rogue:
                case ClassJobType.Ninja:
                case ClassJobType.Dancer:
                case ClassJobType.Samurai:
                case ClassJobType.DarkKnight:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     is a dow character
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsDow(this ClassJobType type)
        {
            return type != ClassJobType.Adventurer &&
                   type != ClassJobType.Alchemist &&
                   type != ClassJobType.Armorer &&
                   type != ClassJobType.Blacksmith &&
                   type != ClassJobType.Botanist &&
                   type != ClassJobType.Carpenter &&
                   type != ClassJobType.Culinarian &&
                   type != ClassJobType.Fisher &&
                   type != ClassJobType.Goldsmith &&
                   type != ClassJobType.Leatherworker &&
                   type != ClassJobType.Miner &&
                   type != ClassJobType.Weaver;
        }

        internal static bool IsDow(this LocalPlayer player)
        {
            return player.CurrentJob.IsDow();
        }
    }
}