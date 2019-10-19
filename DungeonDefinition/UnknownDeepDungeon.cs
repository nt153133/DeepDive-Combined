﻿using Deep.DungeonDefinition.Base;

 namespace Deep.DungeonDefinition
{
    public class UnknownDeepDungeon : DeepDungeonDecorator
    {
        public UnknownDeepDungeon(Base.DeepDungeon deepDungeon) : base(deepDungeon)
        {
            
        }
    }
}