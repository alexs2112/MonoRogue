using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Creature {
        public int X { get; set; }
        public int Y { get; set; }
        public Texture2D Glyph { get; private set; }
        public Color Color { get; private set; }
        public World World { get; set; }

        public int Vision { get; private set; }

        public Creature(Texture2D glyph, Color color) {
            Glyph = glyph;
            Color = color;
            Vision = 9;
        }

        public bool MoveTo(int x, int y) { 
            if (World.IsWall(x, y)) { return false; }
            X = x; 
            Y = y;
            return true;
        }
        public bool MoveRelative(int dx, int dy) { 
            return MoveTo(X + dx, Y + dy);
        }

        public bool CanSee(int x, int y) {
            if ((X-x)*(X-x) + (Y-y)*(Y-y) > Vision * Vision) { return false; }

            foreach (Point p in World.GetLine(X, Y, x, y)) {
                if (World.IsFloor(p.X, p.Y) || (p.X == x && p.Y == y)) { continue; }

                return false;
            }
            return true;
        }
    }
}
