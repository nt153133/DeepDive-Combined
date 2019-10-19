/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Buddy.Coroutines;
using Deep.Forms;
using Deep.Helpers;
using Deep.Logging;
using Deep.Providers;
using Deep.Tasks;
using ff14bot.Behavior;
using TreeSharp;
using ff14bot.NeoProfiles;
using ff14bot;
using ff14bot.Navigation;
using ff14bot.Managers;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;
using Action = TreeSharp.Action;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.Enums;
using Deep.Tasks.Coroutines;
using ff14bot.AClasses;
using System.Windows.Controls;
using Decorator = TreeSharp.Decorator;
using ff14bot.Overlay3D;
using Deep.TaskManager;
using Deep.TaskManager.Actions;
using ff14bot.Helpers;
using FileTest;

namespace Deep
{

    public partial class DeepDungeon : AsyncBotBase
    {
        public override string EnglishName => "Deep Dungeon";
#if RB_CN
        public override string Name => "深层迷宫";
#else
        public override string Name => "Deep Dungeon";
#endif
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public override bool WantButton => true;


        public DeepDungeon()
        {
            //Captain = new GetToCaptain();
          
            if (Settings.Instance.FloorSettings == null || !Settings.Instance.FloorSettings.Any())
                Logger.Warn("Settings are empty?");

            Task.Factory.StartNew(() =>
            {
                Constants.INIT();
                Overlay3D.Drawing += DDNavigationProvider.Render;


                _init = true;
                Logger.Info("INIT DONE");
                
            });
        }

        private volatile bool _init = false;

        private TaskManagerProvider _tasks;

        #region BotBase Stuff

        private SettingsForm _settings;
        private static Version v = new Version(1, 3, 3);

        public override void OnButtonPress()
        {
            if (_settings == null)
            {
#if RB_CN
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-CN");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-CN");
#endif

                _settings = new SettingsForm
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
        /// = true when we stop gets pushed
        /// </summary>
        internal static bool StopPlz;

        public override void Stop()
        {
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
        private DeepTest _debug;

        //private DDServiceNavigationProvider serviceProvider = new DDServiceNavigationProvider();
        public override void Pulse()
        {
            if (Constants.InDeepDungeon)
            {
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
            //setup navigation manager
            Navigator.NavigationProvider = new DDNavigationProvider(new ServiceNavigationProvider());
            Navigator.PlayerMover = new SlideMover();

            TreeHooks.Instance.ClearAll();

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
            _tasks.Add(new GetToCaptiain());
            _tasks.Add(new POTDEntrance());


            _tasks.Add(new CarnOfReturn());
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
            {
                //回避 - sidestep
                //Zekken 
                if (PluginManager.Plugins.Any(i => (i.Plugin.Name.Contains("Zekken") || i.Plugin.Name.Contains("技能躲避")) && i.Enabled))
                {
                    Logger.Error("禁用 AOE技能躲避插件 - Zekken");
                    _root = new ActionAlwaysFail();
                    return;
                }

            }
            if (PluginManager.Plugins.Any(i => i.Plugin.Name == "Zekken" && i.Enabled))
            {
                Logger.Error(
                    "Zekken is currently turned on, It will interfere with DeepDive & SideStep. Please Turn it off and restart the bot.");
                _root = new ActionAlwaysFail();
                return;
            }



            if (!ConditionParser.IsQuestCompleted(67092))
            {
                Logger.Error("You must complete \"The House That Death Built\" to run this base.");
                Logger.Error(
                    "Please switch to \"Order Bot\" and run the profile: \\BotBases\\DeepDive\\Profiles\\PotD_Unlock.xml");
                _root = new ActionAlwaysFail();
                return;
            }

            StopPlz = false;
            
            SetupSettings();
            
            if (ShowDebug)
            {
                if (_debug == null)

                    try
                    {
                        Thread Messagethread = new Thread(new ThreadStart(delegate()
                        {
                            _debug = new DeepTest();
                            _debug.ShowDialog();
                        }));
                        Messagethread.SetApartmentState(ApartmentState.STA);
                        Messagethread.Start();

                        //DeepTracker._debug = _debug;
                    }
                    catch (Exception)
                    { }
            }
            

            _root =
                new ActionRunCoroutine(async x =>
                {
                    if (StopPlz)
                        return false;
                    if (!_init)
                    {
                        ff14bot.Helpers.Logging.Write($"DeepDive is waiting on Initialization to finish");
                        return true;
                    }
                    if (await _tasks.Run())
                    {
                        await Coroutine.Yield();
                    }
                    else
                    {
                        Logger.Warn($"No tasks ran");
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
                ff14bot.Helpers.Logging.Write($"DeepDive is waiting on Initialization to finish");
                return;
            }
            if (await _tasks.Run())
            {
                await Coroutine.Yield();
            }
            else
            {
                Logger.Warn($"No tasks ran");
                await Coroutine.Sleep(1000);
            }
            return;
        }

        private static void SetupSettings()
        {
            Logger.Info("UpdateTrapSettings");
            //mimic stuff
            if (Settings.Instance.OpenMimics)
            {
                //if we have mimics remove them from our ignore list
                if (Constants.IgnoreEntity.Contains(EntityNames.MimicCoffer[0]))
                    Constants.IgnoreEntity = Constants.IgnoreEntity.Except(EntityNames.MimicCoffer).ToArray();
            }
            else
            {
                //if we don't have mimics add them to our ignore list
                if (!Constants.IgnoreEntity.Contains(EntityNames.MimicCoffer[0]))
                    Constants.IgnoreEntity = Constants.IgnoreEntity.Concat(EntityNames.MimicCoffer).ToArray();
            }

            //Exploding Coffers
            if (Settings.Instance.OpenTraps)
            {
                //if we have traps remove them
                if (Constants.IgnoreEntity.Contains(EntityNames.TrapCoffer))
                    Constants.IgnoreEntity = Constants.IgnoreEntity.Except(new[] { EntityNames.TrapCoffer }).ToArray();
            }
            else
            {
                if (!Constants.IgnoreEntity.Contains(EntityNames.TrapCoffer))
                    Constants.IgnoreEntity = Constants.IgnoreEntity.Concat(new[] { EntityNames.TrapCoffer }).ToArray();
            }

            if (Settings.Instance.OpenSilver)
            {
                //if we have traps remove them
                if (Constants.IgnoreEntity.Contains(EntityNames.SilverCoffer))
                    Constants.IgnoreEntity = Constants.IgnoreEntity.Except(new[] { EntityNames.SilverCoffer }).ToArray();
            }
            else
            {
                if (!Constants.IgnoreEntity.Contains(EntityNames.SilverCoffer))
                    Constants.IgnoreEntity = Constants.IgnoreEntity.Concat(new[] { EntityNames.SilverCoffer }).ToArray();
            }
            Settings.Instance.Dump();

        }
    }

}
