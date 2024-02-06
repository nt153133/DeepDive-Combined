/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Clio.Utilities;
using DeepCombined.Helpers;
using DeepCombined.Helpers.Logging;
using DeepCombined.Memory;
using DeepCombined.Properties;
using ff14bot;
using ff14bot.Buddy.Offsets;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Overlay3D;
using ff14bot.Pathing;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.ServiceClient;
using Newtonsoft.Json;

namespace DeepCombined.Providers
{
    internal class DDNavigationProvider : WrappingNavigationProvider
    {
        private const float TrapSize = 2.4f;

        private static Dictionary<uint, List<Vector3>> _walls = new Dictionary<uint, List<Vector3>>();

        private static List<Vector3> _map;

        private static readonly HashSet<BoundingBox3> wallList = new HashSet<BoundingBox3>();
        private static readonly HashSet<BoundingCircle> trapList = new HashSet<BoundingCircle>();

        private uint _detourLevel;

        private int _floorId;

        private List<uint> _traps;
        private HashSet<uint> activeWalls;

        public DDNavigationProvider(NavigationProvider original) : base(original)
        {
        }

        internal static Dictionary<uint, bool> Walls { get; private set; }

        internal static List<Vector3> Traps { get; private set; }


        private void SetupDetour()
        {
            //if we are not on the lobby & we have already reloaded detour for this floor return
            if (_floorId == DeepDungeonManager.Level) return;

            _floorId = DeepDungeonManager.Level;


            var map = Constants.Maps[WorldManager.RawZoneId];
            if (_detourLevel != map)
            {
                _detourLevel = map;
                _walls = LoadWalls(map);
            }

            //load the map
            Walls = new Dictionary<uint, bool>();
            _traps = new List<uint>();
            Traps = new List<Vector3>();
            _map = new List<Vector3>();


            Logger.Verbose("Updating navigation {0}", map);
            wallList.Clear();
            trapList.Clear();
            activeWalls = FindWalls();

            WallCheck();
        }

        private static Dictionary<uint, List<Vector3>> LoadWalls(uint map)
        {
            string text;
            if (map == 70) return new Dictionary<uint, List<Vector3>>();
            switch (map)
            {
                case 1:
                    text = Resources._1;
                    break;
                case 2:
                    text = Resources._2;
                    break;
                case 3:
                    text = Resources._3;
                    break;
                case 4:
                    text = Resources._4;
                    break;
                case 5:
                    text = Resources._5;
                    break;
                case 6:
                    text = Resources._6;
                    break;
                case 7:
                    text = Resources._7;
                    break;
                case 8:
                    text = Resources._8;
                    break;
                case 9:
                    text = Resources._9;
                    break;
                case 770:
                    text = Resources._770;
                    break;
                default:
                    text = "";
                    break;
            }


            return JsonConvert.DeserializeObject<Dictionary<uint, List<Vector3>>>(text);
        }

        public override MoveResult MoveTo(MoveToParameters location)
        {
            //if (AvoidanceManager.IsRunningOutOfAvoid)
            //    return MoveResult.Moving;
            if (!DutyManager.InInstance)
                return Original.MoveTo(location);

            //if we aren't in POTD default to the original mover right away.
            if (!Constants.Maps.ContainsKey(WorldManager.RawZoneId)) return Original.MoveTo(location);

            SetupDetour();

            AddBlackspots();
            WallCheck();

            location.WorldState = new WorldState {MapId = WorldManager.ZoneId, Walls = wallList, Avoids = trapList};
            return Original.MoveTo(location);
        }

        private bool WallCheck()
        {
            var updated = false;
            var me = Core.Me.Location;

            if (_walls == null)
                return false;
            
            if (_walls.Count < 1)
                return false;
            
            foreach (var id in _walls.Where(i => i.Value[0].Distance2D(Core.Me.Location) < 50 && !Walls.ContainsKey(i.Key) && !activeWalls.Contains(i.Key)))
            {
                var wall1 = id.Value[1];
                wall1.Y -= 2;

                wallList.Add(new BoundingBox3 {Min = wall1, Max = id.Value[2]});
                Walls.Add(id.Key, true);
                updated = true;
            }

            //Logger.Info($"[walls] {string.Join(", ", _hit.Keys)}");

            return updated;
        }


        //private int floorCache;
        //private List<uint> _wallcache;

