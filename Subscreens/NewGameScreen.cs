using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class NewGameScreen : BorderedScreen {
        private Main Main;
        private Subscreen LastScreen;
        private string Seed;
        private string SeedError;
        private int DifficultyIndex;
        // 0: Hard, 1: Medium, 2: Easy

        private List<string>[] Texts;

        public NewGameScreen(ContentManager content, Main main, Subscreen lastScreen) : base(content) {
            Main = main;
            LastScreen = lastScreen;
            Seed = "";
            SeedError = "";

            Texts = new List<string>[3];

            Texts[0] = Font.Size16.SplitString(
                "The default difficulty. Offers a challenging dungeon crawling experience.",
                Constants.ScreenWidth - 64);
            
            Texts[1] = Font.Size16.SplitString(
                "Reduces the number of enemies in the dungeon and increases the amount of food and items you will come across.",
                Constants.ScreenWidth - 64);

            Texts[2] = Font.Size16.SplitString(
                "Has the fewest number of enemies and the greatest number of items. Gives you starting damage reduction.",
                Constants.ScreenWidth - 64);
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            switch(key) {
                case Keys.Escape: return LastScreen;
                case Keys.Space:
                case Keys.Enter:
                    EffectPlayer.PlaySoundEffect(EffectType.MenuSelect);
                    if (Seed.Length > 0) {
                        try {
                            Constants.Seed = System.Int32.Parse(Seed); 
                        } catch (System.OverflowException) {
                            SeedError = "Max seed value is 2147483647";
                            Seed = "";
                            break;
                        }
                    }
                    Constants.Difficulty = 3 - DifficultyIndex;
                    Main.CreateWorld();
                    base.CloseSubscreen();
                    return null;
                case Keys.H:
                case Keys.NumPad4:
                case Keys.Left: DifficultyIndex--; if (DifficultyIndex < 0) { DifficultyIndex = 2; } break;
                case Keys.L:
                case Keys.NumPad6:
                case Keys.Right: DifficultyIndex++; if (DifficultyIndex > 2) { DifficultyIndex = 0; } break; 
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
            WriteCentered(spriteBatch, Font.Get(24), $"New Game", new Vector2(x, y), Color.White);
            y += 48;
            WriteCentered(spriteBatch, Font.Get(16), "Hard", new Vector2(x - 256, y), DifficultyIndex == 0 ? Color.LawnGreen : Color.White);
            WriteCentered(spriteBatch, Font.Get(16), "Medium", new Vector2(x, y), DifficultyIndex == 1 ? Color.LawnGreen : Color.White);
            WriteCentered(spriteBatch, Font.Get(16), "Easy", new Vector2(x + 256, y), DifficultyIndex == 2 ? Color.LawnGreen : Color.White);

            y += 48;
            foreach(string s in Texts[DifficultyIndex]) {
                WriteCentered(spriteBatch, Font.Get(16), s, new Vector2(x, y), Color.LightGray);
                y += 32;
            }

            if (SeedError.Length > 0) {
                spriteBatch.DrawString(Font.Get(14), SeedError, new Vector2(32,32), Color.Gray);
            } else if (Seed.Length > 0) {
                spriteBatch.DrawString(Font.Get(14), "Seed: " + Seed, new Vector2(32,32), Color.White);
            } else {
                spriteBatch.DrawString(Font.Get(14), "Input numbers to set the seed.", new Vector2(32,32), Color.Gray);
            }
        }
    }
}
