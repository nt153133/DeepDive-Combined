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
using System.Reflection;
using Clio.Utilities;
using DeepCombined.Helpers;
using DeepCombined.Memory;
using DeepCombined.Properties;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.RemoteAgents;
using Newtonsoft.Json;

namespace DeepCombined
{
    public static class PoiTypes
    {
        public const int ExplorePOI = 9;
        public const int UseCairnOfReturn = 10;
    }

    /// <summary>
    ///     Notable mobs in Deep Dungeon
    /// </summary>
    internal static partial class Mobs
    {
        internal const uint PalaceHornet = 4981;
        internal const uint PalaceSlime = 4990;
    }

    /// <summary>
    ///     Various entity Ids present in Deep Dungeon
    /// </summary>
    internal static class EntityNames
    {
        internal const uint TrapCoffer = 2005808;
        internal const uint GoldCoffer = 2007358;
        internal const uint SilverCoffer = 2007357;

        internal const uint Hidden = 2007542;
        internal const uint BandedCoffer = 2007543;

        internal static readonly uint[] MimicCoffer = {2006020, 2006022};

        internal static uint OfPassage => Constants.SelectedDungeon.OfPassage;
        internal static uint OfReturn => Constants.SelectedDungeon.OfReturn;
        internal static uint BossExit => Constants.SelectedDungeon.BossExit;
        internal static uint LobbyExit => Constants.SelectedDungeon.LobbyExit;
        internal static uint LobbyEntrance => Constants.SelectedDungeon.LobbyEntrance;


        #region Pets

        internal const uint RubyCarby = 5478;

        internal const uint Garuda = 1404;
        internal const uint TopazCarby = 1400;
        internal const uint EmeraldCarby = 1401;
        internal const uint Titan = 1403;
        internal const uint Ifrit = 1402;

        internal const uint Eos = 1398;
        internal const uint Selene = 1399;

        internal const uint Rook = 3666;
        internal const uint Bishop = 3667;

        #endregion
    }

    internal static class Items
    {
        internal const int Antidote = 4564;
        internal const int EchoDrops = 4566;
        internal static int SustainingPotion => Constants.SelectedDungeon.SustainingPotion;
    }

    internal static partial class Auras
    {
        internal const uint Odder = 1546;
        internal const uint Frog = 1101;
        internal const uint Toad = 439;
        internal const uint Toad2 = 441;
        internal const uint Chicken = 1102;
        internal const uint Imp = 1103;


        internal const uint Lust = 565;
        internal const uint Rage = 565;

        internal const uint Steel = 1100;
        internal const uint Strength = 687;

        internal const uint Sustain = 184;

        internal const uint Enervation = 546;
        internal const uint Pacification = 620;
        internal const uint Silence = 7;


        public static readonly uint[] Poisons =
        {
            18, 275, 559, 560, 686, 801
        };

        #region Floor Debuffs

        internal const uint Pox = 1087;
        internal const uint Blind = 1088;
        internal const uint HpDown = 1089;
        internal const uint DamageDown = 1090;
        internal const uint Amnesia = 1092;
        internal const uint UnMagicked = 1549;

        internal const uint ItemPenalty = 1094;
        internal const uint SprintPenalty = 1095;

        internal const uint KnockbackPenalty = 1096;
        internal const uint NoAutoHeal = 1097;

        #endregion
    }

    internal static class Spells
    {
        internal const uint LustSpell = 6274;
        internal const uint RageSpell = 6273;
        internal const uint ResolutionSpell = 6871;
    }

    internal static class WindowNames
    {
        internal const string DDmenu = "DeepDungeonMenu";
        internal const string DDsave = "DeepDungeonSaveData";
        internal const string DDmap = "DeepDungeonMap";
        internal const string DDStatus = "DeepDungeonStatus";
        internal const string DDResult = "DeepDungeonResult";
    }

    internal class Potion
    {
        private float[] HPs;

        [JsonProperty("Id")] public uint Id;

        private Item[] ItemData;

        [JsonProperty("Level")] public uint Level;

        [JsonProperty("Max")] public uint[] Max;

        [JsonProperty("Rate")] public float[] Rate;

        public float RecoverMax => Core.Me.MaxHealth * Rate[1];
        public uint Recovery => (uint) Math.Min(RecoverMax, Max[1]);

        public float LevelScore => Max[1] / RecoverMax;


