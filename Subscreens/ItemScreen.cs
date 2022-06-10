using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class ItemScreen : BorderedScreen {
        Item Item;

        public ItemScreen(ContentManager content, Item item) : base(content) {
            Item = item;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape) { return null; }
            else if (mouse.RightClicked()) { return null; }
            return this;
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouse) {
            base.Draw(gameTime, spriteBatch, mouse);

            int x = 48;
            int y = 32;
            spriteBatch.DrawString(Font24, Item.Name, new Vector2(x - 16, y), Color.White);
            spriteBatch.Draw(Item.Glyph, new Vector2(x + Font24.MeasureString(Item.Name).X, y), Item.Color);
            y += 48;

            if (Item.IsFood) {
                Food food = (Food)Item;
                spriteBatch.DrawString(Font14, $"Food Value: {food.Value}", new Vector2(x, y), Color.White);
            } else if (Item.IsArmor) {
                Armor armor = (Armor)Item;
                spriteBatch.DrawString(Font14, $"Defense: {armor.Defense}/{armor.MaxDefense}", new Vector2(x, y), Color.White);
                y += 32;
                spriteBatch.DrawString(Font14, $"Refresh: {armor.Refresh}", new Vector2(x, y), Color.White);
                if (armor.MovementPenalty != 0) {
                    y += 32;
                    spriteBatch.DrawString(Font14, $"Movement Delay: {armor.MovementPenalty}", new Vector2(x, y), Color.White);
                }
            } else if (Item.IsWeapon) {
                Weapon weapon = (Weapon)Item;
                spriteBatch.DrawString(Font14, $"Damage Increase: {weapon.Damage.Min}-{weapon.Damage.Max}", new Vector2(x, y), Color.White);
                if (weapon.AttackDelay != 0) {
                    y += 32;
                    spriteBatch.DrawString(Font14, $"Attack Delay: {weapon.AttackDelay}", new Vector2(x, y), Color.White);
                }
            }
        }
    }
}
