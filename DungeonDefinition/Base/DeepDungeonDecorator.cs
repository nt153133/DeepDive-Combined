using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Service.Client;
using Clio.Utilities;
using Deep.Helpers;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Deep.DungeonDefinition.Base
{
    public abstract class DeepDungeonDecorator : IDeepDungeon
    {
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
            DisplayName = DataManager.ZoneNameResults[(uint) LobbyId].CurrentLocaleName;
            //DeepDungeonRawIds = GetRawMapIds();
        }

        public int Index { get; }
        public string Name { get; }
        public string NameWithoutArticle { get; }
        public int ContentFinderId { get; }
        public Dictionary<int, int> PomanderMapping { get; }
        public int LobbyId { get; }
        public int UnlockQuest { get; }
        public EntranceNpc Npc { get; }


        //DeepDive Used Properties

        public uint EntranceAetheryte => (ushort) Npc.AetheryteId;
        public uint CaptainNpcId => (uint) Npc.NpcId;
        public Vector3 CaptainNpcPosition => Npc.LocationVector;

        public uint[] DeepDungeonRawIds
        {
            get { return Floors.Select(i => (uint) i.MapId).ToArray(); }
        }


        public virtual string DisplayName { get; }
        public virtual uint OfPassage { get; }
        public virtual uint OfReturn { get; }
        public virtual uint BossExit { get; }
        public virtual uint LobbyExit { get; }

        public virtual uint LobbyEntrance { get; }

        public virtual Dictionary<uint, uint> WallMapData { get; }

        public virtual uint CheckPointLevel { get; }

        public virtual int SustainingPotion { get; }

        public List<FloorSetting> Floors { get; }

        public virtual uint[] GetIgnoreEntity(uint[] baseList)
        {
            return baseList;
        }

        public virtual string GetDDType()
        {
            return "Unknown";
        }

        protected virtual uint[] GetRawMapIds()
        {
            var test = Floors.Select(i => (uint) i.MapId);

            return test.ToArray();
        }
        
        public virtual List<GameObject> GetObjectsByWeight()
        {
            return GameObjectManager.GameObjects
                .Where(Filter)
                .OrderByDescending(Sort)
                .ToList();
        }

        public virtual float Sort(GameObject obj)
        {
            var weight = 100f;

            weight -= obj.Distance2D();

            if (obj.Type == GameObjectType.BattleNpc)
            {
                return weight / 2;
            }

            if (obj.NpcId == EntityNames.BandedCoffer)
                weight += 500;

            if (DeepDungeonManager.PortalActive && Settings.Instance.GoForTheHoard && (obj.NpcId == EntityNames.Hidden))
                weight += 5;
            else if (DeepDungeonManager.PortalActive && Settings.Instance.GoExit && obj.NpcId != EntityNames.OfPassage && PartyManager.IsInParty)
                weight -= 10;

            return weight;
        }

        public virtual bool Filter(GameObject obj)
        {
            if (obj.Location == Vector3.Zero)
                return false;

            
            if (Blacklist.Contains(obj) || Constants.TrapIds.Contains(obj.NpcId) || Constants.IgnoreEntity.Contains(obj.NpcId))
                return false;


            if (obj.Type == GameObjectType.BattleNpc)
            {
                if (DeepDungeonManager.PortalActive)
                    return false;

                var battleCharacter = (BattleCharacter) obj;
                return !battleCharacter.IsDead;
            }

            return obj.Type == GameObjectType.EventObject || obj.Type == GameObjectType.Treasure || obj.Type == GameObjectType.BattleNpc;
        }

        public virtual async Task<bool> BuffMe()
        {
            return false;
        }

        public virtual async Task<bool> BuffBoss()
        {
            return false;
        }

        public virtual async Task<bool> BuffCurrentFloor()
        {
            return false;
        }

        public virtual async Task<bool> BuffNextFloor()
        {
            return false;
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