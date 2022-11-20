using Microsoft.Win32;
using System;
using System.IO;
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
            // Check for config in application directory
            String config_CurrentDir = Path.Combine(Directory.GetCurrentDirectory(), "config.totp");
            String config_AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "config.totp");
            String config_Registry = null;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\MFATOTP");
            if (registryKey != null)
            {
                try
                {
                    config_Registry = (String)registryKey.GetValue("config");
                }
                catch(Exception ex) { }                
            }

            // First check Registry for file
            if (File.Exists(config_Registry))
            {
                String key = File.ReadAllText(config_CurrentDir);

                WindowTOTP windowTOTP = new WindowTOTP(key);
                windowTOTP.Show();
            }

            // 2nd Check Current Directory
            else if (File.Exists(config_CurrentDir))
            {
                // Config File Found, Start TOTP
                String key = File.ReadAllText(config_CurrentDir);

                WindowTOTP windowTOTP = new WindowTOTP(key);
                windowTOTP.Show();
            }

            // 3rd Check AppData
            else if(File.Exists(config_AppData))
            {
                // Config File Found, Start TOTP
                String key = File.ReadAllText(config_AppData);

                WindowTOTP windowTOTP = new WindowTOTP(key);
                windowTOTP.Show();
            }

            // No Config, Show Welcome
            else
            {
                // No Config found, Show Welcome Window
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }

        }
    }
}
