using System.Collections.Generic;
using System.IO;

namespace MonoRogue {
    public struct Constants {
        public static bool Debug;                   // If debug mode is enabled
        public static bool Invincible = false;      // If the player is invincible
        public static bool WriteMessagesToConsole = true;   // Default to writing messages to the console, turn this off if it is annoying
        public static int Difficulty = 3;           // Hard = 3, Medium = 2, Easy = 1

        public static int ScreenWidth = 800;        // Size of the screen in pixels, default is 800 x 480 
        public static int ScreenHeight = 480;
        public static bool Fullscreen = false;

        public static int WorldWidth = 65;          // World dimensions when generated
        public static int WorldHeight = 50;         // Default: 60x35, Small: 40x25

        public static int DungeonIterations = 6;    // How many times we partition the dungeon, will result in up to 2^var rooms, default: 4
        public static int RoomMinSize = 4;          // Minimum Width and Height of a room

        public static long TickHoldRate = System.TimeSpan.TicksPerSecond / 8;   // How long you have to hold the same key for it to register again

        public static int Seed = -1;                // Randomization seed, -1 for a random seed

        public static int NumberOfDungeonWallSprites = 4;   // The number of floor and wall glyphs to load
        public static int NumberOfDungeonFloorSprites = 4;  //
        public static int NumberOfCaveWallSprites = 4;      //
        public static int NumberOfCaveFloorSprites = 4;     //

        public static bool AllowDiagonalMovement = true;    // Keeping this here in case we want to revert it to only Orthogonal movement

        public static bool AllowAnimations = true;         // Enable/Disable attack and projectile animations

        public static float MusicVolume = 0.2f;             // Volume of background soundtrack
        public static float EffectVolume = 0.2f;            // Volume of sound effects

        public static string SettingsPath = "BlueMan.settings";     // Relative path to the settings file
        public static string SavegamePath = "BlueSave.savegame";    // Relative path to the currently saved game
        public static bool IndentedSave = false;                    // If the json of the save file should be formatted
    }

    public class Settings {
        public static void SaveSettings() {
            StreamWriter writer = new StreamWriter(Constants.SettingsPath);

            writer.WriteLine($"ScreenWidth:{Constants.ScreenWidth}");
            writer.WriteLine($"ScreenHeight:{Constants.ScreenHeight}");
            writer.WriteLine($"Fullscreen:{Constants.Fullscreen}");

            writer.WriteLine($"MusicVolume:{Constants.MusicVolume}");
            writer.WriteLine($"EffectVolume:{Constants.EffectVolume}");

            writer.Close();
        }

        public static void LoadSettings() {
            if (!File.Exists(Constants.SettingsPath)) { return; }

            Dictionary<string, string> settings = new Dictionary<string, string>();
            StreamReader reader = new StreamReader(Constants.SettingsPath);

            try {
                do {
                    string line = reader.ReadLine();
                    string[] s = line.Split(':', 2);
                    settings[s[0]] = s[1];
                } while (reader.Peek() != -1);
            } catch {
                System.Console.WriteLine("Failed to load settings file.");
            } finally {
                reader.Close();
            }

            if (settings.ContainsKey("ScreenWidth")) { Constants.ScreenWidth = int.Parse(settings["ScreenWidth"]); }
            if (settings.ContainsKey("ScreenHeight")) { Constants.ScreenHeight = int.Parse(settings["ScreenHeight"]); }

            if (settings.ContainsKey("Fullscreen")) { Constants.Fullscreen = bool.Parse(settings["Fullscreen"]); }

            if (settings.ContainsKey("MusicVolume")) { Constants.MusicVolume = float.Parse(settings["MusicVolume"]); }
            if (settings.ContainsKey("EffectVolume")) { Constants.EffectVolume = float.Parse(settings["EffectVolume"]); }
        }
    }
}
