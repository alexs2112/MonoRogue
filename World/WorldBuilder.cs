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
            
            List<Region> regions = generator.Regions;
            (Region start, Region end) = SetRegionsCostMap(regions);
            SetRegionsDepth(regions, start.ID);

            World w = new World(Width, Height, IntToTiles(tiles, dungeonTiles, generator.Doors));

            SpawnPlayer(w, creatureFactory, start);

            Populator populator = new Populator(Random, w, creatureFactory, equipmentFactory);
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
                tiles[d.X,d.Y] = Tile.GetClosedDoor();
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
    }
}
