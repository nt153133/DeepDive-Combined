using System.Collections.Generic;
using System.Linq;
using ff14bot.Objects;

namespace Deep.DungeonDefinition.Base
{
    public abstract class DeepDungeonDecorator : IDeepDungeon
    {
        public int Index { get; }
        public string Name { get; }
        public string NameWithoutArticle { get; }
        public int ContentFinderId { get; }
        public Dictionary<int, int> PomanderMapping { get; }
        public int LobbyId { get; }
        public int UnlockQuest { get; }
        public EntranceNpc Npc { get; }


        //DeepDive Used Properties
        
        // DataManager.AetheryteCache.Values.FirstOrDefault(i => i.Id == EntranceAetheryte);
        public uint EntranceAetheryte => (ushort) Npc.AetheryteId;
        public uint CaptainNpcId => (uint) Npc.NpcId;
        public uint[] DeepDungeonRawIds { get; }

        public virtual string DisplayName => NameWithoutArticle;

        protected DeepDungeonDecorator(DeepDungeonData deepDungeon)
        {
            Index = deepDungeon.Index;
            Name = deepDungeon.Name;
            NameWithoutArticle = deepDungeon.NameWithoutArticle;
            ContentFinderId = deepDungeon.ContentFinderId;
            PomanderMapping = deepDungeon.PomanderMapping;
            LobbyId = deepDungeon.LobbyId;
            UnlockQuest = deepDungeon.UnlockQuest;
            Npc = deepDungeon.Npc;
            Floors = deepDungeon.Floors;
            DeepDungeonRawIds = GetRawMapIds();
        }

        public List<FloorSetting> Floors { get; }

        public virtual List<GameObject> GetObjectsByWeight()
        {
            return null;
        }

        protected virtual uint[] GetRawMapIds()
        {
            var test = Floors.Select(i => (uint) i.MapId);

            return test.ToArray();
        }
        public virtual string GetDDType()
        {
            return "Unknown";
        }

        public override string ToString()
        {
            var output =
                $"{NameWithoutArticle} ({Index}) is {GetDDType()}\n" +
                $"Lobby: {LobbyId}\n" +
                $"UnlockQuest: {UnlockQuest}\n" +
                $"{Npc}";
                    
            return output;
        }

    }
}