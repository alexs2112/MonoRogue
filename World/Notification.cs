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
            EffectPlayer.PlaySoundEffect(EffectType.Hit);
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
            s += $"for ";
            if (Damage > 0) { s += $"{Damage}"; }
            else { s += "no"; }
            s += " damage.";
            if (Crit) { s += " CRIT!"; }
            return s;
        }
    }

    public class ParryNotification : Notification {
        private Creature Attacker;
        private Creature Target;
        public ParryNotification(Creature attacker, Creature target) {
            Attacker = attacker;
            Target = target;
        }

        public override string Parse(Creature player) {
            if (Attacker == player) {
                return $"The {Target.Name} parries your attack.";
            } else {
                return $"You parry the {Attacker.Name}'s attack.";
            }
        }
    }

    public class DeathNotification : Notification {
        private Creature Creature;
        public DeathNotification(Creature creature) {
            Creature = creature;
        }

        public override string Parse(Creature player) {
            EffectPlayer.PlaySoundEffect(EffectType.Death);
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
            EffectPlayer.PlaySoundEffect(EffectType.Door);
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

    public class FoodNotification : Notification {
        private Creature Creature;
        private Food Food;
        public FoodNotification(Creature creature, Food food) {
            Creature = creature;
            Food = food;
        }

        public override string Parse(Creature player) {
            EffectPlayer.PlaySoundEffect(EffectType.Eat);
            if (Creature == player) { return $"You eat the {Food.Name}."; }
            else { return $"The {Creature.Name} eats the {Food.Name}."; }
        }
    }

    public class EquipNotification : Notification {
        private Creature Creature;
        private Item Item;
        public EquipNotification(Creature creature, Item item) {
            Creature = creature;
            Item = item;
        }

        public override string Parse(Creature player) {
            EffectPlayer.PlaySoundEffect(EffectType.Equip);
            if (Creature == player) { return $"You equip the {Item.Name}."; }
            else { return $"The {Creature.Name} eats the {Item.Name}."; }
        }
    }

    public class HeartstoneNotification : Notification {
        private Creature Creature;
        public HeartstoneNotification(Creature creature) {
            Creature = creature;
        }

        public override string Parse(Creature player) {
            if (Creature == player) {
                EffectPlayer.PlaySoundEffect(EffectType.HeartCrystal);
                return $"You consume the heartstone. Max health increased!";
            }
            return "";
        }
    }

    public class GoldenKeyNotification : Notification {
        private Creature Creature;
        public GoldenKeyNotification(Creature creature) {
            Creature = creature;
        }

        public override string Parse(Creature player) {
            if (Creature == player) {
                EffectPlayer.PlaySoundEffect(EffectType.GoldenKey);
                return $"You pick up the Golden Key!";
            }
            return "";
        }
    }

    public class ShoutNotification : Notification {
        private Creature Creature;
        private string Verb;
        private bool Noise;
        public ShoutNotification(Creature creature, string verb, bool noise = true) {
            Creature = creature;
            Verb = verb;
            Noise = noise;
        }

        public override string Parse(Creature player) {
            if (Noise) { EffectPlayer.PlaySoundEffect(EffectType.Alarm); }
            return $"The {Creature.Name} {Verb}.";
        }
    }
}
