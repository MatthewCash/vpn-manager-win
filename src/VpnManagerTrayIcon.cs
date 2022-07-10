using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

public class VpnManagerTrayIcon : ApplicationContext
{
    private static NotifyIcon trayIcon;

    private static Icon offIcon;
    private static Icon whiteIcon; // Loading
    private static Icon greenIcon; // Success
    private static Icon redIcon;  // IP Address Mismatch

    public VpnManagerTrayIcon ()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        offIcon = new Icon(assembly.GetManifestResourceStream("VpnManager.resources.icons.off.ico"));
        whiteIcon = new Icon(assembly.GetManifestResourceStream("VpnManager.resources.icons.white.ico"));
        greenIcon = new Icon(assembly.GetManifestResourceStream("VpnManager.resources.icons.green.ico"));
        redIcon = new Icon(assembly.GetManifestResourceStream("VpnManager.resources.icons.red.ico"));

        trayIcon = new NotifyIcon();

        trayIcon.Icon = offIcon;
        trayIcon.Visible = true;
        trayIcon.MouseClick += new MouseEventHandler(OnTrayIconClick);

        var contextMenu = new ContextMenuStrip();

        trayIcon.ContextMenuStrip = contextMenu;

        contextMenu.Items.Add(new ToolStripMenuItem("Enable Routing", null, EnableRouting));
        contextMenu.Items.Add(new ToolStripMenuItem("Disable Routing", null, DisableRouting));
        contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, Exit));
    }

    void EnableRouting(object sender, EventArgs e) {
        VpnRouter.EnableRouting();
    }

    void DisableRouting(object sender, EventArgs e) {
        VpnRouter.DisableRouting();
    }

    void Exit(object sender, EventArgs e)
    {
        trayIcon.Visible = false;

        Application.Exit();
    }

    void OnTrayIconClick(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        VpnRouter.ToggleRouting();
    }

    public enum TrayIconColor {
        Off,
        White,
        Red,
        Green,
    }

    public static void SetTrayIconColor(TrayIconColor iconColor) {
        trayIcon.Icon = iconColor switch {
            TrayIconColor.Off => offIcon,
            TrayIconColor.White => whiteIcon,
            TrayIconColor.Green => greenIcon,
            TrayIconColor.Red => redIcon,
            _ => offIcon,
        };
    }
}