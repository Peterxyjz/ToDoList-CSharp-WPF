using System.Configuration;
using System.Data;
using System.Windows;
using Forms = System.Windows.Forms;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Forms.NotifyIcon _notifyIcon;
        public App()
        {
            _notifyIcon = new();

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
            ProfileWindow home = new();
            home.WindowState = WindowState.Normal;
            home.Activate();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();
            base.OnExit(e);
        }

    }
}
