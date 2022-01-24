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
using ff14bot.Objects;
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
    /// Notable Deep Dungeon monsters. Usually have special mechanics.
    /// </summary>
    internal static partial class Mobs
    {
        internal const uint PalaceHornet = 4981;
        internal const uint PalaceSlime = 4990;
    }

    /// <summary>
    /// Various non-monster Deep Dungeon NPCs and objects.
    /// </summary>
    internal static partial class Entities
    {
        #region Treasure Chests
        internal const uint TrapCoffer = 2005808;
        internal const uint GoldCoffer = 2007358;
        internal const uint SilverCoffer = 2007357;

        internal const uint Hidden = 2007542;
        internal const uint BandedCoffer = 2007543;

        internal static readonly uint[] MimicCoffer = { 2006020, 2006022 };
        #endregion

        internal static uint OfPassage => Constants.SelectedDungeon.OfPassage;
        internal static uint OfReturn => Constants.SelectedDungeon.OfReturn;
        internal static uint BossExit => Constants.SelectedDungeon.BossExit;
        internal static uint LobbyExit => Constants.SelectedDungeon.LobbyExit;
        internal static uint LobbyEntrance => Constants.SelectedDungeon.LobbyEntrance;


        #region Pets
        internal const uint RubyCarby = 5478;
        internal const uint TopazCarby = 1400;
        internal const uint EmeraldCarby = 1401;
        internal const uint IfritEgi = 1402;
        internal const uint TitanEgi = 1403;
        internal const uint GarudaEgi = 1404;
        internal const uint DemiBahamut = 6566;
        internal const uint DemiPhoenix = 8228;

        internal const uint Eos = 1398;
        internal const uint Selene = 1399;

        internal const uint RookAutoturret = 3666;
        internal const uint BishopAutoturret = 3667;
        internal const uint AutomatonQueen = 8230;
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
        #region Trap Auras
        internal const uint Otter = 1546;
        internal const uint Frog = 1101;
        internal const uint Toad = 439;
        internal const uint Toad2 = 441;
        internal const uint Chicken = 1102;
        internal const uint Imp = 1103;

        /// <summary>
        /// Enervation. Damage dealt is reduced and damage taken is increased.
        /// </summary>
        internal const uint Enervation = 546;

        /// <summary>
        /// Pacification. Unable to use weaponskills.
        /// </summary>
        internal const uint Pacification = 620;

        /// <summary>
        /// Silence. Unable to cast spells.
        /// </summary>
        internal const uint Silence = 7;
        #endregion

        #region Pomander Auras
        /// <summary>
        /// Transfiguration. Transformed into succubus by Pomander of Lust.
        /// </summary>
        internal const uint Lust = 565;

        /// <summary>
        /// Transfiguration. Transformed into manticore by Pomander of Rage.
        /// </summary>
        internal const uint Rage = 565;

        /// <summary>
        /// Vulnerability Down. Damage taken is reduced by Pomander of Steel.
        /// </summary>
        internal const uint Steel = 1100;

        /// <summary>
        /// Damage Up. Damage dealt is increased by Pomander of Strength.
        /// </summary>
        internal const uint Strength = 687;
        #endregion

        #region Floor Auras
        /// <summary>
        /// Blind. Floor's encroaching darkness is lowering accuracy.
        /// </summary>
        internal const uint Blind = 1088;

        /// <summary>
        /// HP Penalty. Floor is decreasing maximum HP.
        /// </summary>
        internal const uint HpDown = 1089;

        /// <summary>
        /// Damage Down. Floor is reducing damage dealt.
        /// </summary>
        internal const uint DamageDown = 1090;

        /// <summary>
        /// Haste. Floor makes weaponskills, spells, and auto-attacks faster.
        /// </summary>
        internal const uint Haste = 1091;

        /// <summary>
        /// Amnesia. Floor prevents use of abilities/off-GCDs.
        /// </summary>
        internal const uint Amnesia = 1092;

        /// <summary>
        /// HP & MP Boost. Floor is increasing maximum HP and MP.
        /// </summary>
        internal const uint HpMpBoost = 1093;

        /// <summary>
        /// Item Penalty. Floor is preventing use of items and pomanders.
        /// </summary>
        internal const uint ItemPenalty = 1094;

        /// <summary>
        /// Sprint Penalty. Floor is preventing use of Sprint.
        /// </summary>
        internal const uint SprintPenalty = 1095;

        /// <summary>
        /// Knockback Penalty. Floor is preventing knockback and draw-in effects.
        /// </summary>
        internal const uint KnockbackPenalty = 1096;

        /// <summary>
        /// Auto-Heal Penalty. Floor has stopped HP regeneration.
        /// </summary>
        internal const uint NoAutoHeal = 1097;
        #endregion

        /// <summary>
        /// Sustain. Regenerating HP over time from Sustaining/Empyrean Potion.
        /// </summary>
        internal const uint Sustain = 184;

        /// <summary>
        /// Accursed Pox. Damage over time, HP regeneration stopped, damage dealt reduced.
        /// </summary>
        internal const uint Pox = 1087;

        /// <summary>
        /// Vulnerability Up. Applied by Pomdander of Lust's Void Fire II.
        /// </summary>
        internal const uint LustVulnerabilityUp = 714;

        /// <summary>
        /// Various poison aura IDs.
        /// </summary>
        public static readonly uint[] Poisons =
        {
            18, 275, 559, 560, 686, 801
        };
    }

    internal static partial class Spells
    {
        internal const uint LustSpell = 6274;
        internal const uint RageSpell = 6273;
        internal const uint ResolutionSpell = 6871;

        internal const uint FinalSting = 6334;
        internal const uint StoneGaze = 6351;
        internal const uint BlindingBurst1 = 393;
        internal const uint BlindingBurst2 = 12174;

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
        public uint Recovery => (uint)Math.Min(RecoverMax, Max[1]);

        public float LevelScore => Max[1] / RecoverMax;


        public float EffectiveMax(float playerMaxHealth, bool hq)
        {
            int index = hq ? 1 : 0;
            return Math.Min(playerMaxHealth * Rate[index], Max[index]);
        }

        public float EffectiveHPS(float playerMaxHealth, bool hq)
        {
            float effectiveMax = EffectiveMax(playerMaxHealth, hq);

            float cooldown = ItemData[hq ? 1 : 0].Cooldown;
            if (hq)
            {
                cooldown *= 0.89f;
            }

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
        static Constants()
        {
            Potions = ResourceHelpers.LoadResource<Potion[]>(Resources.Potions).ToDictionary(r => r.Id, r => r);
            foreach (KeyValuePair<uint, Potion> pot in Potions)
            {
                pot.Value.Setup();
            }
        }

        public static void INIT()
        {
            Language field = (Language)typeof(DataManager).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .First(i => i.FieldType == typeof(Language)).GetValue(null);

            Lang = field;

            OffsetManager.Init();
        }

        internal static uint[] BaseIgnoreEntity = new uint[]
        {
            5042,
            2002872,  // 2002872 is an unknown object in boss rooms that the bot tries to target
            Entities.RubyCarby, Entities.TopazCarby, Entities.EmeraldCarby,
            Entities.GarudaEgi, Entities.TitanEgi, Entities.IfritEgi,
            Entities.DemiBahamut, Entities.DemiPhoenix,
            Entities.Eos, Entities.Selene,
            Entities.RookAutoturret, Entities.BishopAutoturret, Entities.AutomatonQueen
        };

        internal static Language Lang;

        internal static uint[] IgnoreEntity;

        internal static Vector3 EntranceNpcPosition => SelectedDungeon.CaptainNpcPosition;
        internal static uint EntranceNpcId => SelectedDungeon.CaptainNpcId;
        internal static uint AetheryteId => SelectedDungeon.EntranceAetheryte;
        internal static AetheryteResult EntranceZone => DataManager.AetheryteCache[AetheryteId];
        internal static uint EntranceZoneId => EntranceZone.ZoneId;
        internal static IEnumerable<uint> DeepDungeonRawIds => SelectedDungeon.DeepDungeonRawIds;

        internal static HashSet<uint> Exits = new HashSet<uint> {
            Entities.OfPassage,
            Entities.BossExit,
            Entities.LobbyExit
        };

        /// <summary>
        /// Determines if <see cref="GameObject"/> is current floor exit.
        /// </summary>
        /// <param name="obj"><see cref="GameObject"/> to test.</param>
        /// <returns><see langword="true"/> if exit.</returns>
        public static bool IsFloorExit(GameObject obj)
        {
            return Exits.Any(exit => obj.NpcId == exit);
        }

        /// <summary>
        /// <see langword="true"/> if in any Deep Dungeon zone.
        /// </summary>
        internal static bool InDeepDungeon => DeepDungeonRawIds.Contains(WorldManager.ZoneId);

        /// <summary>
        /// <see langword="true"/> if in post-Deep Dungeon exit lobby.
        /// </summary>
        public static bool InExitLobby => WorldManager.ZoneId == SelectedDungeon.LobbyId;

        /// <summary>
        /// Calculates max pull range.  Capped to prevent pulling around corners
        /// or from other rooms by long-range classes.
        /// </summary>
        internal static float ModifiedCombatReach
        {
            get
            {
                if (Core.Me.CurrentJob.IsMelee())
                {
                    return Math.Max(12, RoutineManager.Current.PullRange + Core.Me.CombatReach);
                }

                return Math.Min(15, RoutineManager.Current.PullRange + Core.Me.CombatReach);
            }
        }

        internal static AgentDeepDungeonSaveData GetSaveInterface()
        {
            //cn = 3
            //64 = 2
            //32 = 1
            return AgentModule.GetAgentInterfaceByType<AgentDeepDungeonSaveData>();
        }

        #region DataAsResource

        internal static Dictionary<uint, uint> Maps => SelectedDungeon.WallMapData;

        internal static readonly HashSet<uint> TrapIds = new HashSet<uint>
        {
            2007182,
            2007183,
            2007184,
            2007185,
            2007186,
            2009504
        };

        internal static Dictionary<uint, Potion> Potions { get; }

        #endregion
    }
}