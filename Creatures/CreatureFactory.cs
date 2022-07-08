using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue
{
    public class CreatureFactory
    {
        private EquipmentFactory Equipment;
        private Dictionary<string, Texture2D> Glyphs;

        // Keep track of who the player is after they are created to give them to enemy AIs
        private Creature Player;
        public void SetPlayer(Creature player) { Player = player; }

        private System.Random Random;

        public CreatureFactory(ContentManager content, EquipmentFactory equipment, System.Random random)
        {
            Glyphs = new Dictionary<string, Texture2D>();

            // Store all loaded sprites in a dictionary so we don't need to store a ton of variables
            Glyphs.Add("Rat", content.Load<Texture2D>("Creatures/Rat"));
            Glyphs.Add("Pig", content.Load<Texture2D>("Creatures/Pig"));
            Glyphs.Add("Spawn", content.Load<Texture2D>("Creatures/Spawn"));
            Glyphs.Add("Undead", content.Load<Texture2D>("Creatures/Undead"));

            Glyphs.Add("Grunt", content.Load<Texture2D>("Creatures/Grunt"));
            Glyphs.Add("Imp", content.Load<Texture2D>("Creatures/Imp"));
            Glyphs.Add("Thug", content.Load<Texture2D>("Creatures/Thug-Spear")); // Default of spear
            Glyphs.Add("Brute", content.Load<Texture2D>("Creatures/Brute"));

            Glyphs.Add("Haunt", content.Load<Texture2D>("Creatures/Haunt"));
            Glyphs.Add("Gatekeeper", content.Load<Texture2D>("Creatures/Gatekeeper-Sword")); // Default of sword
            Glyphs.Add("Tank", content.Load<Texture2D>("Creatures/Tank"));
            Glyphs.Add("Cultist", content.Load<Texture2D>("Creatures/Cultist"));

            Glyphs.Add("Warden", content.Load<Texture2D>("Creatures/Warden-Sword")); // Default of sword

            Equipment = equipment;
            Random = random;
        }

        public Creature CreatureByName(string name, World world, int x, int y) {
            switch(name) {
                case "Player": return NewPlayer(world, x, y);
                case "Rat": return NewRat(world, x, y);
                case "Pig": return NewPig(world, x, y);
                case "Angry Pig": return NewAngryPig(world, x, y);
                case "Spawn": return NewSpawn(world, x, y);
                case "Undead": return NewUndead(world, x, y);
                case "Grunt": return NewGrunt(world, x, y);
                case "Imp": return NewImp(world, x, y);
                case "Thug": return NewThug(world, x, y);
                case "Brute": return NewBrute(world, x, y);
                case "Haunt": return NewHaunt(world, x, y);
                case "Gatekeeper": return NewGatekeeper(world, x, y);
                case "Tank": return NewTank(world, x, y);
                case "Cultist": return NewCultist(world, x, y);
                case "Warden": return NewWarden(world, x, y);
                default: throw new System.Exception($"Cannot find {name}.");
            }
        }

        public Creature NewPlayer(World world, int x, int y)
        {
            Texture2D glyph = PlayerGlyph.GetDefaultGlyph();
            Creature c = new Creature("Player", glyph, Color.SkyBlue);
            c.SetStats(12, 0, (2, 3));
            c.SetDescription("A hapless adventurer.");

            // It is possible for this to overflow the screen with 5 hearts and wardens armor
            if (Constants.Difficulty == 1) { c.ModifyDefense(4); }
            c.AI = new PlayerAI(c);
            c.IsPlayer = true;
            c.World = world;
            world.Creatures.Add(c);
            world.Player = c;
            c.MoveTo(x, y);
            this.Player = c;
            return c;
        }

        public Creature NewWeakCreature(World world, int x, int y)
        {
            switch (Random.Next(4))
            {
                case 0: return NewRat(world, x, y);
                case 1: return NewPig(world, x, y);
                case 2: return NewSpawn(world, x, y);
                case 3: return NewUndead(world, x, y);
                default: return null;
            }
        }
        public Creature NewMediumCreature(World world, int x, int y)
        {
            switch (Random.Next(5))
            {
                case 0: return NewUndead(world, x, y);
                case 1: return NewGrunt(world, x, y);
                case 2: return NewImp(world, x, y);
                case 3: return NewThug(world, x, y);
                case 4: return NewBrute(world, x, y);
                default: return null;
            }
        }
        public Creature NewStrongCreature(World world, int x, int y)
        {
            switch (Random.Next(5))
            {
                case 0: return NewBrute(world, x, y);
                case 1: return NewHaunt(world, x, y);
                case 2: return NewGatekeeper(world, x, y);
                case 3: return NewTank(world, x, y);
                case 4: return NewCultist(world, x, y);
                default: return null;
            }
        }

        private Creature NewRat(World world, int x, int y)
        {
            Creature c = new Creature("Rat", Glyphs["Rat"], Color.SaddleBrown);
            c.SetStats(4, 0, (1, 2), -2);
            c.ModifyMovementDelay(-2);
            c.SetDescription("A large, aggressive, and very dirty rodent. It moves and attacks quite quickly.");
            c.SetAttackText("bite");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 1;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewPig(World world, int x, int y)
        {
            Creature c = new Creature("Pig", Glyphs["Pig"], Color.Pink);
            c.SetStats(6, 0, (1, 2));
            c.SetDescription("A stout-bodied, short-legged, omnivorous mammal. It is easily provoked.");
            c.SetAbilityText("Will become angry if attacked.");
            c.AI = new PigAI(c, Player);
            c.World = world;
            c.Difficulty = 1;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewAngryPig(World world, int x, int y) {
            Creature c = NewPig(world, x, y);
            ((PigAI)c.AI).OnHit(world, Player);
            return c;
        }

        private Creature NewSpawn(World world, int x, int y)
        {
            Creature c = new Creature("Spawn", Glyphs["Spawn"], Color.LightSalmon);
            c.SetStats(6, 4, (2, 3), 2);
            c.SetDescription("A creature of the dungeon. It is vaguely humanoid, however it is rather small and does not have a mouth.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 2;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewUndead(World world, int x, int y)
        {
            Creature c = new Creature("Undead", Glyphs["Undead"], Color.PaleGreen);
            c.SetStats(8, 4, (2, 3), 5);
            c.SetDescription("A decaying corpse of something long deceased. It is animate and angry, however very slow.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 2;
            c.Bleeds = false;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewGrunt(World world, int x, int y)
        {
            Creature c = new Creature("Grunt", Glyphs["Grunt"], Color.PaleVioletRed);
            c.SetStats(8, 4, (2, 3), 2);
            c.SetDescription("A basic entity in the dungeon. It is small in stature, unintelligent, and quite aggressive.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 3;
            // 50% chance to spawn with a weapon
            if (Random.Next(2) < 1) {
                c.Difficulty++;
                switch(Random.Next(3)) {
                    case 0: 
                        c.Equip(Equipment.NewDagger());
                        break;
                    case 1: 
                        c.Equip(Equipment.NewSword());
                        break;
                    case 2:
                        c.Equip(Equipment.NewShortbow());
                        break;
                }
            }
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewImp(World world, int x, int y) { return NewImp(world, x, y, Color.Violet); }
        public Creature NewImp(World world, int x, int y, Color color)
        {
            // Set as public so the cultist can summon one
            Creature c = new Creature("Imp", Glyphs["Imp"], color);
            c.SetStats(2, 6, (2, 3), 3);
            c.SetBaseRange(4);
            c.SetDescription("A mystical creature that can hover in the air. It casts spells that can inflict pain from a distance.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 3;
            c.Bleeds = false;
            c.BaseProjectile = Projectile.Type.Spell;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewThug(World world, int x, int y)
        {
            Creature c = new Creature("Thug", Glyphs["Thug"], Color.SeaGreen);
            c.SetStats(6, 6, (3, 5), 3);
            Item e;
            string w;
            if (Random.Next(3) == 0) {
                e = Equipment.NewBident();
                w = "bident";
            } else {
                e = Equipment.NewFalchion();
                w = "falchion";
            }
            c.Equip(e);
            c.SetDescription($"A humanoid that looks to be related to a type of demon. It hops back and forth and wields a flaming {w}.");
            c.SetAbilityText("Becomes cowardly without any defense.");
            c.AI = new ThugAI(c, Player);
            c.World = world;
            c.Difficulty = 4;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewBrute(World world, int x, int y)
        {
            Creature c = new Creature("Brute", Glyphs["Brute"], Color.Coral);
            c.SetStats(8, 6, (3, 5), 4, 1);
            c.SetDescription("An enemy that is larger than you. It is covered with sharp coral spines.");
            c.SetAbilityText("Deals damage to you when you hit its defense in melee.");
            c.AI = new BruteAI(c, Player);
            c.World = world;
            c.Difficulty = 4;
            // 50% chance to spawn with a weapon
            if (Random.Next(2) < 1) {
                c.Difficulty++;
                switch(Random.Next(2)) {
                    case 0: 
                        c.Equip(Equipment.NewWarhammer());
                        break;
                    case 1: 
                        c.Equip(Equipment.NewWarAxe());
                        break;
                }
            }
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewHaunt(World world, int x, int y)
        {
            Creature c = new Creature("Haunt", Glyphs["Haunt"], Color.Purple);
            c.SetStats(4, 8, (2, 4), 3, 1);
            c.SetDescription("An amorphous being, it slowly floats through the air. It hurls spectral fire from a distance.");
            c.SetBaseRange(5);
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 5;
            c.Bleeds = false;
            c.BaseProjectile = Projectile.Type.Spell;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewGatekeeper(World world, int x, int y)
        {
            Creature c = new Creature("Gatekeeper", Glyphs["Gatekeeper"], Color.MediumTurquoise);
            c.SetStats(12, 0, (4, 6), 2);
            Item e;
            string w;
            switch(Random.Next(4)) {
            case 0:
                e = Equipment.NewMorningstar();
                w = "morningstar";
                break;
            case 1:
                e = Equipment.NewBattleAxe();
                w = "battleaxe";
                break;
            default:
                e = Equipment.NewSawtooth();
                w = "sawtooth blade";
                break;
            }
            c.Equip(e);
            c.Equip(Random.Next(2) == 0 ? Equipment.NewChainMail() : Equipment.NewDragonscale());
            c.SetDescription($"An insectoid creature that stands on two legs. It has four beady eyes, pincers, and wields a {w}.");
            c.AI = new BasicAI(c, Player);
            c.World = world;
            c.Difficulty = 5;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewTank(World world, int x, int y)
        {
            Creature c = new Creature("Tank", Glyphs["Tank"], Color.SkyBlue);
            c.SetStats(12, 12, (3, 6), 5, 2);
            c.SetDescription("A large, heavily armored creature with long, grasping arms.");
            c.SetAbilityText("Can grab you and pull you adjacent.");
            c.AI = new TankAI(c, Player);
            c.World = world;
            c.Difficulty = 6;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        private Creature NewCultist(World world, int x, int y)
        {
            Creature c = new Creature("Cultist", Glyphs["Cultist"], Color.Violet);
            c.SetStats(6, 12, (3, 5), 3);
            c.SetDescription("A cloaked figure. Its bone faceplate peers from under its hood, etched in glowing runes.");
            c.SetAbilityText("Can summon imps.");
            c.AI = new CultistAI(c, Player, this);
            c.World = world;
            c.Difficulty = 6;
            // 33% chance to spawn with a weapon
            if (Random.Next(3) < 1) {
                switch(Random.Next(2)) {
                    case 0: 
                        c.Equip(Equipment.NewRitualDagger());
                        break;
                    default: 
                        c.Equip(Equipment.NewCultistStaff());
                        c.ModifyAttackDelay(2);
                        break;
                }
            }
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }

        public Creature NewWarden(World world, int x, int y)
        {
            Creature c = new Creature("Warden", Glyphs["Warden"], Color.Red);
            c.SetStats(16, 0, (5, 9));
            Item e;
            switch(Random.Next(4)) {
            case 0:
                e = Equipment.NewMorningstar();
                break;
            case 1:
                e = Equipment.NewBattleAxe();
                break;
            case 2:
                e = Equipment.NewGreatspear();
                break;
            default:
                e = Equipment.NewGreatsword();
                break;    
            }
            c.SetDescription("The warden of the dungeon. It is fully armed and armored in red, runic metal. From its belt you can see a dangling golden key.");
            c.SetAbilityText("Will raise the alarm in the dungeon.");
            c.Equip(e);
            c.Equip(Equipment.NewWardensPlate());
            c.AI = new WardenAI(c, Player);
            c.World = world;
            c.Difficulty = 7;
            world.Creatures.Add(c);
            c.MoveTo(x, y);
            return c;
        }
    }
}
