namespace MonoRogue {
    public abstract class Notification {
        public abstract string Parse(Creature player);
    }

    public class BasicNotification : Notification {
        private string Message;
        public BasicNotification(string message) {
            Message = message;
        }

        public override string Parse(Creature player) {
            return Message;
        }
    }

    public class AttackNotification : Notification {
        private Creature Attacker;
        private Creature Target;
        private string AttackText;
        private int Damage;
        private bool Crit;
        public AttackNotification(Creature attacker, Creature target, string attackText, int damage, bool crit) {
            Attacker = attacker;
            Target = target;
            AttackText = attackText;
            Damage = damage;
            Crit = crit;
        }
        
        public override string Parse(Creature player) {
            string s = "";
            if (Attacker == player) { s += $"You {AttackText} "; }
            else { 
                string name;
                if (player.CanSee(Attacker.X, Attacker.Y)) {
                    name = $"The {Attacker.Name}";
                } else {
                    name = "Something";
                }

                if (AttackText.EndsWith('h')) { AttackText += 'e'; }
                s += $"{name} {AttackText}s ";
            }
            if (Target == player) { s += "you "; }
            else {
                if (player.CanSee(Target.X, Target.Y)) { s += $"the {Target.Name} "; }
                else { s += "something "; }
            }
            s += $"for {Damage} damage!";
            if (Crit) { s += " CRIT!"; }
            return s;
        }
    }

    public class DeathNotification : Notification {
        private Creature Creature;
        public DeathNotification(Creature creature) {
            Creature = creature;
        }

        public override string Parse(Creature player) {
            if (Creature == player) { return "You die."; }
            else {
                if (player.CanSee(Creature.X, Creature.Y)) {
                    return $"The {Creature.Name} dies.";
                } else {
                    return "Something dies.";
                }
            }
        }
    }

    public class DoorNotification : Notification {
        private Creature Creature;
        public DoorNotification(Creature creature) {
            Creature = creature;
        }

        public override string Parse(Creature player) {
            if (Creature == player) { return "You break down the door."; }
            else { 
                if (player.CanSee(Creature.X, Creature.Y)) {
                    return $"The {Creature.Name} breaks down the door.";
                } else {
                    return "Something breaks down the door.";
                }
            }
        }
    }

    public class TalkNotification : Notification {
        private Creature Creature;
        private string Message;
        public TalkNotification(Creature creature, string message) {
            Creature = creature;
            Message = message;
        }

        public override string Parse(Creature player) {
            if (Creature == player) { return $"You: {Message}"; }
            else { return $"{Creature.Name}: {Message}"; }
        }
    }

    public class FoodNotification : Notification {
        private Creature Creature;
        private Food Food;
        public FoodNotification(Creature creature, Food food) {
            Creature = creature;
            Food = food;
        }

        public override string Parse(Creature player) {
            if (Creature == player) { return $"You eat the {Food.Name}."; }
            else { return $"The {Creature.Name} eats the {Food.Name}."; }
        }
    }
}
