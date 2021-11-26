/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Linq;
using ff14bot;
using ff14bot.Objects;

namespace DeepCombined.Helpers
{
    internal static class GameObjectExtensions
    {
        internal static bool WaitForAura(this GameObject obj, uint auraId, bool castbyme = false, double timeLeft = 0.0, bool checkTime = true)
        {
            Character character = obj as Character;
            if (character != null && character.IsValid)
            {
                System.Collections.Generic.IEnumerable<Aura> source = castbyme
                    ? from r in character.CharacterAuras
                      where r.CasterId == Core.Me.ObjectId && r.Id == auraId
                      select r
                    : character.CharacterAuras.Where(r => r.Id == auraId);
                if (!checkTime)
                {
                    if (source.Any(aura => aura.TimespanLeft.TotalMilliseconds < 0.0))
                    {
                        return false;
                    }
                }

                return source.Any(aura => aura.TimespanLeft.TotalMilliseconds >= timeLeft);
            }

            return false;
        }

        internal static bool HasAnyAura(this Character c, params uint[] auras)
        {
            foreach (uint id in auras)
            {
                if (c.HasAura(id))
                {
                    return true;
                }
            }

            return false;
        }

        internal static uint MissingHealth(this GameObject player)
        {
            return player.MaxHealth - player.CurrentHealth;
        }

        /// <summary>
        /// Faces the player away from the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="player">Local player.</param>
        /// <param name="obj"><see cref="GameObject"/> to face away from.</param>
        internal static void FaceAway(this LocalPlayer player, GameObject obj)
        {
            // Look at target, then flip to inverse
            obj.Face2D();
            float inverse = (float)(player.Heading - Math.PI);
            player.SetFacing(inverse);
        }
    }
}