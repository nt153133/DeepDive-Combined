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
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities.Helpers;
using DeepCombined.Enums;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using ff14bot;
using ff14bot.Directors;
using ff14bot.Managers;

namespace DeepCombined.Tasks
{
    internal static class Common
    {
        internal static ItemState PomanderState = ItemState.None;
        private static readonly Dictionary<Pomander, WaitTimer> PomanderLockoutTimers = new Dictionary<Pomander, WaitTimer>();

        static Common()
        {
            foreach (Pomander item in Enum.GetValues(typeof(Pomander)).Cast<Pomander>())
            {
                PomanderLockoutTimers.Add(item, new WaitTimer(TimeSpan.FromSeconds(3)));
            }
        }

        private static List<uint> PotIDs => Constants.Potions.Keys.ToList();

        /// <summary>
        ///     Cancel player aura
        /// </summary>
        /// <param name="aura">Aura id to cancel</param>
        /// <returns></returns>
        internal static async Task<bool> CancelAura(uint aura)
        {
            string auraname = $"\"{DataManager.GetAuraResultById(aura).CurrentLocaleName}\"";
            while (Core.Me.HasAura(aura))
            {
                Logger.Verbose("Cancel Aura {0}", auraname);
                ChatManager.SendChat("/statusoff " + auraname);
                await Coroutine.Yield();
            }

            return true;
        }

        internal static async Task<bool> UsePomander(Pomander number, uint auraId = 0)
        {
            if (Core.Me.HasAura(Auras.ItemPenalty) && number != Pomander.Serenity)
            {
                return false;
            }

            //cannot use pomander while under the auras of rage / lust
            if (Core.Me.HasAnyAura(Auras.Lust, Auras.Rage))
            {
                return false;
            }

            DDInventoryItem data = DeepDungeonManager.GetInventoryItem(number);
            if (data.Count == 0)
            {
                return false;
            }

            if (data.HasAura)
            {
                return false;
            }

            if (Core.Me.HasAura(auraId) && Core.Me.GetAuraById(auraId).TimespanLeft > TimeSpan.FromMinutes(1))
            {
                return false;
            }

            WaitTimer lockoutTimer = PomanderLockoutTimers[number];
            if (!lockoutTimer.IsFinished)
            {
                return false;
            }

            await Coroutine.Wait(5000, () => !DeepDungeonManager.IsCasting);

            byte cnt = data.Count;
            await Coroutine.Wait(5000, () => !DeepDungeonManager.IsCasting);
            WaitTimer wt = new WaitTimer(TimeSpan.FromSeconds(30));
            wt.Reset();
            while (cnt == data.Count && !wt.IsFinished)
            {
                Logger.Verbose($"Using Pomander: {number}");
                DeepDungeonManager.UsePomander(number);
                await Coroutine.Sleep(150);

                await Coroutine.Wait(5000, () => !DeepDungeonManager.IsCasting);
                DeepDungeonManager.PomanderChange();
                data = DeepDungeonManager.GetInventoryItem(number);
            }

            //Wait a little so we don't trigger the anti-stuck
            await Coroutine.Sleep(1000);

            //TODO this is probabbly stored somewhere in the client...
            switch (number)
            {
                case Pomander.Rage:
                    PomanderState = ItemState.Rage;
                    break;

                case Pomander.Lust:
                    PomanderState = ItemState.Lust;
                    break;

                case Pomander.Resolution:
                    PomanderState = ItemState.Resolution;
                    break;
            }

            lockoutTimer.Reset();

            return true;
        }

        /// <summary>
        ///     can we use a potion
        /// </summary>
        /// <returns></returns>
        internal static bool CanUsePot()
        {
            BagSlot pot = InventoryManager.FilledSlots.FirstOrDefault(r => PotIDs.Contains(r.RawItemId));
            return pot != null && pot.CanUse();
        }

        internal static bool CanUseItem(int itemid)
        {
            BagSlot pot = InventoryManager.FilledSlots.FirstOrDefault(r => r.RawItemId == itemid);
            return pot != null && pot.CanUse();
        }

        internal static async Task<bool> UseSustain()
        {
            if (!Settings.Instance.UseSustain)
            {
                return false;
            }

            if (Core.Me.CurrentHealthPercent > 50)
            {
                return false;
            }

            if (Core.Me.HasAura(Auras.Sustain))
            {
                return false;
            }

            BagSlot i = InventoryManager.FilledSlots.FirstOrDefault(r => r.RawItemId == Items.SustainingPotion);
            if (i == null)
            {
                return false;
            }

            return i.CanUse() && await UseItem(i);
        }

        /// <summary>
        ///     Recover hp.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        internal static async Task<bool> UsePots(bool force = false)
        {
            ff14bot.Objects.LocalPlayer localPlayer = Core.Me;
            if (localPlayer.CurrentHealthPercent > 99)
            {
                return await Task.FromResult(false);
            }

            uint currentHealth = localPlayer.CurrentHealth;
            uint maxHealth = localPlayer.MaxHealth;
            float healthPercent = currentHealth / (float)maxHealth;
            float healthDelta = 100 - healthPercent;
            healthPercent *= 100;
            if (healthPercent <= 80)
            {
                Tuple<Potion, BagSlot> strongestPotion = InventoryManager.FilledSlots.ToList().Where(r => Constants.Potions.ContainsKey(r.RawItemId)).Select(r => new Tuple<Potion, BagSlot>(Constants.Potions[r.RawItemId], r))
                    .OrderByDescending(r => r.Item1.EffectiveHPS(maxHealth, r.Item2.IsHighQuality)).FirstOrDefault();

                if (strongestPotion != null)
                {
                    if (!force && healthDelta < strongestPotion.Item1.Rate[strongestPotion.Item2.IsHighQuality ? 1 : 0])
                    {
                        Logger.Info($"Waiting for health to drop lower before we use {strongestPotion.Item2.Item} to recover {strongestPotion.Item1.EffectiveMax(maxHealth, strongestPotion.Item2.IsHighQuality)} hp");
                        return await Task.FromResult(false);
                    }

                    if (strongestPotion.Item2.CanUse())
                    {
                        Logger.Info($"Attempting to recover: {strongestPotion.Item1.EffectiveMax(maxHealth, strongestPotion.Item2.IsHighQuality)} hp via {strongestPotion.Item2.Item}");

                        strongestPotion.Item2.UseItem();
                        return await Task.FromResult(true);
                    }
                }
            }

            return await Task.FromResult(false);
        }

        internal static async Task<bool> UseItemById(int id)
        {
            BagSlot pot = InventoryManager.FilledSlots.FirstOrDefault(r => r.RawItemId == id);
            if (pot != null && pot.CanUse() && await UseItem(pot))
            {
                return true;
            }

            return false;
        }

        private static async Task<bool> UseItem(BagSlot i)
        {
            Logger.Warn("Attempting to use {0}", i.Item.CurrentLocaleName);
            i.UseItem();
            await Coroutine.Yield();
            return true;
        }
    }
}