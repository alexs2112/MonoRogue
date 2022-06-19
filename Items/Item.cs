using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {

    // Simple superclass to hold weapons, armors, foods
    public class Item {

        // Store the type of each item, currently only for player sprite purposes
        public enum Type {
            Null,   // Specifically as a catch for PlayerGlyph
            Food,
            Dagger,
            Sword,
            Bow,
            Mace,
            Spear,
            Axe,
            
            LightArmor,
            MediumArmor,
            HeavyArmor,

            Heartstone
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Texture2D Glyph { get; private set; }
        public Color Color { get; private set; }

        public bool IsArmor { get; protected set; }
        public bool IsWeapon { get; protected set; }
        public bool IsFood { get; protected set; }
        public bool IsHeartstone { get; protected set; }
        public Type ItemType { get; protected set; }
        public void SetType(Type itemType) { ItemType = itemType; }
        public void SetDescription(string s) { Description = s; }

        public Item(string name, Texture2D glyph, Color color) {
            Name = name;
            Glyph = glyph;
            Color = color;
        }
    }

    // Specific type of item that increases your max health by 4 when consumed
    public class Heartstone : Item {
        private Heartstone(Texture2D glyph) : base("Heartstone", glyph, Color.Red) {
            SetType(Type.Heartstone);
            IsHeartstone = true;
        }

        private static Heartstone Stone;
        public static void LoadHeartstone(ContentManager content) {
            Stone = new Heartstone(content.Load<Texture2D>("Misc/Heartstone"));
            Stone.SetDescription("A heart-shaped crystal. Heat emanates from it and it seems to shimmer in the air.");
        }

        public static Heartstone GetHeartstone() { return Stone; }

        public void Consume(Creature player) {
            player.MaxHP += 4;
            player.HP += 4;
            player.Notify("You consume the bloodstone.");
            player.Notify("Max health increased!");
        }
    }
}
