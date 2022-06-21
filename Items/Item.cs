using System.Collections.Generic;
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

            Heartstone,
            Key
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Texture2D Glyph { get; private set; }
        public Color Color { get; private set; }

        public bool IsArmor { get; protected set; }
        public bool IsWeapon { get; protected set; }
        public bool IsFood { get; protected set; }
        public bool IsHeartstone { get; protected set; }
        public bool IsKey { get; protected set; }
        public Type ItemType { get; protected set; }
        public void SetType(Type itemType) { ItemType = itemType; }
        public void SetDescription(string s) { Description = s; }

        public List<string> ItemInfo { get; protected set; }

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
            ItemInfo = MainInterface.SplitMessage("Consuming this grants you another health heart!", 18);
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
            player.AddMessage("You consume the bloodstone.");
            player.AddMessage("Max health increased!");
        }
    }

    // You need this key to win the game
    public class GoldenKey : Item {
        private GoldenKey(Texture2D glyph) : base("Golden Key", glyph, Color.Yellow) {
            SetType(Type.Key);
            ItemInfo = MainInterface.SplitMessage("You need this to unlock the exit to the dungeon.", 16);
            IsKey = true;
        }

        public static Texture2D KeyGlyph;
        private static GoldenKey Key;
        public static void LoadKey(ContentManager content) {
            KeyGlyph = content.Load<Texture2D>("Misc/Key");
            Key = new GoldenKey(KeyGlyph);
            Key.SetDescription("A large key made of solid gold. It is the key to victory and escape of the dungeon.");
        }
        public static GoldenKey GetKey() { return Key; }
    }
}
