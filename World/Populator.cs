using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class Populator {
        private System.Random Random;
        private World World;
        private CreatureFactory Creature;
        private EquipmentFactory Equipment;

        // A way to iterate over each dungeon region, keeping track of expected stats to spawn items and creatures to keep a certain level of difficulty
        public Populator(System.Random random, World world, CreatureFactory creatureFactory, EquipmentFactory equipmentFactory) {
            Random = random;
            World = world;
            Creature = creatureFactory;
            Equipment = equipmentFactory;
        }

        public void Populate(Creature player, Region start, Region end, List<Region> regions) {
            int lowDepth = end.Depth / 3;
            int medDepth = lowDepth * 2;
            int highDepth = end.Depth;

            // The starting room should contain a weak item and no enemies
            Point p = start.GetEmptyTile(Random, World);
            Item item = Equipment.WeakItem(Random);
            World.Items.Add(p, item); 

            // Keep track of difficulty for each depth level so we can award items to the player the more they fight
            int[] difficulty = new int[highDepth + 1];
            List<Region>[] depths = new List<Region>[highDepth + 1];

            // Spawn enemies in each region as a function of the region size
            foreach (Region r in regions) {
                if (depths[r.Depth] == null) {
                    depths[r.Depth] = new List<Region>();
                }
                depths[r.Depth].Add(r);
                if (r.Depth == 0) { continue; }

                int enemies = r.Size() / (Constants.RoomMinSize * Constants.RoomMinSize / 2);
                if (enemies > 3) { enemies--; }
                if (enemies > 5) { enemies--; }
                for (int i = 0; i < enemies; i++) {
                    Point tile = r.GetEmptyTile(Random, World);
                    if (tile.X == -1) { break; }

                    Creature c;
                    if (r.Depth <= lowDepth) {
                        c = Creature.NewWeakCreature(Random, World, tile.X, tile.Y);
                    } else if (r.Depth <= medDepth) {
                        c = Creature.NewMediumCreature(Random, World, tile.X, tile.Y);
                    } else {
                        c = Creature.NewStrongCreature(Random, World, tile.X, tile.Y);
                    }
                    difficulty[r.Depth] += c.Difficulty;
                }
            }

            // Spawn items by depth as a function of difficulty
            int mod = 0;
            for (int i = 0; i <= highDepth; i++) {
                int current = difficulty[i] + mod;

                int divisor;
                // Converting total difficulty to number of items
                if (i <= lowDepth) { divisor = 3; }
                else if (i <= medDepth) { divisor = 5; }
                else { divisor = 8; }
                
                int num = current / divisor;
                mod = current % divisor;

                for (int j = 0; j < num; j++) {
                    // Go to the regions of depth i, select one at random
                    Region r = depths[i][Random.Next(depths[i].Count)];

                    Point tile = r.GetEmptyTile(Random, World);
                    Item e;
                    if (r.Depth <= lowDepth) {
                        e = Equipment.WeakItem(Random);
                    } else if (r.Depth <= medDepth) {
                        e = Equipment.MediumItem(Random);
                    } else {
                        e = Equipment.StrongItem(Random);
                    }
                    World.Items.Add(tile, e);
                }
            }

            // Do the same with food
            mod = 0;
            for (int i = 0; i <= highDepth; i++) {
                int current = difficulty[i] + mod;

                int divisor;
                // Converting total difficulty to number of items
                if (i <= lowDepth) { divisor = 3; }
                else if (i <= medDepth) { divisor = 6; }
                else { divisor = 10; }
                
                int num = current / divisor;
                mod = current % divisor;

                for (int j = 0; j < num; j++) {
                    // Go to the regions of depth i, select one at random
                    Region r = depths[i][Random.Next(depths[i].Count)];

                    Point tile = r.GetEmptyTile(Random, World);
                    Food f = Food.RandomFood(Random);
                    World.Items.Add(tile, f);
                }
            }
        }
    }
}