using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class PlayerAI : CreatureAI {
        private List<string> Messages;
        public List<Point> Path {get; private set; }
        public bool Resting { get; private set; }

        public PlayerAI(Creature creature) : base(creature) {
            Messages = new List<string>();
        }

        public override void TakeTurn(World world) {
            if (Resting) { 
                Creature c = CreatureInView(world);
                if (c != null) {
                    AddMessage($"A {c.Name} comes into view.");
                    Resting = false;
                    return;
                } else if (Host.Armor.Defense >= Host.Armor.MaxDefense) {
                    AddMessage("Armor repaired!");
                    Resting = false;
                    return;
                }
                Host.TurnTimer = 10; 
            }
        }

        public void StartResting() {
            Creature c = CreatureInView(Host.World);
            if (c != null) {
                AddMessage("Cannot rest, enemies in view.");
            } else if (Host.Armor == null) {
                AddMessage("You do not have armor to repair.");
            } else if (Host.Armor.Defense >= Host.Armor.MaxDefense) {
                AddMessage("Your armor is already repaired.");
            } else {
                Resting = true;
                Host.TurnTimer = 10;
            }
        }

        public void FollowPath(World world) {
            // If the player has a path to follow, move to the next one if there are no enemies in sight
            if (Path == null) { return; }
            if (Path.Count == 0) { Path = null; return; }
            Creature c = CreatureInView(world);
            if (c != null) {
                AddMessage($"A {c.Name} comes into view.");
                Path = null;
                return;
            }
            ClearMessages();
            Host.MoveTo(Path[0]);
            if (Path != null) { Path.RemoveAt(0); }
        }

        public void SetPath(List<Point> path) { Path = path; }
        public void ClearPath() { Path = null; }

        public bool PathNullOrEmpty() {
            return Path == null || Path.Count == 0;
        }

        public override void AddMessage(string message) {
            Resting = false;
            Path = null;
            Messages.Add(message);
            if (Constants.WriteMessagesToConsole) { System.Console.WriteLine(message); }
        }
        public override List<string> GetMessages() { return Messages; }
        public override void ClearMessages() { Messages.Clear(); }

        public override void OnDeath(World world) { 
            AddMessage("Press ESC to quit.");
            Main.Audio.ChangeSong(SongHandler.DeathSong);
        }
    }
}
