using Microsoft.Win32;
using System.IO;
using System;
using System.Windows;

namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        
    }
}