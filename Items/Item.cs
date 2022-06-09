using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {

    // Simple superclass to hold weapons, armors, foods
    public class Item {
        public string Name { get; private set; }
        public Texture2D Glyph { get; private set; }
        public Color Color { get; private set; }

        public bool IsArmor { get; protected set; }
        public bool IsWeapon { get; protected set; }
        public bool IsFood { get; protected set; }

        public Item(string name, Texture2D glyph, Color color) {
            Name = name;
            Glyph = glyph;
            Color = color;
        }
    }
}
