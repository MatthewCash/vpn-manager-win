using System;
using System.Windows.Forms;
using System.Net;

static class VpnRouter {
    static Boolean currentlyRouted = false;

    public static void ToggleRouting() {
        if (currentlyRouted) {
            DisableRouting();
        } else {
            EnableRouting();
        }
    }

    public static async void EnableRouting() {
        Console.WriteLine("Starting VPN Routing");
        VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.White);

        IPAddress dest = IPAddress.Parse(Config.GetConfig().routeToChange);
        IPAddress mask = IPAddress.Parse(Config.GetConfig().routeMask);
        IPAddress nextHop = IPAddress.Parse(Config.GetConfig().nextHop);

        Boolean success = RoutingManager.AddRoute(dest, mask, nextHop, Config.GetConfig().ifIndex, Config.GetConfig().routeMetric);

        if (!success) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
            Console.WriteLine("VPN Routing Unsuccessful");
            MessageBox.Show("VPN Routing Unsuccessful, failed to apply routing changes!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Boolean correctIp = await CurrentIpAddress.CheckIpAddress(Config.GetConfig().expectedAddress);

        if (!correctIp) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Red);
            Console.WriteLine("VPN Routing Unsuccessful");
            MessageBox.Show("VPN Routing Unsuccessful, new public IP address is incorrect!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } else {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Green);
        }

        currentlyRouted = true;

        Console.WriteLine("VPN Routing Successful");
    }
    
    public static async void DisableRouting() {
        Console.WriteLine("Stopping VPN Routing");
        VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.White);

        IPAddress dest = IPAddress.Parse(Config.GetConfig().routeToChange);
        IPAddress mask = IPAddress.Parse(Config.GetConfig().routeMask);
        IPAddress nextHop = IPAddress.Parse(Config.GetConfig().nextHop);

        Boolean success = RoutingManager.DeleteRoute(dest, mask, nextHop, Config.GetConfig().ifIndex, Config.GetConfig().routeMetric);

        if (!success) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            MessageBox.Show("VPN Un-Routing Unsuccessful, failed to revert routing changes!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Boolean correctIp = !await CurrentIpAddress.CheckIpAddress(Config.GetConfig().expectedAddress);

        if (!correctIp) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Red);
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            MessageBox.Show("VPN Un-Routing Unsuccessful, public IP address remains changed!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } else {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
        }

        currentlyRouted = false;
        
        Console.WriteLine("VPN Un-Routing Successful");
    }
}