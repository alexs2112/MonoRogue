using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Armor : Item {
        public int Defense { get; set; }
        public int MaxDefense { get; private set; }
        private int Timer;
        private int Refresh;    // How many turns out of combat it takes to regenerate

        public Armor(string name, Texture2D glyph, Color color) : base(name, glyph, color) {
            IsArmor = true;
            IsWeapon = false;
        }

        public void SetArmorStats(int defense, int refresh) {
            MaxDefense = defense;
            Defense = defense;
            Refresh = refresh;
        }

        // Updates the armor for it to regenerate
        public void Tick() {
            if (Defense == MaxDefense) { Timer = 0; return; }

            Timer++;
            if (Timer >= Refresh) {
                Timer = 0;
                Defense++;
            }
        }
        
        // When you get hit, this should be called
        public void ResetTimer() { Timer = 0; }
    }

    public class Weapon : Item {
        public (int Min, int Max) Damage { get; private set; }

        public Weapon(string name, Texture2D glyph, Color color) : base(name, glyph, color) {
            IsArmor = false;
            IsWeapon = true;
        }

        public void SetWeaponStats(int minDamage, int maxDamage) {
            SetWeaponStats((minDamage, maxDamage));
        }
        public void SetWeaponStats((int, int) damage) {
            Damage = damage;
        }
    }
}
