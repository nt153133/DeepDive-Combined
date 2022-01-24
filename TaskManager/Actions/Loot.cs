﻿/*
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
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using DeepCombined.Providers;
using DeepCombined.Windows;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Directors;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.RemoteWindows;

namespace DeepCombined.TaskManager.Actions
{
    internal class Loot : ITask
    {
        private static readonly HashSet<uint> pomanderSpellIds = new HashSet<uint>
        {
            6259, 6260, 6262, 6263, 6264, 6265, 6266, 6267, 6268, 6269, 6270, 6271, 6272, 6868, 6869, 6870, 11275, 11276, 11277
        };

        private static readonly HashSet<string> pomanderLocalizedNames = new HashSet<string>();

        internal bool pomanderCapped;

        static Loot()
        {
            foreach (uint spellId in pomanderSpellIds)
            {
                //append and prepend a space so we easily filter out that specific message
                pomanderLocalizedNames.Add($" {DataManager.GetSpellData(spellId).LocalizedName} ");
            }
        }

        private Poi Target => Poi.Current;
        public string Name => "Loot";


        public async Task<bool> Run()
        {
            if (Target.Type != PoiType.Collect)
            {
                return false;
            }

            //let the navigation task handle moving toward the object if we are too far away.
            if (Target.Location.Distance2D(Core.Me.Location) > 3)
            {
                return false;
            }

            if (Target.Unit == null || !Target.Unit.IsValid)
            {
                Poi.Clear("Target not found at location");
                return true;
            }

            if (Target.Unit.NpcId == Constants.SelectedDungeon.LobbyExit)
            {
                Poi.Clear("This is lobby exit - clear to get lobby task to run");
                return true;
            }
            //let the user know we are trying to run a treasure task
            TreeRoot.StatusText = "Treasure";
            if (Target.Unit.IsValid)
            {
                if (Target.Unit?.NpcId == Entities.Hidden)
                {
                    return await HandleCacheOfTheHoard();
                }
            }
            else
            {
                return true;
            }

            //treasure... or an "exit"...
            return await TreasureOrExit();
        }

        public void Tick()
        {
            if (!Constants.InDeepDungeon || CommonBehaviors.IsLoading || QuestLogManager.InCutscene)
            {
                return;
            }

            if (DirectorManager.ActiveDirector is InstanceContentDirector activeAsInstance)
            {
                if (activeAsInstance.TimeLeftInDungeon == TimeSpan.Zero)
                {
                    return;
                }
            }

            GameObject t = DDTargetingProvider.Instance.FirstEntity;

            if (t == null || t.Type == GameObjectType.BattleNpc)
            {
                return;
            }

            //only change if we don't have a poi or are currently performing a collect action.
            if (Poi.Current == null || Poi.Current.Type == PoiType.None || Poi.Current.Type == PoiType.Collect || Poi.Current.Type == (PoiType)PoiTypes.ExplorePOI)
            {
                if (Poi.Current.Unit != null && Poi.Current.Unit.Pointer == t.Pointer)
                {
                    return;
                }

                Poi.Current = new Poi(t, PoiType.Collect);
            }
        }

        /// <summary>
        ///     Handles opening treasure coffers or opening an exit portal
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> TreasureOrExit()
        {
            int tries = 0;
            uint npcid = Target.Unit.NpcId;
            if (Target.Location.Distance2D(Core.Me.Location) >= 3)
            {
                await CommonTasks.MoveAndStop(new MoveToParameters(Target.Location, Target.Name), 2.5f, true);
                return true;
            }

            pomanderCapped = false;
            //Unsubscribe first to prevent subscriptions from persisting
            //GamelogManager.MessageRecevied -= GamelogManagerOnMessageRecevied;
            // GamelogManager.MessageRecevied += GamelogManagerOnMessageRecevied;

            while (!DeepDungeonCombined.StopPlz && Target.Unit != null && Target.Unit.IsValid && tries < 3)
            {
                try
                {
                    //if we are a transformed we can't open a chest
                    if (Constants.AuraTransformed)
                    {
                        Logger.Warn("Unable to open chest. Waiting for aura to end...");
                        await CommonTasks.StopMoving("Waiting on aura to end");
                        await Coroutine.Wait(TimeSpan.FromSeconds(30),
                            () => !Constants.AuraTransformed || Core.Me.InCombat || DeepDungeonCombined.StopPlz);
                        return true;
                    }


                    await Coroutine.Yield();

                    if (Core.Me.HasAura(Auras.Lust))
                    {
                        await Tasks.Common.CancelAura(Auras.Lust);
                    }

                    Logger.Verbose("Attempting to interact with: {0} ({1} / 3)", Target.Name, tries + 1);

                    if (!PartyManager.IsInParty || PartyManager.IsPartyLeader ||
                        PartyManager.IsInParty && Constants.IsFloorExit(Target.Unit))
                    {
                        await CommonTasks.StopMoving("Interacting with chest");
                        if (!await ObjectInteraction(Target.Unit))
                        {
                            break;
                        }
                    }
                    else
                    {
                        await CommonTasks.StopMoving("Waiting for leader to use chest");
                    }

                    await Coroutine.Sleep(700);

                    if (!Target.Unit.IsValid)
                    {
                        break;
                    }

                    if (!Target.Unit.IsTargetable)
                    {
                        break;
                    }

                    if (SelectYesno.IsOpen)
                    {
                        break;
                    }
                }
                finally
                {
                    tries++;
                    DeepDungeonManager.PomanderChange();
                }
            }

            GamelogManager.MessageRecevied -= GamelogManagerOnMessageRecevied;

            await Coroutine.Wait(500, () => SelectYesno.IsOpen);

            //if this is an exit
            if (SelectYesno.IsOpen)
            {
                SelectYesno.ClickYes();
                await Coroutine.Sleep(1000);
                Logger.Verbose("Is window open : {0}", DeepDungeonResult.Instance.IsOpen);
                if (DeepDungeonResult.Instance.IsOpen)
                {
                    Logger.Verbose("Calling Close");
                    await Coroutine.Sleep(2000);
                    DeepDungeonResult.Instance.SendAction(1, 3, uint.MaxValue);
                    DeepTracker.EndRun(false);
                }
                await Coroutine.Wait(TimeSpan.MaxValue,
                    () => DeepDungeonCombined.StopPlz || QuestLogManager.InCutscene || NowLoading.IsVisible);
                return true;
            }

            if (Target.Unit != null && Target.Unit.IsValid)
            {
                Blacklist.Add(Target.Unit.ObjectId, TimeSpan.FromMinutes(5), $"Tried to Interact with the Target {tries} times");
            }

            Poi.Clear($"Tried to Interact with the Target {tries} times");

            return false;
        }

        private void GamelogManagerOnMessageRecevied(object sender, ChatEventArgs e)
        {
            if (e.ChatLogEntry.MessageType == (MessageType)2105)
            {
                foreach (string name in pomanderLocalizedNames)
                {
                    if (e.ChatLogEntry.FullLine.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Logger.Info("Got a message that looks like we are capped on" + name);
                        GamelogManager.MessageRecevied -= GamelogManagerOnMessageRecevied;
                        pomanderCapped = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Handles Cache of the Hoard
        /// </summary>
        /// <returns></returns>
        private async Task<bool> HandleCacheOfTheHoard()
        {
            TreeRoot.StatusText = "Banded Coffer";

            if (
                GameObjectManager.GameObjects.Any(
                    i => Constants.TrapIds.Contains(i.NpcId) && i.Distance2D(Target.Location) < 2))
            {
                Blacklist.Add(Target.Unit, BlacklistFlags.All, TimeSpan.FromMinutes(3), "A trap is close to the Hoard Spawn location. Skipping.");
                Poi.Clear("A trap is close to the Hoard Spawn location. Skipping.");
                await Coroutine.Sleep(250);
                return true;
            }

            if (Target.Location.Distance2D(Core.Me.Location) >= 2)
            {
                Logger.Info("Banded Coffer is >= 3");
                await CommonTasks.MoveAndStop(new MoveToParameters(Target.Location, "Banded Coffer"), 0.5f, true);
                return true;
            }

            await CommonTasks.StopMoving("Spawning Coffer");

            Logger.Info("Found a Cache of the Horde. Waiting for it to spawn... (Giving it a few seconds to spawn)");


            //target will change after the banded coffer is spawned
            GameObject org = Target.Unit;

            //wait for the chest or for us to get into combat.
            await Coroutine.Wait(TimeSpan.FromSeconds(10),
                () =>
                    Core.Me.InCombat || GameObjectManager.NumberOfAttackers > 0 || DeepDungeonCombined.StopPlz ||
                    GameObjectManager.GetObjectsOfType<EventObject>().Any(i => i.NpcId == Entities.BandedCoffer)
            );

            if (Core.Me.InCombat || GameObjectManager.NumberOfAttackers > 0)
            {
                Logger.Info("Entered Combat waiting on coffer to spawn.");
                return true;
            }


            Blacklist.Add(org, BlacklistFlags.All | (BlacklistFlags)DeepDungeonManager.Level, TimeSpan.FromMinutes(3), "Spawned the Coffer or used all of our time...");
            Poi.Clear("Hidden added to blacklist");
            return true;
        }

        /// <summary>
        ///     Object Interaction
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="interactRange"></param>
        /// <returns></returns>
        private static async Task<bool> ObjectInteraction(GameObject obj, float interactRange = 4.5f)
        {
            return await ObjectInteraction(obj, interactRange, () => true);
        }

        /// <summary>
        ///     Made by Zzi - Borrowed from DungeonMaster
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="interactRange"></param>
        /// <param name="canInteract">Return False if we should not interact with the object</param>
        /// <returns></returns>
        private static async Task<bool> ObjectInteraction(GameObject obj, float interactRange, Func<bool> canInteract)
        {
            if (!canInteract())
            {
                return false;
            }

            if (!obj.IsValid || !obj.IsVisible)
            {
                return false;
            }

            if (Core.Me.IsCasting)
            {
                return true;
            }

            if (obj.Distance2D() > interactRange)
            {
                MoveResult mr = await CommonTasks.MoveTo(new MoveToParameters(obj.Location));
                if (mr == MoveResult.PathGenerationFailed && obj.InLineOfSight())
                {
                    Navigator.PlayerMover.MoveTowards(obj.Location);
                    return true;
                }

                if (mr == MoveResult.PathGenerationFailed)
                {
                    Logger.Error($"Unable to move toward {obj.Name} [{obj.NpcId}] (It appears to be out of line of sight and off the mesh)");
                }

                return mr.IsSuccessful();
            }

            if (MovementManager.IsMoving)
            {
                await CommonTasks.StopMoving();
                return true;
            }

            if (Core.Target == null || Core.Target.ObjectId != obj.ObjectId)
            {
                obj.Target();
                return true;
            }

            obj.Interact();
            await Coroutine.Sleep(500);
            return true;
        }
    }
}