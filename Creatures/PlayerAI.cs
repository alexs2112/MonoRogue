using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class PlayerAI : CreatureAI {
        private List<string> Messages;
        public List<Point> Path {get; private set; }

        public PlayerAI(Creature creature) : base(creature) {
            Messages = new List<string>();
        }

        public void FollowPath(World world) {
            // If the player has a path to follow, move to the next one if there are no enemies in sight
            if (Path == null) { return; }
            if (Path.Count == 0) { Path = null; return; }
            Creature c = CreatureInView(world);
            if (c != null) {
                Host.Notify($"A {c.Name} comes into view.");
                Path = null;
                return;
            }
            ClearMessages();
            Host.MoveTo(Path[0]);
            Path.RemoveAt(0);
        }

        public void SetPath(List<Point> path) { Path = path; }
        public void ClearPath() { Path = null; }

        public bool PathNullOrEmpty() {
            return Path == null || Path.Count == 0;
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
}
