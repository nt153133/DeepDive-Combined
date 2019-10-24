using System.Collections.Generic;
using System.Threading.Tasks;
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
        int SustainingPotion { get; }
        
        
        
        //Pomanders
        Dictionary<int, int> PomanderMapping { get; }
        Task<bool> BuffMe();
        Task<bool> BuffBoss();
        Task<bool> BuffCurrentFloor();
        Task<bool> BuffNextFloor();

        //Targeting
        uint[] GetIgnoreEntity(uint[] baseList);

        List<GameObject> GetObjectsByWeight();
        
        string GetDDType();
    }
}