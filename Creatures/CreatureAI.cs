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
    }

    public class PlayerAI : CreatureAI {
        private List<string> Messages;

        public PlayerAI(Creature creature) : base(creature) {
            Messages = new List<string>();
        }

        public override void AddMessage(string message) {
            if (message.Contains(Host.Name)) {
                message = message.Replace(Host.Name, "you");
            }
            Messages.Add(message);
            if (Constants.WriteMessagesToConsole) { System.Console.WriteLine(message); }
        }
        public override List<string> GetMessages() { return Messages; }
        public override void ClearMessages() { Messages.Clear(); }

        public override void OnDeath(World world) { Host.Notify("Press ESC to quit."); }
    }

    public class PigAI : CreatureAI {
        private bool Hostile;
        private Creature Player;

        public PigAI(Creature creature, Creature player) : base(creature) { 
            Player = player;
        }

        public override void TakeTurn(World world) {
            if (Hostile && Host.CanSee(Player.X, Player.Y)) {
                MoveTowards(Player);
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
                Host.Faction = null;
            }
        }

        public override void OnDeath(World world) {
            System.Random random = new System.Random();
            if (random.NextDouble() < 0.5) {
                world.Items[new Point(Host.X, Host.Y)] = Food.PigMeat;
            }
        }
    }

    public class BasicAI : CreatureAI {
        private Creature Player;
        public BasicAI(Creature creature, Creature player) : base(creature) {
            Player = player;
        }

        public override void TakeTurn(World world) {
            if (Host.CanSee(Player.X, Player.Y)) {
                MoveTowards(Player);
            } else {
                Wander();
            }
        }
    }
}
