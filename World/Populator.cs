using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class Populator {
        private System.Random Random;
        private World World;
        private CreatureFactory Creature;
        private EquipmentFactory Equipment;

        private int LowDepth;
        private int MedDepth;
        private int HighDepth;

        // A way to iterate over each dungeon region, keeping track of expected stats to spawn items and creatures to keep a certain level of difficulty
        public Populator(System.Random random, World world, CreatureFactory creatureFactory, EquipmentFactory equipmentFactory) {
            Random = random;
            World = world;
            Creature = creatureFactory;
            Equipment = equipmentFactory;
        }

        public void SetDepth(int low, int med, int high) {
            LowDepth = low;
            MedDepth = med;
            HighDepth = high;
        }

        public void Populate(Creature player, Region start, Region end, List<Region> regions) {
            // The starting room should contain a weak item and no enemies
            Point p = start.GetEmptyTile(Random, World);
            Item item = Equipment.WeakItem(Random);
            World.Items.Add(p, item);

            p = start.GetEmptyTile(Random, World);
            Food startFood = Food.Apple;
            World.Items.Add(p, startFood);

            if (Constants.Difficulty < 3) {
                p = start.GetEmptyTile(Random, World);
                item = Equipment.WeakItem(Random);
                World.Items.Add(p, item); 
            }

            // Keep track of difficulty for each depth level so we can award items to the player the more they fight
            int[] difficulty = new int[HighDepth + 1];
            List<Region>[] depths = new List<Region>[HighDepth + 1];

            // Spawn enemies in each region as a function of the region size
            foreach (Region r in regions) {
                if (depths[r.Depth] == null) {
                    depths[r.Depth] = new List<Region>();
                }
                depths[r.Depth].Add(r);

                // The first region contains no enemies
                if (r.Depth == 0) { continue; }

                // The last region contains only the Warden
                if (r == end) {
                    Point tile = r.GetEmptyTile(Random, World);
                    if (tile.X == -1) { break; }
                    Creature c = Creature.NewWarden(World, tile.X, tile.Y);
                    difficulty[r.Depth] += c.Difficulty;
                    continue;
                }

                int enemies = (int)(System.Math.Ceiling(System.Math.Log(r.Size(), 1.8))) - 5 + Constants.Difficulty;
                if (r.Depth == 1) { enemies--; }
                for (int i = 0; i < enemies; i++) {
                    Point tile = r.GetEmptyTile(Random, World);
                    if (tile.X == -1) { break; }

                    Creature c;
                    if (r.Depth <= LowDepth) {
                        c = Creature.NewWeakCreature(World, tile.X, tile.Y);
                    } else if (r.Depth <= MedDepth) {
                        c = Creature.NewMediumCreature(World, tile.X, tile.Y);
                    } else {
                        c = Creature.NewStrongCreature(World, tile.X, tile.Y);
                    }
                    difficulty[r.Depth] += c.Difficulty;
                }
            }

            // Spawn heartstones at the end of each dungeon "tier"
            for (int i = 0; i < 2; i++) {
                int d;
                if (i == 0) { d = LowDepth; }
                else { d = MedDepth; } // i == 1
                Region r = depths[d][Random.Next(depths[d].Count)];
                Point tile = r.GetEmptyTile(Random, World);
                World.Items.Add(tile, Heartstone.GetHeartstone());
            }

            // Spawn items by depth as a function of difficulty
            int mod = 2;
            for (int i = 0; i <= HighDepth; i++) {
                int current = difficulty[i] + mod;

                int divisor;
                // Converting total difficulty to number of items
                if (i <= LowDepth) { divisor = 5 - (Constants.Difficulty - 3); }
                else if (i <= MedDepth) { divisor = 10 - 2 * (Constants.Difficulty - 3); }
                else { divisor = 15 - 3 * (Constants.Difficulty - 3); }
                
                int num = current / divisor;
                mod = current % divisor;

                for (int j = 0; j < num; j++) {
                    // Go to the regions of depth i, select one at random
                    Region r = depths[i][Random.Next(depths[i].Count)];

                    Point tile = r.GetEmptyTile(Random, World);
                    Item e;
                    if (r.Depth <= LowDepth) {
                        e = Equipment.WeakItem(Random);
                    } else if (r.Depth <= MedDepth) {
                        e = Equipment.MediumItem(Random);
                    } else {
                        e = Equipment.StrongItem(Random);
                    }
                    World.Items.Add(tile, e);
                }
            }

            // Do the same with food
            mod = 4;
            for (int i = 0; i <= HighDepth; i++) {
                int current = difficulty[i] + mod;

                int divisor;
                // Converting total difficulty to number of items
                if (i <= LowDepth) { divisor = 12 - (Constants.Difficulty - 3); }
                else if (i <= MedDepth) { divisor = 17 - 2 * (Constants.Difficulty - 3); }
                else { divisor = 26 - 3 * (Constants.Difficulty - 3); }
                
                int num = current / divisor;
                mod = current % divisor;

                for (int j = 0; j < num; j++) {
                    // Go to the regions of depth i, select one at random
                    Region r = depths[i][Random.Next(depths[i].Count)];

                    Point tile = r.GetEmptyTile(Random, World);
                    if (tile.X == -1) { continue; }
                    Food f = Food.RandomFood(Random, r.Depth <= LowDepth ? 0 : (r.Depth <= MedDepth ? 1 : 2));
                    World.Items.Add(tile, f);
                }
            }
        }
    }
}
