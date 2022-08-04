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
        private int BreakHeight;
        private static int MaxColumnWidth = 500;

        public CreatureScreen(ContentManager content, Creature creature) : base(content) {
            Creature = creature;
            if (Creature.Description != null) {
                int width = Constants.ScreenWidth - 64;
                if (width > MaxColumnWidth * 2 - 64) { width = MaxColumnWidth * 2 - 64; }
                Description = Font.Size14.SplitString(Creature.Description, width);
            }
            if (Creature.AbilityText != null) {
                int width = Constants.ScreenWidth / 2 - 24;
                if (width > MaxColumnWidth - 24) { width = MaxColumnWidth - 24; }
                AbilityText = Font.Size14.SplitString(Creature.AbilityText, width);
            }

            BreakHeight = Constants.ScreenHeight / 40;
            if (BreakHeight > 32) { BreakHeight = 32; }
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            return base.RespondToInput(key, mouse);
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouse) {
            base.Draw(gameTime, spriteBatch, mouse);
            
            int x = Constants.ScreenWidth / 2 - 32;
            int y = 32;
            WriteCentered(spriteBatch, Font.Get(24), Creature.Name, new Vector2(x, y), Color.White);
            spriteBatch.Draw(Creature.Glyph, new Vector2(x + (Font.Size24.Width * Creature.Name.Length) / 2 + 16, y), Creature.Color);

            x = 48;
            if (x < Constants.ScreenWidth / 2 - MaxColumnWidth) { x = Constants.ScreenWidth / 2 - MaxColumnWidth; }
            y += 32 + BreakHeight;
            y = DrawHealthAndArmor(spriteBatch, x, y);
            y = DrawDelay(spriteBatch, x, y);

            (int Min, int Max) damage = Creature.GetDamage();
            spriteBatch.DrawString(Font.Get(14), $"Damage: {damage.Min}-{damage.Max}", new Vector2(x, y), Color.White);
            y += 32;
            spriteBatch.DrawString(Font.Get(14), $"Range: {Creature.GetRange()}", new Vector2(x, y), Color.White);
            y += 32 + BreakHeight;

            int parry = Creature.GetParryChance();
            if (parry > 0) {
                spriteBatch.DrawString(Font.Get(14), $"Parry Chance: {parry}%", new Vector2(x, y), Color.White);
                y += 32 + BreakHeight;
            }
            int crit = Creature.GetCritChance();
            if (crit > 0) {
                spriteBatch.DrawString(Font.Get(14), $"Critical Chance: {crit}%", new Vector2(x, y), Color.White);
                y += 32 + BreakHeight;
            }

            x = Constants.ScreenWidth / 2;
            y = 64 + BreakHeight;
            y = DrawEquipment(spriteBatch, x, y);
            y = DrawAbilities(spriteBatch, x, y);

            DrawDescription(spriteBatch);
        }

        private int DrawHealthAndArmor(SpriteBatch spriteBatch, int x, int y) {
            spriteBatch.DrawString(Font.Get(14), "Health: ", new Vector2(x, y), Color.White);
            MainInterface.DrawHearts(spriteBatch, Creature.MaxHP, Creature.HP, x + 142, y - 8, Color.Red);

            y += 32;
            spriteBatch.DrawString(Font.Get(14), "Defense: ", new Vector2(x, y), Color.White);
            if (Creature.GetDefense().Max == 0) {
                spriteBatch.DrawString(Font.Get(14), "None", new Vector2(x + 160, y), Color.Gray);
            } else {
                MainInterface.DrawHearts(spriteBatch, Creature.GetDefense().Max, Creature.GetDefense().Current, x + 160, y - 8, Color.LightSkyBlue);
            }

            y += 32;
            spriteBatch.DrawString(Font.Get(14), $"Block: {Creature.GetBlock()}", new Vector2(x, y), Creature.GetBlock() > 0 ? Color.White : Color.Gray);

            return y + 32 + BreakHeight;
        }

        private int DrawDelay(SpriteBatch spriteBatch, int x, int y) {
            spriteBatch.DrawString(Font.Get(14), "Move Delay: ", new Vector2(x, y), Color.White);

            Color c = Color.LightGray;
            int moveDelay = Creature.GetMovementDelay();
            if (moveDelay < 8) { c = Color.LightSeaGreen; }
            else if (moveDelay > 12) { c = Color.LightSalmon; }
            spriteBatch.DrawString(Font.Get(14), moveDelay.ToString(), new Vector2(x + 18 * 12, y), c);

            y += 32;
            spriteBatch.DrawString(Font.Get(14), "Attack Delay: ", new Vector2(x, y), Color.White);

            c = Color.LightGray;
            int attackDelay = Creature.GetAttackDelay();
            if (attackDelay < 8) { c = Color.LightSeaGreen; }
            else if (attackDelay > 12) { c = Color.LightSalmon; }
            spriteBatch.DrawString(Font.Get(14), attackDelay.ToString(), new Vector2(x + 18 * 14, y), c);
            
            return y + 32 + BreakHeight;
        }

        private int DrawEquipment(SpriteBatch spriteBatch, int x, int y) {
            if (Creature.Weapon == null) {
                spriteBatch.DrawString(Font.Get(14), "Unarmed", new Vector2(x + 16, y), Color.Gray);
                y += 32;
            } else {
                y = DrawWeapon(spriteBatch, x + 48, y, Creature.Weapon);
            }
            if (Creature.Armor == null) {
                spriteBatch.DrawString(Font.Get(14), "No Armor", new Vector2(x + 16, y), Color.Gray);
                y += 32;
            } else {
                y = DrawArmor(spriteBatch, x + 48, y, Creature.Armor);
            }
            return y + BreakHeight;
        }

        private int DrawWeapon(SpriteBatch spriteBatch, int x, int y, Weapon w) {
            spriteBatch.DrawString(Font.Get(16), w.Name, new Vector2(x - 32, y), Color.White);
            y += 32;
            spriteBatch.DrawString(Font.Get(14), $"Damage: {w.Damage.Min}-{w.Damage.Max}", new Vector2(x, y), Color.White);
            y += 32;
            if (w.Range > 1) {
                spriteBatch.DrawString(Font.Get(14), $"Range: {w.Range}", new Vector2(x, y), Color.White);
                y += 32;
            }
            spriteBatch.DrawString(Font.Get(14), $"Attack Delay: {w.Delay}", new Vector2(x, y), Color.White);
            return y + 32;
        }
        private int DrawArmor(SpriteBatch spriteBatch, int x, int y, Armor a) {
            spriteBatch.DrawString(Font.Get(16), a.Name, new Vector2(x - 32, y), Color.White);
            y += 32;
            spriteBatch.DrawString(Font.Get(14), $"Defense:", new Vector2(x, y), Color.White);
            MainInterface.DrawHearts(spriteBatch, a.MaxDefense, a.Defense, x + 160, y - 8, Color.LightSkyBlue);
            if (a.Block > 0) {
                y += 32;
                spriteBatch.DrawString(Font.Get(14), $"Block: {a.Block}", new Vector2(x, y), Color.White);
            }
            y += 32;
            spriteBatch.DrawString(Font.Get(14), $"Weight: {a.Weight}", new Vector2(x, y), Color.White);
            return y + 32;
        }

        private int DrawAbilities(SpriteBatch spriteBatch, int x, int y) {
            if (AbilityText == null) { return y; }

            spriteBatch.DrawString(Font.Get(16), "Abilities", new Vector2(x - 16, y), Color.White);
            y += 32;

            foreach (string s in AbilityText) {
                spriteBatch.DrawString(Font.Get(14), s, new Vector2(x, y), Color.White);
                y += 32;
            }

            return y + BreakHeight;
        }

        private void DrawDescription(SpriteBatch spriteBatch) {
            if (Description == null) { return; }
            int x = 32;
            if (x < Constants.ScreenWidth / 2 - MaxColumnWidth) { x = Constants.ScreenWidth / 2 - MaxColumnWidth; }

            int y = Constants.ScreenHeight - (Description.Count + 1) * 24;
            if (Constants.ScreenHeight > 480) { y -= 12; }
            foreach (string s in Description) {
                spriteBatch.DrawString(Font.Get(14), s, new Vector2(x, y), Color.Gray);
                y += 24;
            }
        }
    }
}
