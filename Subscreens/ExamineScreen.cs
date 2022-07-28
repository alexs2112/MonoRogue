using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class ExamineScreen : Subscreen {
        private ContentManager Content;
        private WorldView View;
        private Creature Player;
        private World World;
        private int X;
        private int Y;

        public ExamineScreen(ContentManager content, World world, Creature player, WorldView worldView) {
            View = worldView;
            Content = content;
            Player = player;
            World = world;
            X = Player.X;
            Y = Player.Y;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { 
                View.Update(World, Player);
                return null;
            }
            else if (IsRight(key)) { Move(1, 0); }
            else if (IsLeft(key)) { Move(-1, 0); }
            else if (IsUp(key)) { Move(0, -1); }
            else if (IsDown(key)) { Move(0, 1); }
            else if (IsNE(key)) { Move(1, -1); }
            else if (IsNW(key)) { Move(-1, -1); }
            else if (IsSE(key)) { Move(1, 1); }
            else if (IsSW(key)) { Move(-1, 1); }
            else if (key == Keys.Enter || key == Keys.Space) {
                Creature c = null;
                if (Player.CanSee(X, Y)) {
                    c = World.GetCreatureAt(X, Y);
                    if (c != null) { 
                        View.Update(World, Player);
                        return new CreatureScreen(Content, c);
                    }
                }
                if (View.HasSeen[X, Y]) {
                    Item i = World.GetItemAt(X, Y);
                    if (i != null) {
                        View.Update(World, Player);
                        return new ItemScreen(Content, i);
                    }
                }
            }

            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            View.Draw(spriteBatch);
            MainInterface.DrawBorder(spriteBatch);
            
            spriteBatch.Draw(MainInterface.TileHighlight, new Vector2((X - View.OffsetX) * 32, (Y - View.OffsetY) * 32), Color.White);

            if (View.HasSeen[X, Y]) {
                int ix = MainInterface.StartX + 24;
                int iy = 0;

                Creature c = null;
                if (Player.CanSee(X, Y)) {
                    c = World.GetCreatureAt(X, Y);
                    if (c != null) {
                        iy = MainInterface.DrawCreatureStats(spriteBatch, c, ix, iy);
                    }
                }

                Item i = World.GetItemAt(X, Y);
                if (i != null) {
                    iy = MainInterface.DrawItems(spriteBatch, i, null, iy);
                }

                iy = MainInterface.DrawTileHeader(spriteBatch, World.GetTile(X, Y), ix, iy);
            }

            spriteBatch.DrawString(Font.Get(14), "EXAMINING", new Vector2(MainInterface.StartX + 24, Constants.ScreenHeight - 32), Color.Cyan);
        }

        private void Move(int dx, int dy) {
            if (dx < 0 && X > 0) { X--; }
            if (dx > 0 && X < World.Width - 1) { X++; }
            if (dy < 0 && Y > 0) { Y--; }
            if (dy > 0 && Y < World.Height - 1) { Y++; }
            View.Update(World, Player, X, Y);
        }

    }
}