        private HashSet<uint> FindWalls()
        {
            //if (floorCache == DeepDungeonManager.Level && _walls != null) return _wallcache;
            //floorCache = DeepDungeonManager.Level;
            var director = DirectorManager.ActiveDirector.Pointer;

            if (director == IntPtr.Zero)
                return new HashSet<uint>();

            var v187A = Core.Memory.Read<byte>(director + PublicOffsets.DeepDungeonOffsets.DDMapGroup);
            Logger.Info($"v187A {v187A}");

            if (v187A == 0)
            {
                Logger.Error($"Is this a big room? ZoneID: {WorldManager.ZoneId} Floor: {DeepDungeonManager.Level}");
            }
            
            //var v187A = Core.Memory.Read<byte>(director + Offsets.DDMapGroup);

            var v3 = director + PublicOffsets.DeepDungeonOffsets.Map5xStart + v187A * PublicOffsets.DeepDungeonOffsets.Map5xSize;
            var v332 = Core.Memory.Read<ushort>(v3 + PublicOffsets.DeepDungeonOffsets.WallStartingPoint);
            Logger.Info($"v332 {v332}");

            var v29 = v3 + 0x10;
            var v7_location = v29;


            var v7 = Core.Memory.ReadArray<short>(v7_location, 5);
            Logger.Info($"v7 count {v7.Length}");
            var wallset = new HashSet<uint>();

            var v5 = 0;

            var types = new uint[] {1, 2, 4, 8}; //taken from the client

            for (var v30 = 5; v30 > 1; v30--)
            {
                for (var v8 = 0; v8 < 5; v8++)
                {
                    if (v7[v8] != 0)
                    {
                        var v9 = v3 + 0x14 * (v7[v8] - v332);

                        // var wall = Core.Memory.Read<uint>(v9 + Offsets.UNK_StartingCircle);
                        //wallset.Add(wall);

                        var @byte = Core.Memory.Read<byte>(director + v5 + PublicOffsets.DeepDungeonOffsets.WallGroupEnabled);
                        var walls = Core.Memory.ReadArray<uint>(v9 + PublicOffsets.DeepDungeonOffsets.Starting, 4);
                        //Logger.Info($"Walls count {walls.Length}");
                        //Logger.Info($"Walls byte {@byte}");
                        for (var v16 = 0; v16 < 4; v16++)
                        {
                            if (walls[v16] < 2)
                                continue;

                            if ((@byte & types[v16]) != 0) //==0 is closed != 0 is "open"
                                wallset.Add(walls[v16]);
                        } //for3
                    }

                    v5++;
                }

                v7_location = v29 + 0xc;
                v7 = Core.Memory.ReadArray<short>(v7_location, 5);
                v29 = v29 + 0xc;
            }

            //_wallcache = wallset;
            return wallset;
        }

        internal static void Render(object sender, DrawingEventArgs e)
        {
            if (!Settings.Instance.DebugRender) return;
            if (!Constants.InDeepDungeon) return;
            try
            {
                var drawer = e.Drawer;


                //if (_path != null)
                //{
                //    var start = (Vector3)_path.First();
                //    foreach (var x in _path)
                //    {
                //        drawer.DrawLine(start, (Vector3)x, Color.Black);
                //        start = (Vector3)x;
                //    }
                //}

                if (Walls == null)
                    return;

                //List<uint> active = new List<uint>();
                //active.AddRange(_hit.Keys);

                //foreach (var x in active)
                //{
                //    var extents = Bound(_walls[x][2], _walls[x][1]);
                //    drawer.DrawBox(_walls[x][0], extents, Color.FromArgb(150, Color.Goldenrod));
                //}

                var service = new List<BoundingBox3>();
                service.AddRange(wallList);

                foreach (var x in service)
                {
                    var extents = Bound(x.Min, x.Max);
                    drawer.DrawBox(Vector3.Lerp(x.Min, x.Max, 0.5f), extents, Color.FromArgb(100, Color.Turquoise));
                }

                var tarp = new List<BoundingCircle>();
                tarp.AddRange(trapList);
                foreach (var t in tarp)
                {
                    drawer.DrawCircleOutline(t.Center, t.Radius, Color.FromArgb(100, Color.Red));
                }
            }
            catch (Exception)
            {
            }
        }

        public static Vector3 Bound(Vector3 a, Vector3 b)
        {
            var minX = Math.Min(a.X, b.X);
            var minY = Math.Min(a.Y, b.Y);
            var minZ = Math.Min(a.Z, b.Z);

            var maxX = Math.Max(a.X, b.X);
            var maxY = Math.Max(a.Y, b.Y);
            var maxZ = Math.Max(a.Z, b.Z);

            return new Vector3(maxX - minX, maxY - minY, maxZ - minZ) / 2;
        }


        private void AddBlackspots()
        {
            //if we have added blackspots already OR there aren't any traps
            if (!GameObjectManager.GameObjects.Any(
                i => i.Location != Vector3.Zero && Constants.TrapIds.Contains(i.NpcId) && !_traps.Contains(i.ObjectId))) return;
            {
                Logger.Verbose("Adding Black spots {0}", GameObjectManager.GameObjects.Count(i => i.Location != Vector3.Zero && Constants.TrapIds.Contains(i.NpcId)));
                foreach (var i in GameObjectManager.GameObjects.Where(i => i.Location != Vector3.Zero && Constants.TrapIds.Contains(i.NpcId) && !_traps.Contains(i.ObjectId) && i.IsVisible))
                {
                    Logger.Verbose($"[{i.NpcId}] {i.ObjectId} - {i.Location}");
                    //_detour.AddBlackspot(i.Location, TrapSize);
                    trapList.Add(new BoundingCircle {Center = i.Location, Radius = TrapSize});
                    _traps.Add(i.ObjectId);
                    Traps.Add(i.Location);
                }
            }
        }
    }

    internal static class StraightPathHelper
    {
        private static readonly MethodInfo Method;

        static StraightPathHelper()
        {
            Method = typeof(NavigationProvider).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(i => i.ReturnType == typeof(List<Vector3>));
        }

        /// <summary>
        ///     invoke the get straight path information.
        /// </summary>
        /// <returns></returns>
        internal static List<Vector3> GetStraightPath()
        {
            if (Method == null)
            {
                Logger.Warn("GSP is null?");
                return null;
            }

            return RealStraightPath();
        }

        internal static List<Vector3> RealStraightPath()
        {
            return (List<Vector3>) Method.Invoke((Navigator.NavigationProvider as WrappingNavigationProvider).Original, new object[] { });
        }
    }
}