using System.Collections.Generic;

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

        public virtual void TakeTurn(World world) { }
    }

    public class PlayerAI : CreatureAI {
        private List<string> Messages;

        public PlayerAI(Creature creature) : base(creature) {
            Messages = new List<string>();
        }

        public override void AddMessage(string message) {
            Messages.Add(message);
            if (Constants.WriteMessagesToConsole) { System.Console.WriteLine(message); }
        }
        public override List<string> GetMessages() { return Messages; }
        public override void ClearMessages() { Messages.Clear(); }
    }

    public class BasicAI : CreatureAI {
        public BasicAI(Creature creature) : base(creature) { }

        private static List<int[]> moves = new List<int[]>() { new int[] {1, 0}, new int[] {0, 1}, new int[] {-1, 0}, new int[] {0, -1} };
        public override void TakeTurn(World world) {
            // For now, move randomly
            System.Random r = new System.Random();
            int[] XY = moves[r.Next(4)];
            Host.MoveRelative(XY[0], XY[1]);
        }
    }
}
