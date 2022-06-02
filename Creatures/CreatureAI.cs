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
            System.Console.WriteLine(message);
        }
        public override List<string> GetMessages() { return Messages; }
        public override void ClearMessages() { Messages.Clear(); }
    }

    public class BasicAI : CreatureAI {
        public BasicAI(Creature creature) : base(creature) { }

        public override void TakeTurn(World world) {
            // For now, move randomly
            System.Random r = new System.Random();
            int mx;
            int my;
            do {
                mx = r.Next(3) - 1;
                my = r.Next(3) - 1;
            } while (mx == 0 && my == 0);
            Host.MoveRelative(mx, my);
        }
    }
}
