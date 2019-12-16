/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Original work done by zzi, contributions by Omninewb, Freiheit, Kayla D'orden and mastahg
                                                                                 */
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using DeepCombined.DungeonDefinition.Base;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using DeepCombined.TaskManager.Actions;
using ff14bot;
using ff14bot.Directors;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using static DeepCombined.Tasks.Common;

namespace DeepCombined.DungeonDefinition
{
    public class HeavenOnHigh : DeepDungeonDecorator
    {
        private const uint _BeaconOfReturn = 2009506;
        private const uint _BeaconOfPassage = 2009507;
        private const uint _BossExit = 2005809;
        private const uint _LobbyExit = 2009523;
        private const uint _LobbyEntrance = 2009524;
        private const uint _checkPointLevel = 21;
        private const int _sustainingPotion = 23163;

        private readonly uint[] _ignoreEntity =
        {
            _BeaconOfPassage, _BeaconOfReturn, _LobbyEntrance, Mobs.CatThing, Mobs.Inugami, Mobs.Raiun, 377, 7396, 7395
        };

        public HeavenOnHigh(DeepDungeonData deep) : base(deep)
        {
            BossExit = _BossExit;
            OfReturn = _BeaconOfReturn;
            LobbyExit = _LobbyExit;
            OfPassage = _BeaconOfPassage;
            LobbyEntrance = _LobbyEntrance;
            CheckPointLevel = _checkPointLevel;
            SustainingPotion = _sustainingPotion;
        }

        public override uint OfPassage { get; }
        public override uint OfReturn { get; }
        public override uint BossExit { get; }
        public override uint LobbyExit { get; }
        public override uint LobbyEntrance { get; }
        public override uint CheckPointLevel { get; }

        public override Dictionary<uint, uint> WallMapData { get; } = new Dictionary<uint, uint>
        {
            //mapid - wall file
            {770, 770}, //1-10
            {771, 0}, //11-20
            {772, 0}, //21-30
            {773, 0}, //41-50
            {774, 0}, //61-70
            {775, 0}, //81-90
            {782, 0}, //31-40
            {783, 0}, //51-60
            {784, 0}, //71-80
            {785, 0} //91-100
        };

        public override int SustainingPotion { get; }

        public override uint[] GetIgnoreEntity(uint[] baseList)
        {
            return baseList.Concat(_ignoreEntity).ToArray();
        }

        public override async Task<bool> BuffMe()
        {
            if ( GameObjectManager.Attackers.Count > 3) return await UsePomander(Pomander.Petrification);

            if (DeepDungeonManager.GetInventoryItem(Pomander.Petrification).Count == 3)
                return await UsePomander(Pomander.Petrification);

            return false;
        }

        public override async Task<bool> BuffBoss()
        {
            if (DeepDungeonManager.GetMagiciteCount() >= 1)
            {
                Logger.Warn("Magicite >= 1");
                DeepDungeonManager.CastMagicite();
                await Coroutine.Sleep(500);
            }

            return await UsePomander(Pomander.Frailty);
            
        }

        public override async Task<bool> BuffCurrentFloor()
        {
            if (DeepDungeonManager.GetMagiciteCount() >= 1)
            {
                Logger.Warn("Magicite >= 1");
                DeepDungeonManager.CastMagicite();
                await Coroutine.Sleep(500);
            }
            
            if (DeepDungeonManager.GetInventoryItem(Pomander.Frailty).Count > 1)
                return await UsePomander(Pomander.Frailty);

            return false;
        }

        public override float Sort(GameObject obj)
        {
            var weight = 150f;

            if (PartyManager.IsInParty && !PartyManager.IsPartyLeader && !DeepDungeonManager.BossFloor)
            {
                if (PartyManager.PartyLeader.IsInObjectManager && PartyManager.PartyLeader.CurrentHealth > 0)
                {
                    if (PartyManager.PartyLeader.BattleCharacter.HasTarget)
                        if (obj.ObjectId == PartyManager.PartyLeader.BattleCharacter.TargetGameObject.ObjectId)
                            weight += 600;
                    weight -= obj.Distance2D(PartyManager.PartyLeader.GameObject);
                }
                else
                {
                    weight -= obj.Distance2D();
                }
            }
            else
            {
                weight -= obj.Distance2D();
            }

            switch (obj.Type)
            {
                case GameObjectType.BattleNpc:
                    weight /= 2;
                    if ((obj as BattleCharacter).IsTargetingMyPartyMember())
                        weight += 100;
                    break;
                case GameObjectType.Treasure:
                    //weight += 10;
                    break;
            }

            return weight;
        }
        
