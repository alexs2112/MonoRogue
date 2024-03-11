using System.Collections.Generic;

namespace MonoRogue {
    public class SaveData {
        public int Seed { get; set; }
        public int Difficulty { get; set; }
        public int Score { get; set; }
        public long Time { get; set; }
        public WorldData World { get; set; }
        public PlayerData PlayerData { get; set; }
        public List<CooldownData> Cooldowns { get; set; }
        public List<SummonData> Summons { get; set; }
    }

    public class WorldData {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<CreatureData> Creatures { get; set; }
        public List<ItemData> Items { get; set; }
        public List<PointData> BrokenDoors { get; set; }
        public List<PointData> OpenDoors { get; set; }
        public List<PointData> Bloodstains { get; set; }
        public List<ColumnData> HasSeen { get; set; }
    }

    public class PointData {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class CreatureData {
        public string Name { get; set; }
        public PointData Position { get; set; }
        public int Health { get; set; }
        public int Defense { get; set; }
        public ItemData Weapon { get; set; }
        public ItemData Armor { get; set; }
        public int TurnTimer { get; set; }
        public PointData TargetTile { get; set; }
    }

    public class ItemData {
        public string Name { get; set; }
        public PointData Position { get; set; }

        // Armor specific field
        public int Defense { get; set; }
    }

    public class ColumnData {
        public bool[] Map { get; set; }
    }

    // Player specific fields
    public class PlayerData {
        public int MaxHealth { get; set; }
        public bool HasKey { get; set; }
    }

    // AI specific fields
    public class CooldownData {
        // Can't really pass a reference to a creature, use their unique location instead
        public PointData Location { get; set; }
        public int Value { get; set; }
    }

    public class SummonData {
        public PointData Summoner { get; set; }
        public List<PointData> Summons { get; set; }
        public int SummonsLeft { get; set; }
    }
}
