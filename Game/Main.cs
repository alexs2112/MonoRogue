using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class Main : Game {
        private System.Random Random;
        public int Seed;
        public GameLoader GameLoader;
        private World World;
        private Creature Player;
        private Subscreen Subscreen;

        private WorldView WorldView;
        private MainInterface MainInterface;
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;

        private KeyboardTrack KeyTrack;
        private MouseHandler Mouse;

        private CreatureFactory CreatureFactory;
        private EquipmentFactory EquipmentFactory;

        public static AudioPlayer Audio;

        // Keep track of whos turn it is
        private int CurrentIndex;

        public Main(string[] args) {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Settings.LoadSettings();
            ParseParameters(args);

            if (GameLoader.CanLoad()) { GameLoader = new GameLoader(); }
        }

        protected override void Initialize() {
            Window.Title = $"Escape of the {Constants.GetPlayerName()}";
            UpdateScreenSize();

            Tile.LoadTiles(Content);
            Feature.LoadFeatures(Content);
            Food.LoadFood(Content);
            Font.LoadFonts(Content);
            Heartstone.LoadHeartstone(Content);
            GoldenKey.LoadKey(Content);
            PlayerGlyph.LoadGlyphs(Content);
            EnemyGlyph.LoadGlyphs(Content);
            Vault.LoadVaults();
            KeyTrack = new KeyboardTrack();
            Mouse = new MouseHandler();
            WorldView = new WorldView();
            MainInterface = new MainInterface();

            Audio = new AudioPlayer(Content);
            
            if (!Constants.Debug) { 
                Subscreen = new StartScreen(this, Content); 
            } else {
                CreateWorld();
            }
            base.Initialize();
        }

        public void CreateWorld(bool loadFromSave = false) {
            Seed = Constants.Seed;
            if (loadFromSave && GameLoader != null) { 
                Seed = GameLoader.Seed;
                Constants.Difficulty = GameLoader.Difficulty;
            }
            else if (Seed == -1) { Seed = new System.Random().Next(); }
            Random = new System.Random(Seed);
            System.Console.WriteLine($"Using seed {Seed}");

            EquipmentFactory = new EquipmentFactory(Content);
            CreatureFactory = new CreatureFactory(Content, EquipmentFactory, Random);

            if (loadFromSave && GameLoader == null) { loadFromSave = false; }
            if (loadFromSave) {
                Constants.WorldWidth = GameLoader.WorldData.Width;
                Constants.WorldHeight = GameLoader.WorldData.Height;
                WorldView = new WorldView();
            }
            WorldBuilder worldBuilder = new WorldBuilder(Random, Constants.WorldWidth, Constants.WorldHeight);
            World = worldBuilder.GenerateDungeon(Constants.DungeonIterations, CreatureFactory, EquipmentFactory);

            if (loadFromSave) {
                GameLoader.LoadGame(World, WorldView, EquipmentFactory, CreatureFactory);
                World = GameLoader.World;
            } else {
                SaveGame();
            }

            Player = World.Player;
            WorldView.Update(World, Player);
            
            if (Constants.Debug) { World.PrintToTerminal(); }
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MainInterface.LoadTextures(Content, GraphicsDevice);
            Projectile.LoadTextures(Content);
        }

        protected override void Update(GameTime gameTime) {
            Audio.Update(gameTime.ElapsedGameTime);
            Mouse.Update();
            KeyboardState kState = Keyboard.GetState();
            KeyTrack.Update(kState.GetPressedKeys(), gameTime.ElapsedGameTime);

            bool keyJustPressed = KeyTrack.KeyJustPressed();
            bool inputGiven = false;

            if (Subscreen != null) {
                if (keyJustPressed || Mouse.ButtonClicked()) {
                    Keys k = Keys.None;
                    if (keyJustPressed) { k = kState.GetPressedKeys()[0]; }
                    Subscreen = Subscreen.RespondToInput(k, Mouse);

                    if (Subscreen == null) { inputGiven = true; }
                }
            } else if (World.Projectiles.Count > 0) {
                World.UpdateProjectiles(gameTime.ElapsedGameTime, MainInterface, WorldView);
                if (World.Projectiles.Count == 0 && !Player.IsDead()) {
                    TakeTurns();
                }
            } else {
                inputGiven = true;
                if (keyJustPressed || Mouse.ButtonClicked()) {
                    // Whenever the player presses a key, clear messages
                    // We can update this in the future to show old messages without being confusing
                    Player.AI.ClearMessages();
                }

                if (KeyTrack.KeyJustPressed(Keys.Escape)) {
                    if (Player.IsDead()) { Exit(); }
                    Subscreen = new EscapeScreen(this, Content);
                } else if (!Player.IsDead()) {
                    if (!((PlayerAI)Player.AI).PathNullOrEmpty()) {
                        // If the player is automatically following a path, follow it
                        if (KeyTrack.KeyJustPressed() || Mouse.ButtonClicked()) {
                            // Any input cancels the move
                            ((PlayerAI)Player.AI).ClearPath();
                            Player.AddMessage("Input given, cancelling pathing.");
                        } else {
                            ((PlayerAI)Player.AI).FollowPath(World);
                            inputGiven = true;
                        }
                    }
                    else if (KeyTrack.MovementNPressed()) { Player.MoveRelative(0, -1); }
                    else if (KeyTrack.MovementSPressed()) { Player.MoveRelative(0, 1); }
                    else if (KeyTrack.MovementWPressed()) { Player.MoveRelative(-1, 0); }
                    else if (KeyTrack.MovementEPressed()) { Player.MoveRelative(1, 0); }
                    else if (KeyTrack.MovementNEPressed() && Constants.AllowDiagonalMovement) { Player.MoveRelative(1, -1); }
                    else if (KeyTrack.MovementSEPressed() && Constants.AllowDiagonalMovement) { Player.MoveRelative(1, 1); }
                    else if (KeyTrack.MovementNWPressed() && Constants.AllowDiagonalMovement) { Player.MoveRelative(-1, -1); }
                    else if (KeyTrack.MovementSWPressed() && Constants.AllowDiagonalMovement) { Player.MoveRelative(-1, 1); }
                    else if (KeyTrack.WaitPressed()) { Player.TurnTimer = Player.GetMovementDelay(); }
                    else if (KeyTrack.KeyJustPressed(Keys.R)) { ((PlayerAI)Player.AI).StartResting(); }
                    else if (KeyTrack.KeyJustPressed(Keys.F)) {
                        if (FireScreen.CanEnter(Player)) { Subscreen = new FireScreen(WorldView, Player); }
                        else { Player.AddMessage("No enemies in sight."); }
                    } else if (KeyTrack.KeyJustPressed(Keys.Space)) {
                        if (World.GetItemAt(Player.X, Player.Y) != null) { Player.PickUp(true); }
                        else if (Player.X == World.Exit.X && Player.Y == World.Exit.Y) {
                            inputGiven = false;
                            TryToWinGame();
                        }
                    } else if (Mouse.RightClicked()) {
                        Point tile = Mouse.GetTile(WorldView);
                        Creature mouseCreature = GetMouseCreature(tile);
                        Item mouseItem = GetMouseItem(tile);
                        if (mouseCreature != null) { Subscreen = new CreatureScreen(Content, mouseCreature); }
                        else if (mouseItem != null) { Subscreen = new ItemScreen(Content, mouseItem); }
                        inputGiven = false;
                    } else if (Mouse.LeftClicked()) {
                        Point tile = Mouse.GetTile(WorldView);
                        Creature mouseCreature = GetMouseCreature(tile);
                        Creature target = Player.GetCreatureInRange(mouseCreature);

                        if (Mouse.Position().X > MainInterface.StartX + 16) {
                            Subscreen = new CreatureScreen(Content, Player);
                            inputGiven = false;
                        } else if (mouseCreature == Player) {
                            if (Player.X == World.Exit.X && Player.Y == World.Exit.Y) {
                                inputGiven = false;
                                TryToWinGame();
                            } else {
                                Player.PickUp(false);
                            }
                        } else if (target != null) {
                            Player.MoveTo(target.X, target.Y);
                            Player.TurnTimer = Player.GetAttackDelay();
                        } else if (WorldView.HasSeen[tile.X, tile.Y] && Player.CanEnter(tile)) {
                            // If the player has seen the tile and is not clicking a creature, give them a path to automatically follow
                            List<Point> path = Pathfinder.FindPath(Player, tile.X, tile.Y);
                            if (path.Count > 0) { Player.MoveTo(path[0]); path.RemoveAt(0); }

                            if (Player.AI.CreatureInView(World) == null) {
                                if (path.Count > 0) { ((PlayerAI)Player.AI).SetPath(path); }
                            }
                        }
                    } else {     
                        if (KeyTrack.KeyJustPressed(Keys.S)) { Subscreen = new CreatureScreen(Content, Player); }
                        else if (KeyTrack.KeyJustPressed(Keys.OemQuestion)) { Subscreen = new HelpMenuScreen(Content); }
                        else if (KeyTrack.KeyJustPressed(Keys.M)) { Subscreen = new MapScreen(Content, World, WorldView); }
                        inputGiven = false; 
                    }
                }

                // If input has been given, update the world
                if (inputGiven && !Player.IsDead() && World.Projectiles.Count == 0) {
                    TakeTurns();
                }
            }

            base.Update(gameTime);
        }

        private void TakeTurns() {
            WorldView.Update(World, Player);
            while (Player.TurnTimer > 0) {
                // Loop through each creatures timer, decrementing them and taking a turn when it hits 0
                CurrentIndex = (CurrentIndex + 1) % World.Creatures.Count;
                Creature c = World.Creatures[CurrentIndex];
                c.TurnTimer--;
                if (c.TurnTimer <= 0) { 
                    c.TakeTurn(World);

                    if (Player.IsDead()) { break; }
                    if (World.Projectiles.Count > 0) { break; }
                }
            }
            MainInterface.UpdateMessages(Player.AI.GetMessages());
            WorldView.Update(World, Player);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            if (Subscreen != null) {
                SpriteBatch.Begin();
                Subscreen.Draw(gameTime, SpriteBatch, Mouse);
                SpriteBatch.End();
            } else {
                // Some mouse handling in draw so we can keep it as a local variable
                Point tile = Mouse.GetTile(WorldView);
                Item mouseItem = GetMouseItem(tile);
                Item floorItem = Player.IsDead() ? null : World.GetItemAt(Player.X, Player.Y);
                Tile mouseTile = Player.CanSee(tile) ? World.GetTile(tile) : null;
                Creature mouseCreature = GetMouseCreature(tile);
                if (mouseCreature == Player) { mouseCreature = null; }

                SpriteBatch.Begin();

                WorldView.Draw(SpriteBatch);
                
                MainInterface.DrawTileHighlight(SpriteBatch, Mouse, WorldView, Color.White);
                if (mouseCreature != null) {
                    Creature target = Player.GetCreatureInRange(mouseCreature);
                    if (target != null) { MainInterface.DrawLineToCreature(SpriteBatch, Mouse, WorldView, Player, target, Color.Red); }
                    else { MainInterface.DrawTileHighlight(SpriteBatch, Mouse, WorldView, Color.Yellow); }
                }

                foreach (Projectile p in World.Projectiles) {
                    p.Draw(SpriteBatch, WorldView);
                }

                MainInterface.DrawInterface(SpriteBatch, Player, mouseCreature, floorItem, mouseItem, mouseTile);
                SpriteBatch.End();
            }

            base.Draw(gameTime);
        }
        
        private Creature GetMouseCreature(Point tile) {
            if (tile.X != -1) {
                if (Player.CanSee(tile)) {
                    return World.GetCreatureAt(tile);
                }
            }
            return null;
        }
        private Item GetMouseItem(Point tile) {
            if (tile.X != -1) {
                if (Player.CanSee(tile)) {
                    return World.GetItemAt(tile);
                }
            }
            return null;
        }

        private void TryToWinGame() {
            if (Player.HasKey) {
                Subscreen = new WinScreen(this, Content);
            } else {
                Player.AddMessage("You need the Golden Key to unlock the exit to the dungeon!");
            }
        }

        public void UpdateScreenSize() {
            Graphics.IsFullScreen = Constants.Fullscreen;
            Graphics.PreferredBackBufferWidth = Constants.ScreenWidth;
            Graphics.PreferredBackBufferHeight = Constants.ScreenHeight;
            Graphics.ApplyChanges();
            
            MainInterface.UpdateScreen();
            if (WorldView != null) { 
                WorldView.UpdateScreenSize();
                if (World != null) { WorldView.Update(World, Player); }
            }
        }

        public void UpdatePlayer() {
            Window.Title = $"Escape of the {Constants.GetPlayerName()}";

            if (Player == null) { return; }
            Player.SetName(Constants.GetPlayerName());
            Player.SetColor(Constants.Colors[Constants.ColorIndex]);

            WorldView.Update(World, Player);
        }

        private static void ParseParameters(string[] args) {
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

            int difficultyIndex = cmd.IndexOf("--difficulty");
            if (difficultyIndex > -1) {
                try {
                    int d = System.Int32.Parse(cmd[difficultyIndex + 1]);
                    if (d > 3 || d < 1) { throw new System.Exception(); }
                    Constants.Difficulty = d;
                    System.Console.WriteLine($"Setting difficulty to {Constants.Difficulty}.");
                } catch {
                    System.Console.WriteLine("The --difficulty argument requires either 1, 2 or 3 to follow it.");
                    System.Console.WriteLine("Setting difficulty to default (3).");
                }
            }

            if (cmd.Contains("--small")) {
                Constants.WorldWidth = 40;
                Constants.WorldHeight = 25;
            }
            if (cmd.Contains("--no-animations")) { Constants.AllowAnimations = false; }

            Constants.WriteMessagesToConsole = cmd.Contains("--messages");
            Constants.Invincible = cmd.Contains("--invincible");

            if (Constants.Invincible) { System.Console.WriteLine("Player Invincibility Enabled"); }
        }

        public void SaveGame() {
            new GameSaver(Seed, World, WorldView).SaveGame();
        }

        protected override void OnExiting(object sender, System.EventArgs args) {
            base.OnExiting(sender, args);
        }
    }
}
