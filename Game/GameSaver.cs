using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class GameSaver {
        private int Seed;
        private World World;
        private Creature Player;
        private bool[,] HasSeen;

        public GameSaver(int seed, World world, WorldView worldView) {
            Seed = seed;
            World = world;
            Player = world.Player;
            HasSeen = worldView.HasSeen;
        }

        public void SaveGame() {
            SaveData data = new SaveData {
                Seed = Seed,
                Difficulty = Constants.Difficulty
            };

            List<CooldownData> cooldowns = new List<CooldownData>();
            List<SummonData> summons = new List<SummonData>();
            List<CreatureData> creatures = new List<CreatureData>();
            foreach (Creature c in World.Creatures) {
                creatures.Add(PrepareCreature(c));

                if (c.IsPlayer) {
                    data.PlayerData = new PlayerData {
                        HasKey = c.HasKey,
                        MaxHealth = c.MaxHP
                    };
                }

                // Keep track of some AI specific things
                if (c.AI.IsWarden) {
                    cooldowns.Add(new CooldownData {
                        Location = new PointData { X = c.X, Y = c.Y },
                        Value = ((WardenAI)c.AI).AlarmCooldown
                    });
                } else if (c.AI.IsTank) {
                    cooldowns.Add(new CooldownData {
                        Location = new PointData { X = c.X, Y = c.Y },
                        Value = ((TankAI)c.AI).Cooldown
                    });
                } else if (c.AI.IsCultist) {
                    List<PointData> children = new List<PointData>();
                    foreach (Creature s in ((CultistAI)c.AI).Summons) {
                        children.Add(new PointData { X = s.X, Y = s.Y });
                    }
                    summons.Add(new SummonData {
                        Summoner = new PointData { X = c.X, Y = c.Y },
                        Summons = children,
                        SummonsLeft = ((CultistAI)c.AI).SummonsLeft
                    });
                }
            }
            data.Cooldowns = cooldowns;
            data.Summons = summons;

            List<ItemData> items = new List<ItemData>();
            foreach (Point p in World.Items.Keys) {
                items.Add(new ItemData {
                    Name = World.Items[p].Name,
                    Position = new PointData { X = p.X, Y = p.Y },
                    Defense = World.Items[p].IsArmor ? ((Armor)World.Items[p]).Defense : 0
                });
            }

            List<PointData> doors = new List<PointData>();
            List<PointData> blood = new List<PointData>();
            List<ColumnData> hasSeen = new List<ColumnData>();
            for (int x = 0; x < World.Width; x++) {
                bool[] nextColumn = new bool[World.Height];
                for (int y = 0; y < World.Height; y++) {
                    if (World.GetTile(x, y) == Feature.DoorOpen) {
                        doors.Add(new PointData { X = x, Y = y });
                    }
                    if (World.Bloodstains[x,y]) {
                        blood.Add(new PointData { X = x, Y = y });
                    }
                    nextColumn[y] = HasSeen[x, y];
                }
                hasSeen.Add(new ColumnData { Map = nextColumn });
            }

            data.World = new WorldData {
                Width = Constants.WorldWidth,
                Height = Constants.WorldHeight,
                Creatures = creatures,
                Items = items,
                OpenDoors = doors,
                Bloodstains = blood,
                HasSeen = hasSeen
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = Constants.IndentedSave });
            File.WriteAllText(Constants.SavegamePath, json);
        }

        private CreatureData PrepareCreature(Creature c) {
            PointData targetTile = null;
            string name;
            if (!c.IsPlayer) {
                name = c.Name;
                targetTile = new PointData { X = ((BasicAI)c.AI).TargetTile.X, Y = ((BasicAI)c.AI).TargetTile.Y };
            } else {
                name = "Player";
            }
            return new CreatureData {
                Name = name,
                Position = new PointData { X = c.X, Y = c.Y },
                Health = c.HP,
                Defense = c.Defense,
                Weapon = PrepareItem(c.Weapon),
                Armor = PrepareItem(c.Armor),
                TurnTimer = c.TurnTimer,
                TargetTile = targetTile
            };
        }

        private ItemData PrepareItem(Item i) {
            if (i == null) { return null; }
            return new ItemData { 
                Name = i.Name,
                Defense = i.IsArmor ? ((Armor)i).Defense : 0
            };
        }
    }
}
