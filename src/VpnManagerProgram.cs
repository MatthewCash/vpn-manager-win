using System;
using System.Windows.Forms;

namespace VpnManager {
    static class Program {
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Console.WriteLine("Loading config...");
            Config.LoadConfig();
            Console.WriteLine("Config loaded!");

            Console.WriteLine("Starting Tray Service");
            Application.Run(new VpnManagerTrayIcon());
            Console.WriteLine("Tray Service Exited");
        }
    }
}