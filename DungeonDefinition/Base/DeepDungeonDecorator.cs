using System.Collections.Generic;
using System.Linq;
using Buddy.Service.Client;
using Clio.Utilities;
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
        //public uint EntranceAetheryte { get; }
        
        public uint EntranceAetheryte => (ushort) Npc.AetheryteId;
        public uint CaptainNpcId => (uint) Npc.NpcId;
        public Vector3 CaptainNpcPosition => Npc.LocationVector;
        public uint[] DeepDungeonRawIds
        {
            get { return Floors.Select(i => (uint) i.MapId).ToArray(); }
        }


        public virtual string DisplayName => NameWithoutArticle;
        public virtual uint OfPassage { get; }
        public virtual uint OfReturn { get; }
        public virtual uint BossExit { get; }
        public virtual uint LobbyExit { get; }
        
        public virtual uint LobbyEntrance { get; }

        public virtual Dictionary<uint, uint> WallMapData { get; }
    

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
            //DeepDungeonRawIds = GetRawMapIds();
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