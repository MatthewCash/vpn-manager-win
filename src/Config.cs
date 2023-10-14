using System;
using System.IO;
using System.Runtime.Serialization;
using Tomlyn;

class Config {
    [DataMember(Name = "vpn_expected_address")]
    public string ExpectedAddress { get; set; }

    [DataMember(Name = "vpn_if_name")]
    public string IfName { get; set; }

    [DataMember(Name = "vpn_next_hop")]
    public string NextHop { get; set; }

    [DataMember(Name = "route_to_change")]
    public string RouteToChange { get; set; }

    [DataMember(Name = "route_mask")]
    public string RouteMask { get; set; }

    [DataMember(Name = "route_metric_off")]
    public uint RouteMetricOff { get; set; }

    [DataMember(Name = "route_metric_on")]
    public uint RouteMetricOn { get; set; }

    [DataMember(Name = "up_command")]
    public string UpCommand { get; set; }

    [DataMember(Name = "up_args")]
    public string UpArgs { get; set; }

    [DataMember(Name = "down_command")]
    public string DownCommand { get; set; }

    [DataMember(Name = "down_args")]
    public string DownArgs { get; set; }

    [DataMember(Name = "get_ip_url_host")]
    public string GetIpUrlHost { get; set; }

    static Config config;

    public static Config GetConfig() {
        return config;
    }

    static string GetConfigPath() {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string configFilePath = Path.Combine(appDataPath, "VpnManager", "config.toml");

        return configFilePath;
    }

    static void CreateConfig(string configFilePath) {
        Directory.CreateDirectory(Directory.GetParent(configFilePath).FullName);
        File.Copy(@"resources\config.toml", configFilePath);
    }

    static string ReadConfigFile(string configFilePath) {
        string[] lines = File.ReadAllLines(configFilePath);
        string data = string.Join('\n', lines);

        return data;
    }

    static Config ParseConfigData(string data) {
        return Toml.ToModel<Config>(data);
    }

    public static void LoadConfig() {
        string configFilePath = GetConfigPath();

        if (!File.Exists(configFilePath)) CreateConfig(configFilePath);

        string configData = ReadConfigFile(configFilePath);
        Config config = ParseConfigData(configData);

        Config.config = config;
    }
}
