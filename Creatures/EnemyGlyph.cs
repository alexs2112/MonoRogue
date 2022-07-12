using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {

    // Storing the glyphs of enemies when they are wielding their secondary weapons
    public class EnemyGlyph {
        private static Dictionary<string, Texture2D> Glyphs;

        public static void LoadGlyphs(ContentManager content) {
            Glyphs = new Dictionary<string, Texture2D>();
            Glyphs.Add("Grunt-Dagger", content.Load<Texture2D>("Creatures/Grunt-Dagger"));
            Glyphs.Add("Grunt-Sword", content.Load<Texture2D>("Creatures/Grunt-Sword"));
            Glyphs.Add("Grunt-Bow", content.Load<Texture2D>("Creatures/Grunt-Bow"));

            Glyphs.Add("Thug-Sword", content.Load<Texture2D>("Creatures/Thug-Sword"));
            Glyphs.Add("Thug-Spear", content.Load<Texture2D>("Creatures/Thug-Spear"));

            Glyphs.Add("Brute-Mace", content.Load<Texture2D>("Creatures/Brute-Mace"));
            Glyphs.Add("Brute-Axe", content.Load<Texture2D>("Creatures/Brute-Axe"));

            Glyphs.Add("Gatekeeper-Axe", content.Load<Texture2D>("Creatures/Gatekeeper-Axe"));
            Glyphs.Add("Gatekeeper-Mace", content.Load<Texture2D>("Creatures/Gatekeeper-Mace"));
            Glyphs.Add("Gatekeeper-Sword", content.Load<Texture2D>("Creatures/Gatekeeper-Sword"));

            Glyphs.Add("Cultist-Spear", content.Load<Texture2D>("Creatures/Cultist-Spear"));
            Glyphs.Add("Cultist-Dagger", content.Load<Texture2D>("Creatures/Cultist-Dagger"));

            Glyphs.Add("Warden-Axe", content.Load<Texture2D>("Creatures/Warden-Axe"));
            Glyphs.Add("Warden-Mace", content.Load<Texture2D>("Creatures/Warden-Mace"));
            Glyphs.Add("Warden-Spear", content.Load<Texture2D>("Creatures/Warden-Spear"));
            Glyphs.Add("Warden-Sword", content.Load<Texture2D>("Creatures/Warden-Sword"));
        }

        public static Texture2D GetGlyph(Creature c) {
            if (c.Weapon == null) { return null; }

            string s = c.Name;
            s += "-";
            s += TypeToString(c.GetWeaponType());

            if (Glyphs.ContainsKey(s)) { return Glyphs[s]; }
            return null;
        }

        private static string TypeToString(Weapon.Type type) {
            switch(type) {
                case Weapon.Type.Dagger: return "Dagger";
                case Weapon.Type.Sword: return "Sword";
                case Weapon.Type.Mace: return "Mace";
                case Weapon.Type.Axe: return "Axe";
                case Weapon.Type.Spear: return "Spear";
                case Weapon.Type.Bow: return "Bow";
            }
            return "";
        }
    }
}
