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
        public static List<DungeonDefinition.Base.DeepDungeon> deepList;
        public static List<IDeepDungeon> deepListType;
        
        public static DungeonDefinition.Base.DeepDungeon selectedDungeon;

        public static void LoadList()
        {
            //string fileList = "DeepDungeons.json";
            
           //(Resources.);
           deepList = loadResource<List<DungeonDefinition.Base.DeepDungeon>>(Resources.DeepDungeonData);
           /*
            if (!File.Exists(fileList)) return;
            
            using (var trapRead = new StreamReader(fileList))
            {
                var trapJson = trapRead.ReadToEnd();
                deepList = JsonConvert.DeserializeObject<List<DungeonDefinition.Base.DeepDungeon>>(trapJson);
            }
            */
        }

        public static void LoadTypeList()
        {
            deepListType = new List<IDeepDungeon>();
            foreach (var dd in deepList)
            {
                switch (GetDDEnum(dd.Index))
                {
                    case DeepDungeonType.Blank:
                        break;
                    case DeepDungeonType.PotD:
                        deepListType.Add(new PalaceOfTheDead(dd));
                        deepListType.Add(new PalaceOfTheDeadQuick(dd));
                    
                        break;
                    case DeepDungeonType.HoH:
                        deepListType.Add(new HeavenOnHigh(dd));
                        break;
                    case DeepDungeonType.Unknown:
                        deepListType.Add(new UnknownDeepDungeon(dd));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public static DeepDungeonType GetDDEnum(int index)
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
    }
    

    
    public enum DeepDungeonType
    {
        Blank,
        PotD,
        HoH,
        Unknown
    }
    

}