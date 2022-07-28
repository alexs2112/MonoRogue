using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public abstract class Subscreen {
        public abstract Subscreen RespondToInput(Keys key, MouseHandler mouse);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler);

        protected static void WriteCentered(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 center, Color color) {
            Vector2 size = font.MeasureString(text);
            Vector2 origin = size * 0.5f;
            Vector2 place = new Vector2(center.X - origin.X, center.Y);
            spriteBatch.DrawString(font, text, place, color);
        }

        protected static void WriteRight(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 rightEdge, Color color) {
            Vector2 size = font.MeasureString(text);
            Vector2 origin = size;
            Vector2 place = new Vector2(rightEdge.X - origin.X, rightEdge.Y);
            spriteBatch.DrawString(font, text, place, color);
        }

        public static bool IsUp(Keys k) {
            return k == Keys.Up || k == Keys.K || k == Keys.NumPad8 || k == Keys.D8;
        }
        public static bool IsDown(Keys k) {
            return k == Keys.Down || k == Keys.J || k == Keys.NumPad2 || k == Keys.D2;
        }
        public static bool IsLeft(Keys k) {
            return k == Keys.Left || k == Keys.H || k == Keys.NumPad4 || k == Keys.D4;
        }
        public static bool IsRight(Keys k) {
            return k == Keys.Right || k == Keys.L || k == Keys.NumPad6 || k == Keys.D6;
        }

        public static bool IsNE(Keys k) {
            return k == Keys.U || k == Keys.NumPad9 || k == Keys.D9;
        }
        public static bool IsNW(Keys k) {
            return k == Keys.Y || k == Keys.NumPad7 || k == Keys.D7;
        }
        public static bool IsSW(Keys k) {
            return k == Keys.B || k == Keys.NumPad1 || k == Keys.D1;
        }
        public static bool IsSE(Keys k) {
            return k == Keys.N || k == Keys.NumPad3 || k == Keys.D3;
        }
    }
}
