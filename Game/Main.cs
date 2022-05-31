using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class Main : Game {
        System.Random rng;
        World world;
        CreatureFactory creatureFactory;
        Creature player;

        private KeyboardTrack keyTrack;
        private WorldView worldView;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Main() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            keyTrack = new KeyboardTrack();
            worldView = new WorldView(25, 15);

            rng = new System.Random();
            world = new WorldBuilder(rng, 25, 15).Generate(6);

            creatureFactory = new CreatureFactory(Content);

            Tile startTile = world.getStartTile();
            player = creatureFactory.NewPlayer(world, startTile.X, startTile.Y);
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            worldView.LoadContent(Content);
            worldView.Update(world, player);
        }

        protected override void Update(GameTime gameTime) {
            KeyboardState kState = Keyboard.GetState();
            keyTrack.Update(kState.GetPressedKeys());
            if (kState.IsKeyDown(Keys.Escape)) { Exit(); }
            else if (keyTrack.KeyJustPressed(Keys.Up)) { player.MoveRelative(0, -1); }
            else if (keyTrack.KeyJustPressed(Keys.Down)) { player.MoveRelative(0, 1); }
            else if (keyTrack.KeyJustPressed(Keys.Left)) { player.MoveRelative(-1, 0); }
            else if (keyTrack.KeyJustPressed(Keys.Right)) { player.MoveRelative(1, 0); }

            // Update what the world looks like if input has been given
            if (keyTrack.KeyJustPressed()) { worldView.Update(world, player); }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            for (int x = 0; x < world.Width; x++) {
                for (int y = 0; y < world.Height; y++) {
                    _spriteBatch.Draw(worldView.Glyphs[x,y], new Vector2(x * 32, y * 32), worldView.Colors[x,y]);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
