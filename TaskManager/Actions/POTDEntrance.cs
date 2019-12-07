/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities.Helpers;
using DeepCombined.DungeonDefinition.Base;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using DeepCombined.Windows;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;

namespace DeepCombined.TaskManager.Actions
{
    internal class POTDEntrance : ITask
    {
        private static bool _error;
        private readonly object _errorLock = new object();
        private readonly WaitTimer DungeonQueue = new WaitTimer(TimeSpan.FromMinutes(5));
        private byte[] _aetherialLevels = {0, 0};

        //private FloorSetting _targetFloor;
        private FloorSetting _bettertargetFloor;


        private DeepDungeonSaveState[] _saveStates;

        private static uint UseSaveSlot => (uint) Settings.Instance.SaveSlot;

        private AgentDeepDungeonSaveData Sd => Constants.GetSaveInterface();

        internal bool HasWindowOpen => DeepDungeonMenu.IsOpen || DeepDungeonSaveData.IsOpen;

        private static bool IsCrossRealm => PartyManager.CrossRealm;


        public string Name => "PotdWindows";

        public void Tick()
        {
            if (WorldManager.ZoneId != Constants.EntranceZoneId && (!DungeonQueue.IsFinished || _error))
            {
                _error = false;
                DungeonQueue.Stop();
                return;
            }

            if (DungeonQueue.IsFinished) return;
            foreach (var x in GamelogManager.CurrentBuffer.Where(i => i.MessageType == (MessageType) 2876))
            {
                HandleErrorMessages(x);
            }
        }

        public async Task<bool> Run()
        {
            if (WorldManager.ZoneId != Constants.EntranceZoneId) return false;
            if (Settings.Instance.Stop)
            {
                TreeRoot.Stop("Stop Requested");
                DeepTracker.EndRun(true);
                return true;
            }

            if (ContentsFinderConfirm.IsOpen)
            {
                Logger.Warn($"Entering {Constants.SelectedDungeon.GetDDType()} - Currently a Level {Core.Me.ClassLevel} {Core.Me.CurrentJob}");
                DeepTracker.StartRun(Core.Me.ClassLevel);
                ContentsFinderConfirm.Commence();

                await Coroutine.Wait(TimeSpan.FromMinutes(2), () => QuestLogManager.InCutscene || NowLoading.IsVisible);
                DungeonQueue.Stop();

                return true;
            }

            //TODO InQueue
            if (!DungeonQueue.IsFinished)
            {
                TreeRoot.StatusText = "Waiting on Queue";
                await Coroutine.Wait(500, () => ContentsFinderConfirm.IsOpen);
                Logger.Info("Waiting on Queue");
                return true;
            }

            if (!HasWindowOpen)
            {
                if (Constants.UseJobList)
                {
                    Logger.Info("Checking Joblist");
                    await CheckJobQueue();
                }
                
                await OpenMenu();
                return true;
            }

            await MainMenu();

            return true;
        }

        /// <summary>
        ///     Handles reading the chat log for errors while joining the queue.
        /// </summary>
        /// <param name="e"></param>
        private void HandleErrorMessages(ChatLogEntry e)
        {
            _error = true;
            var str = PartyManager.AllMembers.Aggregate(e.Contents, (current, c) => current.Replace(c.Name, "PARTY_MEMBER_NAME"));
            str = str.Replace(Core.Me.Name, "MY_CHARACTER_NAME");

            Logger.Verbose("We detected an error while trying to join the queue. {0}", str);
            DungeonQueue.Stop();
        }

        private async Task OpenMenu()
        {
            Logger.Verbose("Attempting to interact with: {0}", DataManager.GetLocalizedNPCName((int) Constants.EntranceNpcId));

            GameObjectManager.GetObjectByNPCId(Constants.EntranceNpcId).Interact();
            await Coroutine.Yield();
            //wait while false
            await Coroutine.Wait(3000, () => HasWindowOpen || Talk.DialogOpen);
            if (Talk.DialogOpen)
            {
                Talk.Next();
                await Coroutine.Yield();
            }

            if (!HasWindowOpen) Logger.Verbose("Failed to open window. trying again...");
        }

