using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class WorldBuilder {
        private System.Random Random;
        private int Width;
        private int Height;
        public int[,] Tiles;

        public WorldBuilder(System.Random rng, int width, int height) {
            Random = rng;
            Width = width;
            Height = height;
            Tiles = new int[width, height];
        }

        public World GenerateDungeon(int iterations, CreatureFactory creatureFactory, EquipmentFactory equipmentFactory) {
            DungeonGeneration generator = new DungeonGeneration(Random, Width, Height, Tiles);
            int[,] tiles = generator.Generate(iterations);
            bool[,] dungeonTiles = generator.DungeonTiles;

            // Finalize dungeon regions            
            List<Region> regions = generator.Regions;
            (Region start, Region end) = SetRegionsCostMap(regions);
            SetRegionsDepth(regions, start.ID);

            // The three dungeon tiers based on depth, inclusive
            int lowDepth = end.Depth / 3;
            int medDepth = lowDepth * 2;
            int highDepth = end.Depth;

            // Create the world
            World w = new World(Width, Height, IntToTiles(tiles, dungeonTiles, generator.Doors));

            // Add dungeon features to the world
            SetWorldFeatures(w, dungeonTiles, regions);

            // Give the dungeon an array of difficulty
            w.Difficulty = GetDungeonDifficulty(w, regions, lowDepth, medDepth);

            foreach((Point point, Vault vault) in generator.Vaults) {
                vault.Parse(w, point.X, point.Y);
            }

            // Set up start and end regions
            Point p = end.GetEmptyTile(Random, w);
            w.Exit = p;
            SpawnPlayer(w, creatureFactory, start);

            // Populate the dungeon with items and creatures
            Populator populator = new Populator(Random, w, creatureFactory, equipmentFactory);
            populator.SetDepth(lowDepth, medDepth, highDepth);
            populator.Populate(w.Player, start, end, regions);

            return w;
        }

        public Creature SpawnPlayer(World world, CreatureFactory creatureFactory, Region start) {
            Point startTile = start.GetEmptyTile(Random, world);
            return creatureFactory.NewPlayer(world, startTile.X, startTile.Y);
        }

        private Tile[,] IntToTiles(int[,] old, bool[,] dungeonTiles, List<Point> doors) {
            Tile[,] tiles = new Tile[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (old[x,y] == 0) {
                        if (dungeonTiles[x,y]) { tiles[x,y] = Tile.GetDungeonFloor(Random); }
                        else { tiles[x,y] = Tile.GetCaveFloor(Random); }
                    } else {
                        if (dungeonTiles[x,y]) { tiles[x,y] = Tile.GetDungeonWall(Random); }
                        else { tiles[x,y] = Tile.GetCaveWall(Random); }
                    }
                }
            }
            foreach (Point d in doors) {
                tiles[d.X,d.Y] = Feature.GetClosedDoor();
            }
            return tiles;
        }

        private (Region start, Region end) SetRegionsCostMap(List<Region> regions) {
            // Set the cost to get to itself as 0
            // Set the cost to get to each neighbour as 1
            // Set the cost to get to each other region as -1 (unknown)
            foreach (Region r in regions) {
                r.CostMap = new int[regions.Count];
                System.Array.Fill(r.CostMap, -1);
                r.CostMap[r.ID] = 0;
                foreach (Region n in r.Neighbours) {
                    r.CostMap[n.ID] = 1;
                }
            }

            // For each iteration, pass over each region and their neighbours and pass costs between them
            int highest_cost = -1;
            Region start = null;
            Region end = null;
            for (int iteration = 0; iteration < regions.Count / 2; iteration++) {
                foreach (Region r in regions) {
                    foreach (Region n in r.Neighbours) {
                        for (int i = 0; i < regions.Count; i++) {
                            // If either r or n know how much it costs to get to regions[i], share that info
                            // if the other region does not know it
                            int r_cost = r.CostMap[i];
                            int n_cost = n.CostMap[i];
                            
                            if (r_cost == -1 && n_cost > -1) {
                                r.CostMap[i] = n_cost + 1;

                                if (n_cost + 1 > highest_cost) {
                                    highest_cost = n_cost + 1;
                                    start = r;
                                    end = regions[i];
                                }
                            } else if (n_cost == -1 && r_cost > -1) {
                                n.CostMap[i] = r_cost + 1;

                                if (r_cost + 1 > highest_cost) {
                                    highest_cost = r_cost + 1;
                                    start = n;
                                    end = regions[i];
                                }
                            }
                        }
                    }
                }
            }
            // Start should be the smaller region between the two
            if (start.Size() > end.Size()) { return (end, start); }
            else { return (start, end); }
        }
        private void SetRegionsDepth(List<Region> regions, int startID) {
            foreach (Region r in regions) { r.Depth = r.CostMap[startID]; }
        }   
        private int[,] GetDungeonDifficulty(World world, List<Region> regions, int low, int med) {
            int[,] start = new int[Width, Height];
            foreach (Region r in regions) { 
                foreach(Point p in r.Tiles) {
                    start[p.X, p.Y] = r.Depth <= low ? 1 : r.Depth <= med ? 2 : 3;
                }
            }
            int[,] difficulty = new int[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    difficulty[x, y] = GetTileDifficulty(start, x, y);
                }
            }   
            return difficulty;
        }
        private int GetTileDifficulty(int[,] start, int x, int y) {
            if (start[x,y] != 0) { return start[x,y]; }

            int v = 0;
            for (int mx = -1; mx <= 1; mx++) {
                for (int my = -1; my <= 1; my++) {
                    if (x + mx < 0 || x + mx >= Width || y + my < 0 || y + my >= Height) { continue; }
                    if (start[x + mx, y + my] > v) { v = start[x + mx, y + my]; }
                }
            }
            return v;
        }

        private void SetWorldFeatures(World world, bool[,] isDungeon, List<Region> regions) {
            foreach (Region r in regions) {
                List<Point> potential = new List<Point>();
                foreach(Point p in r.Tiles) {
                    if (ValidBrokenWall(world, isDungeon, p)) { potential.Add(p); }
                }

                List<Point> trimmed = new List<Point>(potential);
                foreach(Point p in potential) {
                    for (int i = 0; i < trimmed.Count; i++) {
                        if (AdjacentPoints(p, trimmed[i])) {
                            trimmed.Remove(p);
                            break;
                        }
                    }
                }

                foreach (Point p in trimmed) {
                    world.Tiles[p.X, p.Y] = Tile.GetCrumbledWall(world, isDungeon, p);
                }
            }

            // Mark each tile as either valid or invalid for a feature
            // Features cannot be placed next to walls, to ensure that it won't block off the dungeon
            bool[,] valid = new bool[Width, Height];
            for (int x = 1; x < Width - 1; x++) {
                for (int y = 1; y < Height - 1; y++) {
                    valid[x, y] = ValidFeatureTile(world, x, y);
                }
            }
            foreach (Region r in regions) {
                List<Point> tiles = new List<Point>(r.Tiles);

                for (int i = 0; i < r.Size() / (Constants.RoomMinSize * Constants.RoomMinSize / 2) - 1; i++) {

                    Point p = new Point(0);
                    do {
                        if (tiles.Count == 0) { break; }
                        int index = Random.Next(tiles.Count);
                        p = tiles[index];
                        tiles.RemoveAt(index);
                    } while (!valid[p.X, p.Y]);
                    if (p.X == 0 && p.Y == 0) { break; }

                    if (Random.Next(2) == 0) {
                        world.Tiles[p.X, p.Y] = Feature.GetWallFeature(Random, isDungeon[p.X, p.Y]);
                    } else {
                        world.Tiles[p.X, p.Y] = Feature.GetFloorFeature(Random, isDungeon[p.X, p.Y]);
                    }
                    valid = FinalizeFeatureFile(valid, p);
                }
            }
        }

        private bool ValidBrokenWall(World world, bool[,] isDungeon, Point p) {
            return
                world.IsFloor(p) &&
                isDungeon[p.X, p.Y] &&
                AdjacentToOneWall(world, p) &&
                AdjacentToCaveFloors(world, isDungeon, p);
        }

        private bool AdjacentToOneWall(World world, Point p)  {
            int total = 0;
            if (world.IsWall(p.X - 1, p.Y)) { total++; } 
            if (world.IsWall(p.X + 1, p.Y)) { total++; } 
            if (world.IsWall(p.X, p.Y - 1)) { total++; } 
            if (world.IsWall(p.X, p.Y + 1)) { total++; } 
            return total == 1;
        }
        
        private bool AdjacentToCaveFloors(World w, bool[,] d, Point p) {
            return 
                IsCaveFloor(w, d, p.X - 1, p.Y - 1) && IsCaveFloor(w, d, p.X - 1, p.Y) && IsCaveFloor(w, d, p.X - 1, p.Y + 1) ||
                IsCaveFloor(w, d, p.X + 1, p.Y - 1) && IsCaveFloor(w, d, p.X + 1, p.Y) && IsCaveFloor(w, d, p.X + 1, p.Y + 1) ||
                IsCaveFloor(w, d, p.X - 1, p.Y - 1) && IsCaveFloor(w, d, p.X, p.Y - 1) && IsCaveFloor(w, d, p.X + 1, p.Y - 1) ||
                IsCaveFloor(w, d, p.X - 1, p.Y + 1) && IsCaveFloor(w, d, p.X, p.Y + 1) && IsCaveFloor(w, d, p.X + 1, p.Y + 1);
        }
        private bool IsCaveFloor(World world, bool[,] isDungeon, int x, int y) {
            return !isDungeon[x, y] && world.IsFloor(x, y);
        }

        private bool AdjacentPoints(Point a, Point b) {
            if (a.X == b.X) {
                return a.Y == b.Y - 1 || a.Y == b.Y + 1;
            } else if (a.Y == b.Y) {
                return a.X == b.X - 1 || a.X == b.X + 1;
            }
            return false;
        }

        private bool ValidFeatureTile(World world, int x, int y) {
            for (int mx = x - 1; mx <= x + 1; mx++) {
                for (int my = y - 1; my <= y + 1; my++) {
                    if (world.IsWall(mx, my)) { return false; }
                }
            }
            return true;
        }
        private bool[,] FinalizeFeatureFile(bool[,] valid, Point p) {
            for (int mx = p.X - 1; mx <= p.X + 1; mx++) {
                for (int my = p.Y - 1; my <= p.Y + 1; my++) {
                    valid[mx, my] = false;
                }
            }
            return valid;
        }
    }
}
