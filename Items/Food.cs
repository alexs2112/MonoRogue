using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class Food : Item {
        public int Value;

        // The only item in the game, walking on it heals the creature who eats it
        private Food(string name, int value, Texture2D glyph, Color color) : base(name, glyph, color) {
            Value = value;
            IsFood = true;
            SetType(Type.Food);
        }

        public bool Eat(Creature creature) {
            if (creature.HP < creature.MaxHP) {
                creature.ModifyHP(Value);
                creature.Notify($"You eat the {Name}.");
                creature.Notify($"You heal for {Value} HP!");
                creature.NotifyOthers($"{creature.Name} eats the {Name}.");
                return true;
            } else {
                creature.Notify($"You are too full to eat the {Name}.");
                return false;
            }
        }

        // Essentially an enum that stores the different foods in the game
        public static Food Apple;
        public static Food Cheese;
        public static Food Meat;
        public static Food Fish;
        public static Food PigMeat; // Dropped by pigs when they die

        public static void LoadFood(ContentManager content) {
            Apple = new Food("Apple", 4, content.Load<Texture2D>("Food/Apple"), Color.Green);
            Apple.SetDescription("A delicious, firm green-skinned fruit, tart to the tongue.");
            Cheese = new Food("Cheese", 8, content.Load<Texture2D>("Food/Cheese"), Color.Yellow);
            Cheese.SetDescription("A lump of cheese. Nobody knows how it made its way into the dungeon.");
            Fish = new Food("Fish", 12, content.Load<Texture2D>("Food/Fish"), Color.Pink);
            Fish.SetDescription("Somehow this full cooked fish meal has arrived in this rather land-locked dungeon.");
            Meat = new Food("Meat", 16, content.Load<Texture2D>("Food/Meat"), Color.IndianRed);
            Meat.SetDescription("A hearty chunk of cooked meat from an unknown creature. Best not think too hard about where it came from.");
            PigMeat = new Food("Pig Meat", 8, content.Load<Texture2D>("Food/PigMeat"), Color.IndianRed);
            PigMeat.SetDescription("Meat doesn't get much fresher than this!");
        }

        public static Food RandomFood(System.Random random, int level) {
            if (level == 0) {
                int i = random.Next(10);
                if (i < 3) { return Apple; }
                else if (i < 8) { return Cheese; }
                else { return Fish; }
            } else if (level == 1) {
                int i = random.Next(10);
                if (i < 2) { return Apple; }
                else if (i < 5) { return Cheese; }
                else if (i < 8) { return Fish; }
                else { return Meat; }
            } else {
                int i = random.Next(10);
                if (i < 3) { return Cheese; }
                else if (i < 7) { return Fish; }
                else { return Meat; }
            }
        }
    }
}
