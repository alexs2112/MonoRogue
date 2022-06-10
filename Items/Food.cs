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
        public static Food PigMeat; // Dropped by pigs when they die

        public static void LoadFood(ContentManager content) {
            Apple = new Food("Apple", 2, content.Load<Texture2D>("Food/Apple"), Color.Green);
            Apple.SetDescription("TEMP: A delicious, firm green-skinned fruit, tart to the tongue.");
            Cheese = new Food("Cheese", 4, content.Load<Texture2D>("Food/Cheese"), Color.Yellow);
            Cheese.SetDescription("A lump of cheese. Nobody knows how it made its way into the dungeon.");
            Meat = new Food("Meat", 8, content.Load<Texture2D>("Food/Meat"), Color.IndianRed);
            Meat.SetDescription("TEMP: A hearty chunk of cooked meat from an unknown creature. Best not think too hard about where it came from.");
            PigMeat = new Food("Pig Meat", 6, content.Load<Texture2D>("Food/PigMeat"), Color.IndianRed);
            PigMeat.SetDescription("Meat doesn't get much fresher than this!");
        }

        public static Food RandomFood(System.Random random) {
            int i = random.Next(10);
            if (i < 4) { return Apple; }
            else if (i < 8) { return Cheese; }
            else if (i < 10) { return Meat; }
            else { return null; } // To stop compiler from complaining
        }
    }
}
