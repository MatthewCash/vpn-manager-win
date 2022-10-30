using System;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
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

        bool success;

        if (Config.GetConfig().upCommand == null) {
            IPAddress dest = IPAddress.Parse(Config.GetConfig().routeToChange);
            IPAddress mask = IPAddress.Parse(Config.GetConfig().routeMask);
            IPAddress nextHop = IPAddress.Parse(Config.GetConfig().nextHop);
            uint ifIndex = RoutingManager.GetInterfaceIndex(Config.GetConfig().ifName);

            RoutingManager.DeleteRoute(dest, mask, nextHop, ifIndex, Config.GetConfig().routeMetricOff);
            success = RoutingManager.AddRoute(dest, mask, nextHop, ifIndex, Config.GetConfig().routeMetricOn);
        } else {
            success = await runCommand(Config.GetConfig().upCommand, Config.GetConfig().upArgs);
        }

        if (!success) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
            Console.WriteLine("VPN Routing Unsuccessful");
            new ToastContentBuilder()
                .AddText("VPN Routing Unsuccessful")
                .AddText("Failed to apply routing changes!")
                .Show();
            return;
        }

        bool correctIp = await CurrentIpAddress.CheckIpAddress(Config.GetConfig().expectedAddress);

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

        bool success;

        if (Config.GetConfig().downCommand == null) {
            IPAddress dest = IPAddress.Parse(Config.GetConfig().routeToChange);
            IPAddress mask = IPAddress.Parse(Config.GetConfig().routeMask);
            IPAddress nextHop = IPAddress.Parse(Config.GetConfig().nextHop);
            uint ifIndex = RoutingManager.GetInterfaceIndex(Config.GetConfig().ifName);

            RoutingManager.DeleteRoute(dest, mask, nextHop, ifIndex, Config.GetConfig().routeMetricOn);
            success = RoutingManager.AddRoute(dest, mask, nextHop, ifIndex, Config.GetConfig().routeMetricOff);
        } else {
            success = await runCommand(Config.GetConfig().downCommand, Config.GetConfig().downArgs);
        }

        if (!success) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
            Console.WriteLine("VPN Un-Routing Unsuccessful");
            new ToastContentBuilder()
                .AddText("VPN Routing Unsuccessful")
                .AddText("Failed to revert routing changes!")
                .Show();
            return;
        }

        bool correctIp = !await CurrentIpAddress.CheckIpAddress(Config.GetConfig().expectedAddress);

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

    public static void SetRouted(bool routed) {
        currentlyRouted = routed;

        if (routed) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Green);
        } else {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
        }
    }

    static async Task<bool> runCommand(String command, String args) {
        Process cmdProcess = new Process();

        cmdProcess.StartInfo.UseShellExecute = false;
        cmdProcess.StartInfo.CreateNoWindow = true;
        cmdProcess.StartInfo.FileName = command;
        cmdProcess.StartInfo.Arguments = args;

        cmdProcess.Start();

        await cmdProcess.WaitForExitAsync();

        return cmdProcess.ExitCode == 0;
    }
}
