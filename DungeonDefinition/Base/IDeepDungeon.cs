using System.Collections.Generic;
using ff14bot.Objects;

namespace Deep.DungeonDefinition.Base
{
    public interface IDeepDungeon
    {
        int Index { get; }
        string Name { get; }
        string NameWithoutArticle { get; }
        int ContentFinderId { get; }
        Dictionary<int, int> PomanderMapping { get; }
        int LobbyId { get; }
        int UnlockQuest { get; }
        EntranceNpc Npc { get; }
        List<FloorSetting> Floors { get; }
        string DisplayName { get; }

        uint EntranceAetheryte { get; }
        uint CaptainNpcId { get; }
        uint[] DeepDungeonRawIds { get; }
        string GetDDType();

        List<GameObject> GetObjectsByWeight();
    }
}