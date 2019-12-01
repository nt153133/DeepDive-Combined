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
using System.Globalization;
using System.Threading;
using Buddy.Coroutines;
using DeepCombined.Forms;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using DeepCombined.Providers;
using DeepCombined.TaskManager;
using DeepCombined.TaskManager.Actions;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Overlay3D;
using ff14bot.Pathing.Service_Navigation;
using TreeSharp;

namespace DeepCombined
{
    public class DeepDungeonCombined : AsyncBotBase
    {
        public override string EnglishName => "Deep Dungeon Combined";
#if RB_CN
        public override string Name => "深层迷宫";
#else
        public override string Name => "Deep Dungeon Combined";
#endif
        //public override PulseFlags PulseFlags => PulseFlags.All;
        public override PulseFlags PulseFlags => PulseFlags.ObjectManager | PulseFlags.GameEvents | PulseFlags.Navigator | PulseFlags.Plugins | PulseFlags.Windows | PulseFlags.Avoidance | PulseFlags.Party;
        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public override bool WantButton => true;


        public DeepDungeonCombined()
        {
            Constants.LoadList();
            Constants.SelectedDungeon = Constants.DeepListType[Settings.Instance.SelectedDungeon];

            Task.Factory.StartNew(() =>
            {
                Constants.INIT();
                Overlay3D.Drawing += DDNavigationProvider.Render;


                _init = true;
                Logger.Info("INIT DONE");
            });
        }

        private volatile bool _init;

        private TaskManagerProvider _tasks;

        #region BotBase Stuff

        //private SettingsForm _settings;
        private DungeonSelection _settings;
        private static readonly Version v = new Version(1, 3, 3);

