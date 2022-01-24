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
using Clio.Utilities;
using DeepCombined.Enums;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using DeepCombined.Providers;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Directors;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing;
using TreeSharp;

namespace DeepCombined.TaskManager.Actions
{
    internal class CombatHandler : ITask
    {
        private readonly SpellData LustSpell = DataManager.GetSpellData(Spells.LustSpell);
        private readonly SpellData PummelSpell = DataManager.GetSpellData(Spells.RageSpell);

        private readonly HashSet<uint> GazeAttacks = new HashSet<uint>
        {
            Spells.BlindingBurst1, Spells.BlindingBurst2,
            Spells.StoneGaze
        };

        public CombatHandler()
        {
            _preCombatLogic = new HookExecutor("PreCombatLogic", null, new ActionAlwaysFail());
            _preCombatBuff = new HookExecutor("PreCombatBuff", null, RoutineManager.Current.PreCombatBuffBehavior ?? new ActionAlwaysFail());
            _heal = new HookExecutor("Heal", null, RoutineManager.Current.HealBehavior ?? new ActionAlwaysFail());
            _pull = new HookExecutor("Pull", null, RoutineManager.Current.PullBehavior ?? new ActionAlwaysFail());
            _combatBuff = new HookExecutor("CombatBuff", null, RoutineManager.Current.CombatBuffBehavior ?? new ActionAlwaysFail());
            _combatBehavior = new HookExecutor("Combat", null, RoutineManager.Current.CombatBehavior ?? new ActionAlwaysFail());
            _rest = new HookExecutor("Rest", null, RoutineManager.Current.RestBehavior ?? new ActionAlwaysFail());
        }

        internal Composite _preCombatLogic { get; }
        internal Composite _preCombatBuff { get; }
        internal Composite _heal { get; }
        internal Composite _pull { get; }
        internal Composite _combatBuff { get; }
        internal Composite _combatBehavior { get; }
        internal Composite _rest { get; }

        public string Name => "Combat Handler";