        public float EffectiveMax(float playerMaxHealth, bool hq)
        {
            var index = hq ? 1 : 0;
            return Math.Min(playerMaxHealth * Rate[index], Max[index]);
        }

        public float EffectiveHPS(float playerMaxHealth, bool hq)
        {
            var effectiveMax = EffectiveMax(playerMaxHealth, hq);
            float cooldown = ItemData[hq ? 1 : 0].Cooldown;
            if (hq)
                cooldown = cooldown * .89f;


            //Logger.Info($"{ItemData[hq ? 1 : 0]}  has a effective HPS of {effectiveMax / cooldown}");
            return effectiveMax / cooldown;
        }

        internal void Setup()
        {
            ItemData = new Item[2]
            {
                DataManager.GetItem(Id),
                DataManager.GetItem(Id, true)
            };

            HPs = new[]
            {
                Max[0] / (float) ItemData[0].Cooldown,
                Max[1] / (float) ItemData[1].Cooldown
            };
        }
    }


    internal static partial class Constants
    {
        //2002872 = some random thing that the bot tries to target in boss rooms. actual purpose unknown
        internal static uint[] BaseIgnoreEntity =
        {
            5042, 5402, 2002872, EntityNames.RubyCarby, EntityNames.EmeraldCarby, EntityNames.TopazCarby, EntityNames.Garuda,
            EntityNames.Titan, EntityNames.Ifrit, EntityNames.Eos, EntityNames.Selene, EntityNames.Rook,
            EntityNames.Bishop
        };

        internal static uint MapVersion = 4;

        internal static Language Lang;

        internal static uint[] IgnoreEntity;

        static Constants()
        {
            Pots = loadResource<Potion[]>(Resources.pots).ToDictionary(r => r.Id, r => r);
            foreach (var pot in Pots)
            {
                PotionIds.Add(pot.Key);
                pot.Value.Setup();
            }
        }

        internal static Vector3 EntranceNpcPosition => SelectedDungeon.CaptainNpcPosition;
        internal static uint EntranceNpcId => SelectedDungeon.CaptainNpcId;
        internal static uint AetheryteId => SelectedDungeon.EntranceAetheryte;
        internal static AetheryteResult EntranceZone => DataManager.AetheryteCache[AetheryteId];
        internal static uint EntranceZoneId => EntranceZone.ZoneId;
        internal static IEnumerable<uint> DeepDungeonRawIds => SelectedDungeon.DeepDungeonRawIds;

        internal static IEnumerable<uint> Exits =>
            new[] {EntityNames.OfPassage, EntityNames.BossExit, EntityNames.LobbyExit};

        /// <summary>
        ///     returns true if we are in any of the Deep Dungeon areas.
        /// </summary>
        internal static bool InDeepDungeon => DeepDungeonRawIds.Contains(WorldManager.ZoneId);

        /// <summary>
        ///     Pull range (Max of 15 to stop from attacking around corners on classes with large pull ranges)
        /// </summary>
        internal static float ModifiedCombatReach
        {
            get
            {
                if (Core.Me.CurrentJob.IsMelee())
                    return Math.Max(12, RoutineManager.Current.PullRange + Core.Me.CombatReach);
                
                return Math.Min(15, RoutineManager.Current.PullRange + Core.Me.CombatReach);
            }
        }

        //cn = 3
        //64 = 2
        //32 = 1
        internal static AgentDeepDungeonSaveData GetSaveInterface()
        {
            return AgentModule.GetAgentInterfaceByType<AgentDeepDungeonSaveData>();
        }

        public static void INIT()
        {
            var field = (Language) typeof(DataManager).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .First(i => i.FieldType == typeof(Language)).GetValue(null);

            Lang = field;

            OffsetManager.Init();
        }

        #region DataAsResource

        internal static Dictionary<uint, uint> Maps => SelectedDungeon.WallMapData;

        internal static readonly uint[] TrapIds =
        {
            2007182,
            2007183,
            2007184,
            2007185,
            2007186,
            2009504
        };

        internal static HashSet<uint> PotionIds = new HashSet<uint>();
        internal static Dictionary<uint, Potion> Pots { get; }

        public static bool InExitLevel => WorldManager.ZoneId == SelectedDungeon.LobbyId;

        /// <summary>
        ///     loads a json resource file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        private static T loadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        #endregion
    }
}