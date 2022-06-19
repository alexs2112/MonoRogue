namespace MonoRogue {
    public static class Program {
        [System.STAThread]
        static void Main(string[] args) {
            using (var game = new Main(args))
                game.Run();
        }
    }
}
