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
using System.Text;
using Deep.Helpers;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Deep.Helpers
{
    internal static class Extensions
    {
        
        /// <summary>
        /// Determines if a player is using a tank role job/class.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns true when the player is using a tank job/class</returns>
        internal static bool IsTank(this ClassJobType type)
        {
            if (type == ClassJobType.DarkKnight)
                return true;
            if (type == ClassJobType.Marauder || type == ClassJobType.Warrior)
                return true;
            if (type == ClassJobType.Gladiator || type == ClassJobType.Paladin)
                return true;
            return false;
        }

        /// <summary>
        /// Determines if a player is using a healer role job/class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsHealer(this ClassJobType type)
        {
            if (type == ClassJobType.Astrologian)
                return true;
            if (type == ClassJobType.Conjurer || type == ClassJobType.WhiteMage)
                return true;
            if (type == ClassJobType.Scholar)
                return true;

            return false;
        }

        internal static bool IsCaster(this ClassJobType type)
        {
            return type.IsHealer() || type == ClassJobType.Arcanist || type == ClassJobType.BlackMage ||
                   type == ClassJobType.Conjurer || type == ClassJobType.Summoner || type == ClassJobType.Thaumaturge;
        }
        /// <summary>
        /// is the job melee
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsMelee(this ClassJobType type)
        {

            if (type == ClassJobType.Gladiator || type == ClassJobType.Paladin)
            {
                return true;
            }
            if (type == ClassJobType.Pugilist || type == ClassJobType.Monk)
            {
                return true;
            }
            if (type == ClassJobType.Marauder || type == ClassJobType.Warrior)
            {
                return true;
            }
            if (type == ClassJobType.Lancer || type == ClassJobType.Dragoon)
            {
                return true;
            }

            if (type == ClassJobType.Rogue || type == ClassJobType.Ninja)
            {
                return true;
            }

            if (type == ClassJobType.DarkKnight)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// is a dow character
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

        internal static bool IsDow(this ff14bot.Objects.LocalPlayer player)
        {
            return player.CurrentJob.IsDow();
        }

    }
}