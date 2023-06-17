using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Reflection;
using System;

namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class WindowAbout : MetroWindow
    {
        public WindowAbout()
        {
            InitializeComponent();
            this.Version.Text = getRunningVersion();
        }

        private void Hyperlink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var sInfo = new ProcessStartInfo("https://github.com/kspearrin/Otp.NET")
            {
                UseShellExecute = true,
            };
            Process.Start(sInfo);
        }

        private void HyperLink_TOTPClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var sInfo = new ProcessStartInfo("https://www.rfc-editor.org/rfc/rfc6238")
            {
                UseShellExecute = true,
            };
            Process.Start(sInfo);
        }

        private void HyperLink_MahApps(object sender, System.Windows.RoutedEventArgs e)
        {
            var sInfo = new ProcessStartInfo("https://github.com/MahApps/MahApps.Metro")
            {
                UseShellExecute = true,
            };
            Process.Start(sInfo);
        }

        private void HyperLink_CosturaFody(object sender, System.Windows.RoutedEventArgs e)
        {
            var sInfo = new ProcessStartInfo("https://github.com/Fody/Costura")
            {
                UseShellExecute = true,
            };
            Process.Start(sInfo);
        }

        private String getRunningVersion()
        {
            return "Version:  " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }


    }
}
