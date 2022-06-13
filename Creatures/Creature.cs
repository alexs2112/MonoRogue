using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Creature {
        public string Name { get; private set; }
        public string Description { get; private set; }
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

        // How many game ticks it takes to recover after taking an action, default 10
        public int TurnTimer { get; set; }
        private int MovementDelay { get; set; }
        private int AttackDelay { get; set; }

        public Armor Armor { get; set; }
        public Weapon Weapon { get; set; }

        public Creature(string name, Texture2D glyph, Color color) {
            Name = name;
            Glyph = glyph;
            Color = color;
        }

        public void SetStats(int hp, (int, int) damage) { SetStats(hp, damage, 10, 10); }
        public void SetStats(int hp, (int, int) damage, int movementDelay, int attackDelay) {
            MaxHP = hp;
            HP = hp;
            Damage = damage;
            Vision = 7;
            MovementDelay = movementDelay;
            AttackDelay = attackDelay;
        }

        // Private setters to avoid accidentally changing important attributes
        public void SetColor(Color color) { Color = color; }
        public void SetDescription(string s) { Description = s; }
        public void ModifyDamage(int amount) { ModifyDamage(amount, amount); }
        public void ModifyDamage(int min, int max) { Damage = (Damage.Min + min, Damage.Max + max); }
        public (int Min, int Max) GetDamage() {
            if (Weapon != null) {
                return (Damage.Min + Weapon.Damage.Min, Damage.Max + Weapon.Damage.Max);
            } else {
                return Damage;
            }
        }

        public int GetMovementDelay() {
            if (Armor != null) { return MovementDelay + Armor.MovementPenalty; }
            else { return MovementDelay; }
        }
        public int GetAttackDelay() {
            if (Weapon != null) { return AttackDelay + Weapon.AttackDelay; }
            else { return AttackDelay; }
        }
        public int GetRange() { return Weapon == null ? 1 : Weapon.Range; }
        
        public void ModifyHP(int value) {
            HP += value;
            if (HP <= 0) {
                NotifyOthers($"{Name} dies!");
                Notify("You die.");
                AI.OnDeath(World);
                World.Creatures.Remove(this);
                World.ColorOverlay[X, Y] = Color.DarkRed;
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
                TurnTimer = GetAttackDelay();
            } else {
                X = x; 
                Y = y;
                TurnTimer = GetMovementDelay();

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

        public List<Point> GetLineToPoint(int x, int y) { return GetLineToPoint(new Point(x, y)); }
        public List<Point> GetLineToPoint(Point p) {
            List<Point> line = World.GetLine(X, Y, p.X, p.Y);
            if (line.Count > 0) { line.RemoveAt(0); }
            return line;
        }

        public void TakeTurn(World world) {
            if (Armor != null) { Armor.Tick(); }
            if (AI != null) { AI.TakeTurn(world); }
        }

        public void Attack(Creature target) {
            int damage = new System.Random().Next(GetDamage().Min, GetDamage().Max + 1);
            string action;
            if (Weapon != null && Weapon.AttackText != null) {
                action = Weapon.AttackText;
            } else {
                action = "attack";
            }

            Notify($"You {action} {target.Name} for {damage} damage!");
            NotifyOthers($"{Name} {action}s {target.Name} for {damage} damage!");
            target.TakeDamage(damage);
            target.GetAttacked(this);
        }

        public void GetAttacked(Creature attacker) {
            AI.OnHit(World, attacker);
        }
        
        public Creature GetCreatureFromLine(List<Point> line) {
            // Return the first creature from a line of points, or null
            foreach (Point p in line) {
                Creature c = World.GetCreatureAt(p);
                if (c != null) { return c; }
            }
            return null;
        }
        public Creature GetCreatureInRange(Creature target) {
            // Return the first creature in a line to the target that is in range
            if (target == null) { return null; }
            Creature c = GetCreatureFromLine(GetLineToPoint(target.X, target.Y));
            if (c == null) { return null; }
            if (GetLineToPoint(c.X, c.Y).Count > GetRange()) { c = null; }
            return c;
        }

        public void Notify(string message) { AI.AddMessage(message); }
        public void NotifyOthers(string message) {
            // Notify each creature that can see this one
            foreach (Creature c in World.Creatures) {
                if (c == this) { continue; }
                if (c.CanSee(X, Y)) { c.Notify(message); }
            }
        }

        // OnlyPickup sets if we don't wait 10 ticks if there is nothing to pick up
        public void PickUp(bool OnlyPickup) { PickUp(new Point(X, Y), OnlyPickup); }
        public void PickUp(Point p, bool OnlyPickup) {
            Item i = World.GetItemAt(p);
            if (i == null && OnlyPickup) { 
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
                Notify($"You equip the {i.Name}.");
            } else if (i.IsWeapon) {
                Weapon weapon = (Weapon)i;
                Weapon temp = Weapon;
                Weapon = weapon;

                World.Items.Remove(p);
                if (temp != null) { World.Items.Add(p, temp); }

                Glyph = PlayerGlyph.GetUpdatedGlyph(this);
                Notify($"You equip the {i.Name}.");
            }
            TurnTimer = 10;
        }
    }
}
