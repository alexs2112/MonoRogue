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
            Glyphs.Add("Leather Armor", content.Load<Texture2D>("Equipment/LeatherArmor"));
            Glyphs.Add("Cloth Armor", content.Load<Texture2D>("Equipment/ClothArmor"));
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

        public Armor RandomArmor(System.Random random) {
            int i = random.Next(3);
            if (i < 2) {
                return NewClothArmor();
            } else {
                return NewLeatherArmor();
            }
        }
        public Armor NewClothArmor() {
            Armor a = new Armor("Cloth Armor", Glyphs["Cloth Armor"], Color.AliceBlue);
            a.SetArmorStats(4, 4);
            return a;
        }
        public Armor NewLeatherArmor() {
            Armor a = new Armor("Leather Armor", Glyphs["Leather Armor"], Color.SandyBrown);
            a.SetArmorStats(8, 6);
            return a;
        }
    }
}
