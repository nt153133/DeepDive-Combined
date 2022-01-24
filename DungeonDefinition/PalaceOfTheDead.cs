﻿/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Original work done by zzi, contributions by Omninewb, Freiheit, Kayla D'orden and mastahg
                                                                                 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeepCombined.DungeonDefinition.Base;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using ff14bot;
using ff14bot.Directors;
using ff14bot.Managers;
using static DeepCombined.Tasks.Common;

namespace DeepCombined.DungeonDefinition
{
    public class PalaceOfTheDead : DeepDungeonDecorator
    {
        private const uint _CairnOfPassage = 2007188; //EventObject 7188 Cairn Of Passage
        private const uint _CairnofReturn = 2007187; //EventObject 7187
        private const uint _BossExit = 2005809;
        private const uint _LobbyExit = 2006016; //Exit
        private const uint _LobbyEntrance = 2006012; //Entry Point
        private const uint _checkPointLevel = 51;
        private const int _sustainingPotion = 20309;

        private readonly uint[] _ignoreEntity =
        {
            _CairnOfPassage, _CairnofReturn, _LobbyEntrance
        };

        //public override string DisplayName => Name;

        public PalaceOfTheDead(DeepDungeonData deepDungeon) : base(deepDungeon)
        {
            BossExit = _BossExit;
            OfReturn = _CairnofReturn;
            LobbyExit = _LobbyExit;
            OfPassage = _CairnOfPassage;
            LobbyEntrance = _LobbyEntrance;
            CheckPointLevel = _checkPointLevel;
            SustainingPotion = _sustainingPotion;
        }

        public override uint LobbyEntrance { get; }

        public override uint OfPassage { get; }

        public override uint OfReturn { get; }

        public override uint BossExit { get; }
        public override uint LobbyExit { get; }

        public override uint CheckPointLevel { get; }

        public override int SustainingPotion { get; }

        public override Dictionary<uint, uint> WallMapData { get; } = new Dictionary<uint, uint>
        {
            //mapid - wall file
            {561, 1},
            {562, 2},
            {563, 3},
            {564, 4},
            {565, 4},
            {593, 5},
            {594, 5},
            {595, 5},
            {596, 6},
            {597, 6},
            {598, 6},
            {599, 8},
            {600, 8},
            {601, 9},
            {602, 9},
            {603, 7},
            {604, 7},
            {605, 7},
            {606, 7},
            {607, 7}
        };

        public override uint[] GetIgnoreEntity(uint[] baseList)
        {
            return baseList.Concat(_ignoreEntity).ToArray();
        }

        public override async Task<bool> BuffMe()
        {
            if (Settings.Instance.UsePomRage && CombatTargeting.Instance.LastEntities.Count() > 5 &&
                !Core.Me.HasAura(Auras.Rage))
            {
                return await UsePomander(Pomander.Rage, Auras.Lust);
            }

            return true;
        }

        public override async Task<bool> BuffBoss()
        {
            await LustLogic();

            return true;
        }

        public override async Task<bool> BuffCurrentFloor()
        {
            return await base.BuffCurrentFloor();
        }

        private static async Task LustLogic()
        {
            bool lust = false;
            DDInventoryItem itm = DeepDungeonManager.GetInventoryItem(Pomander.Lust);
            Logger.Info("[LUST] Item Count: {0}", itm.Count);

            //we are inside the dungeon, should be ok to use InParty here.
            if (PartyManager.IsInParty)
            {
                Logger.Info("In A Party. Doing Lust Logic...");
                bool lustFound = false;
                foreach (PartyMember k in PartyManager.AllMembers)
                {
                    if (!k.Class.IsHealer() && !k.Class.IsTank())
                    {
                        lustFound = true;
                        if (k.IsMe)
                        {
                            lust = true;
                        }

                        break;
                    }
                }

                Logger.Info("Party Lust status: {0} :: {1} :: {2}", !lust, !lustFound, PartyManager.IsPartyLeader);
                if (!lust && !lustFound)
                {
                    lust = PartyManager.IsPartyLeader;
                }

                if (!PartyManager.IsPartyLeader && itm.Count > 0)
                {
                    lust = true;
                }
            }
            else
            {
                Logger.Info("Solo Lust Logic");
                lust = true;
            }

            if (lust)
            {
                Logger.Info("Use Pomander Debug: [HasAura: {0}]", itm.HasAura);
                await UsePomander(Pomander.Lust, Auras.Lust);
            }
        }

        public override string GetDDType()
        {
            return "PotD";
        }
    }
}