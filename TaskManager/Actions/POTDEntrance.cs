/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */
using Buddy.Coroutines;
using Clio.Utilities.Helpers;
using Deep.Memory;
using Deep.Structure;
using Deep.Windows;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Pathing;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deep.Helpers.Logging;

namespace Deep.TaskManager.Actions
{
    class POTDEntrance : ITask
    {


        public string Name => "PotdWindows";


        private DeepDungeonSaveState[] _saveStates;
        private byte[] _aetherialLevels = { 0, 0 };

        private static uint UseSaveSlot => (uint)Settings.Instance.SaveSlot;

        private FloorSetting _targetFloor;
        private DungeonDefinition.Base.FloorSetting _bettertargetFloor;

        private AgentDeepDungeonSaveData Sd => Constants.GetSaveInterface();

        internal static bool _error;
        private readonly object _errorLock = new object();

        internal bool HasWindowOpen => (DeepDungeonMenu.IsOpen || DeepDungeonSaveData.IsOpen);
        WaitTimer DungeonQueue = new WaitTimer(TimeSpan.FromMinutes(5));

        public void Tick()
        {
            if (WorldManager.ZoneId != Constants.EntranceZoneId && (!DungeonQueue.IsFinished || _error))
            {
                _error = false;
                DungeonQueue.Stop();
                return;
            }
            if(!DungeonQueue.IsFinished)
            {
                foreach(var x in GamelogManager.CurrentBuffer.Where(i => i.MessageType == (MessageType)2876))
                {
                    HandleErrorMessages(x);
                }
            }
        }

        private static bool IsCrossRealm => PartyManager.CrossRealm;

        public async Task<bool> Run()
        {
            if (WorldManager.ZoneId != Constants.EntranceZoneId) return false;
            if(Settings.Instance.Stop)
            {
                TreeRoot.Stop("Stop Requested");
                return true;
            }

            if(ContentsFinderConfirm.IsOpen)
            {
                Logger.Warn($"Entering {Constants.SelectedDungeon.GetDDType()} - Currently a Level {Core.Me.ClassLevel} {Core.Me.CurrentJob}");
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
                Logger.Info($"Waiting on Queue");
                return true;
            }
            
            if(!HasWindowOpen)
            {
                await OpenMenu();
                return true;
            }

            await MainMenu();

            return true;
            
        }

        /// <summary>
        /// Handles reading the chat log for errors while joining the queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleErrorMessages(ChatLogEntry e)
        {
            _error = true;
            var str = e.Contents;
            foreach (var c in PartyManager.AllMembers)
            {
                str = str.Replace(c.Name, "PARTY_MEMBER_NAME");
            }
            str = str.Replace(Core.Me.Name, "MY_CHARACTER_NAME");

            Logger.Verbose("We detected an error while trying to join the queue. {0}", str);
            DungeonQueue.Stop();
        }

        private async Task OpenMenu()
        {
            Logger.Verbose("Attempting to interact with: {0}", DataManager.GetLocalizedNPCName((int)Constants.EntranceNpcId));

            GameObjectManager.GetObjectByNPCId(Constants.EntranceNpcId).Interact();
            await Coroutine.Yield();
            //wait while false
            await Coroutine.Wait(3000, () => HasWindowOpen || Talk.DialogOpen);
            if (Talk.DialogOpen)
            {
                Talk.Next();
                await Coroutine.Yield();
            }
            if (!HasWindowOpen)
            {
                Logger.Verbose("Failed to open window. trying again...");
            }
        }

