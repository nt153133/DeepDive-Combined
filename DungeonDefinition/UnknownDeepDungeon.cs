using Deep.DungeonDefinition.Base;

namespace Deep.DungeonDefinition
{
    public class UnknownDeepDungeon : DeepDungeonDecorator
    {
        public UnknownDeepDungeon(DeepDungeonData deepDungeon) : base(deepDungeon)
        {
        }
    }
}