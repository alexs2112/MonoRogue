using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class StartScreen : BorderedScreen {
        private Main Main;
        private ContentManager Content;
        private string Seed;
        private string SeedError;
        private bool CanContinue;
        
        private int Index;
        /* Options:
        *  0: Continue (if possible)
        *  1: New Game
        *  2: Settings
        *  3: Help
        *  4: Quit
        */
        
        public StartScreen(Main main, ContentManager content) : base(content) { 
            Main = main;
            Seed = "";
            SeedError = "";
            Content = content;
            CanContinue = GameLoader.CanLoad();
            if (!CanContinue) { Index = 1; }
            Main.Audio.SetSong(SongHandler.StartSong);
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            switch (key) {
                case Keys.Escape: Main.Exit(); break;
                case Keys.Enter:
                    if (Index == 0) {
                        Main.CreateWorld(true);
                        base.CloseSubscreen();
                        return null;
                    } else if (Index == 1) {
                        if (Seed.Length > 0) {
                            try {
                                Constants.Seed = System.Int32.Parse(Seed); 
                            } catch (System.OverflowException) {
                                SeedError = "Max seed value is 2147483647";
                                Seed = "";
                                break;
                            }
                        }
                        Main.CreateWorld();
                        base.CloseSubscreen();
                        return null;
                    } else if (Index == 2) {
                        return new WindowResizeScreen(Content, Main, this);
                    } else if (Index == 3) {
                        return new HelpScreen(Content, this);
                    } else {
                        Main.Exit();
                        break;
                    }
                case Keys.Up: Decrement(); break;
                case Keys.Down: Increment(); break;
                case Keys.D0: AddSeedChar('0'); break;
                case Keys.D1: AddSeedChar('1'); break;
                case Keys.D2: AddSeedChar('2'); break;
                case Keys.D3: AddSeedChar('3'); break;
                case Keys.D4: AddSeedChar('4'); break;
                case Keys.D5: AddSeedChar('5'); break;
                case Keys.D6: AddSeedChar('6'); break;
                case Keys.D7: AddSeedChar('7'); break;
                case Keys.D8: AddSeedChar('8'); break;
                case Keys.D9: AddSeedChar('9'); break;
                case Keys.Back:
                    if (Seed.Length > 0) { Seed = Seed.Remove(Seed.Length - 1); }
                    break;
            }
            return this;
        }

        private void AddSeedChar(char c) {
            if (SeedError.Length > 0) { SeedError = ""; }
            Seed += c;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            int x = Constants.ScreenWidth / 2;
            int y = Constants.ScreenHeight / 2 - 96;
            WriteCentered(spriteBatch, Font24, "Escape of the Blue Man", new Vector2(x, y), Color.White);
            y += 48;
            WriteCentered(spriteBatch, Font16, "Continue", new Vector2(x, y), CanContinue ? Index == 0 ? Color.LawnGreen : Color.White : Color.Gray);
            y += 32;
            WriteCentered(spriteBatch, Font16, "New Game", new Vector2(x, y), Index == 1 ? Color.LawnGreen : Color.White);
            y += 32;
            WriteCentered(spriteBatch, Font16, "Settings", new Vector2(x, y), Index == 2 ? Color.LawnGreen : Color.White);
            y += 32;
            WriteCentered(spriteBatch, Font16, "Help", new Vector2(x, y), Index == 3 ? Color.LawnGreen : Color.White);
            y += 32;
            WriteCentered(spriteBatch, Font16, "Exit", new Vector2(x, y), Index == 4 ? Color.LawnGreen : Color.White);
            y += 32;

            y = Constants.ScreenHeight - 64;
            WriteCentered(spriteBatch, Font14, "Press [?] in game for help", new Vector2(x, y), Color.Gray);

            if (SeedError.Length > 0) {
                spriteBatch.DrawString(Font14, SeedError, new Vector2(32,32), Color.Gray);
            } else if (Seed.Length > 0) {
                spriteBatch.DrawString(Font14, "Seed: " + Seed, new Vector2(32,32), Color.White);
            } else {
                spriteBatch.DrawString(Font14, "Input numbers to set the seed.", new Vector2(32,32), Color.Gray);
            }
        }

        private void Decrement() {
            Index--;
            if (Index == 0 && !CanContinue) { Index = 4; }
            else if (Index < 0) { Index = 4; }
        }
        private void Increment() {
            Index++;
            if (Index > 4) {
                if (CanContinue) { Index = 0; }
                else { Index = 1; }
            }
        }
    }
}
