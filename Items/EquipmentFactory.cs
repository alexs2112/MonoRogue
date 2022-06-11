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
            Glyphs.Add("Shortbow", content.Load<Texture2D>("Equipment/Shortbow"));
            Glyphs.Add("Leather Armor", content.Load<Texture2D>("Equipment/LeatherArmor"));
            Glyphs.Add("Cloth Armor", content.Load<Texture2D>("Equipment/ClothArmor"));
        }

        public Weapon RandomWeapon(System.Random random) {
            int i = random.Next(4);
            if (i < 2) {
                return NewDagger();
            } else if (i < 4) {
                return NewShortbow();
            } else {
                return NewSword();
            }
        }
        public Weapon NewDagger() {
            Weapon w = new Weapon("Dagger", Glyphs["Dagger"], Color.LightSteelBlue);
            w.SetWeaponStats((1, 1));
            w.SetType(Item.Type.Dagger);
            w.SetDescription("A double-edged fighting knife with a sharp point, particularly useful as a form of dispute resolution.");
            w.SetAttackText("stab");
            return w;
        }
        public Weapon NewSword() {
            Weapon w = new Weapon("Sword", Glyphs["Sword"], Color.AliceBlue);
            w.SetWeaponStats((2, 2), 4);
            w.SetType(Item.Type.Sword);
            w.SetDescription("TEMP: A simple sword with a long, slashing blade.");
            w.SetAttackText("slash");
            return w;
        }
        public Weapon NewShortbow() {
            Weapon w = new Weapon("Shortbow", Glyphs["Shortbow"], Color.SandyBrown);
            w.SetWeaponStats((1, 1), 5);
            w.SetRange(5);
            w.SetType(Item.Type.Bow);
            w.SetDescription("TEMP: A curved piece of wood and string, for shooting arrows. It does good damage in combat, and a skilled user can use it to great effect.");
            w.SetAttackText("shoot");
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
            Armor a = new Armor("Cloth Armor", Glyphs["Cloth Armor"], Color.LightBlue);
            a.SetArmorStats(4, 4);
            a.SetType(Item.Type.LightArmor);
            a.SetDescription("TEMP: A large, loose-fitting, wide-sleeved outer garment made of light cloth. It offers little protection against physical harm but does not hinder your movement.");
            return a;
        }
        public Armor NewLeatherArmor() {
            Armor a = new Armor("Leather Armor", Glyphs["Leather Armor"], Color.SaddleBrown);
            a.SetArmorStats(8, 6, 4);
            a.SetType(Item.Type.LightArmor);
            a.SetDescription("TEMP: A suit made from layers of tanned animal hide, this light armour provides basic protection with almost no hindrance to elaborate gestures or swift, stealthy movement.");
            return a;
        }
    }
}
