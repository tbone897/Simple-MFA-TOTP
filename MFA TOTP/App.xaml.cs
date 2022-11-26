using Microsoft.Win32;
using System.IO;
using System;
using System.Windows;
using System.Linq;
using ControlzEx.Theming;

namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            // Check for config in application directory
            string config_CurrentDir = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.totp", SearchOption.TopDirectoryOnly).FirstOrDefault();
            String config_AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "config.totp");
            String config_Registry = null;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\MFATOTP");
            if (registryKey != null)
            {
                try
                {
                    config_Registry = (String)registryKey.GetValue("config");
                }
                catch (Exception ex) { }
            }
            String key = null;
            MainWindow mainWindow = new MainWindow();

            // First check Registry for file
            if (File.Exists(config_Registry))
            {
                key = ReadConfig(config_Registry);
            }

            // 2nd Check Current Directory
            else if (File.Exists(config_CurrentDir))
            {
                key = ReadConfig(config_CurrentDir);
            }

            // 3rd Check AppData
            else if (File.Exists(config_AppData))
            {
                key = ReadConfig(config_AppData);
            }


            if (String.IsNullOrEmpty(key))
            {
                // No Config, Show Welcome                
                mainWindow.ShowDialog();
            } else
            {
                // No Config, Show Welcome
                WindowTOTP windowTOTP = new WindowTOTP(key);
                windowTOTP.Show();
            }

            mainWindow.Close();
        }

        private String ReadConfig(String configFile)
        {
            WindowPin windowPin = new WindowPin(configFile);
            bool? result = windowPin.ShowDialog();

            if (result == true)
            {
                return windowPin._Key;
            }

            return null;
        }

    }
}