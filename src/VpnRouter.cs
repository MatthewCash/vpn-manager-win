using System;
using System.Windows.Forms;
using System.Net;

static class VpnRouter {
    private static Boolean currentlyRouted = false;

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

        IPAddress dest = IPAddress.Parse((String) Config.GetConfig()["route_to_change"]);
        IPAddress mask = IPAddress.Parse((String) Config.GetConfig()["route_mask"]);
        IPAddress nextHop = IPAddress.Parse((String) Config.GetConfig()["vpn_next_hop"]);
        uint metric = Convert.ToUInt32((Int64) Config.GetConfig()["route_metric"]);
        uint ifIndex = Convert.ToUInt32((Int64) Config.GetConfig()["vpn_if_index"]);

        Boolean success = RoutingManager.AddRoute(dest, mask, nextHop, ifIndex, metric);

        if (!success) {
            Console.WriteLine("VPN Routing Unsuccessful");
            MessageBox.Show("VPN Routing Unsuccessful, failed to apply routing changes!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Boolean correctIp = await CurrentIpAddress.CheckIpAddress((String) Config.GetConfig()["vpn_expected_address"]);

        if (!correctIp) {
            Console.WriteLine("VPN Routing Unsuccessful");
            MessageBox.Show("VPN Routing Unsuccessful, new public IP address is incorrect!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Red);
        } else {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Green);
        }

        currentlyRouted = true;

        Console.WriteLine("VPN Routing Successful");
    }
    
    public static async void DisableRouting() {
        Console.WriteLine("Stopping VPN Routing");
        VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.White);

        IPAddress dest = IPAddress.Parse((String) Config.GetConfig()["route_to_change"]);
        IPAddress mask = IPAddress.Parse((String) Config.GetConfig()["route_mask"]);
        IPAddress nextHop = IPAddress.Parse((String) Config.GetConfig()["vpn_next_hop"]);
        uint metric = Convert.ToUInt32((Int64) Config.GetConfig()["route_metric"]);
        uint ifIndex = Convert.ToUInt32((Int64) Config.GetConfig()["vpn_if_index"]);

        Boolean success = RoutingManager.DeleteRoute(dest, mask, nextHop, ifIndex, metric);

        if (!success) {
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            MessageBox.Show("VPN Un-Routing Unsuccessful, failed to revert routing changes!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Boolean correctIp = !await CurrentIpAddress.CheckIpAddress((String) Config.GetConfig()["vpn_expected_address"]);

        if (!correctIp) {
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            MessageBox.Show("VPN Un-Routing Unsuccessful, public IP address remains changed!", "VPN Routing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Red);
        } else {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
        }

        currentlyRouted = false;
        
        Console.WriteLine("VPN Un-Routing Successful");
    }
}