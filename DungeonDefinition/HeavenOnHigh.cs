﻿using Deep.DungeonDefinition.Base;

 namespace Deep.DungeonDefinition
{
    public class HeavenOnHigh : DeepDungeonDecorator
    {
        public HeavenOnHigh(Base.DeepDungeon deep) :base(deep)
        {
            
        }

        public override string GetDDType()
        {
            return "HoH";
        }
        
    }
}