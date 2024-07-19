using System.Configuration;
using System.Data;
using System.Windows;
using Repositories.Entities;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IO;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _contextMenuStrip;

        public App()
        {
            _notifyIcon = new();
            _contextMenuStrip = new();
            var menuItemExit = new ToolStripMenuItem("Exit");
            menuItemExit.Click += MenuItemExit_Click;

            _contextMenuStrip.Items.Add(menuItemExit);
            _notifyIcon.ContextMenuStrip = _contextMenuStrip;

            var img = Path.GetFullPath(@"icon.ico");
            new ToastContentBuilder()

          .AddAppLogoOverride(new Uri(img))
          .AddText("Welcome")
          .AddText("Do it now!")
          .Show();

        }
        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            _notifyIcon.Icon = new System.Drawing.Icon("icon.ico");
            _notifyIcon.Text = "Task Master";
            _notifyIcon.Click += NotifyIcon_Click;
            _notifyIcon.Visible = true;
            base.OnStartup(e);
        }

        protected void NotifyIcon_Click(object sender, EventArgs e)
        {
            HomeWindow homeWindow = new();
            if (!HomeWindow.isShow && ProfileInfo.UserProfile != null)
            {
                HomeWindow.isShow = true;
                homeWindow.Show();
            }
        }

    }

    public static class ProfileInfo
    {
        public static Profile? UserProfile { get; set; } = null;
    }

}