        public override void OnButtonPress()
        {
            if (_settings == null)
            {
/*#if RB_CN
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-CN");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-CN");
#endif*/

                _settings = new DungeonSelection
                {
                    Text = "DeepDive v" + v //title
                };

                _settings.Closed += (o, e) => { _settings = null; };
            }

            try
            {
                _settings.Show();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        /// <summary>
        ///     = true when we stop gets pushed
        /// </summary>
        internal static bool StopPlz;

        public override void Stop()
        {
            foreach (var block in Constants.PerformanceStats)
            {
                Logger.Verbose($"[Performance] {block.Key}");
                Logger.Verbose(block.Value.ToArray().ToString() );
                double sum = 0;
                foreach (var stat in block.Value)
                {
                    sum += stat;
                }
                
                Logger.Verbose($"Average {sum/block.Value.Count()} Count {block.Value.Count()}");
            }
            _root = null;
            StopPlz = true;

            Navigator.NavigationProvider = new NullProvider();
            DDTargetingProvider.Instance.Reset();
            Navigator.Clear();
            Poi.Current = null;
        }

        public override Composite Root => _root;

        private Composite _root;

        private bool ShowDebug = true;
        private DungeonSelection _debug;

        //private DDServiceNavigationProvider serviceProvider = new DDServiceNavigationProvider();
        public override void Pulse()
        {
            if (Constants.SelectedDungeon == null)
                return;

            if (Constants.InDeepDungeon)
            {
                if (Constants.IgnoreEntity == null)
                    Constants.IgnoreEntity = Constants.SelectedDungeon.GetIgnoreEntity(Constants.BaseIgnoreEntity);
                //force a pulse on the director if we are hitting "start" inside of the dungeon
                if (DirectorManager.ActiveDirector == null)
                    DirectorManager.Update();
                DDTargetingProvider.Instance.Pulse();
            }

            if (_tasks != null)
                _tasks.Tick();
        }

        public override void Start()
        {
            Poi.Current = null;
            
            if (DutyManager.InInstance && !Constants.SelectedDungeon.DeepDungeonRawIds.Contains(WorldManager.ZoneId))
            {
                Constants.SelectedDungeon = Constants.GetDeepDungeonByMapid(WorldManager.ZoneId);
                Settings.Instance.BetterSelectedLevel = Constants.SelectedDungeon.Floors.FirstOrDefault(i => i.MapId == WorldManager.ZoneId);
                Logger.Warn($"Started bot inside dungeon (Not currently selected): Using {Constants.SelectedDungeon.DisplayName}");
            }

            if (Constants.SelectedDungeon == null)
            {
                Logger.Error("No Selected Deep Dungeon: Something went really wrong");
                _root = new ActionAlwaysFail();
                return;
            }

            if (Settings.Instance.BetterSelectedLevel == null)
            {
                Settings.Instance.BetterSelectedLevel = Constants.SelectedDungeon.Floors[0];
                Logger.Error($"No floor selected, setting it to use [{Settings.Instance.BetterSelectedLevel.DisplayName}]");
            }

            Logger.Info(Constants.SelectedDungeon.ToString());
            //setup navigation manager
            Navigator.NavigationProvider = new DDNavigationProvider(new ServiceNavigationProvider());
            Navigator.PlayerMover = new SlideMover();

            TreeHooks.Instance.ClearAll();
            
            DeepTracker.InitializeTracker(Core.Me.ClassLevel);

            _tasks = new TaskManagerProvider();


            _tasks.Add(new LoadingHandler());
            _tasks.Add(new DeathWindowHandler());
            _tasks.Add(new SideStepTask());
            //not sure if i want the trap handler to be above combat or not
            _tasks.Add(new TrapHandler());

            //pomanders for sure need to happen before combat so that we can correctly apply Lust for bosses
            _tasks.Add(new Pomanders());

            _tasks.Add(new CombatHandler());

            _tasks.Add(new LobbyHandler());
            _tasks.Add(new GetToCaptain());
            _tasks.Add(new POTDEntrance());


            _tasks.Add(new CairnOfReturn());
            _tasks.Add(new FloorExit());
            _tasks.Add(new Loot());


            _tasks.Add(new StuckDetection());
            _tasks.Add(new POTDNavigation());


            _tasks.Add(new BaseLogicHandler());

            Settings.Instance.Stop = false;
            if (!Core.Me.IsDow())
            {
                Logger.Error("Please change to a DOW class");
                _root = new ActionAlwaysFail();
                return;
            }


            //setup combat manager
            CombatTargeting.Instance.Provider = new DDCombatTargetingProvider();

            GameSettingsManager.FaceTargetOnAction = true;


            if (Constants.Lang == Language.Chn)
                //回避 - sidestep
                //Zekken 
                if (PluginManager.Plugins.Any(i => (i.Plugin.Name.Contains("Zekken") || i.Plugin.Name.Contains("技能躲避")) && i.Enabled))
                {
                    Logger.Error("禁用 AOE技能躲避插件 - Zekken");
                    _root = new ActionAlwaysFail();
                    return;
                }

            if (PluginManager.Plugins.Any(i => i.Plugin.Name == "Zekken" && i.Enabled))
            {
                Logger.Error(
                    "Zekken is currently turned on, It will interfere with DeepDive & SideStep. Please Turn it off and restart the bot.");
                _root = new ActionAlwaysFail();
                return;
            }


            if (!ConditionParser.IsQuestCompleted(Constants.SelectedDungeon.UnlockQuest))
            {
                Logger.Error($"You must complete \"{DataManager.GetLocalizedQuestName(Constants.SelectedDungeon.UnlockQuest)}\" to run this base.");
                Logger.Error(
                    "Please switch to \"Order Bot\" and run the profile: \\BotBases\\DeepDive\\Profiles\\PotD_Unlock.xml");
                _root = new ActionAlwaysFail();
                return;
            }

            if (!ConditionParser.IsQuestCompleted(Settings.Instance.BetterSelectedLevel.QuestId))
            {
                Logger.Error($"You must complete \"{DataManager.GetLocalizedQuestName(Settings.Instance.BetterSelectedLevel.QuestId)}\" to run this floor.");
                Logger.Error("Complete the quest or change the floor selection");
                _root = new ActionAlwaysFail();
                return;
            }

            //Logger.Error($"Quest {Settings.Instance.BetterSelectedLevel.QuestId} - \"{DataManager.GetLocalizedQuestName(Settings.Instance.BetterSelectedLevel.QuestId)}\" to run this base.");

            StopPlz = false;

            SetupSettings();

            _root =
                new ActionRunCoroutine(async x =>
                {
                    if (StopPlz)
                        return false;
                    if (!_init)
                    {
                        Logging.Write("DeepDive is waiting on Initialization to finish");
                        return true;
                    }

                    if (await _tasks.Run())
                    {
                        await Coroutine.Yield();
                    }
                    else
                    {
                        Logger.Warn("No tasks ran");
                        await Coroutine.Sleep(1000);
                    }

                    return true;
                });
        }

        public override async Task AsyncRoot()
        {
            if (StopPlz)
                return;
            if (!_init)
            {
                Logging.Write("DeepDive is waiting on Initialization to finish");
                return;
            }

            if (await _tasks.Run())
            {
                await Coroutine.Yield();
            }
            else
            {
                Logger.Warn("No tasks ran");
                await Coroutine.Sleep(1000);
            }
        }

        private static void SetupSettings()
        {
            Logger.Info("UpdateTrapSettings");


            //mimic stuff
            if (Settings.Instance.OpenMimics)
            {
                //if we have mimics remove them from our ignore list
                if (Constants.BaseIgnoreEntity.Contains(EntityNames.MimicCoffer[0]))
                    Constants.BaseIgnoreEntity = Constants.BaseIgnoreEntity.Except(EntityNames.MimicCoffer).ToArray();
            }
            else
            {
                //if we don't have mimics add them to our ignore list
                if (!Constants.BaseIgnoreEntity.Contains(EntityNames.MimicCoffer[0]))
                    Constants.BaseIgnoreEntity = Constants.BaseIgnoreEntity.Concat(EntityNames.MimicCoffer).ToArray();
            }

            //Exploding Coffers
            if (Settings.Instance.OpenTraps)
            {
                //if we have traps remove them
                if (Constants.BaseIgnoreEntity.Contains(EntityNames.TrapCoffer))
                    Constants.BaseIgnoreEntity = Constants.BaseIgnoreEntity.Except(new[] {EntityNames.TrapCoffer}).ToArray();
            }
            else
            {
                if (!Constants.BaseIgnoreEntity.Contains(EntityNames.TrapCoffer))
                    Constants.BaseIgnoreEntity = Constants.BaseIgnoreEntity.Concat(new[] {EntityNames.TrapCoffer}).ToArray();
            }

            if (Settings.Instance.OpenSilver)
            {
                //if we have traps remove them
                if (Constants.BaseIgnoreEntity.Contains(EntityNames.SilverCoffer))
                    Constants.BaseIgnoreEntity = Constants.BaseIgnoreEntity.Except(new[] {EntityNames.SilverCoffer}).ToArray();
            }
            else
            {
                if (!Constants.BaseIgnoreEntity.Contains(EntityNames.SilverCoffer))
                    Constants.BaseIgnoreEntity = Constants.BaseIgnoreEntity.Concat(new[] {EntityNames.SilverCoffer}).ToArray();
            }

            //Add the current Dungeon's Ignores
            if (!Constants.BaseIgnoreEntity.Contains(EntityNames.OfPassage))
                Constants.BaseIgnoreEntity = Constants.BaseIgnoreEntity.Concat(new[] {EntityNames.OfPassage, EntityNames.OfReturn, EntityNames.LobbyEntrance}).ToArray();

            Constants.IgnoreEntity = Constants.SelectedDungeon.GetIgnoreEntity(Constants.BaseIgnoreEntity);

            Settings.Instance.Dump();
        }
    }
}