namespace MonoRogue {
    public class CaveGeneration {
        private System.Random Random;
        private int Width;
        private int Height;
        private int[,] Tiles;

        public CaveGeneration(System.Random rng, int width, int height, int[,] tiles) {
            Random = rng;
            Width = width;
            Height = height;
            Tiles = tiles;
        }

        // Make each tile either a wall or a floor
        private void RandomizeTiles() {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Tiles[x,y] = Random.Next(2);
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

        // Draw a border around the entire map each iteration
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

        public int[,] Generate(int iterations) {
            RandomizeTiles();
            for (int i = 0; i < iterations; i++) {
                Smooth();
                Border();
            }
            return Tiles;
        }
    }
}