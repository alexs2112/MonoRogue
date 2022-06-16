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
            Glyphs.Add("Undead", content.Load<Texture2D>("Creatures/Undead"));
            Glyphs.Add("Grunt", content.Load<Texture2D>("Creatures/Grunt"));
            Glyphs.Add("Thug", content.Load<Texture2D>("Creatures/Thug"));
            Glyphs.Add("Haunt", content.Load<Texture2D>("Creatures/Haunt"));

            Equipment = equipment;
        }

        public Creature NewPlayer(World world, int x, int y) {
            Texture2D glyph = PlayerGlyph.GetDefaultGlyph();
            Creature c = new Creature("Player", glyph, Color.SkyBlue);
            c.SetStats(12, (2, 3));
            c.SetDescription("A hapless adventurer.");
            c.AI = new PlayerAI(c);
            c.IsPlayer = true;
            c.World = world;
            world.Creatures.Add(c);
            world.Player = c;
            c.MoveTo(x, y);
            this.Player = c;
            return c;
        }

        public Creature NewWeakCreature(System.Random random, World world, int x, int y) {
            switch(random.Next(3)) {
                case 0: return NewRat(world, x, y);
                case 1: return NewPig(world, x, y);
                case 2: return NewUndead(world, x, y);
                default: return null;
            }
        }
        public Creature NewMediumCreature(System.Random random, World world, int x, int y) {
            switch(random.Next(3)) {
                case 0: return NewUndead(world, x, y);
                case 1: return NewGrunt(world, x, y);
                case 2: return NewThug(world, x, y);
                default: return null;
            }
        }
        public Creature NewStrongCreature(System.Random random, World world, int x, int y) {
            switch(random.Next(2)) {
                case 0: return NewThug(world, x, y);
                case 1: return NewHaunt(world, x, y);
                default: return null;
            }
        }

        private Creature NewRat(World world, int x, int y) {
            Creature c = new Creature("Rat", Glyphs["Rat"], Color.SaddleBrown);
            c.SetStats(6, (1,2), 6, 6);
            c.SetDescription("A large, aggressive, and very dirty rodent. It moves and attacks quite quickly.");
            c.SetAttackText("bite");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 1;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewPig(World world, int x, int y) {
            Creature c = new Creature("Pig", Glyphs["Pig"], Color.Pink);
            c.SetStats(8, (1, 2));
            c.SetDescription("A stout-bodied, short-legged, omnivorous mammal. It is easily provoked.");
            c.AI = new PigAI(c, Player);
            c.World = world;
            c.Difficulty = 1;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewUndead(World world, int x, int y) {
            Creature c = new Creature("Undead", Glyphs["Undead"], Color.PaleGreen);
            c.SetStats(8, (2, 3), 15, 15);
            c.SetDescription("A decaying corpse of something long deceased. It is animate and angry, however very slow.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 2;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewGrunt(World world, int x, int y) {
            Creature c = new Creature("Grunt", Glyphs["Grunt"], Color.PaleVioletRed);
            c.SetStats(8, (2, 5), 12, 10);
            c.SetDescription("A basic grunt of the dungeon. It is small in stature and unintelligent.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 2;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewThug(World world, int x, int y) {
            Creature c = new Creature("Thug", Glyphs["Thug"], Color.BlueViolet);
            c.SetStats(10, (3, 5), 12, 12);
            c.SetDescription("Another inhabitant of the dungeon. It is bulkier than you and has menacing shoulder spines.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 3;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewHaunt(World world, int x, int y) {
            Creature c = new Creature("Haunt", Glyphs["Haunt"], Color.Purple);
            c.SetStats(10, (2, 4), 8, 10);
            c.SetDescription("An amorphous being, it floats through the air. It hurls spectral fire.");
            c.SetBaseRange(4);
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 3;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }
    }
}
