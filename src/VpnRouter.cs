using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;

static class VpnRouter {
    static bool currentlyRouted = false;

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

        if (Config.GetConfig().UpCommand == null) {
            IPAddress dest = IPAddress.Parse(Config.GetConfig().RouteToChange);
            IPAddress mask = IPAddress.Parse(Config.GetConfig().RouteMask);
            IPAddress nextHop = IPAddress.Parse(Config.GetConfig().NextHop);
            uint ifIndex = RoutingManager.GetInterfaceIndex(Config.GetConfig().IfName);

            RoutingManager.DeleteRoute(dest, mask, nextHop, ifIndex, Config.GetConfig().RouteMetricOff);
            success = RoutingManager.AddRoute(dest, mask, nextHop, ifIndex, Config.GetConfig().RouteMetricOn);
        } else {
            success = await RunCommand(Config.GetConfig().UpCommand, Config.GetConfig().UpArgs);
        }

        if (!success) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
            Console.Error.WriteLine("VPN Routing Unsuccessful");
            new ToastContentBuilder()
                .AddText("VPN Routing Unsuccessful")
                .AddText("Failed to apply routing changes!")
                .Show();
            return;
        }

        bool correctIp = await CurrentIpAddress.CheckIpAddress(Config.GetConfig().ExpectedAddress);

        if (!correctIp) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Red);
            Console.Error.WriteLine("Public IP address is incorrect!");
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

        if (Config.GetConfig().DownCommand == null) {
            IPAddress dest = IPAddress.Parse(Config.GetConfig().RouteToChange);
            IPAddress mask = IPAddress.Parse(Config.GetConfig().RouteMask);
            IPAddress nextHop = IPAddress.Parse(Config.GetConfig().NextHop);
            uint ifIndex = RoutingManager.GetInterfaceIndex(Config.GetConfig().IfName);

            RoutingManager.DeleteRoute(dest, mask, nextHop, ifIndex, Config.GetConfig().RouteMetricOn);
            success = RoutingManager.AddRoute(dest, mask, nextHop, ifIndex, Config.GetConfig().RouteMetricOff);
        } else {
            success = await RunCommand(Config.GetConfig().DownCommand, Config.GetConfig().DownArgs);
        }

        if (!success) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Off);
            Console.Error.WriteLine("VPN Un-Routing Unsuccessful");
            new ToastContentBuilder()
                .AddText("VPN Routing Unsuccessful")
                .AddText("Failed to revert routing changes!")
                .Show();
            return;
        }

        bool correctIp = !await CurrentIpAddress.CheckIpAddress(Config.GetConfig().ExpectedAddress);

        if (!correctIp) {
            VpnManagerTrayIcon.SetTrayIconColor(VpnManagerTrayIcon.TrayIconColor.Red);
            Console.Error.WriteLine("Public IP address remains changed!");
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

    static async Task<bool> RunCommand(string command, string args) {
        Process cmdProcess = new();

        cmdProcess.StartInfo.UseShellExecute = false;
        cmdProcess.StartInfo.CreateNoWindow = true;
        cmdProcess.StartInfo.FileName = command;
        cmdProcess.StartInfo.Arguments = args;

        cmdProcess.Start();

        await cmdProcess.WaitForExitAsync();

        return cmdProcess.ExitCode == 0;
    }
}
