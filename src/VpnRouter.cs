using System;
using System.Windows.Forms;
using System.Net;
using Microsoft.Toolkit.Uwp.Notifications;

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

        RoutingManager.DeleteRoute(dest, mask, nextHop, Config.GetConfig().ifIndex, Config.GetConfig().routeMetricOff);
        Boolean success = RoutingManager.AddRoute(dest, mask, nextHop, Config.GetConfig().ifIndex, Config.GetConfig().routeMetricOn);

        if (!success) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
            Console.WriteLine("VPN Routing Unsuccessful");
            new ToastContentBuilder()
                .AddText("VPN Routing Unsuccessful")
                .AddText("Failed to apply routing changes!")
                .Show();
            return;
        }

        Boolean correctIp = await CurrentIpAddress.CheckIpAddress(Config.GetConfig().expectedAddress);

        if (!correctIp) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Red);
            Console.WriteLine("VPN Routing Unsuccessful");
            new ToastContentBuilder()
                .AddText("VPN Routing Unsuccessful")
                .AddText("New public IP address is incorrect!")
                .Show();
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

        RoutingManager.DeleteRoute(dest, mask, nextHop, Config.GetConfig().ifIndex, Config.GetConfig().routeMetricOn);
        Boolean success = RoutingManager.AddRoute(dest, mask, nextHop, Config.GetConfig().ifIndex, Config.GetConfig().routeMetricOff);

        if (!success) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            new ToastContentBuilder()
                .AddText("VPN Routing Unsuccessful")
                .AddText("Failed to revert routing changes!")
                .Show();
            return;
        }

        Boolean correctIp = !await CurrentIpAddress.CheckIpAddress(Config.GetConfig().expectedAddress);

        if (!correctIp) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Red);
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            new ToastContentBuilder()
                .AddText("VPN Routing Unsuccessful")
                .AddText("Public IP address remains changed!")
                .Show();
        } else {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
        }

        currentlyRouted = false;
        
        Console.WriteLine("VPN Un-Routing Successful");
    }
}