using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

public class VpnManagerTrayIcon : ApplicationContext
{
    private static NotifyIcon trayIcon;

    private static Icon onIcon; 
    private static Icon offIcon; 

    public VpnManagerTrayIcon ()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        Stream onIconStream = assembly.GetManifestResourceStream("VpnManager.resources.icons.on.ico");
        Stream offIconStream = assembly.GetManifestResourceStream("VpnManager.resources.icons.off.ico");

        onIcon = new Icon(onIconStream);
        offIcon = new Icon(offIconStream);

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