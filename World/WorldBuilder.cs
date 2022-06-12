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

        public World GenerateDungeon(int iterations) {
            DungeonGeneration generator = new DungeonGeneration(Random, Width, Height, Tiles);
            int[,] tiles = generator.Generate(iterations);
            bool[,] dungeonTiles = generator.DungeonTiles;
            return new World(Width, Height, IntToTiles(tiles, dungeonTiles));
        }

        private Tile[,] IntToTiles(int[,] old, bool[,] dungeonTiles) {
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
            return tiles;
        }
    }
}
