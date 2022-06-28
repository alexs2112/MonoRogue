using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class BorderedScreen : Subscreen {
        protected SpriteFont Font24;
        protected SpriteFont Font16;
        protected SpriteFont Font14;
        protected SpriteFont Font12;

        private Texture2D BorderCorners;
        private Texture2D BorderSides;

        // Maximum chars to fit on a screen with a starting X of 32, font 14
        protected static int MaxScreenChars = 40;

        public BorderedScreen(ContentManager content) { 
            Font24 = content.Load<SpriteFont>("Interface/sds24");
            Font16 = content.Load<SpriteFont>("Interface/sds16");
            Font14 = content.Load<SpriteFont>("Interface/sds14");
            Font12 = content.Load<SpriteFont>("Interface/sds12");

            BorderCorners = content.Load<Texture2D>("Interface/BorderCorners");
            BorderSides = content.Load<Texture2D>("Interface/BorderSides");

            Main.Audio.ChangeSong(SongHandler.MenuSong);
        }
        
        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { CloseSubscreen(); return null; }
            return this;
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            DrawBorder(spriteBatch);
        }

        private Rectangle NWCorner = new Rectangle(0,0,16,16);
        private Rectangle NECorner = new Rectangle(16,0,16,16);
        private Rectangle SWCorner = new Rectangle(0,16,16,16);
        private Rectangle SECorner = new Rectangle(16,16,16,16);
        private Rectangle TopSide = new Rectangle(0,0,32,16);
        private Rectangle BotSide = new Rectangle(0,16,32,16);
        private Rectangle RightSide = new Rectangle(32,0,16,32);
        private Rectangle LeftSide = new Rectangle(48,0,16,32);
        private void DrawBorder(SpriteBatch spriteBatch) {
            for (int i = 0; i < (Constants.ScreenWidth - 32) / 32 + 1; i++) {
                spriteBatch.Draw(BorderSides, new Vector2(i * 32, 0), TopSide, Color.LightGray);
                spriteBatch.Draw(BorderSides, new Vector2(i * 32, Constants.ScreenHeight - 16), BotSide, Color.LightGray);
            }
            for (int i = 0; i < (Constants.ScreenHeight - 32) / 32 + 1; i++) {
                spriteBatch.Draw(BorderSides, new Vector2(0, i * 32), LeftSide, Color.LightGray);
                spriteBatch.Draw(BorderSides, new Vector2(Constants.ScreenWidth - 16, i * 32), RightSide, Color.LightGray);
            }
            spriteBatch.Draw(BorderCorners, new Vector2(0,0), NWCorner, Color.LightGray);
            spriteBatch.Draw(BorderCorners, new Vector2(Constants.ScreenWidth - 16, 0), NECorner, Color.LightGray);
            spriteBatch.Draw(BorderCorners, new Vector2(0, Constants.ScreenHeight - 16), SWCorner, Color.LightGray);
            spriteBatch.Draw(BorderCorners, new Vector2(Constants.ScreenWidth - 16, Constants.ScreenHeight - 16), SECorner, Color.LightGray);
        }

        public void CloseSubscreen() {
            Main.Audio.ChangeSong(null);
        }
    }
}
