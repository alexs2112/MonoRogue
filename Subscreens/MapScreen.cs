using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class MapScreen : BorderedScreen {
        private World World;
        private Creature Player;
        private WorldView WorldView;

        private Texture2D Floor;
        private Texture2D Wall;
        private Texture2D Creature;
        private Texture2D Item;
        private Texture2D Door;
        private Texture2D Feature;

        private Texture2D[,] Map;
        private Color[,] Colors;
        private int Width;
        private int Height;
        private int MX;
        private int MY;
        private Rectangle Legend;

        public MapScreen(ContentManager content, World world, WorldView worldView) : base(content) {
            World = world;
            Player = world.Player;
            WorldView = worldView;

            Floor = content.Load<Texture2D>("Interface/MapScreen/Floor");
            Wall = content.Load<Texture2D>("Interface/MapScreen/Wall");
            Creature = content.Load<Texture2D>("Interface/MapScreen/Creature");
            Item = content.Load<Texture2D>("Interface/MapScreen/Item");
            Door = content.Load<Texture2D>("Interface/MapScreen/Door");
            Feature = content.Load<Texture2D>("Interface/MapScreen/Feature");

            Width = Constants.ScreenWidth / 16;
            Height = Constants.ScreenHeight / 16;
            MX = world.Player.X;
            MY = world.Player.Y;

            Legend = new Rectangle(0, Height - 8, 8, 8);

            LoadWorld(World, Player, WorldView);
        }

        private void LoadWorld(World world, Creature player, WorldView worldView) {
            Map = new Texture2D[world.Width, world.Height];
            Colors = new Color[world.Width, world.Height];

            for (int x = 0; x < world.Width; x++) {
                for (int y = 0; y < world.Height; y++) {
                    Point point = new Point(x, y);
                    if (!worldView.HasSeen[x, y]) { continue; }    

                    Creature c = world.GetCreatureAt(point);
                    if (c != null && player.CanSee(point)) {
                        Map[x, y] = Creature;

                        if (c.IsPlayer) { Colors[x, y] = Color.SkyBlue; }
                        else { Colors[x, y] = Color.Red; }
                    } else if (world.Items.ContainsKey(point)) { 
                        Map[x, y] = Item;
                        if (world.Items[point].IsFood) { Colors[x, y] = Color.LawnGreen; }
                        else { Colors[x, y] = Color.Yellow; }
                    } else {
                        if (world.Difficulty[x, y] == 1) { Colors[x, y] = Color.Gold; }
                        else if (world.Difficulty[x, y] == 2) { Colors[x, y] = Color.Orange; }
                        else if (world.Difficulty[x, y] >= 3) { Colors[x, y] = Color.OrangeRed; }
                        else { Colors[x, y] = Color.LightGray; }
                        
                        if (world.IsFloor(point)) {
                            Map[x, y] = Floor;
                        } else if (world.IsDoor(point)) {
                            Map[x, y] = Door;
                            Colors[x, y] = Color.SaddleBrown;
                        } else {
                            if (world.GetTile(point).IsFeature) {
                                Map[x, y] = Feature;
                            } else {
                                Map[x, y] = Wall;
                            }
                        }
                    }
                }
            }
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { base.CloseSubscreen(); return null; }
            else if (IsLeft(key)) { MX--; }
            else if (IsRight(key)) { MX++; }
            else if (IsUp(key)) { MY--; }
            else if (IsDown(key)) { MY++; }
            else if (IsNE(key)) { MX++; MY--; }
            else if (IsNW(key)) { MX--; MY--; }
            else if (IsSE(key)) { MX++; MY++; }
            else if (IsSW(key)) { MX--; MY++; }
            else if (key == Keys.Space) { 
                MX = World.Player.X;
                MY = World.Player.Y;
            } else if (mouse.LeftClicked()) {
                Point pos = mouse.Position();
                pos.X /= 16;
                pos.Y /= 16;
                pos.X += MX - Width / 2;
                pos.Y += MY - Height / 2;
                if (World.InBounds(pos)) {
                    if (WorldView.HasSeen[pos.X, pos.Y]) {
                        if (Player.AI.CreatureInView(World) == null) {
                            List<Point> path = Pathfinder.FindPath(Player, pos.X, pos.Y);
                            if (path.Count > 0) { ((PlayerAI)Player.AI).SetPath(path); }
                        } else {
                            Player.Notify(new BasicNotification("Enemies in sight."));
                        }
                        base.CloseSubscreen();
                        return null;
                    }
                }
            }

            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouse) {
            int offsetX = MX - Width / 2;
            int offsetY = MY - Height / 2;
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (InRectangle(Legend, x, y)) { continue; }
                    int tileX = x + offsetX;
                    int tileY = y + offsetY;
                    if (!World.InBounds(tileX, tileY)) { continue; }
                    if (Map[tileX, tileY] == null) { continue; }
                    spriteBatch.Draw(Map[tileX, tileY], new Vector2(x * 16, y * 16), Colors[tileX, tileY]);
                }
            }
            DrawLegend(spriteBatch);
        }

        private bool InRectangle(Rectangle r, int x, int y) {
            return (x >= r.X && x < r.X + r.Width && y >= r.Y && y < r.Y + r.Height);
        }

        private void DrawLegend(SpriteBatch spriteBatch) {
            int increment = 20;
            Vector2 glyphVector = new Vector2(Legend.X * 16 + 8, Legend.Y * 16 + 4);
            Vector2 textVector = new Vector2(Legend.X * 16 + 32, Legend.Y * 16 + 4);

            spriteBatch.DrawString(Font.Get(12), "Map", glyphVector, Color.White);
            glyphVector.Y += increment;
            textVector.Y += increment;

            spriteBatch.Draw(Creature, glyphVector, Color.SkyBlue);
            spriteBatch.DrawString(Font.Get(12), "You", textVector, Color.White);
            glyphVector.Y += increment;
            textVector.Y += increment;

            spriteBatch.Draw(Creature, glyphVector, Color.Red);
            spriteBatch.DrawString(Font.Get(12), "Enemy", textVector, Color.White);
            glyphVector.Y += increment;
            textVector.Y += increment;

            spriteBatch.Draw(Item, glyphVector, Color.Yellow);
            spriteBatch.DrawString(Font.Get(12), "Item", textVector, Color.White);
            glyphVector.Y += increment;
            textVector.Y += increment;

            spriteBatch.Draw(Item, glyphVector, Color.Green);
            spriteBatch.DrawString(Font.Get(12), "Food", textVector, Color.White);
            glyphVector.Y += increment;
            textVector.Y += increment;

            spriteBatch.Draw(Door, glyphVector, Color.SaddleBrown);
            spriteBatch.DrawString(Font.Get(12), "Door", textVector, Color.White);
        }
    }
}
