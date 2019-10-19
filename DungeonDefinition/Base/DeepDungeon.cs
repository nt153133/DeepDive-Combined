using System.Collections.Generic;
using ff14bot.Objects;
using Newtonsoft.Json;

namespace Deep.DungeonDefinition.Base
{
    public class DeepDungeon 
    {
        public int Index { get; set; }
        public string Name { get; }
        public string NameWithoutArticle { get; }
        public int ContentFinderId { get; }
        public Dictionary<int, int> PomanderMapping { get; }
        public int LobbyId { get; }
        public int UnlockQuest { get; }
        public EntranceNpc Npc { get; }
        
        public List<FloorSetting> Floors { get; } = new List<FloorSetting>();
        //public ushort EntranceZone => (ushort) Npc.MapId;

        [JsonIgnore]
        public string DisplayName => Name;
        [JsonIgnore]
        public int DungeonType
        {
            get => Index;
        }

        public DeepDungeon(int index, string name, int contentFinderId)
        {
            Index = index;
            Name = name;
            ContentFinderId = contentFinderId;
        }

        [JsonConstructor]
        public DeepDungeon(int index, string name, string nameWithoutArticle, int contentFinderId, Dictionary<int, int> pomanderMapping, int lobbyId, int unlockQuest, EntranceNpc npc)
        {
            Index = index;
            Name = name;
            NameWithoutArticle = nameWithoutArticle;
            ContentFinderId = contentFinderId;
            PomanderMapping = pomanderMapping;
            LobbyId = lobbyId;
            UnlockQuest = unlockQuest;
            Npc = npc;
        }

        public override string ToString()
        {
            var output =
                $"{NameWithoutArticle} ({Index})\n" +
                $"Lobby: {LobbyId}\n" +
                $"UnlockQuest: {UnlockQuest}\n" +
                $"{Npc}";
                    
            return output;
        }

        public List<GameObject> GetObjectsByWeight()
        {
            throw new System.NotImplementedException();
        }

        public string GetDDType()
        {
            return "DeepDungeon";
        }

    }

    public class FloorSetting
    {
        public string Name;
        public int InstanceId;
        public int ContentFinderId;
        public int MapId;
        public int QuestId;

        public int Start;

        public int End;
        //public string QuestName;

        public FloorSetting(string name, int instanceId, int mapId, int questId)
        {
            Name = name;
            InstanceId = instanceId;
            MapId = mapId;
            QuestId = questId;
        }

        [JsonConstructor]
        public FloorSetting(string name, int instanceId, int contentFinderId, int mapId, int questId, int start, int end)
        {
            Name = name;
            InstanceId = instanceId;
            ContentFinderId = contentFinderId;
            MapId = mapId;
            QuestId = questId;
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"{Name} Map: {MapId} InstanceId: {InstanceId}";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(FloorSetting))
            {
                return ((FloorSetting) obj).InstanceId == this.InstanceId;
            }
            return base.Equals(obj);
        }

        //Name: {ct.Name} MapId: {ter.Key} Quest: {questid.Key} ContentFinderId: {CF.Key} InstanceID: {ic.Key}
    }
}