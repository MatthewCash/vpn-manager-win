using System;
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
        trayIcon.ContextMenu = new ContextMenu(new MenuItem[] {
            new MenuItem("Enable Routing", EnableRouting),
            new MenuItem("Disable Routing", DisableRouting),
            new MenuItem("Exit", Exit)
        });

        trayIcon.MouseClick += new MouseEventHandler(OnTrayIconClick);
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