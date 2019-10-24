/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Deep.Enums;
using Deep.Structure;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using Deep.Helpers.Logging;

namespace Deep
{


    internal class Settings : JsonSettings
    {

        private static Settings _settings;
        private bool _initialized = false;

        public static Settings Instance
        {
            get
            {
                if (_settings != null)
                    return _settings;
                _settings = new Settings();
                //_settings.LoadFrom(Path.Combine(GetSettingsFilePath(Core.Me.Name, "DeepDive.json")));
                _settings._initialized = true;

                return _settings;
            }
        }


        public Settings() : base(Path.Combine(GetSettingsFilePath(Core.Me.Name, "DeepDive.json")))
        { }


        private SaveSlot _useSaveSlots;

        [Setting]
        [DefaultValue(SaveSlot.First)]
        [JsonProperty("SaveSlot")]
        [Category("General")]
        public SaveSlot SaveSlot
        {
            get => _useSaveSlots;
            set
            {
                _useSaveSlots = value;
                Save();
            }
        }

        private bool _render;

        [Setting]
        [Description("Enable the Debug Renderer")]
        [DefaultValue(false)]
        [JsonProperty("DebugRender")]
        [Category("Debug")]
        public bool DebugRender
        {
            get => _render;
            set
            {
                _render = value;
            }
        }

        private bool _GoExit;

        [Setting]
        [Description("Prioritize the exit - Party mode only")]
        [DefaultValue(false)]
        [JsonProperty("GoExit")]
        [Category("Party")]
        public bool GoExit
        {
            get => _GoExit;
            set
            {
                _GoExit = value;
                Save();
            }
        }

        private bool _useSustain;
        [Setting]
        [Description("UseSustain")]
        [DefaultValue(true)]
        [JsonProperty("Sustain")]
        [Category("Pots & Pomanders")]
        public bool UseSustain
        {
            get => _useSustain;
            set
            {
                _useSustain = value;
                Save();
            }
        }

        private List<FloorSetting> _floorSettings;

        [Browsable(false)]
        [JsonProperty("FloorSettings")]
        public List<FloorSetting> FloorSettings
        {
            get => _floorSettings ?? (EnsureFloorSettings());
            set
            {
                _floorSettings = value;
                Save();
            }
        }
        
        private bool _verboseLogging;
        [Setting]
        [Description("enables verbose logging")]
        [DefaultValue(true)]
        [JsonProperty("VerboseLogging")]
        [Category("General")]
        public bool VerboseLogging
        {
            get => _verboseLogging;
            set
            {
                _verboseLogging = value;
                Save();
            }
        }

        private bool _startAt51;
        [Setting]
        [Description("Start at floor 51 when we can.")]
        [JsonProperty("StartAt51")]
        [DefaultValue(false)]
        [Category("General")]
        public bool StartAt51
        {
            get => _startAt51;
            set
            {
                _startAt51 = value;
                Save();
            }
        }

        private bool _openMimics;
        [Setting]
        [Description("Interact with mimic chests?")]
        [JsonProperty("OpenMimics")]
        [DefaultValue(false)]
        [Category("Chests")]
        public bool OpenMimics
        {
            get => _openMimics;
            set
            {
                _openMimics = value;
                Save();
            }
        }

        private bool _openTraps;
        [Setting]
        [Description("open traps")]
        [JsonProperty("OpenTraps")]
        [DefaultValue(false)]
        [Category("Chests")]
        public bool OpenTraps
        {
            get => _openTraps;
            set {
                _openTraps = value;
                Save();
            }
        }

        private bool _openSilver;
        [Setting]
        [Description("open Silver Chests")]
        [DefaultValue(true)]
        [JsonProperty("OpenSilver")]
        [Category("Chests")]
        public bool OpenSilver
        {
            get { return _openSilver; }
            set
            {
                _openSilver = value;
                Save();
            }
        }

        private float _pullRange;
        [Setting]
        [Description("Modifies the default pull range by this amount (Positive values decrease the default pull range)")]
        [DefaultValue(0)]
        [JsonProperty("PullRange")]
        [Category("Party")]
        public float PullRange
        {
            get => _pullRange;
            set
            {
                _pullRange = value;
                Save();
            }
        }

        private bool _goFortheHoard;
        [Setting]
        [Description("go for the hoard when we are prioritizing the exit")]
        [JsonProperty("GoForTheHoard")]
        [DefaultValue(false)]
        [Category("Chests")]
        public bool GoForTheHoard
        {
            get => _goFortheHoard;
            set { _goFortheHoard = value; Save(); }
        }

        private bool _savestr;
        [Setting]
        [Description("Save Pomander of Strength")]
        [JsonProperty("SaveStr")]
        [DefaultValue(true)]
        [Category("Pots & Pomanders")]
        public bool SaveStr
        {
            get => _savestr;
            set { _savestr = value; Save(); }
        }

