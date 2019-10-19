﻿namespace Deep.DungeonDefinition
{
    public class PalaceOfTheDeadQuick : PalaceOfTheDead
    {
        public override string DisplayName => base.DisplayName + "-Quick";
        
        public PalaceOfTheDeadQuick(Base.DeepDungeon deepDungeon) : base(deepDungeon)
        {
            
        }
        
        public override string GetDDType()
        {
            return "PotD-Quick";
        }
    }
}