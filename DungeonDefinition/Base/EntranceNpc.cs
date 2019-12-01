/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Original work done by zzi, contributions by Omninewb, Freiheit, Kayla D'orden and mastahg
                                                                                 */

using Clio.Utilities;
using Newtonsoft.Json;

namespace DeepCombined.DungeonDefinition.Base
{
    public class EntranceNpc
    {
        public float[] Location;

        [JsonConstructor]
        public EntranceNpc(float[] location, int npcId, string name, int mapId, int aetheryteId)
        {
            Location = location;
            NpcId = npcId;
            Name = name;
            MapId = mapId;
            AetheryteId = aetheryteId;
            LocationVector = new Vector3(Location[0], Location[1], Location[2]);
        }

        public int NpcId { get; }
        public string Name { get; }
        public int MapId { get; }
        public int AetheryteId { get; set; }

        [field: JsonIgnore] public Vector3 LocationVector { get; }

/*
        public EntranceNpc(MappyNPC npc, int aetheryteId)
        {
            Location = new []{npc.CoordinateX, npc.CoordinateZ, npc.CoordinateY};
            NpcId = npc.ENpcResidentID;
            Name = npc.Name.Replace('"',' ').Trim();
            MapId = npc.MapTerritoryID;
            AetheryteId = aetheryteId;
        }
*/
        public override string ToString()
        {
            return $"NPC:\n\tNpcId: {NpcId}\n\tName: {Name}\n\tZoneId: {MapId}\n\tAetheryteId: {AetheryteId}\n\tLocation: {LocationVector}";
        }
    }
}