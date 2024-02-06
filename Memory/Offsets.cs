/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using DeepCombined.Memory.Attributes;

namespace DeepCombined.Memory
{
#pragma warning disable CS0649
    internal static class Offsets
    {
        [Offset("Search 48 8D 05 ? ? ? ? 48 C7 43 ? ? ? ? ? 48 89 03 48 8B C3 48 83 C4 ? 5B C3 ? ? ? ? ? ? ? 48 8D 05 ? ? ? ? 48 89 01 E9 ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? 40 57 Add 3 TraceRelative")] //0x1860
        internal static IntPtr DeepDungeonStatusVtable;
    }
#pragma warning restore CS0649
}