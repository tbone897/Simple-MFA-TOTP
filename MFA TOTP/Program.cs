using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace MFA_TOTP
{

    public class Program
    {
        [STAThread]
        public static void Main()
        {
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

            // First check Registry for file
            if (File.Exists(config_Registry))
            {
                ReadConfig(config_Registry);
            }

            // 2nd Check Current Directory
            else if (File.Exists(config_CurrentDir))
            {
                ReadConfig(config_CurrentDir);
            }

            // 3rd Check AppData
            else if (File.Exists(config_AppData))
            {
                ReadConfig(config_AppData);
            }

            // No Config, Show Welcome
            else
            {
                App.Main();
            }
        }

        private static void ReadConfig(String configFile)
        {
            WindowPin windowPin = new WindowPin(configFile);
            bool? result = windowPin.ShowDialog();

            if (result == true)
            {
                String key = windowPin._Key;

                // Open TOTP Window
                WindowTOTP windowTOTP = new WindowTOTP(key);
                windowTOTP.ShowDialog();
            }
            else
            {
                App.Main();
            }
        }
    }
}
