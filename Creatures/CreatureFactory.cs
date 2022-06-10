using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class CreatureFactory {
        private Dictionary<string, Texture2D> Glyphs;

        // Keep track of who the player is after they are created to give them to enemy AIs
        private Creature Player;
        public void SetPlayer(Creature player) { Player = player; }

        public CreatureFactory(ContentManager content) {
            Glyphs = new Dictionary<string, Texture2D>();
            
            // Store all loaded sprites in a dictionary so we don't need to store a ton of variables
            Glyphs.Add("Rat", content.Load<Texture2D>("Creatures/Rat"));
            Glyphs.Add("Pig", content.Load<Texture2D>("Creatures/Pig"));
            Glyphs.Add("Farmer", content.Load<Texture2D>("Creatures/Farmer"));
        }

        public Creature NewPlayer(World world, int x, int y) {
            Texture2D glyph = PlayerGlyph.GetDefaultGlyph();
            Creature c = new Creature("Player", glyph, Color.SkyBlue);
            c.IsPlayer = true;
            c.SetStats(12, (2, 3));
            c.SetDescription("A hapless adventurer.");
            c.AI = new PlayerAI(c);
            c.World = world;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            this.Player = c;
            return c;
        }

        public Creature NewRat(World world, int x, int y) {
            Creature c = new Creature("Rat", Glyphs["Rat"], Color.SaddleBrown);
            c.SetStats(4, (1,2), 6, 6);
            c.SetDescription("TEMP: A dirty rodent that has grown large and aggressive in the dungeon environment. It moves and attacks quite quickly.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Faction = "Vermin";
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        public Creature NewPig(World world, int x, int y) {
            Creature c = new Creature("Pig", Glyphs["Pig"], Color.Pink);
            c.SetStats(6, (1, 2));
            c.SetDescription("TEMP: A stout-bodied, short-legged, omnivorous mammal.");
            c.AI = new PigAI(c, Player);
            c.World = world;
            c.Faction = "Farmer";
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        public Creature NewFarmer(World world, int x, int y) {
            Creature c = new Creature("Farmer", Glyphs["Farmer"], Color.RosyBrown);
            c.SetStats(8, (1, 3));
            c.SetDescription("A simple farmer, tending to the herd of pigs in the dungeon.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Faction = "Farmer";
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }
    }
}
