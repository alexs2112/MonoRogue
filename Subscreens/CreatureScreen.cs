using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class CreatureScreen : BorderedScreen {
        private Creature Creature;
        private List<string> Description;
        private List<string> AbilityText;

        public CreatureScreen(ContentManager content, Creature creature) : base(content) {
            Creature = creature;
            if (Creature.Description != null) {
                Description = MainInterface.SplitMessage(Creature.Description, MaxScreenChars);
            }
            if (Creature.AbilityText != null) {
                AbilityText = MainInterface.SplitMessage(Creature.AbilityText, MaxScreenChars / 2 - 1);
            }
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            return base.RespondToInput(key, mouse);
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouse) {
            base.Draw(gameTime, spriteBatch, mouse);
            
            int x = 48;
            int y = 32;
            spriteBatch.DrawString(Font24, Creature.Name, new Vector2(x - 16, y), Color.White);
            spriteBatch.Draw(Creature.Glyph, new Vector2(x + Font24.MeasureString(Creature.Name).X, y), Creature.Color);

            y += 64;
            y = DrawHealthAndArmor(spriteBatch, x, y);
            y = DrawDelay(spriteBatch, x, y);

            (int Min, int Max) damage = Creature.GetDamage();
            spriteBatch.DrawString(Font14, $"Damage: {damage.Min}-{damage.Max}", new Vector2(x, y), Color.White);
            y += 32;
            spriteBatch.DrawString(Font14, $"Range: {Creature.GetRange()}", new Vector2(x, y), Color.White);
            y += 48;

            int parry = Creature.GetParryChance();
            if (parry > 0) {
                spriteBatch.DrawString(Font14, $"Parry Chance: {parry}%", new Vector2(x, y), Color.White);
                y += 48;
            }
            int crit = Creature.GetCritChance();
            if (crit > 0) {
                spriteBatch.DrawString(Font14, $"Critical Chance: {crit}%", new Vector2(x, y), Color.White);
                y += 48;
            }

            x = Constants.ScreenWidth / 2;
            y = 32;
            y = DrawEquipment(spriteBatch, x, y);
            y = DrawAbilities(spriteBatch, x, y);

            DrawDescription(spriteBatch);
        }

        private int DrawHealthAndArmor(SpriteBatch spriteBatch, int x, int y) {
            spriteBatch.DrawString(Font14, "Health: ", new Vector2(x, y), Color.White);
            MainInterface.DrawHearts(spriteBatch, Creature.MaxHP, Creature.HP, x + 142, y - 8, Color.Red);

            y += 32;
            spriteBatch.DrawString(Font14, "Defense: ", new Vector2(x, y), Color.White);
            if (Creature.GetDefense().Max == 0) {
                spriteBatch.DrawString(Font14, "None", new Vector2(x + 160, y), Color.Gray);
            } else {
                MainInterface.DrawHearts(spriteBatch, Creature.GetDefense().Max, Creature.GetDefense().Current, x + 160, y - 8, Color.LightSkyBlue);
            }

            y += 32;
            spriteBatch.DrawString(Font14, $"Block: {Creature.GetBlock()}", new Vector2(x, y), Creature.GetBlock() > 0 ? Color.White : Color.Gray);

            return y + 48;
        }

        private int DrawDelay(SpriteBatch spriteBatch, int x, int y) {
            spriteBatch.DrawString(Font14, "Move Delay: ", new Vector2(x, y), Color.White);

            Color c = Color.LightGray;
            int moveDelay = Creature.GetMovementDelay();
            if (moveDelay < 8) { c = Color.LightSeaGreen; }
            else if (moveDelay > 12) { c = Color.LightSalmon; }
            spriteBatch.DrawString(Font14, moveDelay.ToString(), new Vector2(x + 18 * 12, y), c);

            y += 32;
            spriteBatch.DrawString(Font14, "Attack Delay: ", new Vector2(x, y), Color.White);

            c = Color.LightGray;
            int attackDelay = Creature.GetAttackDelay();
            if (attackDelay < 8) { c = Color.LightSeaGreen; }
            else if (attackDelay > 12) { c = Color.LightSalmon; }
            spriteBatch.DrawString(Font14, attackDelay.ToString(), new Vector2(x + 18 * 14, y), c);
            
            return y + 48;
        }

        private int DrawEquipment(SpriteBatch spriteBatch, int x, int y) {
            spriteBatch.DrawString(Font16, "Equipment", new Vector2(x - 16, y), Color.White);
            y += 32;
            if (Creature.Weapon == null) {
                spriteBatch.DrawString(Font14, "Unarmed", new Vector2(x + 16, y), Color.Gray);
                y += 32;
            } else {
                y = DrawWeapon(spriteBatch, x + 48, y, Creature.Weapon);
            }
            if (Creature.Armor == null) {
                spriteBatch.DrawString(Font14, "No Armor", new Vector2(x + 16, y), Color.Gray);
                y += 48;
            } else {
                y = DrawArmor(spriteBatch, x + 48, y, Creature.Armor);
            }
            return y;
        }

        private int DrawWeapon(SpriteBatch spriteBatch, int x, int y, Weapon w) {
            spriteBatch.DrawString(Font16, w.Name, new Vector2(x - 32, y), Color.White);
            y += 32;
            spriteBatch.DrawString(Font14, $"Damage: {w.Damage.Min}-{w.Damage.Max}", new Vector2(x, y), Color.White);
            y += 32;
            spriteBatch.DrawString(Font14, $"Range: {w.Range}", new Vector2(x, y), Color.White);
            y += 32;
            spriteBatch.DrawString(Font14, $"Attack Delay: {w.Delay}", new Vector2(x, y), Color.White);
            return y + 32;
        }
        private int DrawArmor(SpriteBatch spriteBatch, int x, int y, Armor a) {
            spriteBatch.DrawString(Font16, a.Name, new Vector2(x - 32, y), Color.White);
            y += 32;
            spriteBatch.DrawString(Font14, $"Defense:", new Vector2(x, y), Color.White);
            MainInterface.DrawHearts(spriteBatch, a.MaxDefense, a.Defense, x + 160, y - 8, Color.LightSkyBlue);
            if (a.Block > 0) {
                y += 32;
                spriteBatch.DrawString(Font14, $"Block: {a.Block}", new Vector2(x, y), Color.White);
            }
            y += 32;
            spriteBatch.DrawString(Font14, $"Weight: {a.Weight}", new Vector2(x, y), Color.White);
            return y + 32;
        }

        private int DrawAbilities(SpriteBatch spriteBatch, int x, int y) {
            if (AbilityText == null) { return y; }
            
            spriteBatch.DrawString(Font16, "Abilities", new Vector2(x - 16, y), Color.White);
            y += 32;

            foreach (string s in AbilityText) {
                spriteBatch.DrawString(Font14, s, new Vector2(x+16, y), Color.White);
                y += 32;
            }

            return y + 48;
        }

        private void DrawDescription(SpriteBatch spriteBatch) {
            if (Description == null) { return; }
            int x = 32;
            int y = Constants.ScreenHeight - (Description.Count + 1) * 32;
            foreach (string s in Description) {
                spriteBatch.DrawString(Font14, s, new Vector2(x, y), Color.Gray);
                y += 32;
            }
        }
    }
}
