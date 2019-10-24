using ff14bot.Enums;
using ff14bot.Objects;

namespace Deep.Helpers
{
    public static class BattleCharacterExtension
    {
        public static bool IsTargetingMyPartyMember(this BattleCharacter bc)
        {
            return bc.TargetCharacter != null && (bc.TargetCharacter.IsMe || (bc.TargetCharacter.StatusFlags & (StatusFlags.AllianceMember | StatusFlags.PartyMember)) != 0);
        }
        public static bool IsBoss(this BattleCharacter bc)
        {
            //return bc != null && BossManager.BossEncounters != null && BossManager.BossEncounters.Any(i => i.NpcId == bc.NpcId);
            return false;
        }
    }
}