        public async Task<bool> Run()
        {
            if (!DutyManager.InInstance || DeepDungeonManager.Director.TimeLeftInDungeon == TimeSpan.Zero)
            {
                return false;
            }

            //dont try and do combat outside of the dungeon plz
            if (!Constants.InDeepDungeon)
            {
                return false;
            }

            if (AvoidanceManager.IsRunningOutOfAvoid)
            {
                return true;
            }

            if (!Core.Me.InRealCombat())
            {
                if (await Rest())
                {
                    await CommonTasks.StopMoving("Resting");
                    await Tasks.Common.UsePots();
                    await Heal();
                    return true;
                }

                if (await PreCombatBuff())
                {
                    return true;
                }


                // For floors with auto heal penalty or item penalty we will engage normally until we hit
                // the magic sub-40% threshold. Statistically it is smarter to just try and finish the floor
                // instead of waiting around while healing just to encounter additional mobs spawning in.
                if (Core.Me.CurrentHealthPercent <= 40
                    && (Core.Me.HasAura(Auras.ItemPenalty) || Core.Me.HasAura(Auras.NoAutoHeal))
                    && !DeepDungeonManager.BossFloor)
                {
                    await CommonTasks.StopMoving("Resting");
                    await Heal();
                    return true;
                }

                if (Core.Me.CurrentHealthPercent <= 90
                    && !(Core.Me.HasAura(Auras.ItemPenalty) || Core.Me.HasAura(Auras.NoAutoHeal))
                    && !DeepDungeonManager.BossFloor)
                {
                    await CommonTasks.StopMoving("Resting");
                    await Heal();
                    return true;
                }
            }

            if (Poi.Current == null || Poi.Current.Type != PoiType.Kill || Poi.Current.BattleCharacter == null)
            {
                return false;
            }

            if (Poi.Current.BattleCharacter == null || !Poi.Current.BattleCharacter.IsValid || Poi.Current.BattleCharacter.IsDead)
            {
                Poi.Clear("Target is dead");
                return true;
            }

            Poi target = Poi.Current;

            TreeRoot.StatusText = $"Combat: {target.BattleCharacter.Name}";

            //target if we are in range
            //Logger.Info("======= OUT OF RANGE");
            if (target.BattleCharacter.Pointer != Core.Me.PrimaryTargetPtr && target.BattleCharacter.IsTargetable && target.Location.Distance2D(Core.Me.Location) <= Constants.ModifiedCombatReach)
            {
                Logger.Warn("Combat target has changed");
                target.BattleCharacter.Target();
                return true;
            }

            //Logger.Info("======= PRE COMBAT");
            if (await PreCombatBuff())
            {
                return true;
            }

            //Logger.Info("======= OUT OF RANGE2");
            //we are outside of targeting range, walk to the mob
            if ((Core.Me.PrimaryTargetPtr == IntPtr.Zero || target.Location.Distance2D(Core.Me.Location) > Constants.ModifiedCombatReach) && !AvoidanceManager.IsRunningOutOfAvoid)
            {
                //Logger.Info("======= MoveAndStop======");
                await CommonTasks.MoveAndStop(new MoveToParameters(target.Location, target.Name), Constants.ModifiedCombatReach, true);
                return true;
            }

            if (await UseWitching())
            {
                return true;
            }

            //used if we are transformed
            if (await UsePomanderSpell())
            {
                return true;
            }

            if (await PreCombatLogic())
            {
                return true;
            }

            //Logger.Info("======= PULL");
            //pull not in combat
            if (!Core.Me.HasAura(Auras.Lust) && !Core.Me.HasAura(Auras.Rage) && !Core.Me.InRealCombat())
            {
                //if(target.Location.Distance2D(Core.Me.Location) > RoutineManager.Current.PullRange)
                //{
                //    Logger.Info("======= Should be pulling....out");
                //    TreeRoot.StatusText = $"Moving to kill target";
                //    await CommonTasks.MoveAndStop(new MoveToParameters(target.Location, target.Name),  Constants.ModifiedCombatReach, true);
                //    return true;
                //}
                await Pull();
                return true;
            }

            IEnumerable<BattleCharacter> castingEnemies = GameObjectManager.GetObjectsOfType<BattleCharacter>()
                .Where(bc => bc.IsNpc && bc.IsCasting && bc.InCombat);

            foreach (BattleCharacter enemy in castingEnemies)
            {
                if (enemy.CastingSpellId == Spells.FinalSting && Core.Me.CurrentHealthPercent < 90)
                {
                    if (await Tasks.Common.UsePots(force: true))
                    {
                        return true;
                    }
                }

                if (GazeAttacks.Contains(enemy.CastingSpellId))
                {
                    while (enemy != null && enemy.IsCasting)
                    {
                        Core.Me.FaceAway(enemy);
                        await Coroutine.Sleep(100);
                    }

                    return true;
                }
            }

            if (DeepDungeonManager.Level == 30)
            {
                // Handle HoH 30 boss Hiruko's Cloud Call mechanic
                BattleCharacter hiruko = GameObjectManager.Attackers.FirstOrDefault(npc =>
                    npc.IsCasting && npc.NpcId == Mobs.Hiruko && npc.CastingSpellId == Spells.CloudCall
                );

                while (hiruko != null && hiruko.IsCasting && hiruko.CastingSpellId == Spells.CloudCall)
                {
                    Vector3 safeCloud = new Vector3(-299.9771f, -0.01531982f, -320.4548f);
                    const double safeDistance = 1.0f;

                    while (safeCloud.Distance2D(Core.Me.Location) >= safeDistance)
                    {
                        Navigator.PlayerMover.MoveTowards(safeCloud);
                        await Coroutine.Sleep(100);
                    }

                    Navigator.PlayerMover.MoveStop();
                    await Coroutine.Wait(10_000, () => GameObjectManager.GetObjectsByNPCId(Mobs.Raiun).Any());
                }
            }

            if (Core.Me.InRealCombat())
            {
                if (await Tasks.Common.UseSustain())
                {
                    return true;
                }

                if (Settings.Instance.UseAntidote)
                {
                    if (Core.Me.HasAnyAura(Auras.Poisons) && await Tasks.Common.UseItemById(Items.Antidote))
                    {
                        return true;
                    }
                }

                if (await Heal())
                {
                    return true;
                }

                if (await CombatBuff())
                {
                    return true;
                }

                if (await Combat())
                {
                    return true;
                }
            }

            //Logger.Warn($"don't let anything else execute if we are running the kill poi");
            //don't let anything else execute if we are running the kill poi
            return true; //we expected to do combat
        }

        public void Tick()
        {
            if (!Constants.InDeepDungeon || CommonBehaviors.IsLoading || QuestLogManager.InCutscene)
            {
                return;
            }

            if (!DutyManager.InInstance || DeepDungeonManager.Director.TimeLeftInDungeon == TimeSpan.Zero)
            {
                return;
            }

            CombatTargeting.Instance.Pulse();
            if (CombatTargeting.Instance.FirstUnit == null)
            {
                GameObject t = DDTargetingProvider.Instance.FirstEntity;
                if (t == null)
                {
                    return;
                }

                if (t.Type == GameObjectType.BattleNpc && Poi.Current.Type != PoiType.Kill)
                {
                    Logger.Warn($"trying to get into combat with: {t.NpcId}");
                    Poi.Current = new Poi(t, PoiType.Kill);
                    return;
                }

                return;
            }

            if (Poi.Current.Unit != null && Poi.Current.Unit.IsValid && Poi.Current.Type != PoiType.Kill)
            {
                if (!Core.Me.InRealCombat() && Poi.Current.Unit.Distance2D() < CombatTargeting.Instance.FirstUnit.Distance2D())
                {
                    return;
                }
            }

            if (Poi.Current.Unit == null || Poi.Current.Unit.Pointer != CombatTargeting.Instance.FirstUnit.Pointer)
            {
                Poi.Current = new Poi(CombatTargeting.Instance.FirstUnit, PoiType.Kill);
            }
        }

