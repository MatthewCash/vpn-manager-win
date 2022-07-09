using System.Configuration;
using System;
using System.IO;
using Tomlyn;
using Tomlyn.Model;

static class Config {
    static TomlTable config;

    public static TomlTable GetConfig() {
        return config;
    }

    static String GetConfigPath() {
        String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string configFilePath = Path.Combine(appDataPath, "VpnManager", "config.toml");

        return configFilePath;
    }

    static void CreateConfig(String configFilePath) {
        Directory.CreateDirectory(Directory.GetParent(configFilePath).FullName);
        File.Copy(@"resources\config.toml", configFilePath);
    }

    static String ReadConfigFile(String configFilePath) {
        String[] lines = File.ReadAllLines(configFilePath);
        String data = String.Join('\n', lines);

        return data;
    }

    static TomlTable ParseConfigData(String data) {
        return Toml.ToModel(data);
    }

    public static void LoadConfig() {
        String configFilePath = GetConfigPath();

        if (!File.Exists(configFilePath)) CreateConfig(configFilePath);

        String configData = ReadConfigFile(configFilePath);
        TomlTable config = ParseConfigData(configData);

        Config.config = config;
    }
}