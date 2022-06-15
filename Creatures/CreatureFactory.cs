using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class CreatureFactory {
        private EquipmentFactory Equipment;
        private Dictionary<string, Texture2D> Glyphs;

        // Keep track of who the player is after they are created to give them to enemy AIs
        private Creature Player;
        public void SetPlayer(Creature player) { Player = player; }

        public CreatureFactory(ContentManager content, EquipmentFactory equipment) {
            Glyphs = new Dictionary<string, Texture2D>();
            
            // Store all loaded sprites in a dictionary so we don't need to store a ton of variables
            Glyphs.Add("Rat", content.Load<Texture2D>("Creatures/Rat"));
            Glyphs.Add("Pig", content.Load<Texture2D>("Creatures/Pig"));
            Glyphs.Add("Farmer", content.Load<Texture2D>("Creatures/Farmer"));

            Equipment = equipment;
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
            world.Player = c;
            c.MoveTo(x, y);
            this.Player = c;
            return c;
        }

        public Creature NewWeakCreature(System.Random random, World world, int x, int y) {
            switch(random.Next(2)) {
                case 0: return NewRat(world, x, y);
                case 1: return NewPig(world, x, y);
                default: return null;
            }
        }
        public Creature NewMediumCreature(System.Random random, World world, int x, int y) {
            switch(random.Next(3)) {
                case 0: return NewRat(world, x, y);
                case 1: return NewPig(world, x, y);
                case 2: return NewFarmer(world, x, y);
                default: return null;
            }
        }
        public Creature NewStrongCreature(System.Random random, World world, int x, int y) {
            switch(random.Next(3)) {
                case 0: return NewRat(world, x, y);
                case 1: return NewPig(world, x, y);
                case 2: return NewFarmer(world, x, y);
                default: return null;
            }
        }

        private Creature NewRat(World world, int x, int y) {
            Creature c = new Creature("Rat", Glyphs["Rat"], Color.SaddleBrown);
            c.SetStats(4, (1,2), 6, 6);
            c.SetDescription("TEMP: A dirty rodent that has grown large and aggressive in the dungeon environment. It moves and attacks quite quickly.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 1;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewPig(World world, int x, int y) {
            Creature c = new Creature("Pig", Glyphs["Pig"], Color.Pink);
            c.SetStats(6, (1, 2));
            c.SetDescription("TEMP: A stout-bodied, short-legged, omnivorous mammal.");
            c.AI = new PigAI(c, Player);
            c.World = world;
            c.Difficulty = 1;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewFarmer(World world, int x, int y) {
            Creature c = new Creature("Farmer", Glyphs["Farmer"], Color.RosyBrown);
            c.SetStats(8, (1, 3));
            c.SetDescription("A simple farmer, tending to the herd of pigs in the dungeon.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 2;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }
    }
}
