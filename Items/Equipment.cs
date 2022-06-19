using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Armor : Item {
        public int Defense { get; set; }
        public int MaxDefense { get; private set; }
        private int Timer;
        public int Weight { get; private set; }

        public Armor(string name, Texture2D glyph, Color color) : base(name, glyph, color) {
            IsArmor = true;
            IsWeapon = false;
        }

        public void SetArmorStats(int defense) { SetArmorStats(defense, 0); }
        public void SetArmorStats(int defense, int weight) {
            MaxDefense = defense;
            Defense = defense;
            Weight = weight;
        }

        // Updates the armor for it to regenerate
        public void Tick() {
            if (Defense == MaxDefense) { Timer = 0; return; }

            Timer++;
            if (Timer >= 5) {
                Timer = 0;
                Defense++;
            }
        }

        // Whenever the owner is hit, reset, should not regen in combat
        public void ResetTimer() {
            Timer = 0;
        }
    }

    public class Weapon : Item {
        public (int Min, int Max) Damage { get; private set; }
        public int Delay { get; private set; }
        public int Range { get; private set; }
        public string AttackText { get; private set; }

        public Weapon(string name, Texture2D glyph, Color color) : base(name, glyph, color) {
            IsArmor = false;
            IsWeapon = true;
            Range = 1;
        }

        public void SetWeaponStats((int, int) damage) { SetWeaponStats(damage, 10); }
        public void SetWeaponStats((int, int) damage, int attackDelay) {
            Damage = damage;
            Delay = attackDelay;
        }

        public void SetRange(int range) { Range = range; }
        public void SetAttackText(string s) { AttackText = s; }
    }
}
