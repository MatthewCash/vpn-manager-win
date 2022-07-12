using System;
using System.IO;
using Tomlyn;
using System.Runtime.Serialization;

class Config {
    [DataMember(Name = "vpn_expected_address")]
    public String expectedAddress { get; set; }

    [DataMember(Name = "vpn_if_index")]
    public uint ifIndex { get; set; }

    [DataMember(Name = "vpn_next_hop")]
    public String nextHop { get; set; }

    [DataMember(Name = "route_to_change")]
    public String routeToChange { get; set; }

    [DataMember(Name = "route_mask")]
    public String routeMask { get; set; }

    [DataMember(Name = "route_metric")]
    public uint routeMetric { get; set; }

    static Config config;

    public static Config GetConfig() {
        return config;
    }

    static String GetConfigPath() {
        String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        String configFilePath = Path.Combine(appDataPath, "VpnManager", "config.toml");

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

    static Config ParseConfigData(String data) {
        return Toml.ToModel<Config>(data);
    }

    public static void LoadConfig() {
        String configFilePath = GetConfigPath();

        if (!File.Exists(configFilePath)) CreateConfig(configFilePath);

        String configData = ReadConfigFile(configFilePath);
        Config config = ParseConfigData(configData);

        Config.config = config;
    }
}