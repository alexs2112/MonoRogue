using System.Collections.Generic;

namespace MonoRogue {
    // Pregenerated dungeon rooms to add some variety and to improve the quality of the dungeon
    // To add a new vault: Create a template function at the bottom of this clas and make sure to add it to the `LoadVaults` function
    public class Vault {
        public int Width;
        public int Height;
        private string[] Tiles;
        private Dictionary<char, Tile> Map;
        private Vault(string[] tiles) {
            Height = tiles.Length;
            Width = tiles[0].Length;
            Tiles = tiles;
            Map = null;
        }
        private Vault(string[] tiles, Dictionary<char, Tile> map) {
            Height = tiles.Length;
            Width = tiles[0].Length;
            Tiles = tiles;
            Map = map;
        }

        private static List<Vault> Vaults;
        public static void LoadVaults() {
            Vaults = new List<Vault> {
                Pillar(),
                Pillar2(),
                TableVertical(),
                TableHorizontal(),
                SmallCandelabra(),
                Campfire(),
                Bones(),
                SmallLibrary(),
                MediumLibrary(),
                CollapsedMine(),
                CeilingCollapse(),
                TreeSmall(),
                TreeMedium(),
                Prison()
            };
        }
        public static Vault GetVault(System.Random random, int maxWidth, int maxHeight) {
            List<Vault> potential = new List<Vault>(Vaults);
            Vault v = null;
            while (potential.Count > 0 && v == null) {
                int i = random.Next(potential.Count);
                v = potential[i];

                if (v.Width > maxWidth || v.Height > maxHeight) {
                    v = null;
                    potential.RemoveAt(i);
                }
            }
            // Only spawn each vault up to one time
            if (v != null) { Vaults.Remove(v); }
            return v;
        }

        public void Parse(World world, int x, int y) {
            Tile[,] output = new Tile[Width, Height];
            int my = 0;
            foreach(string row in Tiles) {
                int mx = 0;
                foreach(char c in row) {
                    world.Tiles[x + mx, y + my] = GetTile(c);
                    mx++;
                }
                my++;
            }
        }

        private Tile GetTile(char c) {
            switch (c) {
                case '#': return Tile.GetDungeonWall();
                case '%': return Tile.GetCaveWall();
                case '.': return Tile.GetDungeonFloor();
                case ',': return Tile.GetCaveFloor();
                default: return Map[c];
            }
        }

        private static Vault Pillar() {
            string[] tiles = new string[6] {
                "......",
                "......",
                "..##..",
                "..##..",
                "......",
                "......"
            };
            return new Vault(tiles);
        }
        private static Vault Pillar2() {
            string[] tiles = new string[8] {
                "........",
                "...,,...",
                "..,%%,..",
                ".,%%%%,.",
                ".,%%%%,.",
                "..,%%,..",
                "...,,...",
                "........"
            };
            return new Vault(tiles);
        }
        private static Vault TableVertical() {
            string[] tiles = new string[8] {
                "......",
                "......",
                "..TT..",
                "..TT..",
                "..TT..",
                "..TT..",
                "......",
                "......"
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('T', Feature.Table);
            return new Vault(tiles, map);
        }
        private static Vault TableHorizontal() {
            string[] tiles = new string[6] {
                "........",
                "........",
                "..TTTT..",
                "..TTTT..",
                "........",
                "........"
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('T', Feature.Table);
            return new Vault(tiles, map);
        }
        private static Vault SmallCandelabra() {
            string[] tiles = new string[4] {
                "....",
                ".C..",
                "..C.",
                "...."
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('C', Feature.Candelabra);
            return new Vault(tiles, map);
        }
        private static Vault Campfire() {
            string[] tiles = new string[5] {
                "...,.",
                ".,,,.",
                "..F,.",
                ".,,..",
                "....."
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('F', Feature.Campfire);
            return new Vault(tiles, map);
        }
        private static Vault Bones() {
            string[] tiles = new string[5] {
                ".bb.b",
                ".B..b",
                "...b.",
                "..B..",
                "bb..b"
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('B', Feature.Skull);
            map.Add('b', Feature.Bones);
            return new Vault(tiles, map);
        }
        private static Vault SmallLibrary() {
            string[] tiles = new string[5] {
                "......",
                ".SSSS.",
                "......",
                ".SSSS.",
                "......"
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('S', Feature.Bookshelf);
            return new Vault(tiles, map);
        }
        private static Vault MediumLibrary() {
            string[] tiles = new string[6] {
                ".......",
                ".SS.SS.",
                ".......",
                ".......",
                ".SS.SS.",
                "......."
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('S', Feature.Bookshelf);
            map.Add('s', Feature.BookshelfSmall);
            return new Vault(tiles, map);
        }
        private static Vault CollapsedMine() {
            string[] tiles = new string[5] {
                "......",
                ".%%,rr",
                ".,,,Cr",
                ".b,R%.",
                "...,,."
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('C', Feature.Minecart);
            map.Add('r', Feature.RubbleSmall);
            map.Add('R', Feature.Rubble);
            map.Add('b', Feature.Bones);
            return new Vault(tiles, map);
        }
        private static Vault CeilingCollapse() {
            string[] tiles = new string[5] {
                "..,..",
                ".,r,.",
                ".,Rb,",
                ",r,,.",
                "...,."
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('r', Feature.RubbleSmall);
            map.Add('R', Feature.Rubble);
            map.Add('b', Feature.Bones);
            return new Vault(tiles, map);
        }
        private static Vault TreeSmall() {
            string[] tiles = new string[5] {
                "..,..",
                ".,,,.",
                ".,T,,",
                ".,,..",
                "....."
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('T', Feature.Tree);
            return new Vault(tiles, map);
        }
        private static Vault TreeMedium() {
            string[] tiles = new string[6] {
                "..,...",
                ".,,,T.",
                ".,T,,.",
                ".,,T..",
                ".,b.,.",
                "......"
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('T', Feature.Tree);
            map.Add('b', Feature.Bones);
            return new Vault(tiles, map);
        }
        private static Vault Prison() {
            string[] tiles = new string[7] {
                ".........",
                "..#####..",
                "...r#gB..",
                "..#####..",
                "..Bg#,b..",
                "..#####..",
                "........."
            };
            Dictionary<char, Tile> map = new Dictionary<char, Tile>();
            map.Add('B', Feature.Bars);
            map.Add('b', Feature.BarsBroken);
            map.Add('r', Feature.Bones);
            map.Add('g', Feature.ImpassableGround);
            return new Vault(tiles, map);
        }

    }
}
