using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class HistoryScreen : BorderedScreen {
        private Subscreen LastScreen;
        private List<HistoryData> History;

        private int Index;
        private int TotalLines;

        public HistoryScreen(ContentManager content, Subscreen lastScreen) : base(content) {
            History = GameHistory.LoadHistory();
            LastScreen = lastScreen;

            TotalLines = (Constants.ScreenHeight - 164) / 32;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || key == Keys.Enter || key == Keys.Space || mouse.RightClicked()) {
                Main.Audio.SetSong(SongHandler.StartSong);
                return LastScreen;
            }
            else if (IsUp(key)) { if (Index > 0) { Index--; } }
            else if (IsDown(key)) { if (Index + TotalLines < History.Count) { Index++; } }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            WriteCentered(spriteBatch, Font.Get(24), "History", new Vector2(Constants.ScreenWidth / 2, 64), Color.White);

            int y = 114;
            int place = Constants.ScreenWidth /2 - 336;
            int score = place + 64;
            int time = Constants.ScreenWidth / 2;
            if (Constants.ScreenWidth < 1280) { time -= 32; }
            int date = time + 196;

            spriteBatch.DrawString(Font.Get(14), "#", new Vector2(place, y), Color.White);
            spriteBatch.DrawString(Font.Get(14), "Score", new Vector2(score, y), Color.White);
            spriteBatch.DrawString(Font.Get(14), "Time", new Vector2(time, y), Color.White);
            spriteBatch.DrawString(Font.Get(14), "Date", new Vector2(date, y), Color.White);
            y += 32;

            for (int i = 0; i < TotalLines; i++) {
                int j = i + Index;
                if (j >= History.Count) { break; }
                HistoryData d = History[j];
                spriteBatch.DrawString(Font.Get(14), $"{j + 1}", new Vector2(place, y), Color.LightGray);
                spriteBatch.DrawString(Font.Get(14), $"{d.Score}{(d.Victory ? '+' : ' ')}", new Vector2(score, y), Color.LightGray);
                spriteBatch.DrawString(Font.Get(14), $"{(new System.DateTime(d.Time)).ToString("HH:mm:ss")}", new Vector2(time, y), Color.LightGray);
                spriteBatch.DrawString(Font.Get(14), $"{(new System.DateTime(d.Date)).ToString("MM/dd/yyyy")}", new Vector2(date, y), Color.LightGray);
                y += 32;
            }
        }
    }
}


