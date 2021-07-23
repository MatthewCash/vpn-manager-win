using System;
using System.Diagnostics;

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
    public static void EnableRouting() 
    {
        Console.WriteLine("Starting VPN Routing");

        ModifyRoute("0.0.0.0", true);
        ModifyRoute("128.0.0.0", true);


        currentlyRouted = true;
        VpnManagerTrayIcon.SetTrayIconIcon(true);
    }
    public static void DisableRouting() 
    {
        Console.WriteLine("Stopping VPN Routing");

        ModifyRoute("0.0.0.0", false);
        ModifyRoute("128.0.0.0", false);

        currentlyRouted = false;
        VpnManagerTrayIcon.SetTrayIconIcon(false);
    }

    static void ModifyRoute(String ip, Boolean add = true) {
        String action = add ? "add" : "delete";

        Process netshProcess = new Process();

        netshProcess.StartInfo.UseShellExecute = false;
        netshProcess.StartInfo.CreateNoWindow = true;
        netshProcess.StartInfo.FileName = "netsh.exe";
        netshProcess.StartInfo.Arguments = $"interface ipv4 {action} route {ip}/1 NetworkEdge 0.0.0.0";

        netshProcess.Start();
    }
}