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

        public static Item GetItemByName(string name, EquipmentFactory equipment) {
            switch(name) {
                // Misc
                case "Heartstone": return Heartstone.GetHeartstone();
                case "Golden Key": return GoldenKey.GetKey();

                // Food
                case "Apple": return Food.Apple;
                case "Cheese": return Food.Cheese;
                case "Meat": return Food.Meat;
                case "Fish": return Food.Fish;
                case "Pig Meat": return Food.PigMeat;

                // Weapons
                case "Dagger": return equipment.NewDagger();
                case "Club": return equipment.NewClub();
                case "Sword": return equipment.NewSword();
                case "Hand Axe": return equipment.NewHandAxe();
                case "Staff": return equipment.NewStaff();
                case "Shortbow": return equipment.NewShortbow();

                case "Broadknife": return equipment.NewBroadknife();
                case "Warhammer": return equipment.NewWarhammer();
                case "Falchion": return equipment.NewFalchion();
                case "War Axe": return equipment.NewWarAxe();
                case "Spear": return equipment.NewSpear();
                case "Longbow": return equipment.NewLongbow();
                
                case "Double Dagger": return equipment.NewDoubleDagger();
                case "Morningstar": return equipment.NewMorningstar();
                case "Greatsword": return equipment.NewGreatsword();
                case "Battle Axe": return equipment.NewBattleAxe();
                case "Greatspear": return equipment.NewGreatspear();
                case "Recurve Bow": return equipment.NewRecurveBow();

                // Armor
                case "Cloth Armor": return equipment.NewClothArmor();
                case "Ring Mail": return equipment.NewRingMail();
                case "Half Plate": return equipment.NewHalfPlate();
                case "Hide Armor": return equipment.NewHideArmor();
                case "Scale Mail": return equipment.NewScaleMail();
                case "Plate Armor": return equipment.NewPlateArmor();
                case "Leather Armor": return equipment.NewLeatherArmor();
                case "Chain Mail": return equipment.NewChainMail();
                case "Dragonscale": return equipment.NewDragonscale();

                // Enemy Equipment
                case "Bident": return equipment.NewBident();
                case "Sawtooth": return equipment.NewSawtooth();
                case "Cultist Staff": return equipment.NewCultistStaff();
                case "Ritual Dagger": return equipment.NewRitualDagger();
                case "Warden's Plate": return equipment.NewWardensPlate();

                default: throw new System.Exception($"Could not find {name}.");
            }
        }
    }

    // Specific type of item that increases your max health by 4 when consumed
    public class Heartstone : Item {
        private Heartstone(Texture2D glyph) : base("Heartstone", glyph, Color.Red) {
            SetType(Type.Heartstone);
            IsHeartstone = true;
            ItemInfo = Font.Size12.SplitString("Consuming this grants you another health heart!", MainInterface.InterfaceWidth - 16);
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
            player.Notify(new HeartstoneNotification(player));
        }
    }

    // You need this key to win the game
    public class GoldenKey : Item {
        private GoldenKey(Texture2D glyph) : base("Golden Key", glyph, Color.Yellow) {
            SetType(Type.Key);
            ItemInfo = Font.Size12.SplitString("A hefty key made of solid gold. It has a protective sheen to it and it glows with an internal warmth.", MainInterface.InterfaceWidth - 16);
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
