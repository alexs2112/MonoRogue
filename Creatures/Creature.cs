using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Creature {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Texture2D Glyph { get; private set; }
        public Color Color { get; private set; }
        public World World { get; set; }
        public CreatureAI AI { get; set; }
        public bool IsPlayer { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        // Basic Stats
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Vision { get; private set; }
        public int Defense { get; private set; }
        public int MaxDefense { get; private set; }
        public int DefenseTimer { get; private set; }
        public int BaseBlock { get; private set; }
        public int BaseWeight { get; private set; }

        // Attack Stats
        private (int Min, int Max) Damage { get; set; }
        private int BaseRange { get; set; }
        private string BaseAttackText { get; set; }
        public string AbilityText { get; set; }
        public int DamageModifier { get; set; } // Enemies with weapons are dealing too much damage
        public int DamageMark { get; set; }     // Bonus damage that will be consumed on next hit

        // How many game ticks it takes to recover after taking an action, default 10
        public int TurnTimer { get; set; }
        private int MovementDelay { get; set; }
        private int AttackDelay { get; set; }

        // Used for world generation and populating the dungeon
        public int Difficulty { get; set; }

        // If this creature holds the key to the exit of the game or not
        public bool HasKey { get; set; }

        // Some visual flavour
        public bool Bleeds { get; set; }
        public Projectile.Type BaseProjectile { get; set; }

        public Armor Armor { get; set; }
        public Weapon Weapon { get; set; }

        public Creature(string name, Texture2D glyph, Color color) {
            Name = name;
            Glyph = glyph;
            Color = color;
        }

        public void SetStats(int hp, int defense, (int, int) damage, int baseWeight = 0, int block = 0) {
            MaxHP = hp;
            HP = hp; 
            Defense = defense;
            MaxDefense = defense;
            Damage = damage;
            BaseWeight = baseWeight;
            Vision = 7;
            MovementDelay = 10;
            AttackDelay = 10;
            BaseAttackText = "attack";
            BaseRange = 1;
            Bleeds = true;
            BaseBlock = block;
        }

        // Setters for private attributes
        public void SetColor(Color color) { Color = color; }
        public void SetName(string name) { Name = name; }
        public void SetGlyph(Texture2D glyph) { Glyph = glyph; }
        public void SetDescription(string s) { Description = s; }
        public void SetBaseRange(int r) { BaseRange = r; }
        public void SetAttackText(string t) { BaseAttackText = t; }
        public void SetAbilityText(string t) { AbilityText = t; }
        public void ModifyDamage(int amount) { ModifyDamage(amount, amount); }
        public void ModifyDamage(int min, int max) { Damage = (Damage.Min + min, Damage.Max + max); }
        public (int Min, int Max) GetDamage() {
            int v1, v2;
            if (Weapon != null) {
                v1 = Weapon.Damage.Min;
                v2 = Weapon.Damage.Max;
            } else {
                v1 = Damage.Min;
                v2 = Damage.Max;
            }
            return (v1 + DamageModifier, v2 + DamageModifier);
        }
        public int GetArmorWeight() {
            if (Armor == null) { return 0; }
            return Armor.Weight;
        }
        public int GetCritChance() {
            return GetWeaponType() == Weapon.Type.Dagger ? (IsPlayer ? 30 + (10 * Weapon.Strength) - 8 * (GetArmorWeight() + BaseWeight) : 15 - 3 * (GetArmorWeight() + BaseWeight)) : 0;
        }
        public int GetParryChance() {
            return GetWeaponType() == Weapon.Type.Sword ? (IsPlayer ? 30 + (10 * Weapon.Strength) - 6 * (GetArmorWeight() + BaseWeight) : 30 - 5 * (GetArmorWeight() + BaseWeight)) : 0;
        }
        public int GetBlock() {
            return BaseBlock + (Armor != null ? Armor.Block : 0);
        }
        public void ModifyDefense(int value) { MaxDefense += value; Defense += value; }
        public void SetDefense(int value) { Defense = value; }
        public (int Current, int Max) GetDefense() {
            if (Armor == null) { return (Defense, MaxDefense); }
            return (Armor.Defense + Defense, Armor.MaxDefense + MaxDefense);
        }

        public void ModifyMovementDelay(int x) { MovementDelay += x; }
        public int GetMovementDelay() {
            if (Armor != null) { return MovementDelay + Armor.Weight + BaseWeight; }
            else { return MovementDelay + BaseWeight; }
        }
        public void ModifyAttackDelay(int x) { AttackDelay += x; }
        public int GetAttackDelay() {
            int v = AttackDelay;
            if (Weapon != null) { v = Weapon.Delay; }
            if (Armor != null) { v += Armor.Weight; }
            v += BaseWeight;
            return v;
        }
        public int GetRange() { return Weapon == null ? BaseRange : Weapon.Range; }
        public Item.Type GetWeaponType() { 
            if (Weapon == null) { return Item.Type.Null; }
            else { return Weapon.ItemType; }
        }
        
        public void ModifyHP(int value) {
            if (IsPlayer && Constants.Invincible) { return; }
            HP += value;
            if (HP <= 0) {
                Die();
            } else if (HP > MaxHP) {
                HP = MaxHP;
            }
        }
        
        public void Die() {
            NotifyWorld(new DeathNotification(this));
            AI.OnDeath(World);
            World.Creatures.Remove(this);

            if (Bleeds) { World.Bloodstains[X, Y] = true; }
            if (Armor != null) { DropItem(Armor); }
            if (Weapon != null) { DropItem(Weapon); }

            World.Score += (int)(Difficulty * (126 - Difficulty * 2) * (0.2 * Constants.Difficulty + 0.4));
        }

        public void TakeDamage(int value) {
            if (Armor != null) {
                if (Armor.Defense > value) {
                    Armor.Defense -= value;
                    value = 0;
                    return;
                } else {
                    value -= Armor.Defense;
                    Armor.Defense = 0;
                }
            }
            if (Defense > 0) {
                if (Defense > value) {
                    Defense -= value;
                    value = 0;
                    return;
                } else {
                    value -= Defense;
                    Defense = 0;
                }
            }
            ModifyHP(-value);
        }
        public bool IsDead() { return HP <= 0; }

        public bool CanEnter(Point p) { return CanEnter(p.X, p.Y); }
        public bool CanEnter(int x, int y) { return World.IsFloor(x, y) || World.IsDoor(x, y); }

        public bool MoveTo(Point p) { return MoveTo(p.X, p.Y); }
        public bool MoveTo(int x, int y) { 
            if (!CanEnter(x, y)) { return false; }
            if (World.IsDoor(x, y)) {
                bool broken = World.OpenDoor(x, y);
                NotifyWorld(new OpenDoorNotification(this, broken), x, y);
                TurnTimer = GetAttackDelay();
                return true;
            }

            Creature c = World.GetCreatureAt(x, y);
            if (c != null) {
                if (!IsPlayer && !c.IsPlayer) {
                    TurnTimer = GetAttackDelay();
                } else {
                    if (GetWeaponType() == Item.Type.Axe && IsPlayer) {
                        // Axes hit all enemies adjacent to you and regenerate your shields for each additional enemy
                        List<Creature> adjacent = new List<Creature>();
                        for (int mx = -1; mx <= 1; mx++) {
                            for (int my = -1; my <= 1; my++) {
                                if (mx == 0 && my == 0) { continue; }
                                if (World.GetCreatureAt(X + mx, Y + my) != null) adjacent.Add(World.GetCreatureAt(X + mx, Y + my));
                            }
                        }
                        if (adjacent.Count > 1) { Defense = Math.Min(Defense + 2 * (adjacent.Count - 1) * Weapon.Strength, GetDefense().Max); }
                        foreach (Creature adj in adjacent) {
                            Attack(adj);
                        }
                    } else {
                        Attack(c);
                    }
                }
            } else {
                int oldX = X;
                int oldY = Y;
                X = x;
                Y = y;
                TurnTimer = GetMovementDelay();

                // Spears get a free lunge attack if you move towards an enemy and end up in range
                if (GetWeaponType() == Item.Type.Spear) {
                    int dx = X - oldX;
                    int dy = Y - oldY;
                    for (int r = 1; r <= GetRange(); r++) {
                        if (World.IsWall(X + dx*r, Y + dy*r)) { break; }

                        Creature poke = World.GetCreatureAt(X + dx * r, Y + dy * r);
                        if (poke == null) { continue; }
                        else if (poke.IsPlayer != IsPlayer) { 
                            if (IsPlayer) {
                                poke.DamageMark += Weapon.Strength * 2;
                                AddMessage($"You lunge at the {poke.Name}.");
                            }
                            Attack(poke);
                            break;
                        }
                    }
                }

                Item i = World.GetItemAt(X, Y);
                if (i != null) { AddMessage($"You see here a {i.Name}."); }
            }
            return true;
        }
        public bool MoveRelative(int dx, int dy) { 
            return MoveTo(X + dx, Y + dy);
        }

        public bool CloseDoors() {
            bool foundBroken = false;
            bool foundOpen = false;
            bool foundCreature = false;
            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0) { continue; }
                    if (Feature.IsOpenDoor(World.GetTile(X + dx, Y + dy))) {
                        if (World.GetCreatureAt(X + dx, Y + dy) == null) {
                            foundOpen = true;
                            World.Tiles[X + dx, Y + dy] = Feature.GetClosedDoor();
                        } else {
                            foundCreature = true;
                        }
                    }
                    if (Feature.IsBrokenDoor(World.GetTile(X + dx, Y + dy))) {
                        foundBroken = true;
                    }
                }
            }

            NotifyWorld(new CloseDoorNotification(foundOpen, foundBroken, foundCreature), X, Y);
            return foundOpen;
        }

        public bool CanSee(Point p) { return CanSee(p.X, p.Y); }
        public bool CanSee(int x, int y) {
            if ((X-x)*(X-x) + (Y-y)*(Y-y) > Vision * Vision) { return false; }

            foreach (Point p in World.GetLine(X, Y, x, y)) {
                if (!World.BlockSight(p) || (p.X == x && p.Y == y)) { continue; }

                return false;
            }
            return true;
        }

        public List<Point> GetLineToPoint(int x, int y) { return GetLineToPoint(X, Y, x, y); }
        public List<Point> GetLineToPoint(Point p) { return GetLineToPoint(X, Y, p.X, p.Y); }
        public static List<Point> GetLineToPoint(Point source, Point dest) {
            return GetLineToPoint(source.X, source.Y, dest.X, dest.Y);
        }
        public static List<Point> GetLineToPoint(int sx, int sy, int dx, int dy) {
            List<Point> line = World.GetLine(sx, sy, dx, dy);
            if (line.Count > 0) { line.RemoveAt(0); }
            return line;
        }

        public void TakeTurn(World world) {
            if (Defense < MaxDefense) {
                DefenseTimer++;
                if (DefenseTimer >= 5) {
                    Defense++;
                    DefenseTimer = 0;
                }
            } else if (Armor != null) { Armor.Tick(); }

            if (AI != null) { AI.TakeTurn(world); }
        }

        public void Attack(Creature target) {
            if (Constants.AllowAnimations) {
                if (Weapon != null) {
                    World.Projectiles.Add(Projectile.GetProjectile(this, target, Weapon.BaseProjectile));
                } else {
                    World.Projectiles.Add(Projectile.GetProjectile(this, target, BaseProjectile));
                }
            }
            else { FinishAttack(target); }
        }

        public void FinishAttack(Creature target) {
            int damage = new System.Random().Next(GetDamage().Min, GetDamage().Max + 1);

            if (target.DamageMark > 0) {
                damage += target.DamageMark;
                target.DamageMark = 0;
            }

            string action;
            if (Weapon != null && Weapon.AttackText != null) {
                action = Weapon.AttackText;
            } else {
                action = BaseAttackText;
            }

            bool isCrit = false;
            if (GetWeaponType() == Weapon.Type.Dagger) {
                // Daggers can critically hit, dealing double damage
                int chance = new System.Random().Next(100);
                if (chance < GetCritChance()) { damage = damage * (IsPlayer ? 3 : 2); isCrit = true; }
            } else if (GetWeaponType() == Weapon.Type.Mace) {
                // Maces deal additional damage to armor
                if (target.GetDefense().Current > 0) {
                    int bonus = System.Math.Min(target.GetDefense().Current, Weapon.MaceDamage);
                    damage += bonus;
                }

                // Maces also deal bonus damage on subsequent attacks
                if (IsPlayer) target.DamageMark += Weapon.MaceDamage;
            }

            if (target.GetWeaponType() == Weapon.Type.Sword && new System.Random().Next(100) < target.GetParryChance()) {
                // Swords can parry attacks, taking no damage
                NotifyWorld(new ParryNotification(this, target), target.X, target.Y);
            } else {
                damage -= target.GetBlock();
                NotifyWorld(new AttackNotification(this, target, action, damage, isCrit), target.X, target.Y);
                target.TakeDamage(damage);
                target.GetAttacked(this);
            }

            if (IsPlayer) { 
                // It is impossible for a creature to die outside of us attacking it
                if (target.HP > 0) { AI.LastAttacked = target; }
                else { AI.LastAttacked = null; }
            }

            TurnTimer = GetAttackDelay();
        }

        public void GetAttacked(Creature attacker) {
            if (Armor != null) { Armor.ResetTimer(); }
            DefenseTimer = 0;
            AI.OnHit(World, attacker);
        }
        
        public Creature GetCreatureFromLine(List<Point> line) {
            // Return the first creature from a line of points, or null
            foreach (Point p in line) {
                Creature c = World.GetCreatureAt(p);
                if (c != null) { return c; }
            }
            return null;
        }
        public Creature GetCreatureInRange(Creature target) { return GetCreatureInRange(X, Y, target, GetRange()); }
        public Creature GetCreatureInRange(Creature target, int range) { return GetCreatureInRange(X, Y, target, range); }
        public Creature GetCreatureInRange(int sx, int sy, Creature target) { return GetCreatureInRange(sx, sy, target, GetRange()); }
        public Creature GetCreatureInRange(int sx, int sy, Creature target, int range) {
            // Return the first creature in a line to the target that is in range
            if (target == null) { return null; }
            return GetCreatureInRange(sx, sy, target.X, target.Y, range);
        }
        public Creature GetCreatureInRange(int tx, int ty, int range) { return GetCreatureInRange(X, Y, tx, ty, range); }
        public Creature GetCreatureInRange(int sx, int sy, int x, int y, int range) {
            // Return the first creature in a line to the point that is in range
            Creature c = GetCreatureFromLine(GetLineToPoint(sx, sy, x, y));
            if (c == null) { return null; }
            if (GetLineToPoint(sx, sy, c.X, c.Y).Count > range) { c = null; }
            return c;
        }

        public void AddMessage(string message) {
            if (AI == null) { return; }
            AI.AddMessage(message); 
        }
        public void Notify(Notification notification) { 
            if (!IsPlayer) { return; }
            string message = notification.Parse(this);
            AI.AddMessage(message);
        }
        public void NotifyWorld(Notification notification) { NotifyWorld(notification, new Point(X, Y)); }
        public void NotifyWorld(Notification notification, int x, int y) { NotifyWorld(notification, new Point(x,y)); }
        public void NotifyWorld(Notification notification, Point p) {
            // Notify the player if they can see the given point
            if (World.Player.CanSee(p)) {
                World.Player.Notify(notification);
            }
        }

        // OnlyPickup sets if we don't wait 10 ticks if there is nothing to pick up
        public void PickUp(bool OnlyPickup) { PickUp(new Point(X, Y), OnlyPickup); }
        public void PickUp(Point p, bool OnlyPickup) {
            Item i = World.GetItemAt(p);
            if (i == null) { 
                if (OnlyPickup) {
                    AddMessage("There is nothing to pick up.");
                } else {
                    TurnTimer = 10;
                }
                return;
            }

            if (i.IsFood) { World.EatFoodAt(this, p); }
            else if (i.IsArmor || i.IsWeapon) {
                World.Items.Remove(p);
                Equip(i);
            } else if (i.IsHeartstone) {
                ((Heartstone)i).Consume(this);
                World.Items.Remove(p);
            } else if (i.IsKey) {
                World.Items.Remove(p);
                HasKey = true;
                World.Tiles[World.Exit.X, World.Exit.Y] = Feature.ExitOpen;
                Notify(new GoldenKeyNotification(this));
            }
            TurnTimer = 10;
        }
        public void Equip(Item item) {
            if (item == null) { return; }
            if (item.IsArmor) {
                Armor temp = Armor;
                Armor = (Armor)item;

                if (temp != null) { DropItem(temp); }
            } else if (item.IsWeapon) {
                Weapon temp = Weapon;
                Weapon = (Weapon)item;

                if (temp != null) { DropItem(temp); }

                if (!IsPlayer) {
                    Texture2D next = EnemyGlyph.GetGlyph(this);
                    if (next != null) { Glyph = next; }
                }
            }
            if (IsPlayer) { 
                Notify(new EquipNotification(this, item));
                Glyph = PlayerGlyph.GetUpdatedGlyph(this);
            }
        }
        public void DropItem(Item item) {
            if (World.GetItemAt(X, Y) == null && World.Exit.X != X && World.Exit.Y != Y) { 
                World.Items[new Point(X, Y)] = item; 
            } else {
                List<Point> valid = new List<Point>();
                for (int mx = -1; mx <= 1; mx++) {
                    for (int my = -1; my <= 1; my++) {
                        Point p0 = new Point(X + mx, Y + my);
                        if (World.IsFloor(p0) && World.GetItemAt(p0) == null && World.Exit != p0) {
                            valid.Add(p0);
                        }
                    }
                }

                // If the tile and all surrounding tiles are full, the item vanishes into the void
                if (valid.Count == 0) { 
                    // Failsafe, if the key can't drop the game becomes unwinnable
                    if (item.IsKey) { World.Items[new Point(X, Y)] = item; }
                } else {
                    Point p = valid[new System.Random().Next(valid.Count)];
                    World.Items[p] = item;
                }
            }
        }
    }
}
