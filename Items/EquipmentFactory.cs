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
            Glyphs.Add("Club", content.Load<Texture2D>("Equipment/Club"));
            Glyphs.Add("Sword", content.Load<Texture2D>("Equipment/Sword"));
            Glyphs.Add("HandAxe", content.Load<Texture2D>("Equipment/HandAxe"));
            Glyphs.Add("Staff", content.Load<Texture2D>("Equipment/Staff"));
            Glyphs.Add("Shortbow", content.Load<Texture2D>("Equipment/Shortbow"));

            Glyphs.Add("Broadknife", content.Load<Texture2D>("Equipment/Broadknife"));
            Glyphs.Add("Warhammer", content.Load<Texture2D>("Equipment/Warhammer"));
            Glyphs.Add("Falchion", content.Load<Texture2D>("Equipment/Falchion"));
            Glyphs.Add("WarAxe", content.Load<Texture2D>("Equipment/WarAxe"));
            Glyphs.Add("Spear", content.Load<Texture2D>("Equipment/Spear"));
            Glyphs.Add("Longbow", content.Load<Texture2D>("Equipment/Longbow"));

            Glyphs.Add("DoubleDagger", content.Load<Texture2D>("Equipment/DoubleDagger"));
            Glyphs.Add("Morningstar", content.Load<Texture2D>("Equipment/Morningstar"));
            Glyphs.Add("Greatsword", content.Load<Texture2D>("Equipment/Greatsword"));
            Glyphs.Add("BattleAxe", content.Load<Texture2D>("Equipment/BattleAxe"));
            Glyphs.Add("Greatspear", content.Load<Texture2D>("Equipment/Greatspear"));
            Glyphs.Add("RecurveBow", content.Load<Texture2D>("Equipment/RecurveBow"));

            Glyphs.Add("LeatherArmor", content.Load<Texture2D>("Equipment/LeatherArmor"));
            Glyphs.Add("ClothArmor", content.Load<Texture2D>("Equipment/ClothArmor"));
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
            switch (random.Next(6)) {
                case 0: return NewDagger();
                case 1: return NewClub();
                case 2: return NewSword();
                case 3: return NewHandAxe();
                case 4: return NewStaff();
                case 5: return NewShortbow();
                default: return null;
            }
        }
        public Weapon MediumWeapon(System.Random random) {
            switch (random.Next(6)) {
                case 0: return NewBroadknife();
                case 1: return NewWarhammer();
                case 2: return NewFalchion();
                case 3: return NewWarAxe();
                case 4: return NewSpear();
                case 5: return NewLongbow();
                default: return null;
            }
        }
        public Weapon StrongWeapon(System.Random random) {
            switch (random.Next(6)) {
                case 0: return NewDoubleDagger();
                case 1: return NewMorningstar();
                case 2: return NewGreatsword();
                case 3: return NewBattleAxe();
                case 4: return NewGreatspear();
                case 5: return NewRecurveBow();
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
            Weapon w = new Weapon("Dagger", Glyphs["Dagger"], Color.SkyBlue);
            w.SetWeaponStats((2, 3), 6);
            w.SetType(Item.Type.Dagger);
            w.SetDescription("A short blade with a sharp point, particularly useful as a form of dispute resolution.");
            w.SetAttackText("stab");
            return w;
        }
        public Weapon NewClub() {
            Weapon w = new Weapon("Club", Glyphs["Club"], Color.IndianRed);
            w.SetWeaponStats((2, 4), 12);
            w.SetType(Item.Type.Mace);
            w.SetDescription("A makeshift weapon constructed from a piece of hardwood and the strength to swing it.");
            w.SetAttackText("club");
            return w;
        }
        public Weapon NewSword() {
            Weapon w = new Weapon("Sword", Glyphs["Sword"], Color.LightSeaGreen);
            w.SetWeaponStats((3, 5));
            w.SetType(Item.Type.Sword);
            w.SetDescription("A simple sword with a long, slashing blade.");
            w.SetAttackText("slash");
            return w;
        }
        public Weapon NewHandAxe() {
            Weapon w = new Weapon("Hand Axe", Glyphs["HandAxe"], Color.OliveDrab);
            w.SetWeaponStats((2, 4), 12);
            w.SetType(Item.Type.Axe);
            w.SetDescription("A small axe weighted towards a bladed head.");
            w.SetAttackText("hack");
            return w;
        }
        public Weapon NewStaff() {
            Weapon w = new Weapon("Staff", Glyphs["Staff"], Color.BurlyWood);
            w.SetWeaponStats((2, 4), 12);
            w.SetType(Item.Type.spear);
            w.SetDescription("A simple polearm composed of hardened and sturdy wood.");
            w.SetAttackText("jab");
            return w;
        }
        public Weapon NewShortbow() {
            Weapon w = new Weapon("Shortbow", Glyphs["Shortbow"], Color.Peru);
            w.SetWeaponStats((2, 3), 14);
            w.SetRange(4);
            w.SetType(Item.Type.Bow);
            w.SetDescription("A short, curved piece of wood and sinew designed for firing arrows.");
            w.SetAttackText("shoot");
            return w;
        }

        public Weapon NewBroadknife() {
            Weapon w = new Weapon("Broadknife", Glyphs["Broadknife"], Color.PowderBlue);
            w.SetWeaponStats((3, 5), 6);
            w.SetType(Item.Type.Dagger);
            w.SetDescription("A larger knife with a flat blade, broader at the hilt.");
            w.SetAttackText("stab");
            return w;
        }
        public Weapon NewWarhammer() {
            Weapon w = new Weapon("Warhammer", Glyphs["Warhammer"], Color.LightCoral);
            w.SetWeaponStats((3, 6));
            w.SetType(Item.Type.Mace);
            w.SetDescription("A large hammer, blunt and heavy that can strike with maximum impact.");
            return w;
        }
        public Weapon NewFalchion() {
            Weapon w = new Weapon("Falchion", Glyphs["Falchion"], Color.SteelBlue);
            w.SetWeaponStats((4, 7), 8);
            w.SetType(Item.Type.Sword);
            w.SetDescription("A long single-edged sword with a slightly curved blade. It is far lighter and sharper than a standard sword.");
            w.SetAttackText("slash");
            return w;
        }
        public Weapon NewWarAxe() {
            Weapon w = new Weapon("War Axe", Glyphs["WarAxe"], Color.Olive);
            w.SetWeaponStats((3, 6), 12);
            w.SetType(Item.Type.Axe);
            w.SetDescription("A bigger axe with a longer haft and a large, bladed head.");
            w.SetAttackText("hack");
            return w;
        }
        public Weapon NewSpear() {
            Weapon w = new Weapon("Spear", Glyphs["Spear"], Color.Wheat);
            w.SetWeaponStats((3, 6));
            w.SetType(Item.Type.Spear);
            w.SetDescription("A long wooden shaft with a sharpened metal head fastened on the end.");
            w.SetAttackText("jab");
            w.SetRange(2);
            return w;
        }
        public Weapon NewLongbow() {
            Weapon w = new Weapon("Longbow", Glyphs["Longbow"], Color.Sienna);
            w.SetWeaponStats((3, 5), 14);
            w.SetRange(6);
            w.SetType(Item.Type.Bow);
            w.SetDescription("A long, strong bow made of yew.");
            w.SetAttackText("shoot");
            return w;
        }

        public Weapon NewDoubleDagger() {
            Weapon w = new Weapon("Double Dagger", Glyphs["DoubleDagger"], Color.Turquoise);
            w.SetWeaponStats((4, 7), 6);
            w.SetType(Item.Type.Dagger);
            w.SetDescription("A dagger that has two adjacent blades, for double the stabbing potential.");
            w.SetAttackText("stab");
            return w;
        }
        public Weapon NewMorningstar() {
            Weapon w = new Weapon("Morningstar", Glyphs["Morningstar"], Color.MediumVioletRed);
            w.SetWeaponStats((4, 8));
            w.SetType(Item.Type.Mace);
            w.SetDescription("A large mace with the head covered in protruding spikes.");
            return w;
        }
        public Weapon NewGreatsword() {
            Weapon w = new Weapon("Greatsword", Glyphs["Greatsword"], Color.RoyalBlue);
            w.SetWeaponStats((5, 9), 12);
            w.SetType(Item.Type.Sword);
            w.SetDescription("An extremely long sword with an exceptionally heavy blade.");
            w.SetAttackText("slash");
            return w;
        }
        public Weapon NewBattleAxe() {
            Weapon w = new Weapon("Battle Axe", Glyphs["BattleAxe"], Color.LimeGreen);
            w.SetWeaponStats((4, 8), 12);
            w.SetType(Item.Type.Axe);
            w.SetDescription("A hefty axe with a large crescent shaped bladed head.");
            w.SetAttackText("hack");
            return w;
        }
        public Weapon NewGreatspear() {
            Weapon w = new Weapon("Greatspear", Glyphs["Greatspear"], Color.DarkSalmon);
            w.SetWeaponStats((4, 8), 12);
            w.SetType(Item.Type.Spear);
            w.SetDescription("A longer and heavier spear designed for war.");
            w.SetAttackText("jab");
            w.SetRange(2);
            return w;
        }
        public Weapon NewRecurveBow() {
            Weapon w = new Weapon("Recurve Bow", Glyphs["RecurveBow"], Color.SaddleBrown);
            w.SetWeaponStats((4, 7), 14);
            w.SetRange(6);
            w.SetType(Item.Type.Bow);
            w.SetDescription("A bow with limbs that curve away from the user, offering more power and speed to the fired arrow.");
            w.SetAttackText("shoot");
            return w;
        }

        public Armor NewClothArmor() {
            Armor a = new Armor("Cloth Armor", Glyphs["ClothArmor"], Color.LightBlue);
            a.SetArmorStats(4, 4);
            a.SetType(Item.Type.LightArmor);
            a.SetDescription("TEMP: A large, loose-fitting, wide-sleeved outer garment made of light cloth. It offers little protection against physical harm but does not hinder your movement.");
            return a;
        }
        public Armor NewLeatherArmor() {
            Armor a = new Armor("Leather Armor", Glyphs["LeatherArmor"], Color.SaddleBrown);
            a.SetArmorStats(8, 4);
            a.SetType(Item.Type.LightArmor);
            a.SetDescription("TEMP: A suit made from layers of tanned animal hide, this light armour provides basic protection with almost no hindrance to elaborate gestures or swift, stealthy movement.");
            return a;
        }
    }
}
