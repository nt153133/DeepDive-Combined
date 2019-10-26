/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

namespace Deep.Enums
{
    internal enum DeepDungeonItem : uint
    {
        /// <summary>
        ///     <para>Pomander of Safety</para>
        ///     <para>Removes all traps from the current floor.</para>
        /// </summary>
        Safety = 28101,

        /// <summary>
        ///     <para>Pomander of Sight</para>
        ///     <para>Reveals the current floor's map and the location of all hidden traps.</para>
        /// </summary>
        Sight = 28102,

        /// <summary>
        ///     <para>Pomander of Strength</para>
        ///     <para>Increases damage dealt and HP recovered via healing magic by 30%.</para>
        /// </summary>
        Strength = 28103,

        /// <summary>
        ///     <para>Pomander of Steel</para>
        ///     <para>Decreases damage received by 40%.</para>
        /// </summary>
        Steel = 28104,

        /// <summary>
        ///     <para>Pomander of Affluence</para>
        ///     <para>Increases the number of treasure coffers on the next floor.</para>
        /// </summary>
        Affluence = 28105,

        /// <summary>
        ///     <para>Pomander of Flight</para>
        ///     <para>Decreases the number of enemies on the next floor.</para>
        /// </summary>
        Flight = 28106,

        /// <summary>
        ///     <para>Pomander of Alteration</para>
        ///     <para>Changes the enemy occupying a random room on the next floor into a mimic or a mandragora.</para>
        /// </summary>
        Alteration = 28107,

        /// <summary>
        ///     <para>Pomander of Purity</para>
        ///     <para>Removes the Pox status.</para>
        /// </summary>
        Purity = 28108,

        /// <summary>
        ///     <para>Pomander of Fortune</para>
        ///     <para>Increases the chance enemies on the current floor will drop treasure coffers.</para>
        /// </summary>
        Fortune = 28109,

        /// <summary>
        ///     <para>Pomander of Witching</para>
        ///     <para>Transforms all targets within range into a frog, imp, or chicken.</para>
        /// </summary>
        Witching = 28110,

        /// <summary>
        ///     <para>Pomander of Serenity</para>
        ///     <para>Removes all enchantments from the current floor.</para>
        /// </summary>
        Serenity = 28111,

        /// <summary>
        ///     <para>Pomander of Rage</para>
        ///     <para>Transforms user into a manticore.</para>
        /// </summary>
        Rage = 28112,

        /// <summary>
        ///     <para>Pomander of Lust</para>
        ///     <para>Transforms user into a succubus.</para>
        /// </summary>
        Lust = 28113,

        /// <summary>
        ///     <para>Pomander of Intuition</para>
        ///     <para>Reveals the current floor's hidden treasure.</para>
        /// </summary>
        Intuition = 28116,

        /// <summary>
        ///     <para>Pomander of Raising</para>
        ///     <para>Instantly raises the first party member to be KO'd.</para>
        /// </summary>
        Raising = 2814,

        /// <summary>
        ///     <para>Pomander of Resolution</para>
        ///     <para>Transforms user into Kuribu.</para>
        /// </summary>
        Resolution = 2815
    }
}