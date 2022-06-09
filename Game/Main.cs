using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class Main : Game {
        private System.Random rng;
        private World world;
        private Creature player;
        private Subscreen subscreen;

        private WorldView worldView;
        private MainInterface mainInterface;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private KeyboardTrack keyTrack;
        private MouseHandler mouse;

        public Main(string[] args) {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Constants.ScreenWidth;
            graphics.PreferredBackBufferHeight = Constants.ScreenHeight;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Constants.Debug = System.Array.Exists(args, element => element == "--debug");
            if (Constants.Debug) { System.Console.WriteLine("Debug Mode Enabled"); }
        }

        protected override void Initialize() {
            Tile.LoadTiles(Content);
            Food.LoadFood(Content);
            PlayerGlyph.LoadGlyphs(Content);
            keyTrack = new KeyboardTrack();
            mouse = new MouseHandler();
            worldView = new WorldView(Constants.WorldViewWidth, Constants.WorldViewHeight);
            mainInterface = new MainInterface();

            if (Constants.Seed == -1) { rng = new System.Random(); }
            else { rng = new System.Random(Constants.Seed); }

            world = new WorldBuilder(rng, Constants.WorldWidth, Constants.WorldHeight).GenerateDungeon(Constants.DungeonIterations);
            if (Constants.Debug) { world.PrintToTerminal(); }

            CreatureFactory creatureFactory = new CreatureFactory(Content);
            EquipmentFactory equipmentFactory = new EquipmentFactory(Content);
            
            Point startTile = world.GetRandomFloor(rng);
            player = creatureFactory.NewPlayer(world, startTile.X, startTile.Y);

            world.SpawnObjects(rng, creatureFactory, equipmentFactory);
            
            if (!Constants.Debug) { subscreen = new StartScreen(Content); }
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainInterface.LoadTextures(Content);

            worldView.LoadContent(Content);
            worldView.Update(world, player);
        }

        protected override void Update(GameTime gameTime) {
            mouse.Update();
            KeyboardState kState = Keyboard.GetState();
            keyTrack.Update(kState.GetPressedKeys(), gameTime.ElapsedGameTime);

            bool keyJustPressed = keyTrack.KeyJustPressed();

            if (subscreen != null) {
                if (keyJustPressed) {
                    Keys k = kState.GetPressedKeys()[0];
                    subscreen = subscreen.RespondToInput(k, mouse);
                }
            } else {
                bool inputGiven = true;

                if (keyJustPressed) {
                    // Whenever the player presses a key, clear messages
                    // We can update this in the future to show old messages without being confusing
                    player.AI.ClearMessages();
                }

                if (kState.IsKeyDown(Keys.Escape)) { Exit(); }
                else if (keyTrack.KeyJustPressed(Keys.Up)) { player.MoveRelative(0, -1); }
                else if (keyTrack.KeyJustPressed(Keys.Down)) { player.MoveRelative(0, 1); }
                else if (keyTrack.KeyJustPressed(Keys.Left)) { player.MoveRelative(-1, 0); }
                else if (keyTrack.KeyJustPressed(Keys.Right)) { player.MoveRelative(1, 0); }
                else if (keyTrack.KeyJustPressed(Keys.Space)) { player.PickUp(); }
                else if (keyTrack.KeyJustPressed(Keys.OemPeriod)) { } // Do Nothing
                else { inputGiven = false; }

                // If input has been given, update the world
                if (keyJustPressed && inputGiven && !player.IsDead()) { 
                    world.TakeTurns();
                    worldView.Update(world, player);
                    mainInterface.UpdateMessages(player.AI.GetMessages());
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            if (subscreen != null) {
                spriteBatch.Begin();
                subscreen.Draw(gameTime, spriteBatch, mouse);
                spriteBatch.End();
            } else {
                // Some mouse handling in draw so we can keep it as a local variable
                Point tile = mouse.GetTile(worldView);
                Creature mouseCreature = null;
                Item mouseItem = null;

                // Don't worry about tiles we can't see
                if (tile.X != -1) {
                    if (!worldView.HasSeen[tile.X, tile.Y]) { 
                        tile = new Point(-1, -1); 
                    }
                }
                if (tile.X != -1) {
                    if (player.CanSee(tile)) {
                        mouseCreature = world.GetCreatureAt(tile);
                        if (mouseCreature == null) { mouseItem = world.GetItemAt(tile); }
                    }
                }

                spriteBatch.Begin();

                for (int x = 0; x < worldView.Width; x++) {
                    for (int y = 0; y < worldView.Height; y++) {
                        if (worldView.Glyphs[x,y] == null) { continue; }
                        spriteBatch.Draw(worldView.Glyphs[x,y], new Vector2(x * 32, y * 32), worldView.Colors[x,y]);
                    }
                }

                mainInterface.DrawInterface(spriteBatch);
                mainInterface.DrawMessages(spriteBatch);

                if (mouseCreature == null && mouseItem == null) {
                    mainInterface.DrawCreatureStats(spriteBatch, player);
                } else if (mouseCreature != null) {
                    mainInterface.DrawCreatureStats(spriteBatch, mouseCreature);
                } else {
                    mainInterface.DrawItemInfo(spriteBatch, mouseItem);
                }
                mainInterface.DrawTileHighlight(spriteBatch, mouse, worldView);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
