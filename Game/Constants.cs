namespace MonoRogue {
    public struct Constants {
        public static bool Debug;                   // If debug mode is enabled
        public static bool Invincible = false;      // If the player is invincible
        public static bool WriteMessagesToConsole = true;   // Default to writing messages to the console, turn this off if it is annoying
        public static int Difficulty = 3;           // Hard = 3, Medium = 2, Easy = 1

        public static int ScreenWidth = 800;        // Size of the screen in pixels, default is 800 x 480 
        public static int ScreenHeight = 480;
        public static bool Fullscreen = false;

        public static int WorldWidth = 60;          // World dimensions when generated
        public static int WorldHeight = 35;         // Default: 60x35, Small: 40x25

        public static int DungeonIterations = 6;    // How many times we partition the dungeon, will result in up to 2^var rooms, default: 4
        public static int RoomMinSize = 4;          // Minimum Width and Height of a room

        public static int WorldViewWidth = 15;      // How many tiles are displayed from the world at once
        public static int WorldViewHeight = 15;     //

        public static long TickHoldRate = System.TimeSpan.TicksPerSecond / 8;   // How long you have to hold the same key for it to register again

        public static int Seed = -1;                // Randomization seed, -1 for a random seed

        public static int NumberOfDungeonWallSprites = 4;   // The number of floor and wall glyphs to load
        public static int NumberOfDungeonFloorSprites = 4;  //
        public static int NumberOfCaveWallSprites = 4;      //
        public static int NumberOfCaveFloorSprites = 4;     //

        public static bool AllowDiagonalMovement = true;    // Keeping this here in case we want to revert it to only Orthogonal movement
    }
}
