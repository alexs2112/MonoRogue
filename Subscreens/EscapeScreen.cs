using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class EscapeScreen : BorderedScreen {
        private Main Main;
        private int Index;
        public EscapeScreen(Main main, ContentManager content) : base(content) { 
            Main = main;
            Index = 0;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape) { base.CloseSubscreen(); return null; }
            else if (key == Keys.Enter || key == Keys.Space) {
                if (Index > -1 && Index < 5) { EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); }
                if (Index == 0) { base.CloseSubscreen(); return null; }
                else if (Index == 1) { return new WindowResizeScreen(Content, Main, this); }
                else if (Index == 2) { return new GameSettingsScreen(Content, Main, this); }
                else if (Index == 3) { return new HelpMenuScreen(Content, this); }
                else if (Index == 4) {
                    Main.SaveGame();
                    Main.Exit();
                }
            }
            else if (IsUp(key)) { 
                if (Index > 0) { Index--; EffectPlayer.PlaySoundEffect(EffectType.MenuMove); }
                else { Index = 4; }
            }
            else if (IsDown(key)) { 
                if (Index < 4) { Index++; EffectPlayer.PlaySoundEffect(EffectType.MenuMove); }
                else { Index = 0; }
            }
            return this;
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            Vector2 v = new Vector2(Constants.ScreenWidth / 2, 128);
            WriteCentered(spriteBatch, Font.Get(24), "Paused", v, Color.White);
            v.Y += 64;
            WriteCentered(spriteBatch, Font.Get(16), "Continue", v, Index == 0 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Display", v, Index == 1 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Settings", v, Index == 2 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Help", v, Index == 3 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Save and Quit", v, Index == 4 ? Color.LawnGreen : Color.White);

            v.Y = Constants.ScreenHeight - 64;
            WriteCentered(spriteBatch, Font.Get(14), $"Seed: {Main.Seed}", v, Color.Gray);

            v = new Vector2(64, 64);
            spriteBatch.DrawString(Font.Get(14), $"{new System.DateTime(Main.Time.Ticks).ToString("HH:mm:ss")}", v, Color.Gray);
        }
    }
}
