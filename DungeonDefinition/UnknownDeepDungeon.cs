/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Original work done by zzi, contributions by Omninewb, Freiheit, Kayla D'orden and mastahg
                                                                                 */
using Deep.DungeonDefinition.Base;

namespace Deep.DungeonDefinition
{
    public class UnknownDeepDungeon : DeepDungeonDecorator
    {
        public UnknownDeepDungeon(DeepDungeonData deepDungeon) : base(deepDungeon)
        {
        }
    }
}