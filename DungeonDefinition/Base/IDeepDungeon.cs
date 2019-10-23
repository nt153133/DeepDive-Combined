using System.Collections.Generic;
using Clio.Utilities;
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

        uint OfPassage { get; }
        uint OfReturn { get; }
        
        uint BossExit { get; }
        uint LobbyExit { get; }
        uint LobbyEntrance { get; }
        
        Dictionary<uint, uint> WallMapData { get; }
        
        uint EntranceAetheryte { get; }
        uint CaptainNpcId { get; }
        Vector3 CaptainNpcPosition { get; }
        uint[] DeepDungeonRawIds { get; }
        uint CheckPointLevel { get; }
        string GetDDType();

        List<GameObject> GetObjectsByWeight();
    }
}