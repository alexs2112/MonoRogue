using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class Main : Game {
        private System.Random rng;
        private World world;
        private CreatureFactory creatureFactory;
        private Creature player;

        private KeyboardTrack keyTrack;
        private WorldView worldView;
        private MainInterface mainInterface;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

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
            keyTrack = new KeyboardTrack();
            worldView = new WorldView(Constants.WorldViewWidth, Constants.WorldViewHeight);
            mainInterface = new MainInterface();

            rng = new System.Random();
            world = new WorldBuilder(rng, Constants.WorldWidth, Constants.WorldHeight).GenerateDungeon(Constants.DungeonIterations);

            creatureFactory = new CreatureFactory(Content);

            Point startTile = world.GetRandomFloor(rng);
            player = creatureFactory.NewPlayer(world, startTile.X, startTile.Y);

            for (int i = 0; i < 10; i++) {
                Point t = world.GetEmptyFloor(rng);
                Creature p = creatureFactory.NewPig(world, t.X, t.Y);
            }

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainInterface.LoadTextures(Content);

            worldView.LoadContent(Content);
            worldView.Update(world, player);
        }

        protected override void Update(GameTime gameTime) {
            KeyboardState kState = Keyboard.GetState();
            keyTrack.Update(kState.GetPressedKeys(), gameTime.ElapsedGameTime);

            bool keyJustPressed = keyTrack.KeyJustPressed();
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
            else { inputGiven = false; }

            // If input has been given, update the world
            if (keyJustPressed && inputGiven) { 
                world.TakeTurns();
                worldView.Update(world, player);
                mainInterface.UpdateMessages(player.AI.GetMessages());
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            for (int x = 0; x < worldView.Width; x++) {
                for (int y = 0; y < worldView.Height; y++) {
                    if (worldView.Glyphs[x,y] == null) { continue; }
                    spriteBatch.Draw(worldView.Glyphs[x,y], new Vector2(x * 32, y * 32), worldView.Colors[x,y]);
                }
            }

            mainInterface.DrawInterface(spriteBatch);
            mainInterface.DrawCreatureStats(spriteBatch, player);
            mainInterface.DrawMessages(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
