using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class WinScreen : BorderedScreen {
        private Main Main;
        public WinScreen(Main main, ContentManager content) : base(content) {
            Main = main;
            Main.Audio.ChangeSong(SongHandler.VictorySong);
            EffectPlayer.PlaySoundEffect(EffectType.Victory);
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape) { Main.Exit(); }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            Vector2 v = new Vector2(Constants.ScreenWidth / 2, 128);
            WriteCentered(spriteBatch, Font.Get(24), "Congratulations!", v, Color.White);
            v.Y += 64;
            WriteCentered(spriteBatch, Font.Get(16), $"The {Constants.GetPlayerName()} escaped the dungeon!", v, Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Press [esc] to exit", v, Color.Gray);

            v.Y = Constants.ScreenHeight - 64;
            WriteCentered(spriteBatch, Font.Get(14), $"Seed: {Main.Seed}", v, Color.Gray);
        }
    }
}
