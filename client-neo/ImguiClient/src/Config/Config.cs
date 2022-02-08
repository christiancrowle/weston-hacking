using static WestonClient.Helpers;

namespace WestonClient.Config { 
    public class ConfigDisplay {
        public int WindowWidth = 1600;
        public int WindowHeight = 900;

        public string FontPath = Res("font/Roboto-Medium.ttf");
        public float FontSize = 22f;

        public bool FullScreen = false;
    }

    public class ConfigWestonClient { 
        public string ServerAddr = "localhost";
        public int Port = 5000;
    }

    public class Configuration {
        public ConfigDisplay Display = new ConfigDisplay();
        public ConfigWestonClient Client = new ConfigWestonClient();
    }

    public class Settings {
        private static ConfigFile configFile;
        public static Configuration ConfigSettings;

        public static void InitializeSettings() {
            configFile = new ConfigFile(Root("config.conf"));
            ConfigSettings = configFile.ReloadConfig();
        }
    }
}