        /// <summary>
        ///     Determines if we need to reset the floors levels.
        ///     Returns TRUE: Reset the floor data
        ///     Returns FALSE: Go to the next set.
        /// </summary>
        /// <param name="sdSaveStates"></param>
        /// <returns></returns>
        private bool GetFloorStatus(DeepDungeonSaveState[] sdSaveStates)
        {
            var stop = Settings.Instance.BetterSelectedLevel; //Settings.Instance.SelectedLevel;

            try
            {
                _bettertargetFloor = stop;

                Logger.Verbose("Going to floor: {0}", _bettertargetFloor.End);
            }
            catch (Exception)
            {
                Logger.Verbose("Exception with setting floor data. setting target floor to 10");
                stop = Constants.SelectedDungeon.Floors[0];
            }

            Logger.Verbose("Starting Level {0}", _bettertargetFloor.Start);

            var lm = _bettertargetFloor.End < sdSaveStates[UseSaveSlot].Floor;

            var notfixed = !sdSaveStates[UseSaveSlot].FixedParty;
            var cjChanged = sdSaveStates[UseSaveSlot].Class != Core.Me.CurrentJob;
            var partyData = sdSaveStates[UseSaveSlot].PartyMembers.ToList();
            var saved = sdSaveStates[UseSaveSlot].Saved;

            bool partySize;
            //var  partyClass = false;


            if (PartyManager.IsInParty)
                partySize = PartyManager.NumMembers != partyData.Count;
            else
                partySize = partyData.Count != 1;

            if (saved && lm)
                Logger.Verbose("Resetting save data: Level Max ({0}) is Less than floor value: {1}", _bettertargetFloor.End, sdSaveStates[UseSaveSlot].Floor);
            if (saved && notfixed)
                Logger.Verbose("Resetting save data: Our class/job has changed from: {0} to {1}", sdSaveStates[UseSaveSlot].Class, Core.Me.CurrentJob);
            if (saved && partySize)
                Logger.Verbose("Resetting save data: Our Party has changed. {0} != {1}", PartyManager.NumMembers, partyData.Count);
            if (saved && _error)
                Logger.Verbose("Resetting save data: there was a warning waiting for the duty finder.");
            if (Settings.Instance.StartAt51 && sdSaveStates[UseSaveSlot].Floor < Constants.SelectedDungeon.CheckPointLevel)
                Logger.Verbose("Resetting save data: Level start ({0}) is Less than checkpoint floor: {1}", sdSaveStates[UseSaveSlot].Floor, Constants.SelectedDungeon.CheckPointLevel);

            return saved && (lm || notfixed || cjChanged || partySize || _error || Settings.Instance.StartAt51 && sdSaveStates[UseSaveSlot].Floor < Constants.SelectedDungeon.CheckPointLevel);
        }

        private async Task ReadStartingLevel()
        {
            Logger.Verbose("Reading Save Data...");

            await DeepDungeonMenu.OpenSaveMenu();
            _aetherialLevels = Sd.GearLevels;

            Logger.Info(@"

=======================================
Aetherpool Arm: +{0}
Aetherpool Armor: +{1}
=======================================

", _aetherialLevels[0],
                _aetherialLevels[1]);
            _saveStates = Sd.SaveStates;

            for (var i = 0; i < 2; i++)
            {
                Logger.Verbose("[{0}] {1}", i + 1, _saveStates[i]);
            }

            Logger.Warn("Using the {0} Save Slot", Settings.Instance.SaveSlot);
        }

