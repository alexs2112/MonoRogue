using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class GameLoader {
        private EquipmentFactory Equipment;
        private CreatureFactory Creature;
        private SaveData Data;
        public int Seed;
        public int Difficulty;
        public WorldData WorldData;
        public World World;

        public GameLoader() {
            string json = File.ReadAllText(Constants.SavegamePath);
            Data = JsonSerializer.Deserialize<SaveData>(json);
            Seed = Data.Seed;
            Difficulty = Data.Difficulty;
            WorldData = Data.World;
        }

        public static bool CanLoad() {
            return File.Exists(Constants.SavegamePath);
        }

        public void LoadGame(World world, WorldView worldView, EquipmentFactory equipment, CreatureFactory creature) {
            Equipment = equipment;
            Creature = creature;

            world.Creatures.Clear();
            world.Items.Clear();

            OpenDoors(world);
            Bloodstains(world);

            foreach (CreatureData c in Data.World.Creatures) {
                ParseCreature(c, world, Data.PlayerData);
            }
            foreach (CooldownData c in Data.Cooldowns) {
                ParseCooldown(c, world);
            }
            foreach (SummonData s in Data.Summons) {
                ParseSummon(s, world);
            }
            foreach (ItemData i in Data.World.Items) {
                world.Items.Add(new Point(i.Position.X, i.Position.Y), ParseItem(i));
            }

            for (int x = 0; x < world.Width; x++) {
                for (int y = 0; y < world.Height; y++) {
                    worldView.HasSeen[x, y] = WorldData.HasSeen[x].Map[y];
                }
            }

            World = world;
        }

        private Creature ParseCreature(CreatureData creature, World world, PlayerData playerData) {
            Creature c = Creature.CreatureByName(creature.Name, world, creature.Position.X, creature.Position.Y);
            if (c.IsPlayer) { c.MaxHP = playerData.MaxHealth; }
            c.HP = creature.Health;
            c.SetDefense(creature.Defense);

            if (creature.Weapon != null) {
                c.Weapon = (Weapon)ParseItem(creature.Weapon);
                Texture2D next = EnemyGlyph.GetGlyph(c);
                if (next != null) { c.SetGlyph(next); }
            }
            if (creature.Armor != null) {
                c.Armor = (Armor)ParseItem(creature.Armor);
            }

            if (c.IsPlayer) {
                c.SetGlyph(PlayerGlyph.GetUpdatedGlyph(c));
            }

            if (creature.TargetTile != null && creature.TargetTile.X != -1) {
                c.AI.GiveTargetTile(new Point(creature.TargetTile.X, creature.TargetTile.Y));
            }

            c.TurnTimer = creature.TurnTimer;
            if (c.IsPlayer) {
                if (playerData.HasKey) {
                    c.HasKey = true;
                    world.Tiles[world.Exit.X, world.Exit.Y] = Feature.ExitOpen;
                }
            }
            return c;
        }

        private void ParseCooldown(CooldownData cooldown, World world) {
            Creature c = world.GetCreatureAt(cooldown.Location.X, cooldown.Location.Y);
            if (c.AI.IsTank) {
                ((TankAI)c.AI).Cooldown = cooldown.Value;
            } else if (c.AI.IsWarden) {
                ((WardenAI)c.AI).AlarmCooldown = cooldown.Value;
            }
        }

        private void ParseSummon(SummonData data, World world) {
            Creature c = world.GetCreatureAt(data.Summoner.X, data.Summoner.Y);
            foreach (PointData p in data.Summons) {
                ((CultistAI)c.AI).Summons.Add(world.GetCreatureAt(p.X, p.Y));
            }
            ((CultistAI)c.AI).SummonsLeft = data.SummonsLeft;
        }

        private Item ParseItem(ItemData item) {
            Item i = Item.GetItemByName(item.Name, Equipment);
            if (i.IsArmor) { ((Armor)i).Defense = item.Defense; }
            return i;
        }

        private void OpenDoors(World world) {
            foreach (PointData p in Data.World.OpenDoors) {
                world.Tiles[p.X, p.Y] = Feature.DoorOpen;
            }
        }

        private void Bloodstains(World world) {
            foreach (PointData p in Data.World.Bloodstains) {
                world.Bloodstains[p.X, p.Y] = true;
            }
        }
    }
}
