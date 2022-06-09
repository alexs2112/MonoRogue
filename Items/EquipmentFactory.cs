using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class EquipmentFactory {
        private Dictionary<string, Texture2D> Glyphs;

        public EquipmentFactory(ContentManager content) {
            Glyphs = new Dictionary<string, Texture2D>();
            
            // Store all loaded sprites in a dictionary so we don't need to store a ton of variables
            Glyphs.Add("Dagger", content.Load<Texture2D>("Equipment/Dagger"));
            Glyphs.Add("Sword", content.Load<Texture2D>("Equipment/Sword"));
        }

        public Weapon RandomWeapon(System.Random random) {
            int i = random.Next(3);
            if (i < 2) {
                return NewDagger();
            } else {
                return NewSword();
            }
        }
        public Weapon NewDagger() {
            Weapon w = new Weapon("Dagger", Glyphs["Dagger"], Color.LightGray);
            w.SetWeaponStats(1, 1);
            return w;
        }
        public Weapon NewSword() {
            Weapon w = new Weapon("Sword", Glyphs["Sword"], Color.LightGray);
            w.SetWeaponStats(2, 2);
            return w;
        }
    }
}
