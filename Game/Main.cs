using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class Main : Game {

        // Store textures here temporarily
        Texture2D wallTexture;
        Texture2D floorTexture;

        System.Random rng;
        World world;
        CreatureFactory creatureFactory;
        Creature player;

        private KeyboardTrack keyTrack;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Main() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            keyTrack = new KeyboardTrack();

            rng = new System.Random();
            world = new WorldBuilder(rng, 25, 15).Generate(6);

            creatureFactory = new CreatureFactory(Content);
            
            Tile startTile = world.getStartTile();
            player = creatureFactory.NewPlayer(world, startTile.X, startTile.Y);
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            wallTexture = Content.Load<Texture2D>("Tiles/Wall");
            floorTexture = Content.Load<Texture2D>("Tiles/Floor");
        }

        protected override void Update(GameTime gameTime) {
            KeyboardState kState = Keyboard.GetState();
            keyTrack.Update(kState.GetPressedKeys());
            if (kState.IsKeyDown(Keys.Escape)) { Exit(); }
            else if (keyTrack.KeyJustPressed(Keys.Up)) { player.MoveRelative(0, -1); }
            else if (keyTrack.KeyJustPressed(Keys.Down)) { player.MoveRelative(0, 1); }
            else if (keyTrack.KeyJustPressed(Keys.Left)) { player.MoveRelative(-1, 0); }
            else if (keyTrack.KeyJustPressed(Keys.Right)) { player.MoveRelative(1, 0); }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // First draw the grid of tiles in the world
            for (int x = 0; x < world.Width; x++) {
                for (int y = 0; y < world.Height; y++) {
                    Texture2D tile;
                    Color color;
                    if (world.Tiles[x,y] == 0) { tile = floorTexture; color = Color.Gray; }
                    else { tile = wallTexture; color = Color.White; }
                    _spriteBatch.Draw(tile, new Vector2(32 * x, 32 * y), color);
                }
            }

            // Then draw each creature where they are standing, on top of the tile
            _spriteBatch.Draw(player.Sprite, new Vector2(32 * player.X, 32 * player.Y), player.Color);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
