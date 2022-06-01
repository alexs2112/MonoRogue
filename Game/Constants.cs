namespace MonoRogue {
    public struct Constants {
        public static bool Debug;                   // If debug mode is enabled

        public static int WorldWidth = 40;          // World dimensions when generated
        public static int WorldHeight = 25;

        public static int DungeonIterations = 4;    // How many times we partition the dungeon, will result in up to 2^var rooms
        public static int RoomMinSize = 4;          // Minimum Width and Height of a room

        public static int WorldViewWidth = 25;      // How many tiles are displayed from the world at once
        public static int WorldViewHeight = 15;     //
    }
}