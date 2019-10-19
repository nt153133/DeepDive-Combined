﻿using System.Collections.Generic;
 using Deep.DungeonDefinition.Base;

 namespace Deep.DungeonDefinition
{
    public class HeavenOnHigh : DeepDungeonDecorator
    {
        private const uint _BeaconOfReturn = 2009506;
        private const uint _BeaconOfPassage = 2009507;
        private const uint _BossExit = 2005809;
        private const uint _LobbyExit = 2009523;
        private const uint _LobbyEntrance = 2009524;
        
        public HeavenOnHigh(Base.DeepDungeonData deep) :base(deep)
        {
            BossExit = _BossExit;
            OfReturn = _BeaconOfReturn;
            LobbyExit = _LobbyExit;
            OfPassage = _BeaconOfPassage;
            LobbyEntrance = _LobbyEntrance;
        }
        
        public override uint OfPassage { get; }

        public override uint OfReturn { get; }

        public override uint BossExit { get; }
        public override uint LobbyExit { get; }
        public override uint LobbyEntrance { get; }
        
        public override Dictionary<uint, uint> WallMapData { get; } = new Dictionary<uint, uint>
        {
            //mapid - wall file
            {770, 770}, //1-10
            {771, 0}, //11-20
            {772, 0}, //21-30
            {773, 0}, //41-50
            {774, 0}, //61-70
            {775, 0}, //81-90
            {782, 0}, //31-40
            {783, 0}, //51-60
            {784, 0}, //71-80
            {785, 0}  //91-100
        };

        public override string GetDDType()
        {
            return "HoH";
        }
        
    }
}