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
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Tiles[x, y] = 1;
                }
            }
        }

        // Randomly place a bunch of rooms everywhere, ignore hallways and overlap for now
        public World Generate(int rooms) {
            for (int room = 0; room < rooms; room++) {
                int radiusX = Random.Next(1, 4);
                int radiusY = Random.Next(1, 4);
                int originX = Random.Next(radiusX + 1, Width - radiusX - 2);
                int originY = Random.Next(radiusY + 1, Height - radiusY - 2);
                for (int x = originX - radiusX; x < originX + radiusX + 1; x++) {
                    for (int y = originY - radiusY; y < originY + radiusY + 1; y++) {
                        Tiles[x, y] = 0;
                    }
                }
            }
            return new World(Width, Height, Tiles);
        }
    }
}
