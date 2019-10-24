using System.Collections.Generic;
using Deep.DungeonDefinition.Base;

namespace Deep.DungeonDefinition
{
    public class PalaceOfTheDead : DeepDungeonDecorator
    {
        private const uint _CairnOfPassage = 2007188; //EventObject 7188 Cairn Of Passage
        private const uint _CairnofReturn = 2007187; //EventObject 7187
        private const uint _BossExit = 2005809;
        private const uint _LobbyExit = 2006016; //Exit
        private const uint _LobbyEntrance = 2006012; //Entry Point
        private const uint _checkPointLevel = 51;

        //public override string DisplayName => Name;

        public PalaceOfTheDead(DeepDungeonData deepDungeon) : base(deepDungeon)
        {
            BossExit = _BossExit;
            OfReturn = _CairnofReturn;
            LobbyExit = _LobbyExit;
            OfPassage = _CairnOfPassage;
            LobbyEntrance = _LobbyEntrance;
            CheckPointLevel = _checkPointLevel;
        }

        public override uint LobbyEntrance { get; }

        public override uint OfPassage { get; }

        public override uint OfReturn { get; }

        public override uint BossExit { get; }
        public override uint LobbyExit { get; }

        public override uint CheckPointLevel { get; }

        public override Dictionary<uint, uint> WallMapData { get; } = new Dictionary<uint, uint>
        {
            //mapid - wall file
            {561, 1},
            {562, 2},
            {563, 3},
            {564, 4},
            {565, 4},
            {593, 5},
            {594, 5},
            {595, 5},
            {596, 6},
            {597, 6},
            {598, 6},
            {599, 8},
            {600, 8},
            {601, 9},
            {602, 9},
            {603, 7},
            {604, 7},
            {605, 7},
            {606, 7},
            {607, 7}
        };

        public override string GetDDType()
        {
            return "PotD";
        }
    }
}