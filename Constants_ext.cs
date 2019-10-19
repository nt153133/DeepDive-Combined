using System;
using System.Collections.Generic;
using System.IO;
using Deep.DungeonDefinition;
using Deep.DungeonDefinition.Base;
using Deep.Properties;
using Newtonsoft.Json;

namespace Deep
{
    internal static partial class Constants
    {
        public static List<IDeepDungeon> DeepListType;

        public static IDeepDungeon SelectedDungeon;

        public static void LoadList()
        {
            var deepList = loadResource<List<DeepDungeonData>>(Resources.DeepDungeonData);

            DeepListType = new List<IDeepDungeon>();
            foreach (var dd in deepList)
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

        internal static DeepDungeonType GetDDEnum(int index)
        {
            switch (index)
            {
                case 0: return DeepDungeonType.Blank;
                case 1: return DeepDungeonType.PotD;
                case 2: return DeepDungeonType.HoH;

                default:
                    return DeepDungeonType.Unknown;
            }
        }
        
        internal static Dictionary<int, int> Percent = new Dictionary<int, int>
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
    }

    public enum DeepDungeonType
    {
        Blank,
        PotD,
        HoH,
        Unknown
    }
    
    
}