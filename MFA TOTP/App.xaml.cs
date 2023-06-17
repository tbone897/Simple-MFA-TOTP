using Microsoft.Win32;
using System.IO;
using System;
using System.Windows;
using System.Linq;
using ControlzEx.Theming;
using AutoUpdaterDotNET;
using System.Net;
using System.Windows.Forms;

namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Update Check
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start("https://raw.githubusercontent.com/tbone897/Simple-MFA-TOTP/master/Autoupdate/Versions.xml");
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    AutoUpdater.ShowUpdateForm(args);
                }
                else
                {
                    // No Update, Start App
                    StartApp();
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    System.Windows.Forms.MessageBox.Show(
                        @"There is a problem reaching update server. Please check your internet connection and try again later.",
                        @"Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void StartApp()
        {
            try
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
                }
                else
                {
                    // No Config, Show Welcome
                    WindowTOTP windowTOTP = new WindowTOTP(key);
                    windowTOTP.Show();
                }
                mainWindow.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Oh No, An Error has occured :( This should never happen, but Please report to ct.thibeau@gmail.com", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
            }
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