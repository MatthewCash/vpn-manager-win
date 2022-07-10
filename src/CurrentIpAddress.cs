using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

static class CurrentIpAddress {
    public static async Task<Boolean> CheckIpAddress(String tryAddress) {
        String currentAddress = await GetIpAddress();

        return tryAddress == currentAddress;
    }

    static async Task<String> GetIpAddress() {
        return await new HttpClient().GetStringAsync("https://ifconfig.me");
    }
}