        /// <summary>
        ///     will use an available pomander ability
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UsePomanderSpell()
        {
            LocalPlayer player = Core.Me;
            if (player.HasAura(Auras.Lust) || player.HasAura(Auras.Rage))
            {
                if (DeepDungeonManager.BossFloor)
                {
                    if ((Core.Target as Character)?.GetAuraById(Auras.LustVulnerabilityUp)?.Value == 5 && player.ClassLevel > 30 ||
                        player.CurrentHealthPercent < 65)
                    {
                        await Tasks.Common.CancelAura(Auras.Lust);
                        ActionManager.StopCasting();
                        return true;
                    }
                }

                if (player.IsCasting)
                {
                    await Coroutine.Yield();
                    return true;
                }

                if (HasSpell(LustSpell.Id))
                {
                    await CastPomanderAbility(LustSpell);
                    return true;
                }

                if (HasSpell(PummelSpell.Id))
                {
                    await CastPomanderAbility(PummelSpell);
                    return true;
                }

                Logger.Warn("I am under the effects of Lust or Rage and Don't know either spell. Please send help!");
                await Coroutine.Yield();
                return false;
            }

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (Tasks.Common.PomanderState != ItemState.None)
            {
                Tasks.Common.PomanderState = ItemState.None;
            }

            return false;
        }

        /// <summary>
        ///     hacky method to check what transform we are in
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool HasSpell(uint id)
        {
            HotbarSlot[] hbs = HotbarManager.HotbarsSlot;
            for (uint i = 0; i < hbs.Length; i++)
            {
                HotbarSlot hb = hbs[i];
                if (hb.ActionId1 == id)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task CastPomanderAbility(SpellData spell)
        {
            if (!RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement | CapabilityFlags.Facing))
            {
                if (!ActionManager.CanCast(spell, Poi.Current.BattleCharacter) || Poi.Current.BattleCharacter.Distance2D() > (float)spell.Range + Poi.Current.BattleCharacter.CombatReach)
                {
                    await CommonTasks.MoveAndStop(new MoveToParameters(Core.Target.Location, $"Moving to {Poi.Current.Name} to cast {spell.Name}"),
                        (float)spell.Range + Poi.Current.BattleCharacter.CombatReach, true);
                }

                Poi.Current.BattleCharacter.Face2D();
            }

            ActionManager.DoAction(spell, Poi.Current.BattleCharacter);
            await Coroutine.Yield();
        }

        /// <summary>
        ///     Uses a pomander of witching when there are 3 mobs in combat around us
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UseWitching()
        {
            if (
                !DeepDungeonManager.BossFloor &&
                DeepDungeonManager.GetInventoryItem(Pomander.Witching).Count > 0 &&
                GameObjectManager.NumberOfAttackers >= 3 &&
                !GameObjectManager.Attackers.Any(i =>
                    i.HasAura(Auras.Frog) ||
                    i.HasAura(Auras.Imp) ||
                    i.HasAura(Auras.Otter) ||
                    i.HasAura(Auras.Chicken)) //Toad
                &&
                (!PartyManager.IsInParty || PartyManager.IsPartyLeader)
            )
            {
                Logger.Info("Witching debug: {0} {1} {2} {3} {4}",
                    !DeepDungeonManager.BossFloor,
                    DeepDungeonManager.GetInventoryItem(Pomander.Witching).Count,
                    GameObjectManager.NumberOfAttackers,
                    !GameObjectManager.Attackers.Any(i =>
                        i.HasAura(Auras.Frog) ||
                        i.HasAura(Auras.Imp) ||
                        i.HasAura(Auras.Otter) ||
                        i.HasAura(Auras.Chicken)),
                    !PartyManager.IsInParty || PartyManager.IsPartyLeader
                );
                await CommonTasks.StopMoving("Use Pomander");
                bool res = await Tasks.Common.UsePomander(Pomander.Witching);

                await Coroutine.Yield();
                return res;
            }

            return false;
        }

        #region Combat Routine

        private readonly object context = new object();

        internal async Task<bool> Rest()
        {
            return await _rest.ExecuteCoroutine(context);
        }

        internal async Task<bool> Pull()
        {
            return await _pull.ExecuteCoroutine(context);
        }

        internal async Task<bool> Heal()
        {
            if (await Tasks.Common.UsePots())
            {
                return true;
            }

            return await _heal.ExecuteCoroutine(context);
        }

        internal async Task<bool> PreCombatBuff()
        {
            if (!Core.Me.InCombat)
            {
                return await _preCombatBuff.ExecuteCoroutine(context);
            }

            return false;
        }

        private async Task<bool> PreCombatLogic()
        {
            //if(!Core.Me.InCombat)
            return await _preCombatLogic.ExecuteCoroutine(context);
            //return false;
        }

        private async Task<bool> CombatBuff()
        {
            return await _combatBuff.ExecuteCoroutine(context);
        }

        private async Task<bool> Combat()
        {
            return await _combatBehavior.ExecuteCoroutine(context);
        }

        #endregion
    }
}