using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class Feature : Tile {
        public string Name;
        private Feature(string name, bool walkable, bool seeThrough,Texture2D glyph, Color color) : base(walkable, seeThrough, glyph, color) { 
            IsFeature = true;
            Name = name;
        }

        public static Feature Exit;
        public static Feature ExitOpen;

        public static Feature DoorClosed;
        public static Feature DoorOpen;
        public static Feature DoorBroken0;
        public static Feature DoorBroken1;
        public static Feature DoorBroken2;
        public static Feature DoorBroken3;
        public static Feature Bookshelf;
        public static Feature BookshelfSmall;
        public static Feature Table;
        public static Feature Cauldron;
        public static Feature Candelabra;
        public static Feature Well;
        public static Feature Campfire;
        public static Feature Minecart;
        public static Feature Rubble;
        public static Feature Pillar;
        public static Feature Bones;
        public static Feature Skull;
        public static Feature Grass;
        public static Feature RubbleSmall;

        // Vault specific features
        public static Feature Tree;
        public static Feature Bars;
        public static Feature BarsBroken;
        public static Feature ImpassableGround;

        public static void LoadFeatures(ContentManager content) {
            Exit = new Feature("Exit", true, true, content.Load<Texture2D>("Misc/ExitLocked"), Color.Gold);
            ExitOpen = new Feature("Exit", true, true, content.Load<Texture2D>("Misc/ExitOpen"), Color.SkyBlue);

            DoorClosed = new Feature("Door", false, false, content.Load<Texture2D>("Tiles/DoorClosed"), Color.Brown);
            DoorClosed.CanOpen = true;
            DoorOpen = new Feature("Open Door", true, true, content.Load<Texture2D>("Tiles/DoorOpen"), Color.Brown);
            DoorBroken0 = new Feature("Broken Door", true, true, content.Load<Texture2D>("Tiles/DoorBroken0"), Color.Brown);
            DoorBroken1 = new Feature("Broken Door", true, true, content.Load<Texture2D>("Tiles/DoorBroken1"), Color.Brown);
            DoorBroken2 = new Feature("Broken Door", true, true, content.Load<Texture2D>("Tiles/DoorBroken2"), Color.Brown);
            DoorBroken3 = new Feature("Broken Door", true, true, content.Load<Texture2D>("Tiles/DoorBroken3"), Color.Brown);

            Bookshelf = new Feature("Bookshelf", false, false, content.Load<Texture2D>("Tiles/Bookshelf"), Color.RosyBrown);
            BookshelfSmall = new Feature("Bookshelf", false, true, content.Load<Texture2D>("Tiles/BookshelfSmall"), Color.RosyBrown);
            Table = new Feature("Table", false, true, content.Load<Texture2D>("Tiles/Table"), Color.Sienna);
            Cauldron = new Feature("Cauldron", false, true, content.Load<Texture2D>("Tiles/Cauldron"), Color.MediumSlateBlue);
            Candelabra = new Feature("Candelabra", false, true, content.Load<Texture2D>("Tiles/Candelabra"), Color.Gold);

            Well = new Feature("Well", false, true, content.Load<Texture2D>("Tiles/Well"), Color.LightSteelBlue);
            Campfire = new Feature("Campfire", false, true, content.Load<Texture2D>("Tiles/Campfire"), Color.DarkOrange);
            Minecart = new Feature("Minecart", false, true, content.Load<Texture2D>("Tiles/Minecart"), Color.CornflowerBlue);
            Rubble = new Feature("Rubble", false, false, content.Load<Texture2D>("Tiles/Rubble"), Color.SaddleBrown);
            Pillar = new Feature("Pillar", false, true, content.Load<Texture2D>("Tiles/Pillar"), Color.DarkGoldenrod);

            Bones = new Feature("Bones", true, true, content.Load<Texture2D>("Tiles/Bones"), Color.LightGray);
            Skull = new Feature("Bones", true, true, content.Load<Texture2D>("Tiles/Skull"), Color.LightGray);
            Grass = new Feature("Grass", true, true, content.Load<Texture2D>("Tiles/Grass"), Color.ForestGreen);
            RubbleSmall = new Feature("Rubble", true, true, content.Load<Texture2D>("Tiles/RubbleSmall"), Color.LightGray);

            Tree = new Feature("Tree", false, false, content.Load<Texture2D>("Tiles/Tree"), Color.ForestGreen);
            Bars = new Feature("Bars", false, true, content.Load<Texture2D>("Tiles/Bars"), Color.LightGray);
            BarsBroken = new Feature("Bars", true, true, content.Load<Texture2D>("Tiles/BarsBroken"), Color.LightGray);
            ImpassableGround = new Feature("Cell", false, true, content.Load<Texture2D>("Tiles/Floor0"), Color.LightGray);
        }

        public static Feature GetClosedDoor() {
            return DoorClosed;
        }
        public static Feature GetBrokenDoor() {
            switch(new System.Random().Next(4)) {
                case 0: return DoorBroken0;
                case 1: return DoorBroken1;
                case 2: return DoorBroken2;
                default: return DoorBroken3;
            }
        }
        public static bool IsBrokenDoor(Tile f) {
            return f == DoorBroken0 ||
                   f == DoorBroken1 ||
                   f == DoorBroken2 ||
                   f == DoorBroken3;
        }
        public static bool IsOpenDoor(Tile f) {
            return f == DoorOpen;
        }

        // Movement blocking features
        public static Feature GetWallFeature(System.Random random, bool IsDungeon) {
            if (IsDungeon) {
                switch(random.Next(5)) {
                    case 0: return Bookshelf;
                    case 1: return BookshelfSmall;
                    case 2: return Table;
                    case 3: return Cauldron;
                    default: return Candelabra;
                }
            } else {
                switch(random.Next(5)) {
                    case 0: return Minecart;
                    case 1: return Rubble;
                    case 2: return Campfire;
                    case 3: return Well;
                    default: return Pillar;
                }
            }
        }

        // Non-movement blocking features (floor decorations)
        public static Feature GetFloorFeature(System.Random random, bool IsDungeon) {
            switch(random.Next(4)) {
                case 0: return Bones;
                case 1: return Skull;
                case 2: return Grass;
                default: return RubbleSmall;
            }
        }

    }
}
