using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class FireScreen : Subscreen {
        private WorldView WorldView;
        private Creature Player;
        private List<(Creature Creature, bool Valid)> Enemies;
        private int Index;

        public FireScreen(WorldView worldView, Creature player) {
            WorldView = worldView;
            Player = player;

            Enemies = new List<(Creature Creature, bool Valid)>();
            Dictionary<Creature, int> distances = new Dictionary<Creature, int>();
            foreach (Creature e in Player.World.Creatures) {
                if (e == Player) { continue; }
                if (Player.CanSee(e.X, e.Y)) { 
                    Creature target = player.GetCreatureInRange(e);
                    if (target == e) { Enemies.Add((e, true)); }
                    else { Enemies.Add((e, false)); }
                }
            }
            Enemies = Enemies.OrderBy(e => Player.GetLineToPoint(e.Creature.X, e.Creature.Y).Count).ThenBy(e => e.Creature.HP).ToList<(Creature Creature, bool Valid)>();
        }

        public static bool CanEnter(Creature player) {
            foreach (Creature e in player.World.Creatures) {
                if (e == player) { continue; }
                if (player.CanSee(e.X, e.Y)) { 
                    return true;
                }
            }
            return false;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape) { return null; }
            else if (mouse.RightClicked()) { return null; }

            // @todo: For now only handle the arrow keys, fix this to use all "movement" commands later
            else if (key == Keys.Up) { if (Index > 0) { Index--; }}
            else if (key == Keys.Down) { if (Index < Enemies.Count - 1) { Index++; }}
            else if (key == Keys.Enter || key == Keys.Space) {
                Creature target = GetCreature();
                if (target != null) {
                    Player.Attack(target);
                } else {
                    Player.Notify("Out of range.");
                }
                return null;
            }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            WorldView.Draw(spriteBatch);
            spriteBatch.Draw(MainInterface.InterfaceDivider, new Vector2(MainInterface.StartX, 0), Color.Gray);

            int x = MainInterface.StartX + 24;
            int y = 0;
            y = MainInterface.DrawCreatureStats(spriteBatch, Player, x, y);

            for (int i = 0; i < Enemies.Count; i++) {
                (Creature creature, bool valid) = Enemies[i];
                y = DrawCreature(spriteBatch, creature, valid, i == Index, x, y);
            }

            Creature target = GetCreature();
            if (target != null) {
                List<Point> line = Player.GetLineToPoint(new Point(target.X, target.Y));
                foreach (Point point in line) {
                    spriteBatch.Draw(MainInterface.TileHighlight, new Vector2((point.X - WorldView.OffsetX) * 32, (point.Y - WorldView.OffsetY) * 32), Color.Red);
                }
            } else {
                target = Enemies[Index].Creature;
                spriteBatch.Draw(MainInterface.TileHighlight, new Vector2((target.X - WorldView.OffsetX) * 32, (target.Y - WorldView.OffsetY) * 32), Color.Yellow);
            }
        }

        private Creature GetCreature() {
            Creature target;
            if (Enemies[Index].Valid) { target = Enemies[Index].Creature; }
            else { target = Player.GetCreatureInRange(Enemies[Index].Creature); }
            return target;
        }

        private int DrawCreature(SpriteBatch spriteBatch, Creature creature, bool valid, bool selected, int x, int y) {
            y += 8;
            Color color;
            if (valid) {
                color = selected ? Color.LawnGreen : Color.White;
            } else {
                color = selected ? Color.Green : Color.Gray;
            }
            y = MainInterface.DrawCreatureHeader(spriteBatch, creature, x, y, color);

            if (valid) {
                // Draw creature health and defense
                MainInterface.DrawHearts(spriteBatch, creature.MaxHP, creature.HP, x, y, Color.Red);
                MainInterface.DrawHearts(spriteBatch, creature.GetDefense().Max, creature.GetDefense().Current, x + 32 * ((creature.MaxHP + 3) / 4), y, Color.LightSkyBlue);
                y += 32;
            }
            y += 4;
            spriteBatch.Draw(MainInterface.InterfaceLine, new Vector2(MainInterface.StartX + 8, y), Color.Gray);
            return y;
        }
    }
}
