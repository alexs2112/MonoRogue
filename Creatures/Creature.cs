using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Creature {
        public string Name { get; private set; }
        public Texture2D Glyph { get; private set; }
        public Color Color { get; private set; }
        public World World { get; set; }
        public CreatureAI AI { get; set; }
        public bool IsPlayer { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Vision { get; private set; }
        public string Faction { get; set; }

        private (int Min, int Max) Damage { get; set; }

        public Armor Armor { get; set; }
        public Weapon Weapon { get; set; }

        public Creature(string name, Texture2D glyph, Color color) {
            Name = name;
            Glyph = glyph;
            Color = color;
        }

        public void SetStats(int hp, (int, int)damage) {
            MaxHP = hp;
            HP = hp;
            Damage = damage;
            Vision = 7;
        }

        // Private setters to avoid accidentally changing important attributes
        public void SetColor(Color color) { Color = color; }
        public void ModifyDamage(int amount) { ModifyDamage(amount, amount); }
        public void ModifyDamage(int min, int max) { Damage = (Damage.Min + min, Damage.Max + max); }
        public (int Min, int Max) GetDamage() {
            if (Weapon != null) {
                return (Damage.Min + Weapon.Damage.Min, Damage.Max + Weapon.Damage.Max);
            } else {
                return Damage;
            }
        }

        
        public void ModifyHP(int value) {
            HP += value;
            if (HP <= 0) {
                NotifyOthers($"{Name} dies!");
                Notify("You die.");
                AI.OnDeath(World);
            } else if (HP > MaxHP) {
                HP = MaxHP;
            }
        }
        public void TakeDamage(int value) {
            if (Armor != null) {
                if (Armor.Defense > value) {
                    Armor.Defense -= value;
                    value = 0;
                    return;
                } else {
                    value -= Armor.Defense;
                    Armor.Defense = 0;
                }
            }
            ModifyHP(-value);
        }
        public bool IsDead() { return HP <= 0; }

        public bool CanEnter(Point p) { return CanEnter(p.X, p.Y); }
        public bool CanEnter(int x, int y) { return World.IsFloor(x, y); }

        public bool MoveTo(Point p) { return MoveTo(p.X, p.Y); }
        public bool MoveTo(int x, int y) { 
            if (!CanEnter(x, y)) { return false; }

            Creature c = World.GetCreatureAt(x, y);
            if (c != null) {
                if (Faction != null && Faction == c.Faction) {
                    c.NotifyOthers($"The {Name} bumps into the {c.Name}.");
                } else {
                    Attack(c);
                }
            } else {
                X = x; 
                Y = y;

                Item i = World.GetItemAt(X, Y);
                if (i != null) { Notify($"You see here a {i.Name}."); }
            }
            return true;
        }
        public bool MoveRelative(int dx, int dy) { 
            return MoveTo(X + dx, Y + dy);
        }

        public bool CanSee(Point p) { return CanSee(p.X, p.Y); }
        public bool CanSee(int x, int y) {
            if ((X-x)*(X-x) + (Y-y)*(Y-y) > Vision * Vision) { return false; }

            foreach (Point p in World.GetLine(X, Y, x, y)) {
                if (World.IsFloor(p.X, p.Y) || (p.X == x && p.Y == y)) { continue; }

                return false;
            }
            return true;
        }

        public void TakeTurn(World world) {
            if (Armor != null) { Armor.Tick(); }
            if (AI != null) { AI.TakeTurn(world); }
        }

        public void Attack(Creature target) {
            int damage = new System.Random().Next(GetDamage().Min, GetDamage().Max + 1);
            Notify($"You attack {target.Name} for {damage} damage!");
            NotifyOthers($"{Name} attacks {target.Name} for {damage} damage!");
            target.TakeDamage(damage);
            target.GetAttacked(this);
        }

        public void GetAttacked(Creature attacker) {
            AI.OnHit(World, attacker);
        }

        public void Notify(string message) { AI.AddMessage(message); }
        public void NotifyOthers(string message) {
            // Notify each creature that can see this one
            foreach (Creature c in World.Creatures) {
                if (c == this) { continue; }
                if (c.CanSee(X, Y)) { c.Notify(message); }
            }
        }

        public void PickUp() { PickUp(new Point(X, Y)); }
        public void PickUp(Point p) {
            Item i = World.GetItemAt(p);
            if (i == null) { 
                Notify("There is nothing to pick up.");
                return;
            }

            if (i.IsFood) { World.EatFoodAt(this, p); }
            else if (i.IsArmor) {
                Armor armor = (Armor)i;
                Armor temp = Armor;
                Armor = armor;

                World.Items.Remove(p);
                if (temp != null) { World.Items.Add(p, temp); }

                Glyph = PlayerGlyph.GetUpdatedGlyph(this);
            } else if (i.IsWeapon) {
                Weapon weapon = (Weapon)i;
                Weapon temp = Weapon;
                Weapon = weapon;

                World.Items.Remove(p);
                if (temp != null) { World.Items.Add(p, temp); }

                Glyph = PlayerGlyph.GetUpdatedGlyph(this);
            }
        }
    }
}
