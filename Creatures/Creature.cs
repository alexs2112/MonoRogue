using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Creature {
        public string Name { get; private set; }
        public Texture2D Glyph { get; private set; }
        public Color Color { get; private set; }
        public World World { get; set; }
        public CreatureAI AI { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Damage { get; private set; }
        public int Vision { get; private set; }

        public Creature(string name, Texture2D glyph, Color color) {
            Name = name;
            Glyph = glyph;
            Color = color;
        }

        public void SetStats(int hp, int damage) {
            MaxHP = hp;
            HP = hp;
            Damage = damage;
            Vision = 7;
        }

        public void ModifyHP(int value) {
            HP += value;
            if (HP <= 0) {
                NotifyOthers($"{Name} dies!");
            } else if (HP > MaxHP) {
                HP = MaxHP;
            }
        }
        public bool IsDead() { return HP <= 0; }

        public bool CanEnter(Point p) { return CanEnter(p.X, p.Y); }
        public bool CanEnter(int x, int y) { return World.IsFloor(x, y); }

        public bool MoveTo(Point p) { return MoveTo(p.X, p.Y); }
        public bool MoveTo(int x, int y) { 
            if (!CanEnter(x, y)) { return false; }

            Creature c = World.GetCreatureAt(x, y);
            if (c != null) {
                Attack(c);
            } else {
                X = x; 
                Y = y;
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
            if (AI != null) { AI.TakeTurn(world); }
        }

        public void Attack(Creature target) {
            Notify($"You attack {target.Name} for {Damage} damage!");
            NotifyOthers($"{Name} attacks {target.Name} for {Damage} damage!");
            target.ModifyHP(-Damage);
        }

        public void Notify(string message) { AI.AddMessage(message); }
        public void NotifyOthers(string message) {
            // Notify each creature that can see this one
            foreach (Creature c in World.Creatures) {
                if (c == this) { continue; }
                if (c.CanSee(X, Y)) { c.Notify(message); }
            }
        }
    }
}
