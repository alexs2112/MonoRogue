using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class StartScreen : BorderedScreen {
        private Main Main;
        private string Seed;
        private string SeedError;
        public StartScreen(Main main, ContentManager content) : base(content) { 
            Main = main;
            Seed = "";
            SeedError = "";
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            switch (key) {
                case Keys.Escape: Main.Exit(); break;
                case Keys.Enter:  
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
                    return null;
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
            int y = Constants.ScreenHeight / 2 - 64;
            WriteCentered(spriteBatch, Font24, "MonoRogue", new Vector2(x, y), Color.White);
            y += 48;
            WriteCentered(spriteBatch, Font14, "Press Enter to start", new Vector2(x, y), Color.White);
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
    }
}
