/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System.Collections.Generic;
using ProtoBuf;

namespace Deep.Structure
{
    [ProtoContract]
    internal class FileList
    {
        [ProtoMember(1)] public List<FileData> Files { get; set; }

        [ProtoMember(2)] public uint Version { get; set; }
    }

    [ProtoContract]
    internal class FileData
    {
        [ProtoMember(1)] public string FileName { get; set; }

        [ProtoMember(2)] public string Hash { get; set; }
    }
}