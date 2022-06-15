using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class EquipmentFactory {
        private Dictionary<string, Texture2D> Glyphs;

        public EquipmentFactory(ContentManager content) {
            Glyphs = new Dictionary<string, Texture2D>();
            LastItem = LastItemType.NULL;
            
            // Store all loaded sprites in a dictionary so we don't need to store a ton of variables
            Glyphs.Add("Dagger", content.Load<Texture2D>("Equipment/Dagger"));
            Glyphs.Add("Sword", content.Load<Texture2D>("Equipment/Sword"));
            Glyphs.Add("Shortbow", content.Load<Texture2D>("Equipment/Shortbow"));
            Glyphs.Add("Leather Armor", content.Load<Texture2D>("Equipment/LeatherArmor"));
            Glyphs.Add("Cloth Armor", content.Load<Texture2D>("Equipment/ClothArmor"));
        }

        // Alternate between giving the player armor and weapons
        private enum LastItemType {
            NULL,
            WEAPON,
            ARMOR
        }
        private LastItemType LastItem;

        public Item WeakItem(System.Random random) {
            if (LastItem == LastItemType.NULL) {
                LastItem = random.Next(2) == 0 ? LastItemType.WEAPON : LastItemType.ARMOR;
            }
            if (LastItem == LastItemType.WEAPON) {
                LastItem = LastItemType.ARMOR;
                return WeakArmor(random);
            } else {
                LastItem = LastItemType.WEAPON;
                return WeakWeapon(random);
            }
        }
        public Item MediumItem(System.Random random) {
            if (LastItem == LastItemType.WEAPON) {
                LastItem = LastItemType.ARMOR;
                return MediumArmor(random);
            } else {
                LastItem = LastItemType.WEAPON;
                return MediumWeapon(random);
            }
        }
        public Item StrongItem(System.Random random) {
            if (LastItem == LastItemType.WEAPON) {
                LastItem = LastItemType.ARMOR;
                return StrongArmor(random);
            } else {
                LastItem = LastItemType.WEAPON;
                return StrongWeapon(random);
            }
        }

        public Weapon WeakWeapon(System.Random random) {
            switch (random.Next(1)) {
                case 0: return NewDagger();
                default: return null;
            }
        }
        public Weapon MediumWeapon(System.Random random) {
            switch (random.Next(2)) {
                case 0: return NewSword();
                case 1: return NewShortbow();
                default: return null;
            }
        }
        public Weapon StrongWeapon(System.Random random) {
            switch (random.Next(2)) {
                case 0: return NewSword();
                case 1: return NewShortbow();
                default: return null;
            }
        }

        public Armor WeakArmor(System.Random random) {
            switch (random.Next(1)) {
                case 0: return NewClothArmor();
                default: return null;
            }
        }
        public Armor MediumArmor(System.Random random) {
            switch (random.Next(2)) {
                case 0: return NewClothArmor();
                case 1: return NewLeatherArmor();
                default: return null;
            }
        }
        public Armor StrongArmor(System.Random random) {
            switch (random.Next(1)) {
                case 0: return NewLeatherArmor();
                default: return null;
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
            w.SetWeaponStats((0, 1), 5);
            w.SetRange(5);
            w.SetType(Item.Type.Bow);
            w.SetDescription("TEMP: A curved piece of wood and string, for shooting arrows. It does good damage in combat, and a skilled user can use it to great effect.");
            w.SetAttackText("shoot");
            return w;
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
