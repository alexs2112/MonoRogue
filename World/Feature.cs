using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class Feature : Tile {
        private Feature(bool walkable, bool seeThrough,Texture2D glyph, Color color) : base(walkable, seeThrough, glyph, color) { }

        private static Feature DoorClosed;
        private static Feature DoorOpen;
        private static Feature Bookshelf;
        private static Feature BookshelfSmall;
        private static Feature Table;
        private static Feature Well;
        private static Feature Campfire;
        private static Feature Minecart;
        private static Feature Rubble;
        private static Feature Bones;
        private static Feature Skull;
        private static Feature Grass;
        public static void LoadFeatures(ContentManager content) {
            DoorClosed = new Feature(false, false, content.Load<Texture2D>("Tiles/DoorClosed"), Color.Brown);
            DoorClosed.Breakable = true;
            DoorOpen = new Feature(true, true, content.Load<Texture2D>("Tiles/DoorOpen"), Color.Brown);
            Bookshelf = new Feature(false, false, content.Load<Texture2D>("Tiles/Bookshelf"), Color.RosyBrown);
            BookshelfSmall = new Feature(false, true, content.Load<Texture2D>("Tiles/BookshelfSmall"), Color.RosyBrown);
            Table = new Feature(false, true, content.Load<Texture2D>("Tiles/Table"), Color.Sienna);
            Well = new Feature(false, true, content.Load<Texture2D>("Tiles/Well"), Color.LightSteelBlue);
            Campfire = new Feature(false, true, content.Load<Texture2D>("Tiles/Campfire"), Color.DarkOrange);
            Minecart = new Feature(false, true, content.Load<Texture2D>("Tiles/Minecart"), Color.CornflowerBlue);
            Rubble = new Feature(false, false, content.Load<Texture2D>("Tiles/Rubble"), Color.SaddleBrown);

            Bones = new Feature(true, true, content.Load<Texture2D>("Tiles/Bones"), Color.LightGray);
            Skull = new Feature(true, true, content.Load<Texture2D>("Tiles/Skull"), Color.LightGray);
            Grass = new Feature(true, true, content.Load<Texture2D>("Tiles/Grass"), Color.ForestGreen);
        }

        public static Feature GetClosedDoor() {
            return DoorClosed;
        }
        public static Feature GetOpenDoor() {
            return DoorOpen;
        }

        // Movement blocking features
        public static Feature GetWallFeature(System.Random random, bool IsDungeon) {
            if (IsDungeon) {
                switch(random.Next(3)) {
                    case 0: return Bookshelf;
                    case 1: return BookshelfSmall;
                    default: return Table;
                }
            } else {
                switch(random.Next(4)) {
                    case 0: return Minecart;
                    case 1: return Rubble;
                    case 2: return Campfire;
                    default: return Well;
                }
            }
        }

        // Non-movement blocking features (floor decorations)
        public static Feature GetFloorFeature(System.Random random, bool IsDungeon) {
            switch(random.Next(3)) {
                case 0: return Bones;
                case 1: return Skull;
                default: return Grass;
            }
        }

    }
}