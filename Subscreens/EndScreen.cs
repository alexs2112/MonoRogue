using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class EndScreen : BorderedScreen {
        private Main Main;
        private bool Victory;
        private HistoryData Game;
        private List<HistoryData> History;
        private int SectionIncrement;

        public EndScreen(Main main, ContentManager content, bool victory, HistoryData game, List<HistoryData> history) : base(content) {
            Main = main;
            Victory = victory;
            if (Victory) {
                Main.Audio.ChangeSong(SongHandler.VictorySong);
                EffectPlayer.PlaySoundEffect(EffectType.Victory);
            } else {
                Main.Audio.ChangeSong(SongHandler.DeathSong);
            }

            Game = game;
            History = history;

            SectionIncrement = Constants.ScreenHeight / 50;
            if (SectionIncrement > 96) { SectionIncrement = 96; }
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape) { Main.Exit(); }
            else if (key == Keys.Enter || key == Keys.Space) { return new StartScreen(Main, Content); }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            Vector2 v = new Vector2(Constants.ScreenWidth / 2, 96);

            string title = Victory ? "Congratulations!" : "Game Over";
            WriteCentered(spriteBatch, Font.Get(24), title, v, Color.White);
            v.Y += 48 + SectionIncrement;

            string t = $"The {Constants.GetPlayerName()} {(Victory ? "escaped the dungeon!" : "died a horrible death.")}";
            foreach (string s in Font.Size16.SplitString(t, Constants.ScreenWidth - 96)) {
                WriteCentered(spriteBatch, Font.Get(16), s, v, Color.White);
                v.Y += 24;
            }

            v.Y += SectionIncrement;
            v.Y = DrawHistory(spriteBatch, (int)v.Y);
            v.Y += SectionIncrement;

            WriteCentered(spriteBatch, Font.Get(16), "[space] to return to the main menu", v, Color.Gray);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "[esc] to exit", v, Color.Gray);

            v.Y = Constants.ScreenHeight - 64;
            WriteCentered(spriteBatch, Font.Get(14), $"Seed: {Main.Seed}", v, Color.Gray);
        }

        // Most of this is copied and pasted from the History Screen
        private int DrawHistory(SpriteBatch spriteBatch, int y) {
            int place = Constants.ScreenWidth / 2 - 336;
            int score = place + 64;
            int time = Constants.ScreenWidth / 2;
            if (Constants.ScreenWidth < 1280) { time -= 32; }
            int date = time + 196;

            spriteBatch.DrawString(Font.Get(14), "#", new Vector2(place, y), Color.White);
            spriteBatch.DrawString(Font.Get(14), "Score", new Vector2(score, y), Color.White);
            spriteBatch.DrawString(Font.Get(14), "Time", new Vector2(time, y), Color.White);
            spriteBatch.DrawString(Font.Get(14), "Date", new Vector2(date, y), Color.White);
            y += 32;

            int index = History.IndexOf(Game);
            int total = (Constants.ScreenHeight - y - 160) / 24 - 1;
            if (total < 2) { total = 2; }
            int start = index - total / 2 <= 0 ? 0 : index + total >= History.Count ? History.Count - 1 - total : index - total / 2;
            int end = start + total;
            for (int mi = start; mi <= end; mi++) {
                Color c = mi == index ? Color.LawnGreen : Color.LightGray;
                if (mi < 0 || mi >= History.Count) { continue; }
                HistoryData d = History[mi];
                spriteBatch.DrawString(Font.Get(14), $"{mi + 1}", new Vector2(place, y), c);
                spriteBatch.DrawString(Font.Get(14), $"{d.Score}", new Vector2(score, y), c);
                spriteBatch.DrawString(Font.Get(14), $"{(new System.DateTime(d.Time)).ToString("HH:mm:ss")}", new Vector2(time, y), c);
                spriteBatch.DrawString(Font.Get(14), $"{(new System.DateTime(d.Date)).ToString("dd/MM/yyyy")}", new Vector2(date, y), c);
                y += 24;
            }

            return y;
        }
    }
}
