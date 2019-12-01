/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using DeepCombined.Memory.Attributes;

namespace DeepCombined.Memory
{
#pragma warning disable CS0649
    internal static class Offsets
    {
        [Offset("Search 0F B6 81 ? ? ? ? 49 8D 95 ? ? ? ? Add 3 Read32")] //0x1BEA
        //[OffsetCN("Search 0F B6 81 ? ? ? ? 44 0F B6 CA Add 3 Read32")]
        //0x187A
        internal static int DDMapGroup;

        [Offset("Search 49 8D 95 ? ? ? ? 48 69 C8 ? ? ? ?  Add 3 Read32")] //[OffsetCN("Search 48 8D 91 ? ? ? ? 48 C7 84 24 ? ? ? ? ? ? ? ? Add 3 Read32")] //0x11c8
        internal static int Map5xStart;

        [Offset("Search 48 69 C8 ? ? ? ? 48 03 D1 45 Add 3 Read32")] //[OffsetCN("Search 4C 69 C0 ? ? ? ? 48 8D 91 ? ? ? ? Add 3 Read32")] //0x338
        internal static int Map5xSize;

        [Offset("Search 0F B7 82 ? ? ? ? 2B C8 48 8D 0C 89 Add 3 Read32")] // 0x332
        internal static int WallStartingPoint;

        [Offset("Search 49 8D BE ? ? ? ? 0F 1F 44 00 ? 8B 17 Add 3 Read32")] //0x140
        internal static int Starting;

        [Offset("Search 41 8B 96 ? ? ? ? 84 C9 Add 3 Read32")] //0x13c
        internal static int UNK_StartingCircle;

        [Offset("Search 42 0F B6 8C 28 ? ? ? ? 84 0E Add 5 Read32")] //0x1860
        internal static int WallGroupEnabled;
    }
#pragma warning restore CS0649
}