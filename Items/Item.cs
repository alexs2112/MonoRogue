using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            
            LightArmor
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Texture2D Glyph { get; private set; }
        public Color Color { get; private set; }

        public bool IsArmor { get; protected set; }
        public bool IsWeapon { get; protected set; }
        public bool IsFood { get; protected set; }
        public Type ItemType { get; protected set; }
        public void SetType(Type itemType) { ItemType = itemType; }
        public void SetDescription(string s) { Description = s; }

        public Item(string name, Texture2D glyph, Color color) {
            Name = name;
            Glyph = glyph;
            Color = color;
        }
    }
}
