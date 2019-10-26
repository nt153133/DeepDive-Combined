/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using Deep.DungeonDefinition.Base;
using Deep.Enums;
using Deep.Helpers.Logging;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using Newtonsoft.Json;

namespace Deep
{
    internal class Settings : JsonSettings
    {
        private static Settings _settings;


        private bool _antidote;

        private FloorSetting _BetterSelectedLevel;

        private bool _echoDrop;

        private List<Structure.FloorSetting> _floorSettings;

        private bool _GoExit;

        private bool _goFortheHoard;
        private bool _initialized;

        private bool _openMimics;

        private bool _openSilver;

        private bool _openTraps;

        private float _pullRange;

        private bool _saveSteel;

        private bool _savestr;

        private Structure.FloorSetting _selectedLevel;

        private bool _startAt51;

        private bool _stop;

        private bool _stopsolo;

        private bool _usePomAlt;
        private bool _usePomRage;


        private SaveSlot _useSaveSlots;

        private bool _useSustain;

        private bool _verboseLogging;
        private int _SelectedDungeonIndex;


        public Settings() : base(Path.Combine(GetSettingsFilePath(Core.Me.Name, "DeepDive.json")))
        {
        }

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

        [Setting]
        [Description("Enable the Debug Renderer")]
        [DefaultValue(false)]
        [JsonProperty("DebugRender")]
        [Category("Debug")]
        public bool DebugRender { get; set; }

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

        [Browsable(false)]
        [JsonProperty("FloorSettings")]
        public List<Structure.FloorSetting> FloorSettings
        {
            get => _floorSettings ?? EnsureFloorSettings();
            set
            {
                _floorSettings = value;
                Save();
            }
        }

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

        [Setting]
        [Description("open traps")]
        [JsonProperty("OpenTraps")]
        [DefaultValue(false)]
        [Category("Chests")]
        public bool OpenTraps
        {
            get => _openTraps;
            set
            {
                _openTraps = value;
                Save();
            }
        }

        [Setting]
        [Description("open Silver Chests")]
        [DefaultValue(true)]
        [JsonProperty("OpenSilver")]
        [Category("Chests")]
        public bool OpenSilver
        {
            get => _openSilver;
            set
            {
                _openSilver = value;
                Save();
            }
        }

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

        [Setting]
        [Description("go for the hoard when we are prioritizing the exit")]
        [JsonProperty("GoForTheHoard")]
        [DefaultValue(false)]
        [Category("Chests")]
        public bool GoForTheHoard
        {
            get => _goFortheHoard;
            set
            {
                _goFortheHoard = value;
                Save();
            }
        }

        [Setting]
        [Description("Save Pomander of Strength")]
        [JsonProperty("SaveStr")]
        [DefaultValue(true)]
        [Category("Pots & Pomanders")]
        public bool SaveStr
        {
            get => _savestr;
            set
            {
                _savestr = value;
                Save();
            }
        }

        [Setting]
        [Description("Save Pomander of Steel")]
        [JsonProperty("SaveSteel")]
        [DefaultValue(true)]
        [Category("Pots & Pomanders")]
        public bool SaveSteel
        {
            get => _saveSteel;
            set
            {
                _saveSteel = value;
                Save();
            }
        }

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
                if (_initialized) Logger.Verbose($"Stop state has changed to: {value}");
                Save();
            }
        }

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

        [Browsable(false)]
        [JsonProperty("SelectedLevel")]
        public Structure.FloorSetting SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                _selectedLevel = value;
                Save();
            }
        }

        [Browsable(false)]
        [JsonProperty("BetterSelectedLevel")]
        public FloorSetting BetterSelectedLevel
        {
            get => _BetterSelectedLevel;
            set
            {
                _BetterSelectedLevel = value;
                Save();
            }
        }
        
        [Browsable(false)]
        [DefaultValue(0)]
        [JsonProperty("SelectedDungeon")]
        public int SelectedDungeon
        {
            get => _SelectedDungeonIndex;
            set
            {
                _SelectedDungeonIndex = value;
                Save();
            }
        }

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

        internal List<Structure.FloorSetting> EnsureFloorSettings()
        {
            if (!_initialized) return _floorSettings;

            if (_floorSettings == null || !_floorSettings.Any())
            {
                var llnext = new List<Structure.FloorSetting>();

                for (var i = 10; i <= 100; i += 10)
                {
                    llnext.Add(new Structure.FloorSetting
                    {
                        LevelMax = i
                    });
                }

                _floorSettings = llnext;
            }

            if (SelectedLevel == null)
                SelectedLevel = FloorSettings.First();

            return _floorSettings;
        }

        #region Dump

        #endregion
    }
}