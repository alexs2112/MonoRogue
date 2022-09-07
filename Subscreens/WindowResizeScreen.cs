using System.Collections.Generic;
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
        // 1 Screen Resolution
        // 2 Fullscreen
        // 3 Reset
        // 4 Save Changes

        private int Width;
        private int Height;
        private int ResolutionIndex;
        private bool Fullscreen;

        private (int Width, int Height)[] Resolutions;

        public WindowResizeScreen(ContentManager content, Main main, Subscreen lastScreen) : base(content) {
            Main = main;
            LastScreen = lastScreen;
            Resolutions = new (int, int)[] { (800, 480), (1280, 720), (1366, 768), (1600, 900), (1920, 1080) };

            Fullscreen = Constants.Fullscreen;

            ResolutionIndex = LoadResolutionIndex();
            SetLocalDimensions();
        }

        // Get the resolution index from Constants
        private int LoadResolutionIndex() {
            for (int i = 0; i < Resolutions.Length; i++) {
                if ((Constants.ScreenWidth, Constants.ScreenHeight) == Resolutions[i]) {
                    return i;
                }
            }
            return 0;
        }

        private void SetLocalDimensions() {
            (Width, Height) = Resolutions[ResolutionIndex];
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { return LastScreen; }
            else if (IsDown(key)) {
                if (Index < 4) { Index++; EffectPlayer.PlaySoundEffect(EffectType.MenuMove); }
                else { Index = 0; }
            }
            else if (IsUp(key)) {
                if (Index > 0) { Index--; EffectPlayer.PlaySoundEffect(EffectType.MenuMove); }
                else { Index = 4; }
            }
            else if (IsLeft(key)) { if (Index == 1 && ResolutionIndex > 0) { ResolutionIndex--; EffectPlayer.PlaySoundEffect(EffectType.MenuMove); }}
            else if (IsRight(key)) { if (Index == 1 && ResolutionIndex < Resolutions.Length - 1) { ResolutionIndex++; EffectPlayer.PlaySoundEffect(EffectType.MenuMove); }}
            else if (key == Keys.Enter || key == Keys.Space) {
                if (Index == 0) { EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); return LastScreen; }
                else if (Index == 1) { EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); SetLocalDimensions(); }
                else if (Index == 2) { EffectPlayer.PlaySoundEffect(EffectType.MenuMove); Fullscreen = !Fullscreen; }
                else if (Index == 3) { EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); ResolutionIndex = LoadResolutionIndex(); SetLocalDimensions(); Fullscreen = Constants.Fullscreen; }
                else if (Index == 4) {
                    EffectPlayer.PlaySoundEffect(EffectType.MenuSelect);
                    SetLocalDimensions();
                    Constants.Fullscreen = Fullscreen;
                    if (Constants.Fullscreen) {
                        Constants.ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                        Constants.ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    }  else {
                        Constants.ScreenWidth = Width;
                        Constants.ScreenHeight = Height;
                    }
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
            int y = Constants.ScreenHeight / 2 - 154;
            Vector2 pos = new Vector2(x, y);

            WriteCentered(spriteBatch, Font.Get(24), "Display Settings", pos, Color.White);
            pos.Y += 64;

            WriteCentered(spriteBatch, Font.Get(16), "Back", pos, Index == 0 ? Color.LawnGreen : Color.White);
            pos.Y += 48;

            WriteCentered(spriteBatch, Font.Get(16), FormatDimension(ResolutionIndex), pos, Index == 1 ? Color.LawnGreen : Color.White);
            if (ResolutionIndex > 0) {
                WriteCentered(spriteBatch, Font.Get(16), FormatDimension(ResolutionIndex - 1), new Vector2(pos.X - 240, pos.Y), Color.LightGray);
            }
            if (ResolutionIndex > 1) {
                spriteBatch.DrawString(Font.Get(16), "<", new Vector2(pos.X - 360, pos.Y), Color.White);
            }
            if (ResolutionIndex < Resolutions.Length - 1) {
                WriteCentered(spriteBatch, Font.Get(16), FormatDimension(ResolutionIndex + 1), new Vector2(pos.X + 240, pos.Y), Color.LightGray);
            }
            if (ResolutionIndex < Resolutions.Length - 2) {
                spriteBatch.DrawString(Font.Get(16), ">", new Vector2(pos.X + 320, pos.Y), Color.White);
            }
            pos.Y += 48;

            WriteCentered(spriteBatch, Font.Get(16), $"Fullscreen: {Fullscreen}", pos, Index == 2 ? Color.LawnGreen : Color.White);
            pos.Y += 48;

            WriteCentered(spriteBatch, Font.Get(16), "Reset", pos, Index == 3 ? Color.LawnGreen : Color.White);
            pos.Y += 48;

            WriteCentered(spriteBatch, Font.Get(16), "Continue", pos, Index == 4 ? Color.LawnGreen : Color.White);
            pos.Y += 48;

            if (Fullscreen) {
                List<string> warning = Font.Size14.SplitString("Note: Fullscreen will overwrite resolution to fit your screen.", Constants.ScreenWidth - 64);
                pos.Y = Constants.ScreenHeight - 64 - 32 * warning.Count;
                if (Constants.ScreenHeight < 1000) { pos.Y += 32; }
                foreach (string s in warning) {
                    WriteCentered(spriteBatch, Font.Get(14), s, pos, Color.Gray);
                    pos.Y += 32;
                }
            }
        }

        private string FormatDimension(int index) {
            try {
                return FormatDimension(Resolutions[index].Width, Resolutions[index].Height);
            } catch {
                throw new System.Exception(ResolutionIndex.ToString());
            }
        }
        private string FormatDimension(int width, int height) {
            string s;
            if (width < 1000) { s = $" {width}"; }
            else { s = $"{width}"; }

            s += "x";

            if (height < 1000) { s += $"{height} "; }
            else { s += $"{height}"; }

            return s;
        }
    }
}
