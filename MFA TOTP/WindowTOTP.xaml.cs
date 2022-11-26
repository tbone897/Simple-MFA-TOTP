using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using OtpNet;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowTOTP : MetroWindow
    {
        private DispatcherTimer timer1 = new DispatcherTimer();
        private Totp totp;
        private String _Key;

        public WindowTOTP(String key)
        {
            InitializeComponent();
            this._Key = key;
            Start_TOTP();
        }

        //
        // Generate TOTP Code with Timer
        //
        private void Start_TOTP()
        {
            // Will fail if invalid key, Run in Try/Catch
            try
            {
                // Convert code to byte[]
                byte[] secretKey = Base32Encoding.ToBytes(_Key);

                // Initilize Totp
                totp = new Totp(secretKey, mode: OtpHashMode.Sha1, step: 30, totpSize: 6);

                // Start Timer to show how long code is valid for
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = new TimeSpan(0,0,0,0,500);
                timer1.Start();
                this.Label1.Content = $"Valid for ${totp.RemainingSeconds().ToString()} Seconds";
            }
            catch (Exception ex) {
                this.Button_TOTP.Content = "Invalid Secret Key. Check Settings.";
            }         
        }

        //
        // Timer Tick Event
        //
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Generate Code
            var totpCode = totp.ComputeTotp();

            // Set Code in TextBox
            this.Button_TOTP.Content = totpCode;

            // Update Valid Time
            this.Label1.Content = $"Valid for {totp.RemainingSeconds().ToString()} Seconds";
        }

        //
        // Button_TOTP Click - Copy Code to Clipboard
        //
        private void Button_TOTP_Click(object sender, RoutedEventArgs e)
        {
            this.StatusBar_TextBlock.Text = "Copied Code";
            if (totp != null){
                Clipboard.SetText(totp.ComputeTotp(), TextDataFormat.Text);
            }           
        }

        private void MenuItem_ClickOpen(object sender, RoutedEventArgs e)
        {
            // Show Open File Dialog
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
                }
                catch (Exception ex) { }

                // Get Pin
                WindowPin windowPin = new WindowPin(configFile);
                bool? result = windowPin.ShowDialog();

                if (result == true)
                {
                    // Stop Timer
                    timer1.Stop();

                    // Clear StatusBar Text
                    this.StatusBar_TextBlock.Text = "";

                    // Generate new code
                    _Key = windowPin._Key;
                    Start_TOTP();
                }
            }
        }

        private void MenuItem_ClickNew(object sender, RoutedEventArgs e)
        {
            WindowSettings settings = new WindowSettings();
            settings.Left = this.Left;
            settings.Top = this.Top;
            bool? results = settings.ShowDialog();
            if (results == true)
            {
                this.Close();
            }
        }

        private void MenuItem_ClickAbout(object sender, RoutedEventArgs e)
        {
            WindowAbout about = new WindowAbout();
            about.Owner= this;
            about.Show();
        }

        private void MenuItem_ClickExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_ClickCopy(object sender, RoutedEventArgs e)
        {
            // Get Config Path from registry
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

            // Get Pin
            WindowPin windowPin = new WindowPin(config_Registry);
            bool? result = windowPin.ShowDialog();

            // If WindowPin is cancel, return
            if (result != true) { return; }


            String key = windowPin._Key;
            String pin = windowPin._Pin;

            // Open Save Window Dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TOTP file (*.totp)|*.totp";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Write Config
                new Tools().Write(saveFileDialog.FileName, pin, key);

                // Copy Exe
                String exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                String saveDirectory = new FileInfo(saveFileDialog.FileName).DirectoryName;
                File.Copy(exePath, Path.Combine(saveDirectory, new FileInfo(exePath).Name));
            }
        }
    }
}
