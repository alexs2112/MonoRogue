using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace MonoRogue {
    public class GameSettingsScreen : BorderedScreen {
        private Subscreen LastScreen;
        private Main Main;

        private int Index;
        /*  Indices
        *   0: Back
        *   1: Color
        *   2: Gender
        *   3: Fast Mode
        *   4: Music Volume
        *   5: Effect Volume
        *   6: Defaults
        *   7: Save Changes
        */

        private int ColorIndex;
        private string Gender;
        private bool Animations;
        private float MusicVolume;
        private float EffectVolume;

        private string[] Tooltips;

        public  GameSettingsScreen(ContentManager content, Main main, Subscreen lastScreen) : base(content) {
            LastScreen = lastScreen;
            Main = main;

            ColorIndex = Constants.ColorIndex;
            Gender = Constants.Gender;
            Animations = Constants.AllowAnimations;
            MusicVolume = Constants.MusicVolume;
            EffectVolume = Constants.EffectVolume;

            Tooltips = new string[] {
                "",
                "The color of your character",
                "The gender of your character",
                "Disables minor attack animations",
                "Volume of background music",
                "Volume of sound effects",
                "Reset to defaults",
                ""
            };
        }
        
        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { return LastScreen; }
            else if (key == Keys.Down) { if (Index < 7) { Index++; } }
            else if (key == Keys.Up) { if (Index > 0) { Index--; } }
            else if (key == Keys.Left) {
                if (Index == 1) {
                    ColorIndex--;
                    if (ColorIndex < 0) { ColorIndex = Constants.Colors.Length - 1; }
                } else if (Index == 2) {
                    Gender = ChangeGender(false);
                } else if (Index == 3) {
                    Animations = !Animations;
                } else if (Index == 4) {
                    if (MusicVolume > 0) { MusicVolume -= 0.05f; UpdateAudio(); }
                } else if (Index == 5) {
                    if (EffectVolume > 0) { EffectVolume -= 0.05f; UpdateAudio(); }
                }
            } else if (key == Keys.Right || key == Keys.Enter) {
                if (Index == 1) {
                    ColorIndex++;
                    if (ColorIndex >= Constants.Colors.Length) { ColorIndex = 0; }
                } else if (Index == 2) {
                    Gender = ChangeGender(true);
                } else if (Index == 3) {
                    Animations = !Animations;
                } else if (Index == 4) {
                    if (MusicVolume < 0.5) { MusicVolume += 0.05f; UpdateAudio(); }
                } else if (Index == 5) {
                    if (EffectVolume < 0.5) { EffectVolume += 0.05f; UpdateAudio(); }
                } else if (key == Keys.Enter) {
                    if (Index == 0) { return LastScreen; }
                    else if (Index == 6) { ResetDefaults(); }
                    else if (Index == 7) { SaveChanges(); return LastScreen; }
                }
            }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            int x = Constants.ScreenWidth / 2;
            int y = Constants.ScreenHeight / 2 - 218;

            WriteCentered(spriteBatch, Font.Get(24), "Game Settings", new Vector2(x, y), Color.White);
            y += 64;

            WriteCentered(spriteBatch, Font.Get(16), "Back", new Vector2(x, y), Index == 0 ? Color.LawnGreen : Color.White);
            y += 32;

            WriteRight(spriteBatch, Font.Get(16), "Color:", new Vector2(x - 16, y), Index == 1 ? Color.LawnGreen : Color.White);
            spriteBatch.DrawString(Font.Get(16), Constants.ColorNames[ColorIndex], new Vector2(x + 16, y), Constants.Colors[ColorIndex]);
            y += 32;

            WriteRight(spriteBatch, Font.Get(16), "Gender:", new Vector2(x - 16, y), Index == 2 ? Color.LawnGreen : Color.White);
            spriteBatch.DrawString(Font.Get(16), Gender, new Vector2(x + 16, y), Color.White);
            y += 32;

            WriteRight(spriteBatch, Font.Get(16), "Fast Mode:", new Vector2(x - 16, y), Index == 3 ? Color.LawnGreen : Color.White);
            spriteBatch.DrawString(Font.Get(16), (!Animations).ToString(), new Vector2(x + 16, y), Color.White);
            y += 32;

            WriteRight(spriteBatch, Font.Get(16), "Music Volume:", new Vector2(x - 16, y), Index == 4 ? Color.LawnGreen : Color.White);
            spriteBatch.DrawString(Font.Get(16), GetDisplayVolume(MusicVolume).ToString(), new Vector2(x + 16, y), Color.White);
            y += 32;

            WriteRight(spriteBatch, Font.Get(16), "Effect Volume:", new Vector2(x - 16, y), Index == 5 ? Color.LawnGreen : Color.White);
            spriteBatch.DrawString(Font.Get(16), GetDisplayVolume(EffectVolume).ToString(), new Vector2(x + 16, y), Color.White);
            y += 32;

            WriteCentered(spriteBatch, Font.Get(16), "Defaults", new Vector2(x, y), Index == 6 ? Color.LawnGreen : Color.White);
            y += 32;

            WriteCentered(spriteBatch, Font.Get(16), "Save Changes", new Vector2(x, y), Index == 7 ? Color.LawnGreen : Color.White);
            y += 32;

            WriteCentered(spriteBatch, Font.Get(14), Tooltips[Index], new Vector2(x, Constants.ScreenHeight - 64), Color.Gray);
        }

        private int GetDisplayVolume(float f) {
            return (int)(f * 20);
        }

        // Only 3 genders, hardcode it
        private string ChangeGender(bool increment) {
            if (increment) {
                switch(Gender) {
                    case "Man": return "Woman";
                    case "Woman": return "Person";
                    default: return "Man";
                }
            } else {
                switch(Gender) {
                    case "Man": return "Person";
                    case "Woman": return "Man";
                    default: return "Woman";
                }
            }
        }

        private void ResetDefaults() {
            ColorIndex = 0;
            Gender = "Man";
            Animations = true;
            MusicVolume = 0.2f;
            EffectVolume = 0.2f;
            UpdateAudio();
        }

        private void SaveChanges() {
            Constants.ColorIndex = ColorIndex;
            Constants.Gender = Gender;
            Constants.AllowAnimations = Animations;
            Constants.MusicVolume = MusicVolume;
            Constants.EffectVolume = EffectVolume;
            UpdateAudio();
            Main.UpdatePlayer();
            Settings.SaveSettings();
        }

        private void UpdateAudio() {
            MediaPlayer.Volume = MusicVolume;
        }
    }
}
