/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using Buddy.Coroutines;
using Deep.Enums;
using Deep.Helpers;
using Deep.Memory;
using ff14bot;
using ff14bot.Directors;
using ff14bot.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deep.Helpers.Logging;
using ff14bot.Behavior;
using static Deep.Tasks.Common;

namespace Deep.TaskManager.Actions
{
    internal class Pomanders : ITask
    {
        private bool _runbuf = false;

        /// <summary>
        ///     stores the floor # for the level we last removed traps from
        /// </summary>
        private int _trapPomanderUsageCheck = 0;

        private int PortalPercent => Constants.Percent[DeepDungeonManager.PortalStatus];
        public string Name => "Pomanders";

        public async Task<bool> Run()
        {
            if (!Constants.InDeepDungeon)
                return false;

            if (PortalPercent < 10)
                if (await BuffCurrentFloor())
                    return true;

            if (DeepDungeonManager.BossFloor && !Core.Me.InCombat) return await BuffBoss();

            _runbuf = false;
            return await BuffMe();
        }

        public void Tick()
        {
        }

        /// <summary>
        ///     buff the stuff
        /// </summary>
        /// <returns></returns>
        private async Task<bool> BuffBoss()
        {
            if (Core.Me.HasAura(Auras.Lust) || _runbuf)
                return false;

            _runbuf = true;
            if (Core.Me.HasAura(Auras.Enervation) || Core.Me.HasAura(Auras.Silence)) return true;

            if (DeepDungeonManager.PortalActive) return false;
            
            await UsePomander(Pomander.Strength, Auras.Strength);
            await UsePomander(Pomander.Steel, Auras.Steel);

            //Move To SelectedDungeonLogic Boss()
            await LustLogic();

            return false;
        }

        private static async Task LustLogic()
        {
            var lust = false;
            var itm = DeepDungeonManager.GetInventoryItem(Pomander.Lust);
            Logger.Info("[LUST] Item Count: {0}", itm.Count);

            //we are inside the dungeon, should be ok to use InParty here.
            if (PartyManager.IsInParty)
            {
                Logger.Info("In A Party. Doing Lust Logic...");
                var lustFound = false;
                foreach (var k in PartyManager.AllMembers)
                    if (!k.Class.IsHealer() && !k.Class.IsTank())
                    {
                        lustFound = true;
                        if (k.IsMe)
                            lust = true;
                        break;
                    }

                Logger.Info("Party Lust status: {0} :: {1} :: {2}", !lust, !lustFound, PartyManager.IsPartyLeader);
                if (!lust && !lustFound) lust = PartyManager.IsPartyLeader;
            }
            else
            {
                Logger.Info("Solo Lust Logic");
                lust = true;
            }

            if (lust)
            {
                Logger.Info("Use Pomander Debug: [HasAura: {0}]", itm.HasAura);
                await UsePomander(Pomander.Lust, Auras.Lust);
            }
        }


        /// <summary>
        ///     Player pomander buffs
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> BuffMe()
        {
            if (Settings.Instance.UsePomRage && CombatTargeting.Instance.LastEntities.Count() > 5 &&
                !Core.Me.HasAura(Auras.Rage)) return await UsePomander(Pomander.Rage, Auras.Lust);


            if (Core.Me.HasAura(Auras.ItemPenalty))
                return false;

            if (await UsePomander(Pomander.Raising))
                return true;


            if (await UsePomander(Pomander.Intuition))
                return true;

            if (!Settings.Instance.SaveSteel || DeepDungeonManager.GetInventoryItem(Pomander.Steel).Count > 1)
                if (await UsePomander(Pomander.Steel, Auras.Steel))
                    return true;

            if (!Settings.Instance.SaveStr || DeepDungeonManager.GetInventoryItem(Pomander.Strength).Count > 1)
                if (await UsePomander(Pomander.Strength, Auras.Strength))
                    return true;

            if (Core.Me.HasAura(Auras.Pox))
                if (await UsePomander(Pomander.Purity))
                    return true;

            return false;
        }

        /// <summary>
        ///     buffs the start of the level
        /// </summary>
        /// <returns></returns>
        private async Task<bool> BuffCurrentFloor()
        {
            //do not buff the floor if there is a boss...
            if (DeepDungeonManager.BossFloor) return false;

            if (PartyManager.IsPartyLeader &&
                (Core.Me.HasAura(Auras.Amnesia) || Core.Me.HasAura(Auras.ItemPenalty) ||
                 Core.Me.HasAura(Auras.NoAutoHeal)))
                await UsePomander(Pomander.Serenity);

            if (await Traps())
                return true;
            if (await UsePomander(Pomander.Fortune))
                return true;
            if (await UsePomander(Pomander.Intuition))
                return true;

            if (await BuffNextFloor())
                return true;

            return false;
        }

        /// <summary>
        ///     remove traps
        ///     only uses one of the two trap removers per level
        /// </summary>
        /// <returns></returns>
        private async Task<bool> Traps()
        {
            if (Core.Me.HasAura(Auras.ItemPenalty)) return false;

            if (DeepDungeonManager.PortalActive || _trapPomanderUsageCheck == DeepDungeonManager.Level) return false;

            if (await UsePomander(Pomander.Safety))
            {
                _trapPomanderUsageCheck = DeepDungeonManager.Level;
                return true;
            }

            if (await UsePomander(Pomander.Sight))
            {
                _trapPomanderUsageCheck = DeepDungeonManager.Level;
                return true;
            }

            return false;
        }


        /// <summary>
        ///     buffs for the next floor
        /// </summary>
        /// <returns></returns>
        private async Task<bool> BuffNextFloor()
        {
            if (Core.Me.HasAura(Auras.ItemPenalty) || DeepDungeonManager.BossFloor ||
                DeepDungeonManager.NextFloorIsBossFloor)
                return false;

            if (await UsePomander(Pomander.Flight))
                return true;

            if (Settings.Instance.UsePomAlt)
                if (await UsePomander(Pomander.Alteration))
                    return true;

            return await UsePomander(Pomander.Affluence);
        }
    }
}