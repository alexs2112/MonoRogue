namespace MonoRogue {
    public struct Constants {
        public static bool Debug;
        public static int DungeonIterations = 3; // How many times we partition the dungeon, will result in up to 2^var rooms
        public static int RoomMinSize = 3;  // Minimum Width and Height of a room
    }
}