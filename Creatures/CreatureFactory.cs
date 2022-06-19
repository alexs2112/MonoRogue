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
            Glyphs.Add("Spawn", content.Load<Texture2D>("Creatures/Spawn"));
            Glyphs.Add("Undead", content.Load<Texture2D>("Creatures/Undead"));

            Glyphs.Add("Grunt", content.Load<Texture2D>("Creatures/Grunt"));
            Glyphs.Add("Imp", content.Load<Texture2D>("Creatures/Imp"));
            Glyphs.Add("Thug", content.Load<Texture2D>("Creatures/Thug"));
            Glyphs.Add("Brute", content.Load<Texture2D>("Creatures/Brute"));

            Glyphs.Add("Haunt", content.Load<Texture2D>("Creatures/Haunt"));
            Glyphs.Add("Gatekeeper", content.Load<Texture2D>("Creatures/Gatekeeper"));
            Glyphs.Add("Tank", content.Load<Texture2D>("Creatures/Tank"));
            Glyphs.Add("Cultist", content.Load<Texture2D>("Creatures/Cultist"));
            Glyphs.Add("CultistArmed", content.Load<Texture2D>("Creatures/Cultist2"));

            Glyphs.Add("Warden", content.Load<Texture2D>("Creatures/Warden"));

            Equipment = equipment;
        }

        public Creature NewPlayer(World world, int x, int y) {
            Texture2D glyph = PlayerGlyph.GetDefaultGlyph();
            Creature c = new Creature("Player", glyph, Color.SkyBlue);
            c.SetStats(12, 0, (2, 3));
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
            switch(random.Next(4)) {
                case 0: return NewRat(world, x, y);
                case 1: return NewPig(world, x, y);
                case 2: return NewSpawn(world, x, y);
                case 3: return NewUndead(world, x, y);
                default: return null;
            }
        }
        public Creature NewMediumCreature(System.Random random, World world, int x, int y) {
            switch(random.Next(5)) {
                case 0: return NewUndead(world, x, y);
                case 1: return NewGrunt(world, x, y);
                case 2: return NewImp(world, x, y);
                case 3: return NewThug(world, x, y);
                case 4: return NewBrute(world, x, y);
                default: return null;
            }
        }
        public Creature NewStrongCreature(System.Random random, World world, int x, int y) {
            switch(random.Next(5)) {
                case 0: return NewBrute(world, x, y);
                case 1: return NewHaunt(world, x, y);
                case 2: return NewGatekeeper(world, x, y);
                case 3: return NewTank(world, x, y);
                case 4: return NewCultist(world, x, y);
                default: return null;
            }
        }

        private Creature NewRat(World world, int x, int y) {
            Creature c = new Creature("Rat", Glyphs["Rat"], Color.SaddleBrown);
            c.SetStats(4, 0, (1,2), 6, 8);
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
            c.SetStats(6, 0, (1, 2));
            c.SetDescription("A stout-bodied, short-legged, omnivorous mammal. It is easily provoked.");
            c.AI = new PigAI(c, Player);
            c.World = world;
            c.Difficulty = 1;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewSpawn(World world, int x, int y) {
            Creature c = new Creature("Spawn", Glyphs["Spawn"], Color.LightSalmon);
            c.SetStats(6, 4, (2, 3), 10, 12);
            c.SetDescription("A creature of the dungeon. It is vaguely humanoid, however it is rather small and does not have a mouth.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 2;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewUndead(World world, int x, int y) {
            Creature c = new Creature("Undead", Glyphs["Undead"], Color.PaleGreen);
            c.SetStats(8, 4, (2, 3), 15, 15);
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
            c.SetStats(8, 4, (2, 4), 10, 12);
            c.SetDescription("A basic entity in the dungeon. It is small in stature, unintelligent, and quite aggressive.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 3;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewImp(World world, int x, int y) { return NewImp(world, x, y, Color.Violet); }
        public Creature NewImp(World world, int x, int y, Color color) {
            // Set as public so the cultist can summon one
            Creature c = new Creature("Imp", Glyphs["Imp"], color);
            c.SetStats(2, 6, (1, 2), 10, 16);
            c.SetBaseRange(4);
            c.SetDescription("A mystical creature that can hover in the air. It casts spells that can inflict pain from a distance.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 3;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewThug(World world, int x, int y) {
            Creature c = new Creature("Thug", Glyphs["Thug"], Color.SeaGreen);
            c.SetStats(6, 6, (3, 5), 12, 14);
            c.SetBaseRange(2);
            c.Equip(Equipment.NewBident());
            c.SetDescription("A humanoid that looks to be related to a type of demon. It hops back and forth and wields a flaming bident.");
            c.AI = new ThugAI(c, Player);
            c.World = world;
            c.Difficulty = 4;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewBrute(World world, int x, int y) {
            Creature c = new Creature("Brute", Glyphs["Brute"], Color.Coral);
            c.SetStats(8, 6, (3, 5), 14, 14);
            c.SetDescription("An enemy that is larger than you. It is covered with sharp coral spines.");
            c.AI = new BruteAI(c, Player);
            c.World = world;
            c.Difficulty = 4;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewHaunt(World world, int x, int y) {
            Creature c = new Creature("Haunt", Glyphs["Haunt"], Color.Purple);
            c.SetStats(4, 8, (2, 4), 20, 20);
            c.SetDescription("An amorphous being, it slowly floats through the air. It hurls spectral fire from a distance.");
            c.SetBaseRange(4);
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 5;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewGatekeeper(World world, int x, int y) {
            Creature c = new Creature("Gatekeeper", Glyphs["Gatekeeper"], Color.MediumTurquoise);
            c.SetStats(12, 0, (4, 7));
            c.Equip(Equipment.NewSawtooth());
            c.Equip(Equipment.StrongArmor(new System.Random()));
            c.SetDescription("An insectoid creature that stands on two legs. It has four beady eyes, pincers, and wields a sawtooth blade.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 5;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewTank(World world, int x, int y) {
            Creature c = new Creature("Tank", Glyphs["Tank"], Color.SkyBlue);
            c.SetStats(12, 12, (3, 6), 15, 15);
            c.SetDescription("A large, heavily armored creature with long and sturdy arms. It can grab you and pull you close.");
            c.AI = new TankAI(c, Player);
            c.World = world;
            c.Difficulty = 6;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewCultist(World world, int x, int y) {
            string glyph;
            Item item;
            if (new System.Random().Next(3) < 1) {
                glyph = "CultistArmed";
                item = Equipment.NewCultistStaff();
            } else {
                glyph = "Cultist";
                item = null;
            }
            Creature c = new Creature("Cultist", Glyphs[glyph], Color.Violet);
            c.SetStats(6, 12, (3, 6), 12, 10);
            c.SetDescription("A cloaked figure. Its bone faceplate peers from under its hood, etched in glowing runes.");
            c.Equip(item);
            c.AI = new CultistAI(c, Player, this);
            c.World = world;
            c.Difficulty = 6;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        public Creature NewWarden(World world, int x, int y) {
            Creature c = new Creature("Warden", Glyphs["Warden"], Color.Red);
            c.SetStats(16, 16, (5, 9), 12, 12);
            c.SetDescription("The warden of the dungeon. It is fully armed and armored in red, runic metal. From its belt you can see a dangling golden key.");
            c.Equip(Equipment.NewGreatsword());
            c.AI = new WardenAI(c, Player);
            c.World = world;
            c.Difficulty = 7;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }
    }
}
