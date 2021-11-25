using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DeepCombined.DungeonDefinition;
using DeepCombined.DungeonDefinition.Base;
using DeepCombined.Properties;
using DeepCombined.Structure;
using ff14bot;
using ff14bot.Directors;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;

namespace DeepCombined
{
    internal static partial class Constants
    {
        public static List<IDeepDungeon> DeepListType;

        public static IDeepDungeon SelectedDungeon;

        public static BindingList<ClassLevelTarget> ClassLevelTargets = new BindingList<ClassLevelTarget>();

        public static bool UseJobList = false;

        internal static readonly Dictionary<int, int> Percent = new Dictionary<int, int>
        {
            {0, 0},
            {1, 9},
            {2, 18},
            {3, 27},
            {4, 36},
            {5, 45},
            {6, 54},
            {7, 63},
            {8, 72},
            {9, 81},
            {10, 90},
            {11, 100}
        };

        public static readonly Dictionary<ClassJobType, ClassJobType> ClassMap = new Dictionary<ClassJobType, ClassJobType>
        {
            {ClassJobType.Adventurer, ClassJobType.Adventurer},
            {ClassJobType.Gladiator, ClassJobType.Gladiator},
            {ClassJobType.Pugilist, ClassJobType.Pugilist},
            {ClassJobType.Marauder, ClassJobType.Marauder},
            {ClassJobType.Lancer, ClassJobType.Lancer},
            {ClassJobType.Archer, ClassJobType.Archer},
            {ClassJobType.Conjurer, ClassJobType.Conjurer},
            {ClassJobType.Thaumaturge, ClassJobType.Thaumaturge},
            {ClassJobType.Carpenter, ClassJobType.Carpenter},
            {ClassJobType.Blacksmith, ClassJobType.Blacksmith},
            {ClassJobType.Armorer, ClassJobType.Armorer},
            {ClassJobType.Goldsmith, ClassJobType.Goldsmith},
            {ClassJobType.Leatherworker, ClassJobType.Leatherworker},
            {ClassJobType.Weaver, ClassJobType.Weaver},
            {ClassJobType.Alchemist, ClassJobType.Alchemist},
            {ClassJobType.Culinarian, ClassJobType.Culinarian},
            {ClassJobType.Miner, ClassJobType.Miner},
            {ClassJobType.Botanist, ClassJobType.Botanist},
            {ClassJobType.Fisher, ClassJobType.Fisher},
            {ClassJobType.Paladin, ClassJobType.Gladiator},
            {ClassJobType.Monk, ClassJobType.Pugilist},
            {ClassJobType.Warrior, ClassJobType.Marauder},
            {ClassJobType.Dragoon, ClassJobType.Lancer},
            {ClassJobType.Bard, ClassJobType.Archer},
            {ClassJobType.WhiteMage, ClassJobType.Conjurer},
            {ClassJobType.BlackMage, ClassJobType.Thaumaturge},
            {ClassJobType.Arcanist, ClassJobType.Arcanist},
            {ClassJobType.Summoner, ClassJobType.Arcanist},
            {ClassJobType.Scholar, ClassJobType.Arcanist},
            {ClassJobType.Rogue, ClassJobType.Rogue},
            {ClassJobType.Ninja, ClassJobType.Rogue},
            {ClassJobType.Machinist, ClassJobType.Machinist},
            {ClassJobType.DarkKnight, ClassJobType.DarkKnight},
            {ClassJobType.Astrologian, ClassJobType.Astrologian},
            {ClassJobType.Samurai, ClassJobType.Samurai},
            {ClassJobType.RedMage, ClassJobType.RedMage},
            {ClassJobType.BlueMage, ClassJobType.BlueMage},
            {ClassJobType.Gunbreaker, ClassJobType.Gunbreaker},
            {ClassJobType.Dancer, ClassJobType.Dancer}
        };

        public static Dictionary<GearSet, int> GearSetLevels()
        {
            Dictionary<GearSet, int> gearsets = GearsetManager.GearSets.Where(i => i.InUse).ToDictionary<GearSet, GearSet, int>(gs => gs, gs => Core.Me.Levels[ClassMap[gs.Class]]);
            return gearsets;
        }

        public static bool AuraTransformed => Core.Me.HasAura(Auras.Toad) || Core.Me.HasAura(Auras.Frog) ||
                                              Core.Me.HasAura(Auras.Toad2) || Core.Me.HasAura(Auras.Lust) ||
                                              Core.Me.HasAura(Auras.Odder);

        public static Dictionary<string, List<double>> PerformanceStats = new Dictionary<string, List<double>>();

        public static void LoadList()
        {
            List<DeepDungeonData> deepList = loadResource<List<DeepDungeonData>>(Resources.DeepDungeonData);

            DeepListType = new List<IDeepDungeon>();
            foreach (DeepDungeonData dd in deepList)
            {
                switch (GetDDEnum(dd.Index))
                {
                    case DeepDungeonType.Blank:
                        break;
                    case DeepDungeonType.PotD:
                        DeepListType.Add(new PalaceOfTheDead(dd));
                        DeepListType.Add(new PalaceOfTheDeadQuick(dd));

                        break;
                    case DeepDungeonType.HoH:
                        DeepListType.Add(new HeavenOnHigh(dd));
                        break;
                    case DeepDungeonType.Unknown:
                        DeepListType.Add(new UnknownDeepDungeon(dd));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private static DeepDungeonType GetDDEnum(int index)
        {
            switch (index)
            {
                case 0:
                    return DeepDungeonType.Blank;
                case 1:
                    return DeepDungeonType.PotD;
                case 2:
                    return DeepDungeonType.HoH;

                default:
                    return DeepDungeonType.Unknown;
            }
        }

        public static int PomanderInventorySlot(Pomander p)
        {
            return SelectedDungeon.PomanderMapping[(int)p];
        }

        public static bool IsExitObject(GameObject obj)
        {
            return Exits.Any(exit => obj.NpcId == exit);
        }

        public static IDeepDungeon GetDeepDungeonByMapid(uint mapId)
        {
            return DeepListType.FirstOrDefault(deepDungeon => deepDungeon.Floors.Any(i => i.MapId == mapId));
        }
    }

    public enum DeepDungeonType
    {
        Blank,
        PotD,
        HoH,
        Unknown
    }

    internal static partial class Mobs
    {
        internal const uint HeavenlyShark = 7272;
        internal const uint CatThing = 7398;
        internal const uint Inugami = 7397;
        internal const uint Raiun = 7479;
    }

    internal static partial class Auras
    {
        internal const uint Haste = 1091; //Buff
        internal const uint HpBoost = 1093; //Buff
    }
}