        /// <summary>
        /// Determines if we need to reset the floors levels.
        /// Returns TRUE: Reset the floor data
        /// Returns FALSE: Go to the next set.
        /// </summary>
        /// <param name="apLevels"></param>
        /// <param name="sdSaveStates"></param>
        /// <returns></returns>
        public bool GetFloorStatus(byte[] apLevels, DeepDungeonSaveState[] sdSaveStates)
        {
            var stop = Settings.Instance.BetterSelectedLevel;//Settings.Instance.SelectedLevel;

            try
            {
                if (!PartyManager.IsInParty)
                {
                    Logger.Warn("You are solo, Setting the bot to do 1-10.");
                    stop = Constants.SelectedDungeon.Floors[0];  //Settings.Instance.FloorSettings[0];
                }

                //_targetFloor = stop;
                _bettertargetFloor = stop;

                //Logger.Verbose("Going to floor: {0}", _targetFloor.LevelMax);
                Logger.Verbose("Going to floor: {0}", _bettertargetFloor.End);

            }
            catch (Exception)
            {
                Logger.Verbose("Exception with setting floor data. setting target floor to 10");
                _targetFloor = new FloorSetting { LevelMax = 10 };
            }

            //Logger.Verbose("Starting Level {0}", _targetFloor.LevelMax - 9);
            Logger.Verbose("Starting Level {0}", _bettertargetFloor.Start);

            //var lm = _targetFloor.LevelMax < sdSaveStates[UseSaveSlot].Floor;
            var lm = _bettertargetFloor.End < sdSaveStates[UseSaveSlot].Floor;
            
            var notfixed = !sdSaveStates[UseSaveSlot].FixedParty;
            var cjChanged = sdSaveStates[UseSaveSlot].Class != Core.Me.CurrentJob;
            var partyData = sdSaveStates[UseSaveSlot].PartyMembers.ToList();
            var saved = sdSaveStates[UseSaveSlot].Saved;

            bool partySize;
            var partyClass = false;

            //if (saved && partyData.Count == PartyManager.NumMembers)
            //{
            //    foreach (var r in partyData)
            //    {
            //        var c = PartyManager.AllMembers.FirstOrDefault(i => i.Name == r.Name);
            //        if (c == null)
            //        {
            //            Logger.Warn("Resetting save data: A member in the party has changed.");
            //            partyClass = true;
            //            break;
            //        }

            //        if (c.Class == r.Class) continue;

            //        Logger.Warn("Resetting save data: Someone has changed jobs");
            //        partyClass = true;
            //        break;
            //    }
            //}

            if (PartyManager.IsInParty)
                partySize = PartyManager.NumMembers != partyData.Count;
            else
                partySize = partyData.Count != 1;


            if (saved && lm)
                Logger.Verbose("Resetting save data: Level Max ({0}) is Less than floor value: {1}", _bettertargetFloor.End, sdSaveStates[UseSaveSlot].Floor);
                //Logger.Verbose("Resetting save data: Level Max ({0}) is Less than floor value: {1}", _targetFloor.LevelMax, sdSaveStates[UseSaveSlot].Floor);
            if (saved && notfixed)
                Logger.Verbose("Resetting save data: Our class/job has changed from: {0} to {1}", sdSaveStates[UseSaveSlot].Class, Core.Me.CurrentJob);
            if (saved && partySize)
                Logger.Verbose("Resetting save data: Our Party has changed. {0} != {1}", PartyManager.NumMembers, partyData.Count);
            if (saved && _error)
                Logger.Verbose("Resetting save data: there was a warning waiting for the duty finder.");

            return saved && (lm || notfixed || cjChanged || partySize || partyClass || _error);
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

                if (DeepDungeon.StopPlz)
                    return;

                Logger.Warn("Everyone is now in the zone");
                for (var i = 0; i < 6; i++)
                {
                    Logger.Warn("Giving them {0} seconds to do what they need to at the NPC", 60 - i * 10);
                    await Coroutine.Sleep(TimeSpan.FromSeconds(10));
                    if (DeepDungeon.StopPlz)
                        return;
                }
            }

            //read the current level state
            await ReadStartingLevel();

            // have save data and our max level is 
            if (GetFloorStatus(_aetherialLevels, _saveStates))
            {
                Logger.Verbose("Resetting the floor");
                await DeepDungeonSaveData.ClickReset(UseSaveSlot);
                
                // todo: wait for server response in a better way.
                await Coroutine.Sleep(1000);
            }
            if (_error)
                lock (_errorLock)
                    _error = false;


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
                            Logger.Verbose("Start at 51: {0}", _targetFloor.LevelMax > 50);

                        if (Settings.Instance.StartAt51 && _targetFloor.LevelMax > 50)
                        {
                            SelectString.ClickSlot(1);
                        }
                        else
                        {
                            SelectString.ClickSlot(0);
                        }
                        await Coroutine.Sleep(1000);
                    }
                    Logger.Verbose("Done with window interaction.");
                }
                else
                {
                    Logger.Verbose($"ContentsFinderConfirm is open: {ContentsFinderConfirm.IsOpen} so we aren't going through the main menu.");
                }
                _targetFloor = null;
            }
            Logger.Info("Waiting on the queue, Or for an error.");
            DungeonQueue.Reset();

        }

        /// <summary>
        /// returns false if any party member is not on the map
        /// </summary>
        /// <returns></returns>
        private bool PartyLeaderWaitConditions()
        {
            return PartyManager.VisibleMembers.Count() == PartyManager.AllMembers.Count();
        }
    }
}
