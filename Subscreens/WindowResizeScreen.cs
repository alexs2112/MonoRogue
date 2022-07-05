using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class WindowResizeScreen : BorderedScreen {
        private Main Main;
        private Subscreen LastScreen;
        private int Index;
        // Indices
        // 0 Back
        // 1 Preset Size:
        // 2 <preset selection list>
        // 3 Custom Size:
        // 4 <custom width>
        // 5 <custom height>
        // 6 Fullscreen
        // 7 Reset
        // 8 Save Changes

        private int Width;
        private int Height;
        private int ResolutionIndex;
        private bool UsePreset;
        private bool Fullscreen;

        private (int Width, int Height)[] Resolutions;

        public WindowResizeScreen(ContentManager content, Main main, Subscreen lastScreen) : base(content) {
            Main = main;
            LastScreen = lastScreen;
            Resolutions = new (int, int)[] { (800, 480), (1280, 800), (1920, 1056) };

            Fullscreen = Constants.Fullscreen;
            
            for (int i = 0; i < Resolutions.Length; i++) {
                if ((Constants.ScreenWidth, Constants.ScreenHeight) == Resolutions[i]) {
                    ResolutionIndex = i;
                    UsePreset = true;
                }
            }
            Width = Constants.ScreenWidth / 32 - 10;
            Height = Constants.ScreenHeight / 32;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { return LastScreen; }
            else if (key == Keys.Down) {
                if (Index == 0) {
                    if (UsePreset) { Index += 2; }
                    else { Index++; }
                } else if (Index == 1) {
                    if (UsePreset) { Index++; }
                    else { Index += 2; }
                } else if (Index == 3) {
                    if (UsePreset) { Index = 6; }
                    else { Index++; }
                } else if (Index == 8) {
                    // Do nothing
                } else {
                    Index++;
                }
            } else if (key == Keys.Up) {
                if (Index == 0) {
                    // Do nothing
                } else if (Index == 2) {
                    if (UsePreset) { Index = 0; }
                    else { Index--; }
                } else if (Index == 3) {
                    if (UsePreset) { Index--; }
                    else { Index -= 2; }
                } else if (Index == 4) {
                    if (UsePreset) { Index--; }
                    else { Index -= 2; }
                } else if (Index == 6) {
                    if (UsePreset) { Index = 3; }
                    else { Index--; }
                } else {
                    Index--;
                }
            } else if (key == Keys.Right) {
                if (Index == 2) {
                    if (ResolutionIndex == Resolutions.Length - 1) {
                        ResolutionIndex = 0;
                    } else {
                        ResolutionIndex++;
                    }
                } else if (Index == 4) {
                    if (Width < 50) { Width++; }
                } else if (Index == 5) {
                    if (Height < 33) { Height++; }
                }
            } else if (key == Keys.Left) {
                if (Index == 2) {
                    if (ResolutionIndex == 0) {
                        ResolutionIndex = Resolutions.Length - 1;
                    } else {
                        ResolutionIndex--;
                    }
                } else if (Index == 4) {
                    if (Width > 15) { Width--; }
                } else if (Index == 5) {
                    if (Height > 15) { Height--; }
                }
            } else if (key == Keys.Enter || key == Keys.Space) {
                if (Index == 0) { return LastScreen; }
                else if (Index == 1) { UsePreset = true; }
                else if (Index == 2) {
                    Width = Resolutions[ResolutionIndex].Width / 32 - 10;
                    Height = Resolutions[ResolutionIndex].Height / 32;
                    Index++;
                } else if (Index == 3) { UsePreset = false; }
                else if (Index == 6) { Fullscreen = !Fullscreen; }
                else if (Index == 7) { Width = 15; Height = 15; Fullscreen = false; }
                else if (Index == 8) {
                    (int width, int height) next = GetResult();
                    Constants.ScreenWidth = next.width;
                    Constants.ScreenHeight = next.height;
                    Constants.WorldViewWidth = Width;
                    Constants.WorldViewHeight = Height;
                    Constants.Fullscreen = Fullscreen;
                    Main.UpdateScreenSize();
                    Settings.SaveSettings();
                    return LastScreen;
                }
            }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);
            int x = Constants.ScreenWidth / 2;
            int y = Constants.ScreenHeight / 2 - 218;
            Vector2 pos = new Vector2(x, y);

            (int Width, int Height) result = GetResult();
            WriteCentered(spriteBatch, Font24, $"Window Size: {result.Width}x{result.Height}", pos, Color.White);
            pos.Y += 64;

            WriteCentered(spriteBatch, Font16, $"Back", pos, Index == 0 ? Color.LawnGreen : Color.White);
            pos.Y += 48;

            WriteCentered(spriteBatch, Font16, $"Use Preset", pos, Index == 1 ? Color.LawnGreen : UsePreset ? Color.White : Color.Gray);
            pos.Y += 32;

            Vector2 posPreset = new Vector2(pos.X - (Resolutions.Length / 2) * 192 - 85, pos.Y);
            for (int i = 0; i < Resolutions.Length; i++) {
                (int Width, int Height) r = Resolutions[i];
                string w;
                string h;
                if (r.Width < 1000) { w = $" {r.Width}"; }
                else { w = $"{r.Width}"; }
                if (r.Height < 1000) { h = $"{r.Height} "; }
                else { h = $"{r.Height}"; }

                spriteBatch.DrawString(Font14, $"{w}x{h}", posPreset, UsePreset ? (ResolutionIndex == i && Index == 2 ? Color.LawnGreen : Color.White) : Color.Gray);
                posPreset.X += 192;
            }

            pos.Y += 48;
            WriteCentered(spriteBatch, Font16, $"Use Custom", pos, Index == 3 ? Color.LawnGreen : UsePreset ? Color.Gray : Color.White);
            pos.Y += 32;
            WriteCentered(spriteBatch, Font14, $"Width: {Width}", pos, UsePreset ? Color.Gray : Index == 4 ? Color.LawnGreen : Color.White);
            pos.Y += 32;
            WriteCentered(spriteBatch, Font14, $"Height: {Height}", pos, UsePreset ? Color.Gray : Index == 5 ? Color.LawnGreen : Color.White);

            pos.Y += 48;
            WriteCentered(spriteBatch, Font16, $"Fullscreen: {Fullscreen}", pos, Index == 6 ? Color.LawnGreen : Color.White);

            pos.Y += 48;
            WriteCentered(spriteBatch, Font16, "Reset", pos, Index == 7 ? Color.LawnGreen : Color.White);

            pos.Y += 48;
            WriteCentered(spriteBatch, Font16, "Continue", pos, Index == 8 ? Color.LawnGreen : Color.White);
        }

        private (int Width, int Height) GetResult() {
            if (UsePreset) {
                return Resolutions[ResolutionIndex];
            } else {
                return ((Width + 10) * 32, Height * 32);
            }
        }
    }
}
