using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

static class CurrentIpAddress {
    static string cachedCheckHost = string.Empty;

    public static async Task<bool> CheckIpAddress(string tryAddress) {
        string currentAddress = string.Empty;

        // Getting the IP address may fail if the network is down, retry a few times with a delay
        for (int i = 0; i < 20; i++)
            try {
                currentAddress = await GetIpAddress();
                break;
            } catch (Exception) { await Task.Delay(50); };

        return tryAddress == currentAddress;
    }

    static async Task<string> GetIpAddress() {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("Host", "ifconfig.me");

        if (string.IsNullOrEmpty(cachedCheckHost)) await UpdateCheckHost();

        return await http.GetStringAsync("http://" + cachedCheckHost);
    }

    static async Task UpdateCheckHost() {
        var ips = await Dns.GetHostAddressesAsync("ifconfig.me");
        cachedCheckHost = ips[0].ToString();
    }

    static async Task PollIpAddress() {
        try {
            await UpdateCheckHost();
        } catch (Exception) { } // The DNS lookup can fail
        bool routed = await CheckIpAddress(Config.GetConfig().ExpectedAddress);

        VpnRouter.SetRouted(routed);
    }

    public static async void StartIpAddressPolling() {
        while (true) {
            await Task.Delay(5000);
            await PollIpAddress();
        }
    }
}
