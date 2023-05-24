using System.Linq;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;

namespace DeepCombined.Helpers
{
    //Original methods made by ZZi for dungeonMaster
    public static class BattleCharacterExtension
    {
        public static bool IsTargetingMyPartyMember(this BattleCharacter bc)
        {
            return bc.TargetCharacter != null && PartyManager.RawMembers.Any(i => i.ObjectId == bc.TargetCharacter.ObjectId);
        }

        public static bool IsBoss(this BattleCharacter bc)
        {
            //return bc != null && BossManager.BossEncounters != null && BossManager.BossEncounters.Any(i => i.NpcId == bc.NpcId);
            return false;
        }
    }
}