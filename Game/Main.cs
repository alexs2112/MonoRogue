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

        // Keep track of whos turn it is
        private int currentIndex;

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
            MainInterface.LoadTextures(Content);

            worldView.LoadContent(Content);
            worldView.Update(world, player);
        }

        protected override void Update(GameTime gameTime) {
            mouse.Update();
            KeyboardState kState = Keyboard.GetState();
            keyTrack.Update(kState.GetPressedKeys(), gameTime.ElapsedGameTime);

            bool keyJustPressed = keyTrack.KeyJustPressed();

            if (subscreen != null) {
                if (keyJustPressed || mouse.ButtonClicked()) {
                    Keys k = Keys.None;
                    if (keyJustPressed) { k = kState.GetPressedKeys()[0]; }
                    subscreen = subscreen.RespondToInput(k, mouse);
                }
            } else {
                bool inputGiven = true;

                if (keyJustPressed) {
                    // Whenever the player presses a key, clear messages
                    // We can update this in the future to show old messages without being confusing
                    player.AI.ClearMessages();
                }

                if (keyTrack.KeyJustPressed(Keys.Escape)) { Exit(); }
                else if (keyTrack.KeyJustPressed(Keys.Up)) { player.MoveRelative(0, -1); }
                else if (keyTrack.KeyJustPressed(Keys.Down)) { player.MoveRelative(0, 1); }
                else if (keyTrack.KeyJustPressed(Keys.Left)) { player.MoveRelative(-1, 0); }
                else if (keyTrack.KeyJustPressed(Keys.Right)) { player.MoveRelative(1, 0); }
                else if (keyTrack.KeyJustPressed(Keys.Space)) { player.PickUp(); }
                else if (keyTrack.KeyJustPressed(Keys.OemPeriod)) { player.TurnTimer = 10; } // Do Nothing
                else {
                    if (mouse.RightClicked()) {
                        Point tile = mouse.GetTile(worldView);
                        Creature mouseCreature = GetMouseCreature(tile);
                        Item mouseItem = GetMouseItem(tile);
                        if (mouseCreature != null) { subscreen = new CreatureScreen(Content, mouseCreature); }
                        else if (mouseItem != null) { subscreen = new ItemScreen(Content, mouseItem); }
                    } else if (mouse.LeftClicked() && mouse.Position().X > MainInterface.StartX + 16) {
                        subscreen = new CreatureScreen(Content, player);
                    } else if (keyTrack.KeyJustPressed(Keys.S)) { subscreen = new CreatureScreen(Content, player); }
                    inputGiven = false; 
                }

                // If input has been given, update the world
                if (keyJustPressed && inputGiven && !player.IsDead()) {
                    while (player.TurnTimer > 0) {
                        // Loop through each creatures timer, decrementing them and taking a turn when it hits 0
                        currentIndex = (currentIndex + 1) % world.Creatures.Count;
                        Creature c = world.Creatures[currentIndex];
                        c.TurnTimer--;
                        if (c.TurnTimer <= 0) { 
                            c.TakeTurn(world);
                            worldView.Update(world, player);

                            if (player.IsDead()) { break; }
                        }
                    }
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
                Creature mouseCreature = GetMouseCreature(tile);
                Item mouseItem = GetMouseItem(tile);

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
        
        private Creature GetMouseCreature(Point tile) {
            if (tile.X != -1) {
                if (player.CanSee(tile)) {
                    return world.GetCreatureAt(tile);
                }
            }
            return null;
        }
        private Item GetMouseItem(Point tile) {
            if (tile.X != -1) {
                if (player.CanSee(tile)) {
                    return world.GetItemAt(tile);
                }
            }
            return null;
        }
    }
}
