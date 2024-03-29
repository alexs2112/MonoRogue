using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Armor : Item {
        public int Strength { get; private set; }
        public int Defense;
        public int MaxDefense { get; private set; }
        private int Timer;
        public int Weight { get; private set; }
        public int Block { get; private set; }

        public Armor(string name, Texture2D glyph, Color color) : base(name, glyph, color) {
            IsArmor = true;
            IsWeapon = false;
        }

        public void SetArmorStats(int strength, int defense) { SetArmorStats(strength, defense, 0, 0); }
        public void SetArmorStats(int strength, int defense, int weight) { SetArmorStats(strength, defense, weight, 0); }
        public void SetArmorStats(int strength, int defense, int weight, int block) {
            Strength = strength;
            MaxDefense = defense;
            Defense = defense;
            Weight = weight;
            Block = block;
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
        public int Strength { get; private set; }
        public (int Min, int Max) Damage { get; private set; }
        public int Delay { get; private set; }
        public int Range { get; private set; }
        public int MaceDamage { get; private set; } // Bonus damage against defense
        public string AttackText { get; private set; }
        public Projectile.Type BaseProjectile { get; private set; }

        public Weapon(string name, Texture2D glyph, Color color) : base(name, glyph, color) {
            IsArmor = false;
            IsWeapon = true;
            Range = 1;
        }

        public void SetWeaponStats(int strength, (int, int) damage, int attackDelay = 10, int maceDamage = 0) {
            Strength = strength;
            Damage = damage;
            Delay = attackDelay;
            MaceDamage = maceDamage;
        }

        public void SetRange(int range) { Range = range; }
        public void SetAttackText(string s) { AttackText = s; }
        public void SetProjectileType(Projectile.Type type) { BaseProjectile = type; }
    }
}
