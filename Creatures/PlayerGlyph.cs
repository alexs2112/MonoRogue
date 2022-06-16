using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {

    // A simple class to update the players glyph based on their equipment
    public class PlayerGlyph {

        // Store all combinations of glyphs. Priority is given to weapon, then armor
        // Default to null if the given [Weapon][Armor] doesnt exist
        /* Example
        * Unarmed
        *  - Unarmed with no armor
        * Dagger
        *  - Dagger with no armor
        * Sword
        *  - Sword with no armor
        */
        private static Dictionary<Item.Type, Dictionary<Item.Type, Texture2D>> glyphs;

        public static void LoadGlyphs(ContentManager content) {
            glyphs = new Dictionary<Item.Type, Dictionary<Item.Type, Texture2D>>();

            glyphs.Add(Item.Type.Null, new Dictionary<Item.Type, Texture2D>());
            glyphs[Item.Type.Null].Add(Item.Type.Null, content.Load<Texture2D>("Player/Null-Null"));

            glyphs.Add(Item.Type.Dagger, new Dictionary<Item.Type, Texture2D>());
            glyphs[Item.Type.Dagger].Add(Item.Type.Null, content.Load<Texture2D>("Player/Dagger-Null"));

            glyphs.Add(Item.Type.Sword, new Dictionary<Item.Type, Texture2D>());
            glyphs[Item.Type.Sword].Add(Item.Type.Null, content.Load<Texture2D>("Player/Sword-Null"));

            glyphs.Add(Item.Type.Bow, new Dictionary<Item.Type, Texture2D>());
            glyphs[Item.Type.Bow].Add(Item.Type.Null, content.Load<Texture2D>("Player/Bow-Null"));

            glyphs.Add(Item.Type.Axe, new Dictionary<Item.Type, Texture2D>());
            glyphs[Item.Type.Axe].Add(Item.Type.Null, content.Load<Texture2D>("Player/Axe-Null"));

            glyphs.Add(Item.Type.Mace, new Dictionary<Item.Type, Texture2D>());
            glyphs[Item.Type.Mace].Add(Item.Type.Null, content.Load<Texture2D>("Player/Mace-Null"));

            glyphs.Add(Item.Type.Spear, new Dictionary<Item.Type, Texture2D>());
            glyphs[Item.Type.Spear].Add(Item.Type.Null, content.Load<Texture2D>("Player/Spear-Null"));
        }

        public static Texture2D GetDefaultGlyph() {
            return glyphs[Item.Type.Null][Item.Type.Null];
        }

        public static Texture2D GetUpdatedGlyph(Creature player) {
            Item.Type weapon = Item.Type.Null;
            if (player.Weapon != null) { weapon = player.Weapon.ItemType; }
            Item.Type armor = Item.Type.Null;
            if (player.Armor != null) { armor = player.Armor.ItemType; }

            Dictionary<Item.Type, Texture2D> dict = GetWeaponDict(weapon);
            return GetGlyph(dict, armor);
        }

        private static Dictionary<Item.Type, Texture2D> GetWeaponDict(Item.Type type) {
            if (glyphs.ContainsKey(type)) { return glyphs[type]; } 
            else { return glyphs[Item.Type.Null]; }
        }
        private static Texture2D GetGlyph(Dictionary<Item.Type, Texture2D> dict, Item.Type type) {
            if (dict.ContainsKey(type)) { return dict[type]; }
            else { return dict[Item.Type.Null]; }
        }
    }
}