        private float SortComplete(GameObject obj)
        {
            var weight = 150f;

            if (PartyManager.IsInParty && !PartyManager.IsPartyLeader && !DeepDungeonManager.BossFloor)
            {
                if (PartyManager.PartyLeader.IsInObjectManager && PartyManager.PartyLeader.CurrentHealth > 0)
                {
                    if (PartyManager.PartyLeader.BattleCharacter.HasTarget)
                        if (obj.ObjectId == PartyManager.PartyLeader.BattleCharacter.TargetGameObject.ObjectId)
                            weight += 600;
                    weight -= obj.Distance2D(PartyManager.PartyLeader.GameObject);
                }
                else
                {
                    if (FloorExit.location != Vector3.Zero)
                        weight -= Core.Me.Distance2D(Vector3.Lerp(obj.Location, FloorExit.location, 0.25f));
                }
            }
            else
            {
                if (FloorExit.location != Vector3.Zero)
                    weight -= Core.Me.Distance2D(Vector3.Lerp(obj.Location, FloorExit.location, 0.25f));
                else
                    weight -= obj.Distance2D();
            }

            switch (obj.Type)
            {
                case GameObjectType.BattleNpc when !PartyManager.IsInParty:
                    return weight / 2;

                case GameObjectType.BattleNpc:
                    weight /= 2;
                    break;
                case GameObjectType.Treasure:
                    break;
            }

            if (DeepDungeonManager.PortalActive && Settings.Instance.GoForTheHoard && obj.NpcId == EntityNames.Hidden)
                weight += 5;

            return weight;
        }
        public List<GameObject> StartList()
        {
            var result = new List<GameObject>();
            foreach (var obj in GameObjectManager.GameObjects)
            {
                if (obj.Location == Vector3.Zero)
                    continue;
                
                if (!obj.IsValid || !obj.IsVisible)
                    continue;

                if (Blacklist.Contains(obj) || Constants.TrapIds.Contains(obj.NpcId) ||
                    Constants.IgnoreEntity.Contains(obj.NpcId))
                    continue;

                switch (obj.Type)
                {
                    case GameObjectType.Treasure:
                        if (obj.NpcId == EntityNames.BandedCoffer)
                        {
                            result.Add(obj);
                            break;
                        }
                        
                        if (!DeepDungeonManager.PortalActive && FloorExit.location != Vector3.Zero)
                            result.Add(obj);
                        
                        break;
                    case GameObjectType.EventObject:
                        result.Add(obj);
                        break;
                    case GameObjectType.BattleNpc:
                        if (DeepDungeonManager.PortalActive && !((BattleCharacter) obj).InCombat && FloorExit.location != Vector3.Zero)
                            continue;
                        if (!((BattleCharacter) obj).IsDead)
                            result.Add(obj);
                        break;
                    default:
                        continue;
                }
            }
            //Blacklists
            return result;
        }
        
        public override bool Filter(GameObject obj)
        {
            //Blacklists
            if (Blacklist.Contains(obj) || Constants.TrapIds.Contains(obj.NpcId) ||
                Constants.IgnoreEntity.Contains(obj.NpcId))
                return false;

            if (obj.Location == Vector3.Zero)
                return false;

            switch (obj.Type)
            {
                case GameObjectType.Treasure:
                    return !(HaveMainPomander() && DeepDungeonManager.PortalActive &&
                             FloorExit.location != Vector3.Zero);
                case GameObjectType.EventObject:
                    return true;
                case GameObjectType.BattleNpc:
                    return !((BattleCharacter) obj).IsDead;
                default:
                    return false;
            }
        }
        
        public override List<GameObject> GetObjectsByWeight()
        {
            if (DeepDungeonManager.PortalActive)
                return StartList().OrderByDescending(SortComplete)
                    .ToList();

            return StartList()
                .OrderByDescending(Sort)
                .ToList();
        }
        
        public override string GetDDType()
        {
            return "HoH";
        }
        
        private bool HaveMainPomander()
        {
            return DeepDungeonManager.GetInventoryItem(Pomander.Frailty).Count > 0 &&
                   DeepDungeonManager.GetInventoryItem(Pomander.Strength).Count > 0 &&
                   DeepDungeonManager.GetInventoryItem(Pomander.Steel).Count > 0;
        }
    }
}