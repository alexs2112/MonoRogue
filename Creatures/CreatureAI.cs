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

        // Some movement methods
        protected void MoveTowards(Creature target) { MoveTowards(target.X, target.Y); }
        protected void MoveTowards(Point p) { MoveTowards(p.X, p.Y); }
        protected void MoveTowards(int x, int y) {
            List<Point> path = Pathfinder.FindPath(Host, x, y);
            if (path.Count > 0) { Host.MoveTo(path[0]); }
        }

        private static List<int[]> RandomMoves = new List<int[]>() { new int[] {1, 0}, new int[] {0, 1}, new int[] {-1, 0}, new int[] {0, -1} };
        protected void Wander() {
            System.Random r = new System.Random();
            int[] XY = RandomMoves[r.Next(4)];
            Host.MoveRelative(XY[0], XY[1]);
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
        private bool Hostile;

        public PigAI(Creature creature, Creature player) : base(creature, player) { }

        public override void TakeTurn(World world) {
            if (Hostile) {
                base.TakeTurn(world);
            } else {
                Wander();
            }
        }

        public override void OnHit(World world, Creature attacker) {
            if (!Hostile && attacker.IsPlayer) {
                if (Host.IsDead()) { return; }
                Hostile = true;
                Host.SetColor(Color.HotPink);
                Host.NotifyOthers($"The {Host.Name} flies into a rage!");
                Host.ModifyDamage(1);
                Host.ModifyMovementDelay(-3);
                Host.ModifyAttackDelay(-2);
                Host.SetName($"Angry {Host.Name}");
            }
        }

        public override void OnDeath(World world) {
            System.Random random = new System.Random();
            if (random.NextDouble() < 0.5) {
                world.Items[new Point(Host.X, Host.Y)] = Food.PigMeat;
            }
        }
    }
}
