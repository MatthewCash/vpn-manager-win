using System;
using System.Threading.Tasks;
using System.Net.Http;

static class CurrentIpAddress {
    public static async Task<Boolean> CheckIpAddress(String tryAddress) {
        String currentAddress = await GetIpAddress();

        return tryAddress == currentAddress;
    }

    static async Task<String> GetIpAddress() {
        return await new HttpClient().GetStringAsync("https://ifconfig.me");
    }
}