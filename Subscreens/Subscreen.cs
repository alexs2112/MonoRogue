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
    }
}
