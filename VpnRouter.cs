using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using Tomlyn.Model;

static class VpnRouter {
    private static Boolean currentlyRouted = false;

    public static void ToggleRouting() 
    {
        if (currentlyRouted)
        {
            DisableRouting();
        } else {
            EnableRouting();
        }
    }
    public static async void EnableRouting() 
    {
        Console.WriteLine("Starting VPN Routing");

        TomlArray routes = (TomlArray) Config.GetConfig()["routes_to_change"];

        Boolean success = (await Task.WhenAll(
            routes.Select(route => ModifyRoute((String) route, true))
        )).All(x => x);

        if (!success) {
            Console.WriteLine("VPN Routing Unsuccessful");
            MessageBox.Show("VPN Routing Unsuccessful, failed to apply routing changes!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Boolean correctIp = await IpAddress.CheckIpAddress((String) Config.GetConfig()["vpn_expected_address"]);

        if (!correctIp) {
            Console.WriteLine("VPN Routing Unsuccessful");
            MessageBox.Show("VPN Routing Unsuccessful, new public IP address is incorrect!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        currentlyRouted = true;
        VpnManagerTrayIcon.SetTrayIconIcon(true);

        Console.WriteLine("VPN Routing Successful");
    }
    public static async void DisableRouting() 
    {
        Console.WriteLine("Stopping VPN Routing");

        TomlArray routes = (TomlArray) Config.GetConfig()["routes_to_change"];

        Boolean success = (await Task.WhenAll(
            routes.Select(route => ModifyRoute((String) route, false))
        )).All(x => x);

        if (!success) {
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            MessageBox.Show("VPN Un-Routing Unsuccessful, failed to revert routing changes!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Boolean correctIp = !await IpAddress.CheckIpAddress((String) Config.GetConfig()["vpn_expected_address"]);

        if (!correctIp) {
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            MessageBox.Show("VPN Un-Routing Unsuccessful, public IP address remains changed!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        currentlyRouted = false;
        VpnManagerTrayIcon.SetTrayIconIcon(false);
        
        Console.WriteLine("VPN Un-Routing Successful");
    }

    static async Task<Boolean> ModifyRoute(String ip, Boolean add = true) {
        String netDeviceName = (String) Config.GetConfig()["vpn_net_device_name"];
        String action = add ? "add" : "delete";

        Process netshProcess = new Process();

        netshProcess.StartInfo.UseShellExecute = false;
        netshProcess.StartInfo.CreateNoWindow = true;
        netshProcess.StartInfo.FileName = "netsh.exe";
        netshProcess.StartInfo.Arguments = $"interface ipv4 {action} route {ip} {netDeviceName} 0.0.0.0";

        Console.WriteLine($"netsh.exe: interface ipv4 {action} route {ip} {netDeviceName} 0.0.0.0");

        netshProcess.Start();

        await netshProcess.WaitForExitAsync();

        return netshProcess.ExitCode == 0;
    }
}