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
            Sprites.Add("Pig", content.Load<Texture2D>("Creatures/Pig"));
        }

        public Creature NewPlayer(World world, int x, int y) {
            Creature c = new Creature("Player", Sprites["Player"], Color.SkyBlue);
            c.SetStats(10, 3);
            c.AI = new PlayerAI(c);
            c.World = world;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        public Creature NewPig(World world, int x, int y) {
            Creature c = new Creature("Pig", Sprites["Pig"], Color.Pink);
            c.SetStats(5, 1);
            c.AI = new BasicAI(c);
            c.World = world;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }
    }
}
