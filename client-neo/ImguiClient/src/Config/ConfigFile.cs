using Newtonsoft.Json;

namespace WestonClient.Config {
    public class ConfigFile {
        public ConfigFile(string configPath) {
            this.configPath = configPath;
        }

        private string configPath = "";

        public Configuration ReloadConfig() {
            if (!File.Exists(configPath)) {
                Configuration c = new Configuration();
                SaveConfig(c);
                return c;
            } else {
                string configData = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<Configuration>(configData);
            }
        }

        public void SaveConfig(Configuration c) {
            string configData = JsonConvert.SerializeObject(c);
            File.WriteAllText(configPath, configData);
        }
    }
}