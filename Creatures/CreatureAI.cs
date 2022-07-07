using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class CreatureAI {
        protected Creature Host;

        public CreatureAI(Creature creature) {
            Host = creature;
        }

        // Store messages in the players AI, ignore this for everything else
        public virtual void AddMessage(string message) { }
        public virtual List<string> GetMessages() { return null; }
        public virtual void ClearMessages() { }

        // Override this
        public virtual void TakeTurn(World world) { }

        // Some ways to let AI handle certain triggers
        public virtual void OnDeath(World world) { }
        public virtual void OnHit(World world, Creature attacker) { }

        // For specifically the Warden, to tell each other creature where the player is
        public virtual void GiveTargetTile(Point p) { }

        // Some movement methods
        protected void MoveTowards(Creature target) { MoveTowards(target.X, target.Y); }
        protected void MoveTowards(Point p) { MoveTowards(p.X, p.Y); }
        protected void MoveTowards(int x, int y) {
            List<Point> path = Pathfinder.FindPath(Host, x, y);
            if (path != null && path.Count > 0) { Host.MoveTo(path[0]); }
        }

        private static List<Point> RandomMoves = new List<Point>() { new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1) };
        protected void Wander() {
            System.Random r = new System.Random();
            List<Point> moves = new List<Point>(RandomMoves);
            for (int i = 0; i < 4; i++) {
                Point p = moves[r.Next(moves.Count)];
                if (Host.CanEnter(p.X + Host.X, p.Y + Host.Y) && !Host.World.IsDoor(p.X + Host.X, p.Y + Host.Y)) {
                    Host.MoveRelative(p.X, p.Y);
                    return;
                }
            }
            Host.TurnTimer = Host.GetMovementDelay();
        }

        // Some vision methods
        public Creature CreatureInView(World world) { 
            foreach (Creature c in world.Creatures) {
                if (c == Host) { continue; }
                if (Host.CanSee(c.X, c.Y)) {
                    return c;
                }
            }
            return null;
        }
    }

    public class BasicAI : CreatureAI {
        protected Creature Player;

        // If the creature has seen the player but the player is out of sight, move towards
        // where the player was last seen
        protected Point TargetTile;
        public override void GiveTargetTile(Point p) { TargetTile = p; }

        public BasicAI(Creature creature, Creature player) : base(creature) {
            Player = player;
            TargetTile = new Point(-1);
        }

        public override void TakeTurn(World world) {
            if (Host.CanSee(Player.X, Player.Y)) {
                TargetTile = new Point(Player.X, Player.Y);
                TryToAttack(Player);
            } else if (TargetTile.X != -1) {
                MoveTowards(TargetTile);
                if (TargetTile.X == Host.X && TargetTile.Y == Host.Y) {
                    TargetTile = new Point(-1);
                }
            } else {
                Wander();
            }
        }

        protected void TryToAttack(Creature target) {
            // If we are in range of the target, try to attack it
            if (Host.GetLineToPoint(target.X, target.Y).Count <= Host.GetRange()) {
                if (Host.GetCreatureInRange(target) == target) {
                    Host.Attack(target);
                } else if (Host.GetRange() > 1) {
                    // If our range > 1 and we can't hit the target from here, there must be another enemy in the way
                    for (int mx = -1; mx <= 1; mx++) {
                        for (int my = -1; my <= 1; my++) {
                            if (!Host.CanEnter(Host.X + mx, Host.Y + my) || 
                                Host.World.GetCreatureAt(Host.X + mx, Host.Y + my) != null) { 
                                continue; 
                            }
                            if (Host.GetCreatureInRange(Host.X + mx, Host.Y + my, target) == target) {
                                Host.MoveRelative(mx, my);
                                return;
                            }
                        }
                    }
                    MoveTowards(target);
                }

            // Otherwise, move towards being in range
            } else {
                MoveTowards(target);
            }
        }
    }

    public class PigAI : BasicAI {
        // Does not actively seek out the player to attack until they are first attacked, then they become an angry pig
        // On death, sometimes drops a pig meat
        // Oinks periodically
        private bool Hostile;

        public PigAI(Creature creature, Creature player) : base(creature, player) { }

        public override void TakeTurn(World world) {
            if (new System.Random().Next(10) < 2) {
                if (Hostile) { Host.NotifyWorld(new TalkNotification(Host, "Angry oink")); } 
                else { Host.NotifyWorld(new TalkNotification(Host, "Oink")); }
            }
            if (Hostile) {
                base.TakeTurn(world);
            } else {
                Wander();
            }
        }

        public override void OnHit(World world, Creature attacker) {
            if (!Hostile && attacker.IsPlayer) {
                BecomeHostile();
            }
        }

        public override void GiveTargetTile(Point p) {
            if (!Hostile) { BecomeHostile(); }
            base.GiveTargetTile(p);
        }

        public override void OnDeath(World world) {
            System.Random random = new System.Random();
            if (random.NextDouble() < 0.5) {
                Host.DropItem(Food.PigMeat);
            }
        }

        private void BecomeHostile() {
            if (Host.IsDead()) { return; }
            Hostile = true;
            Host.SetColor(Color.HotPink);
            Host.NotifyWorld(new BasicNotification($"The {Host.Name} flies into a rage!"));
            Host.ModifyDamage(1);
            Host.ModifyMovementDelay(-3);
            Host.ModifyAttackDelay(-2);
            Host.SetName($"Angry {Host.Name}");
        }
    }

    public class ThugAI : BasicAI {
        // Whenever it runs out of armor it runs away in the opposite direction of the player
        public ThugAI(Creature creature, Creature player) : base(creature, player) { }

        public override void TakeTurn(World world) {
            if (Host.GetDefense().Current <= 0 && Host.CanSee(Player.X, Player.Y) && Player.GetWeaponType() != Weapon.Type.Bow) {
                int dx = Host.X - Player.X;
                int dy = Host.Y - Player.Y;
                int mx = dx >= dy ? (dx > 0 ? 1 : -1) : 0;
                int my = dy >= dx ? (dy > 0 ? 1 : -1) : 0;

                List<Point> points = new List<Point>();
                if (mx != 0 && my != 0) {
                    points.Add(new Point(mx, my));
                    points.Add(new Point(mx, 0));
                    points.Add(new Point(0, my));
                } else if (my == 0) {
                    points.Add(new Point(mx, 0));
                    points.Add(new Point(mx, -1));
                    points.Add(new Point(mx, 1));
                } else {
                    points.Add(new Point(0, my));
                    points.Add(new Point(-1, my));
                    points.Add(new Point(1, my));
                }
                Point p;
                System.Random random = new System.Random();
                while (points.Count > 0) {
                    int i = random.Next(points.Count);
                    p = points[i];
                    if (Host.CanEnter(p.X + Host.X, p.Y + Host.Y) && world.GetCreatureAt(p.X + Host.X, p.Y + Host.Y) == null) {
                        Host.MoveRelative(p.X, p.Y);
                        return;
                    }
                    points.RemoveAt(i);
                }

                // If there isnt a place to move away from the player
                if (Host.GetCreatureInRange(Player) == Player) {
                    // If the player is in range, attack them
                    Host.Attack(Player);
                } else {
                    // Else, wait here
                    Host.TurnTimer = Host.GetMovementDelay();
                }
            } else {
                base.TakeTurn(world);
            }
        }
    }

    public class BruteAI : BasicAI {
        // Basically a regular creature that deals damage when the player hits them and they still have armor
        public BruteAI(Creature creature, Creature player) : base(creature, player) { }

        public override void OnHit(World world, Creature attacker) {
            if (Host.GetDefense().Current > 0 && Host.GetLineToPoint(attacker.X, attacker.Y).Count <= 1) {
                int damage = new System.Random().Next(1, 3);
                attacker.AddMessage($"You take {damage} damage. (Spines)");
            }
        }
    }

    public class TankAI : BasicAI {
        // If the player is two tiles away, pull them adjacent
        public TankAI(Creature creature, Creature player) : base(creature, player) { }

        private int Cooldown;
        public override void TakeTurn(World world) {
            if (Cooldown > 0) { Cooldown--; }
            if (Host.CanSee(Player.X, Player.Y) && 
                Cooldown <= 0 && 
                Host.GetLineToPoint(Player.X, Player.Y).Count == 2 && 
                Host.GetCreatureInRange(Player, 2) == Player) {
                    List<Point> path = Pathfinder.FindPath(Host, Player.X, Player.Y);
                    Player.MoveTo(path[0]);
                    Player.AddMessage($"The {Host.Name} pulls you in.");
                    Cooldown = 3;
                    Host.TurnTimer = 10;
            } else {
                base.TakeTurn(world);
            }
        }
    }

    public class CultistAI : BasicAI {
        // Can spawn up to 2 imps that die when the cultist does
        private CreatureFactory Factory;
        private int SummonsLeft;
        private List<Creature> Summons;
        public CultistAI(Creature creature, Creature player, CreatureFactory factory) : base(creature, player) {
            Factory = factory;
            SummonsLeft = 2;
            Summons = new List<Creature>();
        }

        public override void TakeTurn(World world) {
            if (SummonsLeft > 0 && Host.CanSee(Player.X, Player.Y)) {
                TargetTile = new Point(Player.X, Player.Y);

                // A lot of work with a list so that we can take a random valid tile without breaking the game if there isnt one
                List<Point> tiles = new List<Point>();
                for (int x = -Host.Vision; x <= Host.Vision; x++) {
                    for (int y = -Host.Vision; y <= Host.Vision; y++) {
                        Point p = new Point(Host.X + x, Host.Y + y);
                        if (Host.CanSee(p) && world.IsFloor(p) && world.GetCreatureAt(p) == null) { tiles.Add(p); }
                    }
                }
                if (tiles.Count > 0) {
                    System.Random random = new System.Random();
                    int i = random.Next(tiles.Count);
                    Point tile = tiles[i];

                    Color color;
                    i = random.Next(4);
                    if (i == 0) { color = Color.Tomato; }
                    else if (i == 1) { color = Color.Turquoise; }
                    else if (i == 2) { color = Color.LimeGreen; }
                    else { color = Color.Gold; }

                    Creature summon = Factory.NewImp(world, tile.X, tile.Y, color);
                    summon.AI.GiveTargetTile(TargetTile);
                    summon.TurnTimer = 10;
                    Host.TurnTimer = 20;
                    SummonsLeft--;
                    Host.NotifyWorld(new BasicNotification($"The {Host.Name} summons a new {summon.Name}."));
                    Summons.Add(summon);
                    return;
                }
            }
            base.TakeTurn(world);
        }

        public override void OnDeath(World world) {
            foreach (Creature c in Summons) {
                c.ModifyHP(-100);
            }
        }
    }

    public class WardenAI : BasicAI {
        // The final boss, it can notify nearby enemies where the player is, and drops a golden key when he dies
        public WardenAI(Creature creature, Creature player) : base(creature, player) { }

        public bool PlayerInSight;
        public int AlarmCooldown;
        public override void TakeTurn(World world) {
            if (Host.CanSee(Player.X, Player.Y)) {
                if (!PlayerInSight && AlarmCooldown <= 0) {
                    PlayerInSight = true;
                    AlarmCooldown = 5;
                    Alarm(world);
                    return;
                }
                PlayerInSight = true;
            } else {
                PlayerInSight = false;
                if (AlarmCooldown > 0) { AlarmCooldown--; }
            }
            base.TakeTurn(world);
        }

        private static int Radius = 30;
        private void Alarm(World world) {
            Host.TurnTimer = 10;
            Host.NotifyWorld(new TalkNotification(Host, "Brethren, come to me."));
            Host.NotifyWorld(new BasicNotification($"The {Host.Name} glows bright red."));
            foreach (Creature c in world.Creatures) {
                if ((c.X - Host.X) * (c.X - Host.X) + (c.Y - Host.Y) * (c.Y - Host.Y) > Radius * Radius) { continue; }

                List<Point> path = Pathfinder.FindPath(c, Player.X, Player.Y);
                if (path != null && path.Count < 30) {
                    c.AI.GiveTargetTile(new Point(Player.X, Player.Y));
                }
            }
        }

        public override void OnDeath(World world) {
            Host.DropItem(GoldenKey.GetKey());
        }
    }
}
