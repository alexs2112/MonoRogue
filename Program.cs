using System;

namespace MonoRogue {
    public static class Program {
        [STAThread]
        static void Main(string[] args) {
            using (var game = new Main(args))
                game.Run();
        }
    }
}
