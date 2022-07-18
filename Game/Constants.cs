using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public struct Constants {
        public static bool Debug;                   // If debug mode is enabled
        public static bool Invincible = false;      // If the player is invincible
        public static bool WriteMessagesToConsole = true;   // Default to writing messages to the console, turn this off if it is annoying
        public static int Difficulty = 3;           // Hard = 3, Medium = 2, Easy = 1
        public static int FPS = 45;                 // Frames per second

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

        public static int ColorIndex = 0;                   // Some player customization
        public static string Gender = "Man";                //
        public static Color[] Colors = new Color[] { Color.SkyBlue, Color.Yellow, Color.Red, Color.Chartreuse };
        public static string[] ColorNames = new string[] { "Blue", "Yellow", "Red", "Green" };
        public static string GetPlayerName() {
            return $"{Constants.ColorNames[Constants.ColorIndex]} {Constants.Gender}";
        }
    }

    public class SaveSettings {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public bool Fullscreen { get; set; }

        public float MusicVolume { get; set; }
        public float EffectVolume { get; set; }

        public int PlayerColor { get; set; }
        public string PlayerGender { get; set; }

        public bool AllowAnimations { get; set; }
    }

    public class Settings {
        public static void SaveSettings() {
            SaveSettings data = new SaveSettings() {
                ScreenWidth = Constants.ScreenWidth,
                ScreenHeight = Constants.ScreenHeight,
                Fullscreen = Constants.Fullscreen,
                MusicVolume = Constants.MusicVolume,
                EffectVolume = Constants.EffectVolume,
                PlayerColor = Constants.ColorIndex,
                PlayerGender = Constants.Gender,
                AllowAnimations = Constants.AllowAnimations
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Constants.SettingsPath, json);
        }

        public static void LoadSettings() {
            if (!File.Exists(Constants.SettingsPath)) { return; }

            string json = File.ReadAllText(Constants.SettingsPath);
            SaveSettings data;
            try {
                data = JsonSerializer.Deserialize<SaveSettings>(json);
            } catch (System.Exception e) {
                System.Console.WriteLine($"Broken settings data ({Constants.SettingsPath})");
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.StackTrace);
                return;
            }

            if (data.ScreenWidth > 0) { Constants.ScreenWidth = data.ScreenWidth; }
            if (data.ScreenHeight > 0) { Constants.ScreenHeight = data.ScreenHeight; }
            Constants.Fullscreen = data.Fullscreen;
            Constants.MusicVolume = data.MusicVolume;
            Constants.EffectVolume = data.EffectVolume;
            
            if (data.PlayerColor > -1 && data.PlayerColor < Constants.Colors.Length) { Constants.ColorIndex = data.PlayerColor; }
            if (data.PlayerGender.Length > 0) { Constants.Gender = data.PlayerGender; }

            Constants.AllowAnimations = data.AllowAnimations;
        }
    }
}
