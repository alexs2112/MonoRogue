using System.Collections.Generic;
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

            List<string> cmd = new List<string>(args);
            Constants.Debug = cmd.Contains("--debug");
            if (Constants.Debug) { System.Console.WriteLine("Debug Mode Enabled"); }

            int seedIndex = cmd.IndexOf("--seed");
            if (seedIndex > -1) {
                try {
                    Constants.Seed = System.Int32.Parse(cmd[seedIndex + 1]);
                } catch {
                    System.Console.WriteLine("The --seed argument requires an integer parameter to follow it.");
                    System.Console.WriteLine("Using random seed instead.");
                }
            }
        }

        protected override void Initialize() {
            CreatureFactory creatureFactory = new CreatureFactory(Content);
            EquipmentFactory equipmentFactory = new EquipmentFactory(Content);
            Tile.LoadTiles(Content);
            Food.LoadFood(Content);
            PlayerGlyph.LoadGlyphs(Content);
            keyTrack = new KeyboardTrack();
            mouse = new MouseHandler();
            worldView = new WorldView(Constants.WorldViewWidth, Constants.WorldViewHeight);
            mainInterface = new MainInterface();

            int seed = Constants.Seed;
            if (seed == -1) { seed = new System.Random().Next(); }
            rng = new System.Random(seed);
            if (Constants.Debug) { System.Console.WriteLine($"Using seed {seed}"); }

            WorldBuilder worldBuilder = new WorldBuilder(rng, Constants.WorldWidth, Constants.WorldHeight);
            world = worldBuilder.GenerateDungeon(Constants.DungeonIterations, creatureFactory, equipmentFactory);
            player = world.Player;
            
            if (Constants.Debug) { world.PrintToTerminal(); }
            
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

                if (keyJustPressed || mouse.ButtonClicked()) {
                    // Whenever the player presses a key, clear messages
                    // We can update this in the future to show old messages without being confusing
                    player.AI.ClearMessages();
                }

                if (keyTrack.KeyJustPressed(Keys.Escape)) { Exit(); }
                
                if (!player.IsDead()) {
                    if (keyTrack.MovementNPressed()) { player.MoveRelative(0, -1); }
                    else if (keyTrack.MovementSPressed()) { player.MoveRelative(0, 1); }
                    else if (keyTrack.MovementWPressed()) { player.MoveRelative(-1, 0); }
                    else if (keyTrack.MovementEPressed()) { player.MoveRelative(1, 0); }
                    else if (keyTrack.MovementNEPressed() && Constants.AllowDiagonalMovement) { player.MoveRelative(1, -1); }
                    else if (keyTrack.MovementSEPressed() && Constants.AllowDiagonalMovement) { player.MoveRelative(1, 1); }
                    else if (keyTrack.MovementNWPressed() && Constants.AllowDiagonalMovement) { player.MoveRelative(-1, -1); }
                    else if (keyTrack.MovementSWPressed() && Constants.AllowDiagonalMovement) { player.MoveRelative(-1, 1); }
                    else if (keyTrack.KeyJustPressed(Keys.Space)) { player.PickUp(true); }
                    else if (keyTrack.WaitPressed()) { player.PickUp(false); }
                    else if (mouse.RightClicked()) {
                        Point tile = mouse.GetTile(worldView);
                        Creature mouseCreature = GetMouseCreature(tile);
                        Item mouseItem = GetMouseItem(tile);
                        if (mouseCreature != null) { subscreen = new CreatureScreen(Content, mouseCreature); }
                        else if (mouseItem != null) { subscreen = new ItemScreen(Content, mouseItem); }
                        inputGiven = false;
                    } else if (mouse.LeftClicked()) {
                        Point tile = mouse.GetTile(worldView);
                        Creature mouseCreature = GetMouseCreature(tile);
                        Creature target = player.GetCreatureInRange(mouseCreature);

                        if (mouse.Position().X > MainInterface.StartX + 16) {
                            subscreen = new CreatureScreen(Content, player);
                            inputGiven = false;
                        } else if (mouseCreature == player) {
                            player.PickUp(false);
                        } else if (target != null) {
                            player.Attack(target);
                            player.TurnTimer = player.GetAttackDelay();
                        } else if (worldView.HasSeen[tile.X, tile.Y] && player.CanEnter(tile)) {
                            // If the player has seen the tile and is not clicking a creature, give them a path to automatically follow
                            List<Point> path = Pathfinder.FindPath(player, tile.X, tile.Y);
                            if (path.Count > 0) { player.MoveTo(path[0]); path.RemoveAt(0); }

                            if (player.AI.CreatureInView(world) == null) {
                                if (path.Count > 0) { ((PlayerAI)player.AI).SetPath(path); }
                            }
                        }
                    } else {     
                        if (keyTrack.KeyJustPressed(Keys.S)) { subscreen = new CreatureScreen(Content, player); }
                        else if (keyTrack.KeyJustPressed(Keys.OemQuestion)) { subscreen = new HelpScreen(Content); }
                        inputGiven = false; 
                    }

                    // If input has been given, update the world
                    if ((keyJustPressed || mouse.ButtonClicked()) && inputGiven && !player.IsDead()) {
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
                Item mouseItem = GetMouseItem(tile);
                Creature mouseCreature = GetMouseCreature(tile);
                if (mouseCreature == player) { mouseCreature = null; }

                spriteBatch.Begin();

                for (int x = 0; x < worldView.Width; x++) {
                    for (int y = 0; y < worldView.Height; y++) {
                        if (worldView.Glyphs[x,y] == null) { continue; }
                        spriteBatch.Draw(worldView.Glyphs[x,y], new Vector2(x * 32, y * 32), worldView.Colors[x,y]);
                    }
                }

                mainInterface.DrawInterface(spriteBatch);
                mainInterface.DrawMessages(spriteBatch);
                mainInterface.DrawTileHighlight(spriteBatch, mouse, worldView, Color.White);

                if (mouseCreature == null && mouseItem == null) {
                    mainInterface.DrawCreatureStats(spriteBatch, player);
                } else if (mouseCreature != null) {
                    mainInterface.DrawCreatureStats(spriteBatch, mouseCreature, player);

                    Creature target = player.GetCreatureInRange(mouseCreature);
                    if (target != null) { mainInterface.DrawLineToCreature(spriteBatch, mouse, worldView, player, target, Color.Red); }
                    else { mainInterface.DrawTileHighlight(spriteBatch, mouse, worldView, Color.Yellow); }
                } else {
                    mainInterface.DrawItemInfo(spriteBatch, mouseItem);
                }

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