        private bool _saveSteel;
        [Setting]
        [Description("Save Pomander of Steel")]
        [JsonProperty("SaveSteel")]
        [DefaultValue(true)]
        [Category("Pots & Pomanders")]
        public bool SaveSteel
        {
            get => _saveSteel;
            set { _saveSteel = value; Save(); }
        }


        private bool _antidote;
        [Setting]
        [Description("Antidote usage")]
        [DefaultValue(false)]
        [JsonProperty("UseAntidote")]
        [Category("Pots & Pomanders")]
        public bool UseAntidote
        {
            get => _antidote;
            set
            {
                _antidote = value;
                Save();
            }
        }

        private bool _echoDrop;
        [Setting]
        [Description("EchoDrop usage")]
        [DefaultValue(false)]
        [JsonProperty("UseEchoDrops")]
        [Category("Pots & Pomanders")]
        public bool UseEchoDrops
        {
            get => _echoDrop;
            set
            {
                _echoDrop = value;
                Save();
            }
        }
        private bool _usePomRage;
        [Setting]
        [Description("Pomander of Rage usage")]
        [DefaultValue(false)]
        [JsonProperty("UsePomRage")]
        [Category("Pots & Pomanders")]
        public bool UsePomRage
        {
            get => _usePomRage;
            set
            {
                _usePomRage = value;
                Save();
            }
        }

        private bool _usePomAlt;
        [Setting]
        [Description("Pomander of Alteration usage")]
        [DefaultValue(false)]
        [JsonProperty("UsePomAlt")]
        [Category("Pots & Pomanders")]
        public bool UsePomAlt
        {
            get => _usePomAlt;
            set
            {
                _usePomAlt = value;
                Save();
            }
        }

        private bool _stop;
        [Setting]
        [Description("Stop the bot after we finish the current dungeon")]
        [JsonProperty("Stop")]
        [DefaultValue(false)]
        [Category("_Stop after current run")]
        public bool Stop
        {
            get => _stop;
            set
            {
                _stop = value;
                if (_initialized)
                {
                    Logger.Verbose($"Stop state has changed to: {value}");
                }
                Save();
            } 
        }

        private bool _stopsolo;
        [Browsable(false)]
        [JsonProperty("SoloStop")]
        [DefaultValue(false)]
        public bool SoloStop
        {
            get => _stopsolo;
            set
            {
                _stopsolo = value;
                Save();
            }
        }

        private FloorSetting _selectedLevel;
        [Browsable(false)]
        [JsonProperty("SelectedLevel")]
        public FloorSetting SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                _selectedLevel = value;
                Save();
            }
        }

        private DungeonDefinition.Base.FloorSetting _BetterSelectedLevel;
        [Browsable(false)]
        [JsonProperty("BetterSelectedLevel")]
        public DungeonDefinition.Base.FloorSetting BetterSelectedLevel
        {
            get => _BetterSelectedLevel;
            set
            {
                _BetterSelectedLevel = value;
                Save();
            }
        }
        
        #region Dump



        #endregion
        internal void Dump()
        {
            _saveSteel = true;
            _savestr = true;
            _openMimics = false;
            _openTraps = false;
            _useSustain = true;
            //Logger.Verbose("Save Steel: {0}", _saveSteel);
            //Logger.Verbose("Save Strength: {0}", _savestr);
            Logger.Verbose("Go For Cache: {0}", _goFortheHoard);
            Logger.Verbose("Open Silver: {0}", _openSilver);
            //Logger.Verbose("Open Mimics: {0}", _openMimics);
            //Logger.Verbose("Open Traps: {0}", _openTraps);
            
            Logger.Verbose("Exit Priority: {0}", _GoExit);
            Logger.Verbose("Save slot: {0}", SaveSlot);
            
            //Logger.Verbose("Use Sustain Pot: {0}", UseSustain);
            
            Logger.Verbose("Combat Pull range: {0}", Constants.ModifiedCombatReach);
            Logger.Verbose("In Party: {0}", PartyManager.IsInParty);
            //Logger.Verbose("StopSolo: {0}", SoloStop);
            Logger.Verbose("Start at {1} : {0}", _startAt51, Constants.SelectedDungeon.CheckPointLevel);
            Logger.Verbose($"Selected Dungeon: {Constants.SelectedDungeon.DisplayName}");
            Logger.Verbose($"Selected Floor: {_BetterSelectedLevel.DisplayName}");

            /*
            EnsureFloorSettings();
            foreach (var f in FloorSettings)
            {
                Logger.Verbose(f.Display);
            }
            */
        }

        internal List<FloorSetting> EnsureFloorSettings()
        {
            if (!_initialized) return _floorSettings;

            if (_floorSettings == null || !_floorSettings.Any())
            {
                var llnext = new List<FloorSetting>();

                for (var i = 10; i <= 100; i += 10)
                {
                    llnext.Add(new FloorSetting
                    {
                        LevelMax = i,
                    });
                }

                _floorSettings = llnext;
            }
            if (SelectedLevel == null)
                SelectedLevel = FloorSettings.First();

            return _floorSettings;
        }
    }
}