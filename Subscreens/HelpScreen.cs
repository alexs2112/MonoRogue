using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {

    // Base class for help screens to provide info about the game
    public class HelpScreen : BorderedScreen {
        protected List<(string Text, int Size, int Y)> Text;
        private Subscreen LastScreen;
        private int Y;
        private int YIncrement = 24;

        // Where text starts an ends, accounting for border
        protected int StartY;
        private int EndY;

        public HelpScreen(ContentManager content, Subscreen lastScreen = null) : base(content) {
            Text = new List<(string text, int size, int y)>();
            LastScreen = lastScreen;
            StartY = 24;
            EndY = Constants.ScreenHeight - 48;
        }

        protected List<string> SplitText(string[] text, int size = -1) {
            List<string> output = new List<string>();
            foreach (string line in text) {
                output.AddRange(Font.Size12.SplitString(line, size == -1 ? Constants.ScreenWidth - 64 : size));
            }
            return output;
        }

        protected List<string> SplitString(string text) {
            List<string> output = new List<string>();
            output.AddRange(Font.Size12.SplitString(text, Constants.ScreenWidth - 64));
            return output;
        }

        protected List<string> SplitList(string[] text) {
            // For formatting a list of elements, just assume that list elements look like " - {texttexttext}"
            // If we have a list element that starts with ".", dont format it
            List<string> old = SplitText(text, Constants.ScreenWidth - 112);
            List<string> output = new List<string>();
            bool lastIsUnformatted = false;
            foreach (string s in old) {
                if (s.StartsWith(".")) {
                    output.Add(s.Remove(0,1));
                    lastIsUnformatted = true;
                } else if (s.StartsWith(" - ")) {
                    output.Add(s);
                    lastIsUnformatted = false;
                } else if (lastIsUnformatted) {
                    output.Add(s);
                    lastIsUnformatted = true;
                } else {
                    output.Add($"   {s}");
                    lastIsUnformatted = false;
                }
            }
            return output;
        }

        private int GetY() {
            if (Text.Count == 0) { return StartY; }
            return Text[Text.Count - 1].Y + Text[Text.Count - 1].Size + 24;
        }

        protected void AddTextArea(string title, List<string> text) {
            int y = GetY();
            if (title != null) {    
                Text.Add((title, 16, y));
                y += 32;
            }

            if (text == null) { return; }
            for (int i = 0; i < text.Count; i++) {
                Text.Add((text[i], 12, y));
                y += 24;
            }
        }

        private int LastY() {
            if (Text.Count == 0) { return 0; }
            return Text[Text.Count - 1].Y;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { CloseSubscreen(); return LastScreen; }
            else if (IsDown(key)) { if (LastY() - Y > EndY - 8) { 
                Y += YIncrement;
            } }
            else if (IsUp(key)) { if (Y > 0) { Y -= YIncrement; } }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            int x = 32;
            for (int i = 0; i < Text.Count; i++) {
                (string text, int size, int y) = Text[i];
                if (y + size < Y + 8) { continue; }

                spriteBatch.DrawString(Font.Get(size), text, new Vector2(x, y - Y), Color.White);
                if (y - Y >= Constants.ScreenHeight - 8) { break; }
            }
            base.Draw(gameTime, spriteBatch, mouseHandler);
        }
    }

    // The initial menu screen that will direct to different help screens
    public class HelpMenuScreen : BorderedScreen {
        private Subscreen LastScreen;
        private int Index;
        /*
        *   0: Controls
        *   1: Introduction
        *   2: Stats Overview
        *   3: Items Overview
        *   4: Saving Issues
        *   5: Credits
        *   6: Continue
        */
        public HelpMenuScreen(ContentManager content, Subscreen lastScreen = null) : base(content) { 
            LastScreen = lastScreen;
            Index = 0;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { CloseSubscreen(); return LastScreen; }
            else if (IsDown(key)) { if (Index < 6) { Index++; EffectPlayer.PlaySoundEffect(EffectType.MenuMove); } }
            else if (IsUp(key)) { if (Index > 0) { Index--; EffectPlayer.PlaySoundEffect(EffectType.MenuMove); } }
            else if (key == Keys.Enter || key == Keys.Space) {
                switch(Index) {
                    case 0: EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); return new HelpControlsScreen(Content, this);
                    case 1: EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); return new HelpIntroductionScreen(Content, this);
                    case 2: EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); return new HelpStatsScreen(Content, this);
                    case 3: EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); return new HelpItemsScreen(Content, this);
                    case 4: EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); return new HelpSavingScreen(Content, this);
                    case 5: EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); return new HelpCreditsScreen(Content, this);
                    default: EffectPlayer.PlaySoundEffect(EffectType.MenuSelect); return LastScreen;
                }
            }

            return this;
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            Vector2 v = new Vector2(Constants.ScreenWidth / 2, Constants.ScreenHeight / 2 - 154);
            WriteCentered(spriteBatch, Font.Get(24), "Help Menu", v, Color.White);
            v.Y += 64;
            WriteCentered(spriteBatch, Font.Get(16), "Controls", v, Index == 0 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Introduction", v, Index == 1 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Stats Overview", v, Index == 2 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Items Overview", v, Index == 3 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Saving Issues", v, Index == 4 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Credits", v, Index == 5 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font.Get(16), "Continue", v, Index == 6 ? Color.LawnGreen : Color.White);
            v.Y += 32;
        }
    }

    // Subscreen to display the games controls
    public class HelpControlsScreen : HelpScreen {
        public HelpControlsScreen(ContentManager content, Subscreen lastScreen = null) : base(content, lastScreen) {
            Text.Add(("Keybindings", 24, StartY));

            string title = "Movement Controls:";
            string[] text = new string[] {
                ".There are a few different keybindings for movement and navigating menus.",
                ".You can move into enemies to attack them, and move into doors to open them.",
                " - Arrow Keys",
                " - Left Click",
                " - Number Keys",
                " - Vi Keys"
            };
            AddTextArea(title, SplitList(text));

            title = "Interaction Controls:";
            text = new string[] {
                " - Spacebar or click yourself to interact with items on the ground",
                " - [.] or [5] to wait one turn",
                " - [r] to rest and repair armor",
                " - [f] to fire a ranged weapon without using the mouse"
            };
            AddTextArea(title, SplitList(text));

            title = "Menu Controls:";
            text = new string[] {
                " - Right click creatures or items to see their stats",
                " - [s] to see your own stats",
                " - [m] to view your world map",
                " - [x] to examine a tile without using the mouse",
                " - [esc] to quit the game or exit menus",
                " - [?] to show this menu"
            };
            AddTextArea(title, SplitList(text));
        }
    }

    public class HelpIntroductionScreen : HelpScreen {
        public HelpIntroductionScreen(ContentManager content, Subscreen lastScreen = null) : base(content, lastScreen) {
            Text.Add(("Introduction", 24, StartY));

            AddTextArea("Welcome to the Dungeon!", SplitString(
                "Escape of the Blue Man is a very simple dungeon crawling roguelike. Your goal is to explore the dungeon, gathering equipment and fighting enemies, until you can defeat The Warden, collect the Golden Key, and find the exit."
            ));

            AddTextArea(null, SplitString(
                "The dungeon is broadly divided into three sections of increasing difficulty, viewable through the Map Screen ([m]). The further you make it into the dungeon, the harder the enemies will get and the more powerful equipment you will find."
            ));

            AddTextArea(null, SplitString(
                "There are no levels or abilities. Progression is through equipment and finding Heartstones that will increase your maximum health."
            ));

            AddTextArea(null, SplitString(
                "Attacking enemies can be done by bumping into them, left clicking them, or firing a ranged weapon through the Fire Screen ([f])."
            ));

            AddTextArea(null, SplitText(
                new string[] {
                "Different equipment will change how you survive and fight enemies.",
                "Heavier weapons such as Axes and Maces benefit from heavy armor, allowing you to fight enemies in close combat for longer.", 
                "Lighter armor benefits weapons such as Bows and Daggers, increasing your critical chance and allowing you to run away when things get difficult.",
                "Mixing and Matching weapons and armor will allow you to come up with strategies for different encounters."
                }
            ));

            AddTextArea(null, SplitString(
                "This game is meant to be as simple as possible, focusing on item builds and a short dungeon crawling experience."
            ));
        }
    }

    public class HelpStatsScreen : HelpScreen {
        public HelpStatsScreen(ContentManager content, Subscreen lastScreen = null) : base(content, lastScreen) {
            Text.Add(("Stats", 24, StartY));
            AddTextArea(null, SplitString(
                "Each creature has 4 main stats, with 3 extra stats that can be obtained by the player through equipment."
            ));
            AddTextArea("Main Stats:", SplitList(
                new string[] {
                " - Health: Takes the form of red hearts. Each full red heart is equal to 4 health. When this reaches 0, you die!",
                " - Defense: Takes the  form of blue hearts. Functions similarly to health, but regenerates over time.",
                " - Attack Delay: How long it takes to make an attack. Creatures with lower Attack Delay will attack more often.",
                " - Move Delay: Similar to Attack Delay, creatures with lower Move Delay will move more often than."
                }
            ));

            AddTextArea("Extra Stats:", SplitList(
                new string[] {
                " - Block: Granted by heavy armor, this reduces all damage you take by a flat amount.",
                " - Critical Chance: Granted by Daggers, the percentage chance to deal double damage on attack.",
                " - Parry Chance: Granted by Swords, the percentage chance to take no damage upon being attacked."
                }
            ));

            AddTextArea(null, SplitString(
                "Enemies also have a difficulty rating, depicted by a flame icon next to their name when you mouse over them. The difficulty of the dungeon increases as you progress deeper into it."
            ));
        }
    }

    public class HelpItemsScreen : HelpScreen {
        public HelpItemsScreen(ContentManager content, Subscreen lastScreen = null) : base(content, lastScreen) {
            Text.Add(("Items", 24, StartY));
            AddTextArea(null, SplitString(
                "There are three types of items, along with two other miscellaneous items."
            ));
            AddTextArea("Food:", SplitString(
                "There is no passive health regeneration in the dungeon. Survivability is through Food, found laying around the dungeon. Pressing [space] or clicking yourself when you are standing over food will eat it, healing you for health equal to the yellow heart value of the food."
            ));

            AddTextArea("Weapons:", SplitList(
                new string[] {
                ".There are 6 different weapon types in the game. Each functions differently to promote different builds and strategies.",
                " - Swords have a chance to parry incoming attacks. Parry chance decreased by armor weight.",
                " - Spears have increased range. Moving towards an enemy let you \"lunge\" and get a free attack.",
                " - Axes hit all enemies adjacent to you whenever you attack.",
                " - Daggers lower your attack delay and can critically hit. Critical hit chance decreased by armor weight.",
                " - Maces deal bonus damage to blue hearts.",
                " - Bows have extended range but have increased attack delay."
                }
            ));

            AddTextArea("Armor:", SplitList(
                new string[] {
                ".Armor consists of three different stats:",
                " - Defense: Increases the equipped creatures base defense. See the Stats Help screen for more info.",
                " - Weight: Increases both Attack Delay and Movement Delay, slowing you down. Also reduces chance to make Critical attacks and Parry incoming attacks.",
                " - Block: Found on heavier armor. Increases the equipped creatures base Block, reducing incoming damage by a flat amount.",
                }
            ));

            AddTextArea(null, SplitList(
                new string[] {
                ".There are three types of armor:",
                " - Light Armor: Has no weight but does not provide much protection. Best used with bows and daggers as the lack of weight allows you to run away from melee enemies and make multiple attacks against slower ones.",
                " - Medium Armor: Has minor weight and provides moderate protection. Best used with swords and spears as melee weapons that want the defense, but don't want too much weight for parrying attacks or avoiding enemies.",
                " - Heavy Armor: Has the greatest weight and the highest defense. Reduces incoming damage with Block. Best used with axes and maces as heavy hitting melee weapons that benefit the most from being in close combat."
                }
            ));

            AddTextArea("Heartstone:", SplitText(
                new string[] {
                "Heartstone is found in the end of each dungeon region. It permanently increases your max health by one heart when consumed.",
                "Press [space] or click yourself when standing over a Heartstone to consume it."
                }
            ));

            AddTextArea("Golden Key:", SplitString(
                "Dropped by the Warden in the last room of the dungeon. You need this to unlock the exit to make your escape."
            ));
        }
    }

    public class HelpSavingScreen : HelpScreen {
        public HelpSavingScreen(ContentManager content, Subscreen lastScreen = null) : base(content, lastScreen) {
            Text.Add(("Persistency", 24, StartY));

            AddTextArea(null, SplitString(
                "This section is to go over some potential issues with saving games and settings. Important if your changes to settings do not persist through game sessions or you are unable to load a previously saved game."
            ));

            AddTextArea("Settings", SplitText(
                new string[] {
                "The game will save your changes to settings when you enter \"Save and Continue\" from the settings screen.",
                $"These settings will be saved in a file called \"{Constants.SettingsPath}\" and should be in the same folder as the game executable."
                }
            ));

            AddTextArea("Save Game", SplitText(
                new string[] {
                "The game will save the game on exit. Starting a new game will overwrite the old save game.",
                $"Your game will be saved in \"{Constants.SavegamePath}\" and should be in the same folder as the game executable for it to load."
                }
            ));

            AddTextArea("Game History", SplitText(
                new string[] {
                $"Game history will be saved in a file called \"{Constants.HistoryPath}.\" This keeps track of certain stats from previous games, to record your scores.",
                }
            ));

            AddTextArea(null, SplitString(
                "If you move the game executable somewhere where it is not in the same folder as the save file and the settings file, it will replace each one with a fresh file."
            ));
        }
    }

    public class HelpCreditsScreen : HelpScreen {
        public HelpCreditsScreen(ContentManager content, Subscreen lastScreen = null) : base(content, lastScreen) {
            Text.Add(("Credits", 24, StartY));

            AddTextArea(null, SplitString(
                "Developed by myself, Urist2112. https://urist2112.itch.io"
            ));
            AddTextArea(null, SplitString(
                "Written using the C# MonoGame framework. https://www.monogame.net/"
            ));
            AddTextArea(null, SplitString(
                "A large portion of the tileset is from https://www.kenney.nl"
            ));
            AddTextArea(null, SplitString(
                "Background soundtracks from https://opengameart.org/content/game-game"
            ));
        }
    }
}
