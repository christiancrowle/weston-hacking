using LibWestonClient;

namespace WestonClient {
    public class Program {
        public static void Main(string[] argv) {
            Config.Settings.InitializeSettings();
            using (var game = new WestonClientGame()) game.Run();
        }
    }
}