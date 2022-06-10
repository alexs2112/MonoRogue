using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Armor : Item {
        public int Defense { get; set; }
        public int MaxDefense { get; private set; }
        private int Timer;
        public int Refresh { get; private set; }    // How many turns out of combat it takes to regenerate

        public int MovementPenalty { get; private set; }

        public Armor(string name, Texture2D glyph, Color color) : base(name, glyph, color) {
            IsArmor = true;
            IsWeapon = false;
        }

        public void SetArmorStats(int defense, int refresh) { SetArmorStats(defense, refresh, 0); }
        public void SetArmorStats(int defense, int refresh, int movementPenalty) {
            MaxDefense = defense;
            Defense = defense;
            Refresh = refresh;
            MovementPenalty = movementPenalty;
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
    }

    public class Weapon : Item {
        public (int Min, int Max) Damage { get; private set; }
        public int AttackDelay { get; private set; }

        public Weapon(string name, Texture2D glyph, Color color) : base(name, glyph, color) {
            IsArmor = false;
            IsWeapon = true;
        }

        public void SetWeaponStats((int, int) damage) {
            SetWeaponStats(damage, 0);
        }
        public void SetWeaponStats((int, int) damage, int attackDelay) {
            Damage = damage;
            AttackDelay = attackDelay;
        }
    }
}
