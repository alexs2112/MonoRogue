using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class DungeonGeneration {
        private System.Random Random;
        private int Width;
        private int Height;
        private int[,] Tiles;
        public int[,] RegionMap;
        public bool[,] DungeonTiles;
        public List<Region> Regions { get; private set; }
        public List<Point> Doors { get; private set; }
        public List<(Point, Vault)> Vaults { get; private set; }

        // Algorithm Overview:
        /*
        * Use binary space partitioning to divide the world into a bunch of relatively equal sized partitions.
        * Mark each partition as either CAVE or ROOM.
        * Randomly place floor and wall tiles. ROOM partitions have bias towards walls, CAVE partitions are biased towards floors.
        * Run cellular automata to smooth the world a bunch of times to create natural looking caves.
        * For each ROOM partition, randomly place a rectangle of floors to serve as a room.
        * Run a flood fill algorithm to determine world regions. Fill regions that are too small.
        * For each region, find a line to each other region.
        * Run Kruskal's algorithm using DFS on the resulting list of regions to create a minimum spanning tree.
        * Turn those edges into hallways to have a fully connected dungeon of caves and rooms with no cycles.
        */

        public DungeonGeneration(System.Random rng, int width, int height, int[,] tiles) {
            Random = rng;
            Width = width;
            Height = height;
            Tiles = tiles;
        }

        // Two types of partitions for now
        private enum PartitionType {
            ROOM,
            CAVE
        }

        public int[,] Generate(int iterations) {
            // First set everything to be a wall
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Tiles[x, y] = 1;
                }
            }

            // Keep track of which tiles use which tile set
            DungeonTiles = new bool[Width, Height];

            // Keep track of where the vaults are, to generate then when the world is being finalized
            Vaults = new List<(Point, Vault)>();

            // Divide up the world into partitions, generate a room in each partition
            Partitioner partitioner = new Partitioner(Random, Width - 2, Height - 2);
            List<Rectangle> partitions = partitioner.Partition(1, 1, iterations);
            Dictionary<Rectangle, PartitionType> partitionTypes = new Dictionary<Rectangle, PartitionType>();
            foreach (Rectangle p in partitions) {
                partitionTypes[p] = Random.Next(3) < 2 ? PartitionType.ROOM : PartitionType.CAVE;
            }

            RandomizeTiles(partitionTypes);
            FinalizeCaves(3);

            // For each ROOM partition, randomly place a room
            foreach (Rectangle p in partitions) {
                if (partitionTypes[p] == PartitionType.CAVE) { continue; }

                if (Random.Next(10) < 4) {
                    (Point point, Vault vault) output = PlaceVault(p);
                    if (output.vault == null) { PlaceRoom(p); }
                    else { Vaults.Add(output); }
                } else {
                    PlaceRoom(p);
                }
            }

            // Find all possible hallways, use Kruskal's algorithm to construct a minimum spanning tree
            Regions = GetRegions();
            List<Point> origins = GetRegionOrigins(Regions);
            List<Edge> edges = GetHallwayCandidates(origins);
            List<Edge> hallways = FinalizeHallways(origins, edges);
            ConstructHallwaysAndDoors(hallways);

            return Tiles;
        }

        // Randomize all tiles in each partition, ROOMS gravitate towards walls, CAVES gravitate towards floors
        private void RandomizeTiles(Dictionary<Rectangle, PartitionType> partitions) {
            foreach (Rectangle p in partitions.Keys) {
                for (int x = p.X; x < p.X + p.Width; x++) {
                    for (int y = p.Y; y < p.Y + p.Height; y++) {
                        int t = Random.Next(10);
                        if (partitions[p] == PartitionType.ROOM) {
                            if (t < 4) { Tiles[x, y] = 0; }
                        } else {
                            if (t < 7) { Tiles[x, y] = 0; }
                        }
                    }
                }
            }
        }

        // For each tile, convert it to a wall or floor depending on its neighbours
        private void Smooth() {
            int[,] tilesNext = new int[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    int floors = 0;
                    int walls = 0;
                    for (int mx = -1; mx <= 1; mx++) {
                        for (int my = -1; my <= 1; my++) {
                            if (x == 0 && y == 0) { continue; }
                            if (x + mx < 0 || x + mx >= Width || y + my < 0 || y + my >= Height) { continue; }

                            if (Tiles[x + mx, y + my] == 0) { floors++; }
                            else { walls++; }
                        }
                    }
                    tilesNext[x, y] = (floors >= walls ? 0 : 1);
                }
            }
            Tiles = tilesNext;
        }

        // Repeatedly smooth the randomized tiles to generate natural looking caves
        public int[,] FinalizeCaves(int iterations) {
            for (int i = 0; i < iterations; i++) {
                Smooth();
                Border();
            }
            return Tiles;
        }
        
        // Draw a border around the entire map after each smooth iteration so there isnt a random opening
        private void Border() {
            for (int x = 0; x < Width; x++) {
                Tiles[x, 0] = 1;
                Tiles[x, Height-1] = 1;
            }
            for (int y = 0; y < Height; y++) {
                Tiles[0, y] = 1;
                Tiles[Width-1, y] = 1;
            }
        }

        // Given a rectangular partition of space, place a room in it somewhere and return the room rectangle
        private Rectangle PlaceRoom(Rectangle partition) {
            int width = Random.Next(Constants.RoomMinSize, partition.Width);
            int height = Random.Next(Constants.RoomMinSize, partition.Height);
            int x = Random.Next(partition.X, partition.X + partition.Width - width);
            int y = Random.Next(partition.Y, partition.Y + partition.Height - height);
            for (int dx = -1; dx < width + 1; dx++) {
                for (int dy = -1; dy < height + 1; dy++) {
                    DungeonTiles[dx + x, dy + y] = true;
                    if (dx == -1 || dx == width || dy == -1 || dy == height) { continue; }
                    Tiles[dx + x, dy + y] = 0;
                }
            }
            return new Rectangle(x, y, width, height);
        }

        private (Point, Vault) PlaceVault(Rectangle partition) {
            Vault v = Vault.GetVault(Random, partition.Width - 1, partition.Height - 1);
            if (v == null) { return (new Point(0, 0), null); }

            int x = Random.Next(partition.X, partition.X + partition.Width - v.Width);
            int y = Random.Next(partition.Y, partition.Y + partition.Height - v.Height);
            for (int dx = -1; dx < v.Width + 1; dx++) {
                for (int dy = -1; dy < v.Height + 1; dy++) {
                    DungeonTiles[dx + x, dy + y] = true;
                    if (dx == -1 || dx == v.Width || dy == -1 || dy == v.Height) { continue; }
                    Tiles[dx + x, dy + y] = 0;
                }
            }

            return (new Point(x, y), v);
        }

        // Flood fill each available region of the dungeon, return the list of regions
        private List<Region> GetRegions() {
            Regions = new List<Region>();

            // Keep track of each tile that has already been placed into a region
            RegionMap = new int[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    RegionMap[x, y] = -1;
                }
            }

            int id = 0;

            // For each tile that is not in a region and is a floor, run the flood fill algorithm on it
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (Tiles[x, y] == 1) { continue; }
                    if (RegionMap[x, y] > -1) { continue; }
                    Region r = FloodFillRegion(x, y, id);

                    if (r.Tiles.Count < (Constants.RoomMinSize - 1) * (Constants.RoomMinSize - 1)) {
                        foreach (Point p in r.Tiles) { Tiles[p.X, p.Y] = 1; RegionMap[p.X, p.Y] = -1; }
                    } else {
                        r.ID = id;
                        id++;
                        Regions.Add(r); 
                    }
                }
            }
            return Regions;
        }

        private Region FloodFillRegion(int sx, int sy, int id) {
            List<Point> tiles = new List<Point>();
            
            // A bit of a clunky way to find the origin of the region, sum each point in the region and find the average
            int totalX = 0;
            int totalY = 0;
            int totalPoints = 0;

            Stack<Point> open = new Stack<Point>();
            open.Push(new Point(sx, sy));

            while (open.Count > 0) {
                Point p = open.Pop();
                if (RegionMap[p.X, p.Y] == -1) {
                    RegionMap[p.X, p.Y] = id;
                    totalX += p.X;
                    totalY += p.Y;
                    totalPoints++;
                    tiles.Add(p);
                    foreach (Point n in GetNeighbours(p)) {
                        if (n.X < 0 || n.X >= Width || n.Y < 0 || n.Y >= Height) { continue; }
                        if (Tiles[n.X, n.Y] == 1) { continue; }
                        if (RegionMap[n.X, n.Y] == -1) { open.Push(n); }
                    }
                }
            }
            Point origin = new Point(totalX / totalPoints, totalY / totalPoints);
            return new Region(tiles, origin);
        }

        private List<Point> GetNeighbours(Point p) {
            List<Point> output = new List<Point>();
            output.Add(new Point(p.X - 1, p.Y));
            output.Add(new Point(p.X + 1, p.Y));
            output.Add(new Point(p.X, p.Y - 1));
            output.Add(new Point(p.X, p.Y + 1));
            return output;
        }

        private List<Point> GetRegionOrigins(List<Region> regions) {
            List<Point> origins = new List<Point>();
            foreach (Region r in regions) { origins.Add(r.Origin); }
            return origins;
        }

        // Get all possible hallways by drawing a line from each room origin to each other origin
        private List<Edge> GetHallwayCandidates(List<Point> origins) {
            List<Edge> edges = new List<Edge>();
            List<Point> origins2 = new List<Point>(origins);

            foreach (Point s in origins) {
                origins2.Remove(s);
                foreach (Point d in origins2) {
                    List<Point> hallway = GetHallwayLine(s, d);
                    edges.Add(new Edge(s, d, hallway));
                }
            }
            return edges;
        }

        // Go over all hallways, use kruskals algorithm to find the edges that create a minimum spanning tree
        private List<Edge> FinalizeHallways(List<Point> origins, List<Edge> edges) {
            // Sort the list by minimum length of edges
            List<Edge> sorted = edges.OrderBy(e => e.Path.Count).ToList<Edge>();

            DFS dfs = new DFS(origins);

            // Kruskal's Algorithm implementation
            int i = 0;
            List<Edge> hallways = new List<Edge>();
            while (hallways.Count < origins.Count - 1) {
                Edge candidate = sorted[i];

                if (!dfs.ContainsCycleWith(candidate)) { 
                    hallways.Add(candidate);                     
                    dfs.AddHallway(candidate);
                    (Region a, Region b) = GetRegionsByEdge(candidate);
                    a.Neighbours.Add(b);
                    b.Neighbours.Add(a);
                }
                i++;
            }
            return hallways;
        }
        private (Region, Region) GetRegionsByEdge(Edge e) {
            Region a = null; Region b = null;
            foreach (Region r in Regions) {
                if (e.A == r.Origin || e.B == r.Origin) {
                    if (a == null) { a = r; }
                    else { b = r; return (a, b); }
                }
            }
            throw new System.Exception("Failed to find region neighbours, this should never happen...");
        }

        // Once they are finalized, we go and mark each hallway tile as a floor
        private void ConstructHallwaysAndDoors(List<Edge> hallways) {
            Doors = new List<Point>();
            foreach (Edge e in hallways) {
                bool isDungeon;
                if (DungeonTiles[e.A.X, e.A.Y] && DungeonTiles[e.B.X, e.B.Y]) {
                    isDungeon = true;
                } else {
                    isDungeon = false;
                }

                List<Point> potentialDoors = new List<Point>();
                foreach (Point p in e.Path) {
                    if (Tiles[p.X, p.Y] == 1) {
                        Tiles[p.X, p.Y] = 0;
                        potentialDoors.Add(p);
                    }
                }
                if (isDungeon) { 
                    for (int i = 1; i < e.Path.Count; i++) {
                        SetHallwayTileDungeon(e.Path[i]); 
                    }
                }

                 // For each new hallway tile, find the first and last valid door
                Point first = Point.Zero;
                Point last = Point.Zero;
                int j;
                for (j = 0; j < potentialDoors.Count; j++) {
                    if (IsValidDoor(potentialDoors[j])) {
                        first = potentialDoors[j];
                        j += 1; // So that two doors arent placed next to each other
                        break;
                    }
                }
                for (int k = potentialDoors.Count - 1; k > j; k--) {
                    if (IsValidDoor(potentialDoors[k])) {
                        last = potentialDoors[k];
                        break;
                    }
                }
                if (first != Point.Zero) { Doors.Add(first); }
                if (last != Point.Zero) { Doors.Add(last); }
            }
        }
        private void SetHallwayTileDungeon(Point p) {
            for (int x = -1; x < 2; x++) {
                for (int y = -1; y < 2; y++) {
                    if (p.X + x < 0 || p.X + x >= Width || p.Y + y < 0 || p.Y + y >= Height) { continue; }
                    DungeonTiles[p.X + x, p.Y + y] = true;
                }
            }
        }
        private bool IsValidDoor(Point p) {
            // A valid door is a tile that is either
            /*  x = any, # = wall, . = floor
            * x#x
            * ...
            * x#x

            * x.x
            * #.#
            * x.x
            */
            return DungeonTiles[p.X, p.Y] &&
                Tiles[p.X - 1, p.Y] == Tiles[p.X + 1, p.Y] &&
                Tiles[p.X, p.Y - 1] == Tiles[p.X, p.Y + 1] &&
                Tiles[p.X - 1, p.Y] != Tiles[p.X, p.Y - 1];
        }

        // Return a line of points between a source and destination without including diagonals
        private List<Point> GetHallwayLine(Point source, Point dest) {
            List<Point> points = new List<Point>();

            int sx = source.X < dest.X ? 1 : -1;
            int sy = source.Y < dest.Y ? 1 : -1;
            int dx = System.Math.Abs(dest.X - source.X);
            int dy = System.Math.Abs(dest.Y - source.Y);
            int err = dx - dy;

            while (true) {
                points.Add(source);
                if (source == dest) { break; }

                int e2 = err * 2;
                if (e2 >= -dy) {
                    err -= dy;
                    source.X += sx;
                } else if (e2 < dx) {
                    err += dx;
                    source.Y += sy;
                }
            }
            return points;
        }

        // Simple way to print the current world to the console
        private void PrintToTerminal() {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    char c;
                    if (Tiles[x,y] == 0) { c = '.'; }
                    else { c = '#'; }
                    System.Console.Write(c);
                }
                System.Console.Write("\n");
            }
        }
    }

    struct Edge {
        public Point A;
        public Point B;
        public List<Point> Path;
        public Edge(Point a, Point b, List<Point> path) {
            A = a;
            B = b;
            Path = path;
        }

        public bool ContainsPoint(Point p) {
            return p == A || p == B;
        }
    }

    public class Region {
        public List<Point> Tiles;
        public Point Origin;
        public List<Region> Neighbours;
        public int ID;

        // How many regions are between this one and each other region, Depth is distance from the start region
        public int[] CostMap;
        public int Depth;

        public Region(List<Point> tiles, Point origin) {
            Tiles = tiles;
            Origin = origin;
            Neighbours = new List<Region>();
        }

        public Point GetEmptyTile(System.Random random, World world) {
            while (Tiles.Count > 0) {
                int i = random.Next(Tiles.Count);
                Point p = Tiles[i];
                if (world.IsFloor(p) && world.GetCreatureAt(p) == null && world.GetItemAt(p) == null)  {
                    return p;
                }
                Tiles.RemoveAt(i);
            }
            if (Constants.Debug) { System.Console.WriteLine($"Could not find an empty tile in region {ID}"); }
            return new Point(-1, -1);
        }

        public int Size() { return Tiles.Count; }
    }
}
