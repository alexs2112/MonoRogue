using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public abstract class Projectile {
        public List<(Point, Texture2D, Color)> Steps;
        protected Creature Attacker;
        protected Creature Target;
        public bool Finished;

        protected static long FrameLength = System.TimeSpan.TicksPerSecond / 30;
        private long Time;

        public Projectile(Creature attacker, Creature target) {
            Steps = new List<(Point, Texture2D, Color)>();
            Attacker = attacker;
            Target = target;
        }

        public static void LoadTextures(ContentManager content) {
            ArrowProjectile.Load(content);
            SpellProjectile.Load(content);
        }

        public void Draw(SpriteBatch spriteBatch, WorldView worldView) {
            if (Finished) { return; }

            (Point point, Texture2D glyph, Color color) = Steps[0];
            spriteBatch.Draw(glyph, new Vector2((point.X - worldView.OffsetX)*32, (point.Y - worldView.OffsetY)*32), color);
        }

        public void Update(System.TimeSpan elapsed) {
            if (Finished) { return; }

            Time += elapsed.Ticks;
            if (Time >= FrameLength) {
                Time = 0;
                Steps.RemoveAt(0);
                if (Steps.Count == 0) {
                    Finished = true;
                    End();
                }
            }
        }

        // Make the hit creature flash white when they are hit
        protected void AddHitFlash(Creature target) {
            Point p = new Point(target.X, target.Y);
            Steps.Add((p, target.Glyph, Color.White));
            Steps.Add((p, target.Glyph, Color.White));
        }
        
        private void End() {
            Attacker.FinishAttack(Target);
        }

        public void EndEarly() {
            Steps.Clear();
            Finished = true;
            End();
        }

        public enum Type {
            None,
            Arrow,
            Spell
        }

        public static Projectile GetProjectile(Creature attacker, Creature target, Type type) {
            switch(type) {
                case Type.Arrow:    return new ArrowProjectile(attacker, target);
                case Type.Spell:    return new SpellProjectile(attacker, target);
                default:            return new MeleeProjectile(attacker, target);
            }
        }
    }

    public class MeleeProjectile : Projectile {
        public MeleeProjectile(Creature attacker, Creature target) : base(attacker, target) {
            AddHitFlash(target);
        }
    }

    public class ArrowProjectile : Projectile {
        private static Texture2D ArrowVertical;
        private static Texture2D ArrowHorizontal;
        private static Texture2D ArrowDiagonal0;
        private static Texture2D ArrowDiagonal1;

        public ArrowProjectile(Creature attacker, Creature target) : base(attacker, target) {
            Point lastPoint = new Point(attacker.X, attacker.Y);
            List<Point> line = attacker.GetLineToPoint(target.X, target.Y);
            if (line.Count > 0) {
                line.RemoveAt(line.Count - 1);
                foreach (Point p in line) {
                    Steps.Add((p, GetFrame(lastPoint, p), Color.White));
                    lastPoint = p;
                }
            }
            AddHitFlash(target);
        }

        private static Texture2D GetFrame(Point last, Point p) {
            if (last.X != p.X && last.Y == p.Y) { return ArrowHorizontal; }
            else if (last.Y != p.Y && last.X == p.X) { return ArrowVertical; }
            else if (last.X < p.X && last.Y > p.Y) { return ArrowDiagonal0; }
            else if (last.X > p.X && last.Y < p.Y) { return ArrowDiagonal0; }
            else if (last.X < p.X && last.Y < p.Y) { return ArrowDiagonal1; }
            else if (last.X > p.X && last.Y > p.Y) { return ArrowDiagonal1; }
            else { return ArrowHorizontal; }
        }

        public static void Load(ContentManager content) {
            ArrowVertical = content.Load<Texture2D>("Projectiles/ArrowVertical");
            ArrowHorizontal = content.Load<Texture2D>("Projectiles/ArrowHorizontal");
            ArrowDiagonal0 = content.Load<Texture2D>("Projectiles/ArrowDiagonal0");
            ArrowDiagonal1 = content.Load<Texture2D>("Projectiles/ArrowDiagonal1");
        }
    }

    public class SpellProjectile : Projectile {
        private static List<Texture2D> Spells;
        private static List<Color> Colors;

        private Color Color;
        private Texture2D Spell;

        public SpellProjectile(Creature attacker, Creature target) : base(attacker, target) {
            Color = Colors[new System.Random().Next(Colors.Count)];
            Spell = Spells[new System.Random().Next(Spells.Count)];

            List<Point> line = attacker.GetLineToPoint(target.X, target.Y);
            if (line.Count > 0) {
                line.RemoveAt(line.Count - 1);
                foreach (Point p in line) {
                    Steps.Add((p, Spell, Color));
                }
            }
            AddHitFlash(target);
        }

        public static void Load(ContentManager content) {
            Spells = new List<Texture2D>();
            Spells.Add(content.Load<Texture2D>("Projectiles/Spell0"));
            Spells.Add(content.Load<Texture2D>("Projectiles/Spell1"));
            Spells.Add(content.Load<Texture2D>("Projectiles/Spell2"));
            Spells.Add(content.Load<Texture2D>("Projectiles/Spell3"));

            Colors = new List<Color>();
            Colors.Add(Color.Cyan);
            Colors.Add(Color.Chartreuse);
            Colors.Add(Color.Lime);
            Colors.Add(Color.Yellow);
            Colors.Add(Color.Red);
        }
    }
}
