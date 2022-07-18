using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class ItemScreen : BorderedScreen {
        private Item Item;
        private List<string> Description;

        public ItemScreen(ContentManager content, Item item) : base(content) {
            Item = item;
            if (item.Description != null) {
                Description = Font.Size14.SplitString(Item.Description, Constants.ScreenWidth - 64);
            }
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            return base.RespondToInput(key, mouse);
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouse) {
            base.Draw(gameTime, spriteBatch, mouse);

            int x = 48;
            int y = 32;
            spriteBatch.DrawString(Font.Get(24), Item.Name, new Vector2(x - 16, y), Color.White);
            spriteBatch.Draw(Item.Glyph, new Vector2(x + Font.Size24.Width * Item.Name.Length, y), Item.Color);
            y += 48;

            if (Item.IsFood) {
                Food food = (Food)Item;
                spriteBatch.DrawString(Font.Get(14), $"Food Value:", new Vector2(x, y), Color.White);
                MainInterface.DrawHearts(spriteBatch, food.Value, food.Value, x + 216, y - 8, Color.Yellow);
            } else if (Item.IsArmor) {
                Armor armor = (Armor)Item;
                spriteBatch.DrawString(Font.Get(14), $"Defense:", new Vector2(x, y), Color.White);
                MainInterface.DrawHearts(spriteBatch, armor.MaxDefense, armor.Defense, x + 160, y - 8, Color.LightSkyBlue);
                if (armor.Block > 0) {
                    y += 32;
                    spriteBatch.DrawString(Font.Get(14), $"Block: {armor.Block}", new Vector2(x, y), Color.White);
                }
                y += 32;
                spriteBatch.DrawString(Font.Get(14), $"Weight: {armor.Weight}", new Vector2(x, y), Color.White);
            } else if (Item.IsWeapon) {
                Weapon weapon = (Weapon)Item;
                spriteBatch.DrawString(Font.Get(14), $"Damage: {weapon.Damage.Min}-{weapon.Damage.Max}", new Vector2(x, y), Color.White);
                y += 32;
                spriteBatch.DrawString(Font.Get(14), $"Range: {weapon.Range}", new Vector2(x, y), Color.White);
                y += 32;
                spriteBatch.DrawString(Font.Get(14), $"Attack Delay: {weapon.Delay}", new Vector2(x, y), Color.White);
            }
            y += 48;
            DrawDescription(spriteBatch, 32, y);
        }

        private int DrawDescription(SpriteBatch spriteBatch, int x, int y) {
            if (Description == null) { return y; }
            foreach (string s in Description) {
                spriteBatch.DrawString(Font.Get(14), s, new Vector2(x, y), Color.Gray);
                y += 32;
            }
            return y + 48;
        }
    }
}
