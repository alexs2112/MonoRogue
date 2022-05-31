using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class CreatureFactory {
        private Dictionary<string, Texture2D> Sprites { get; set; }

        public CreatureFactory(ContentManager content) {
            Sprites = new Dictionary<string, Texture2D>();
            
            // Store all loaded sprites in a dictionary so we don't need to store a ton of variables
            Sprites.Add("Player", content.Load<Texture2D>("Creatures/Player"));
        }

        public Creature NewPlayer(World world, int x, int y) {
            Creature c = new Creature(Sprites["Player"], Color.SkyBlue);
            c.World = world;
            c.MoveTo(x, y);
            return c;
        }
    }
}