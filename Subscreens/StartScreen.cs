using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class StartScreen : BorderedScreen {
        private Main Main;
        private bool CanContinue;
        
        private int Index;
        /* Options:
        *  0: Continue (if possible)
        *  1: New Game
        *  2: Display
        *  3: Settings
        *  4: Help
        *  5: History
        *  6: Quit
        */
        
        public StartScreen(Main main, ContentManager content) : base(content) { 
            Main = main;
            CanContinue = GameLoader.CanLoad();
            if (!CanContinue) { Index = 1; }
            Main.Audio.SetSong(SongHandler.StartSong);
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            switch (key) {
                case Keys.Escape: Main.Exit(); break;
                case Keys.Space:
                case Keys.Enter:
                    EffectPlayer.PlaySoundEffect(EffectType.MenuSelect);
                    if (Index == 0) {
                        Main.CreateWorld(true);
                        base.CloseSubscreen();
                        return null;
                    } else if (Index == 1) {
                        return new NewGameScreen(Content, Main, this);
                    } else if (Index == 2) {
                        return new WindowResizeScreen(Content, Main, this);
                    } else if (Index == 3) {
                        return new GameSettingsScreen(Content, Main, this);
                    } else if (Index == 4) {
                        return new HelpMenuScreen(Content, this);
                    } else if (Index == 5) {
                        return new HistoryScreen(Content, this);
                    } else {
                        Main.Exit();
                        break;
                    }
            }
            if (IsUp(key)) { Decrement(); EffectPlayer.PlaySoundEffect(EffectType.MenuMove); }
            else if (IsDown(key)) { Increment(); EffectPlayer.PlaySoundEffect(EffectType.MenuMove); }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            int x = Constants.ScreenWidth / 2;
            int y = Constants.ScreenHeight / 2 - 112;
            y = WriteTitle(spriteBatch, x, y);
            WriteCentered(spriteBatch, Font.Get(16), "Continue", new Vector2(x, y), CanContinue ? Index == 0 ? Color.LawnGreen : Color.White : Color.Gray);
            y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "New Game", new Vector2(x, y), Index == 1 ? Color.LawnGreen : Color.White);
            y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Display", new Vector2(x, y), Index == 2 ? Color.LawnGreen : Color.White);
            y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Settings", new Vector2(x, y), Index == 3 ? Color.LawnGreen : Color.White);
            y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Help", new Vector2(x, y), Index == 4 ? Color.LawnGreen : Color.White);
            y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "History", new Vector2(x, y), Index == 5 ? Color.LawnGreen : Color.White);
            y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Exit", new Vector2(x, y), Index == 6 ? Color.LawnGreen : Color.White);
            y += 32;
        }

        private int WriteTitle(SpriteBatch spriteBatch, int x, int y) {
            string s = $"Escape of the {Constants.GetPlayerName()}";
            if (Font.Size24.Width * s.Length > Constants.ScreenWidth - 64) {
                WriteCentered(spriteBatch, Font.Get(24), "Escape of the", new Vector2(x, y - 48), Color.White);
                WriteCentered(spriteBatch, Font.Get(24), Constants.GetPlayerName(), new Vector2(x, y), Color.White);
            } else {
                WriteCentered(spriteBatch, Font.Get(24), s, new Vector2(x, y), Color.White);
            }
            y += 48;
            return y;
        }

        private void Decrement() {
            Index--;
            if (Index == 0 && !CanContinue) { Index = 6; }
            else if (Index < 0) { Index = 6; }
        }
        private void Increment() {
            Index++;
            if (Index > 6) {
                if (CanContinue) { Index = 0; }
                else { Index = 1; }
            }
        }
    }
}