        private async Task MainMenu()
        {
            TreeRoot.StatusText = "Running Main Menu";
            if (PartyManager.IsInParty && PartyManager.IsPartyLeader)
            {
                if (!IsCrossRealm)
                {
                    Logger.Warn("I am a Party Leader, waiting for everyone to join the zone.");
                    await Coroutine.Wait(TimeSpan.FromMinutes(30), PartyLeaderWaitConditions);
                }
                else
                {
                    Logger.Warn("I am a Party Leader in a XRealm Party. I assume everyone is in the zone.");
                }

                if (DeepDungeonCombined.StopPlz)
                    return;

                Logger.Warn("Everyone is now in the zone");
                for (var i = 0; i < 6; i++)
                {
                    Logger.Warn("Giving them {0} seconds to do what they need to at the NPC", 60 - i * 10);
                    await Coroutine.Sleep(TimeSpan.FromSeconds(10));
                    if (DeepDungeonCombined.StopPlz)
                        return;
                }
            }

            //read the current level state
            await ReadStartingLevel();

            // have save data and our max level is 
            if (GetFloorStatus(_saveStates))
            {
                Logger.Verbose("Resetting the floor");
                await DeepDungeonSaveData.ClickReset(UseSaveSlot);

                // todo: wait for server response in a better way.
                await Coroutine.Sleep(1000);
            }

            if (_error)
                lock (_errorLock)
                {
                    _error = false;
                }


            if (!PartyManager.IsInParty || PartyManager.IsPartyLeader)
            {
                Logger.Verbose("Starting Save Slot Selection process");

                await DeepDungeonSaveData.ClickSaveSlot(UseSaveSlot);

                await Coroutine.Wait(2000, () => SelectString.IsOpen || ContentsFinderConfirm.IsOpen || SelectYesno.IsOpen);

                // if select yesno is open (new as of 4.36 hotfixes)
                if (SelectYesno.IsOpen)
                {
                    SelectYesno.ClickYes();
                    await Coroutine.Sleep(1000);
                }

                // if we are using an "empty" save slot
                if (SelectString.IsOpen)
                {
                    Logger.Verbose("Using Empty Save Slot");
                    Logger.Verbose("Going through the Talk dialogs...");

                    await Coroutine.Sleep(1000);

                    SelectString.ClickSlot(0);

                    await Coroutine.Sleep(1000);

                    //                    Logger.Verbose("Are you sure Fixed Party");
                    await Coroutine.Wait(1000, () => SelectYesno.IsOpen);
                    await Coroutine.Sleep(150);
                    if (SelectYesno.IsOpen)
                    {
                        SelectYesno.ClickYes();
                        await Coroutine.Sleep(150);
                    }

                    await Coroutine.Sleep(1000);

                    //-- Are you sure you want to enter alone?
                    if (!PartyManager.IsInParty)
                    {
                        //                        Logger.Verbose("Enter Alone Talk");
                        //talk stuff
                        await Coroutine.Wait(1000, () => Talk.DialogOpen);
                        await Coroutine.Sleep(150);
                        Talk.Next();

                        await Coroutine.Sleep(500);
                        //                        Logger.Verbose("Enter Alone?");
                        await Coroutine.Wait(1000, () => SelectYesno.IsOpen);
                        SelectYesno.ClickYes();
                        await Coroutine.Sleep(1000);
                    }

                    //                    Logger.Verbose("Floor 51 wait");
                    //--floor 51 logic
                    await Coroutine.Wait(1000, () => SelectString.IsOpen || ContentsFinderConfirm.IsOpen);
                    if (SelectString.IsOpen)
                    {
                        await Coroutine.Sleep(1000);

                        if (Settings.Instance.StartAt51)
                            Logger.Verbose("Start at {1}: {0}", _bettertargetFloor.End > Constants.SelectedDungeon.CheckPointLevel - 1, Constants.SelectedDungeon.CheckPointLevel);

                        if (Settings.Instance.StartAt51 && _bettertargetFloor.End > Constants.SelectedDungeon.CheckPointLevel - 1)
                            SelectString.ClickSlot(1);
                        else
                            SelectString.ClickSlot(0);
                        await Coroutine.Sleep(1000);
                    }

                    Logger.Verbose("Done with window interaction.");
                }
                else
                {
                    Logger.Verbose($"ContentsFinderConfirm is open: {ContentsFinderConfirm.IsOpen} so we aren't going through the main menu.");
                }

                _bettertargetFloor = null;
            }

            Logger.Info("Waiting on the queue, Or for an error.");
            DungeonQueue.Reset();
        }

        /// <summary>
        ///     returns false if any party member is not on the map
        /// </summary>
        /// <returns></returns>
        private bool PartyLeaderWaitConditions()
        {
            return PartyManager.VisibleMembers.Count() == PartyManager.AllMembers.Count();
        }

        private async Task CheckJobQueue()
        {
            foreach (var classLevelTarget in Constants.ClassLevelTargets)
            {
                if (Core.Me.CurrentJob == classLevelTarget.Job)
                    if (Core.Me.ClassLevel >= classLevelTarget.Level)
                    {
                        Logger.Info("Current job >= level");
                        continue;
                    }

                if (Core.Me.CurrentJob == classLevelTarget.Job)
                    if (Core.Me.Levels[Constants.ClassMap[classLevelTarget.classJobType]] < classLevelTarget.Level)
                    {
                        //GearsetManager.ChangeGearset(classLevelTarget.GearSlot);
                        Logger.Info("Still under level target");
                        break;
                    }
                
                if (Core.Me.CurrentJob != classLevelTarget.Job)
                    if (Core.Me.Levels[Constants.ClassMap[classLevelTarget.classJobType]] < classLevelTarget.Level)
                    {
                        Logger.Info($"Switching to {classLevelTarget.Job}");
                        GearsetManager.ChangeGearset(classLevelTarget.GearSlot);
                        break;
                    }
            }
        }
    }
}