using System;
using System.Threading.Tasks;
using System.Net.Http;

static class CurrentIpAddress {
    public static async Task<bool> CheckIpAddress(String tryAddress) {
        String currentAddress = String.Empty;

        // Getting the IP address may fail if the network is down, retry a few times with a delay
        for (int i = 0; i < 20; i++) try {
            currentAddress = await GetIpAddress();
            break;
        } catch (Exception) { await Task.Delay(50); };

        return tryAddress == currentAddress;
    }

    static async Task<String> GetIpAddress() {
        return await new HttpClient().GetStringAsync("https://ifconfig.me");
    }
}