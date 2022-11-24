using Microsoft.Win32;
using System;
using System.Windows;


namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "TOTP files (*.totp)|*.totp";

            if (openFileDialog.ShowDialog() == true)
            {
                String configFile = openFileDialog.FileName;

                // Update Registry Key
                try
                {
                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", true);
                    registryKey.CreateSubKey("MFATOTP");
                    registryKey = registryKey.OpenSubKey("MFATOTP", true);
                    registryKey.SetValue("config", configFile);
                }catch(Exception ex) { }

                // Prompt for Pin
                WindowPin windowPin = new WindowPin(configFile);
                if (windowPin.ShowDialog() == true)
                {
                    String key = windowPin._Key;

                    // Open TOTP Window
                    WindowTOTP windowTOTP = new WindowTOTP(key);
                    windowTOTP.Left = this.Left;
                    windowTOTP.Top = this.Top;
                    windowTOTP.Show();

                    this.Close();
                }
            }
        }

        private void ButtonNewConfig_Click(object sender, RoutedEventArgs e)
        {
            WindowSettings windowSettings= new WindowSettings();
            windowSettings.Owner= this;
            bool? result = windowSettings.ShowDialog();
            if (result == true)
            {
                this.Close();
            }            
        }
    }
}
