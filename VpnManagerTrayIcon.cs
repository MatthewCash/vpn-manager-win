using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

public class VpnManagerTrayIcon : ApplicationContext
{
    private static NotifyIcon trayIcon;

    private static Icon onIcon = new Icon("resources/icons/on.ico"); 
    private static Icon offIcon = new Icon("resources/icons/off.ico"); 

    public VpnManagerTrayIcon ()
    {
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

    public static void SetTrayIconIcon(Boolean currentlyRouted) {
        trayIcon.Icon = currentlyRouted ? onIcon : offIcon;
    }
}