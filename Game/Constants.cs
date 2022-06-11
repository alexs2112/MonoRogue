namespace MonoRogue {
    public struct Constants {
        public static bool Debug;                   // If debug mode is enabled
        public static bool WriteMessagesToConsole = true;   // Default to writing messages to the console, turn this off if it is annoying

        public static int ScreenWidth = 800;        // Size of the screen in pixels, default for monogame is 800 x 480 
        public static int ScreenHeight = 480;       // Currently broken

        public static int WorldWidth = 40;          // World dimensions when generated
        public static int WorldHeight = 25;

        public static int DungeonIterations = 4;    // How many times we partition the dungeon, will result in up to 2^var rooms
        public static int RoomMinSize = 4;          // Minimum Width and Height of a room

        public static int WorldViewWidth = 15;      // How many tiles are displayed from the world at once
        public static int WorldViewHeight = 15;     //

        public static long TickHoldRate = System.TimeSpan.TicksPerSecond / 8;   // How long you have to hold the same key for it to register again

        public static int Seed = -1;                // Randomization seed, -1 for a random seed

        public static int NumberOfWallSprites = 4;  // The number of floor and wall sprites to load
        public static int NumberOfFloorSprites = 4; //

        public static bool AllowDiagonalMovement = true;    // Keeping this here in case we want to revert it to only Orthogonal movement
    }
}
