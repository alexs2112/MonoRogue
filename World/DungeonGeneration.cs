using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class DungeonGeneration {
        private System.Random Random;
        private int Width;
        private int Height;
        private int[,] Tiles;
        private bool[,] Regionized;
        private List<Region> Regions;
        public bool[,] DungeonTiles;

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

            // Divide up the world into partitions, generate a room in each partition
            List<Rectangle> rooms = new List<Rectangle>();
            Partitioner partitioner = new Partitioner(Random, Width - 2, Height - 2);
            List<Rectangle> partitions = partitioner.Partition(1, 1, iterations);
            Dictionary<Rectangle, PartitionType> partitionTypes = new Dictionary<Rectangle, PartitionType>();
            foreach (Rectangle p in partitions) {
                partitionTypes[p] = Random.Next(2) == 0 ? PartitionType.ROOM : PartitionType.CAVE;
            }

            RandomizeTiles(partitionTypes);
            FinalizeCaves(3);

            // For each ROOM partition, randomly place a room
            foreach (Rectangle p in partitions) {
                if (partitionTypes[p] == PartitionType.CAVE) { continue; }
                Rectangle room = PlaceRoom(p);
                rooms.Add(room);
            }

            // Find all possible hallways, use Kruskal's algorithm to construct a minimum spanning tree
            List<Region> regions = GetRegions();
            List<Point> origins = GetRegionOrigins(regions);

            System.Console.WriteLine(origins.Count);

            List<Edge> edges = GetHallwayCandidates(origins);
            //List<Edge> trimmed = TrimHallwayCandidates(edges);
            List<Edge> hallways = FinalizeHallways(origins, edges);
            ConstructHallways(hallways);

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

        // Flood fill each available region of the dungeon, return the list of regions
        private List<Region> GetRegions() {
            Regions = new List<Region>();

            // Keep track of each tile that has already been placed into a region
            Regionized = new bool[Width, Height];

            // For each tile that is not in a region and is a floor, run the flood fill algorithm on it
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (Tiles[x, y] == 1) { continue; }
                    if (Regionized[x, y]) { continue; }
                    Region r = FloodFillRegion(x, y);
                    Regions.Add(r);
                }
            }
            return Regions;
        }

        private Region FloodFillRegion(int sx, int sy) {
            List<Point> tiles = new List<Point>();
            
            // A bit of a clunky way to find the origin of the region, sum each point in the region and find the average
            int totalX = 0;
            int totalY = 0;
            int totalPoints = 0;

            Stack<Point> open = new Stack<Point>();
            open.Push(new Point(sx, sy));

            while (open.Count > 0) {
                Point p = open.Pop();
                if (!Regionized[p.X, p.Y]) {
                    Regionized[p.X, p.Y] = true;
                    totalX += p.X;
                    totalY += p.Y;
                    totalPoints++;
                    tiles.Add(p);
                    foreach (Point n in GetNeighbours(p)) {
                        if (n.X < 0 || n.X >= Width || n.Y < 0 || n.Y >= Height) { continue; }
                        if (Tiles[n.X, n.Y] == 1) { continue; }
                        if (!Regionized[n.X, n.Y]) { open.Push(n); }
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

        // Go over each edge, remove points that are already floors (overlap with rooms)
        private List<Edge> TrimHallwayCandidates(List<Edge> edges) {
            List<Edge> trimmed = new List<Edge>();
            foreach (Edge e in edges) {
                List<Point> newPath = new List<Point>();
                foreach (Point p in e.Path) {
                    if (Tiles[p.X, p.Y] == 1) { newPath.Add(p); }
                }
                trimmed.Add(new Edge(e.A, e.B, newPath));
            }
            return trimmed;
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
                }
                i++;
            }
            return hallways;
        }

        // Once they are finalized, we go and mark each hallway tile as a floor
        private void ConstructHallways(List<Edge> hallways) {
            foreach (Edge e in hallways) {
                bool isDungeon;
                if (DungeonTiles[e.A.X, e.A.Y] && DungeonTiles[e.B.X, e.B.Y]) {
                    isDungeon = true;
                } else {
                    isDungeon = false;
                }

                foreach (Point p in e.Path) {
                    Tiles[p.X, p.Y] = 0;

                    if (isDungeon) { SetHallwayTileDungeon(p); }
                }
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
    }

    struct Region {
        public List<Point> Tiles;
        public Point Origin;

        public Region(List<Point> tiles, Point origin) {
            Tiles = tiles;
            Origin = origin;
        }
    }
}
