using Clio.Utilities;
using Newtonsoft.Json;

namespace Deep.DungeonDefinition.Base
{
    public class EntranceNpc
    {
        public int NpcId { get; }
        public string Name { get; }
        public float[] Location;
        public int MapId { get; }
        public int AetheryteId { get; set; }
        [field: JsonIgnore] public Vector3 LocationVector { get; }

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