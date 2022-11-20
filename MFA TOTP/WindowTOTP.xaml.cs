using Microsoft.Win32;
using OtpNet;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowTOTP : Window
    {
        private DispatcherTimer timer1 = new DispatcherTimer();
        private Totp totp;
        private String _Key;

        public WindowTOTP(String key)
        {
            InitializeComponent();
            this._Key = key;
            Start_TOTP(key);
        }

        //
        // Generate TOTP Code with Timer
        //
        private void Start_TOTP(String key)
        {
            // Will fail if invalid key, Run in Try/Catch
            try
            {
                // Convert code to byte[]
                byte[] secretKey = Base32Encoding.ToBytes(key);

                // Initilize Totp
                totp = new Totp(secretKey, mode: OtpHashMode.Sha1, step: 30, totpSize: 6);

                // Generate Code
                var totpCode = totp.ComputeTotp();

                // Set Code in TextBox
                this.Button_TOTP.Content = totpCode;

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
            // If 1 second or less on timer:
            if (totp.RemainingSeconds() <= 1)
            {
                // Stop Timer
                timer1.Stop();

                // Clear StatusBar Text
                this.StatusBar_TextBlock.Text = "";

                // Generate new code
                Start_TOTP(_Key);
            }

            // Update Valid Time
            this.Label1.Content = $"Valid for {totp.RemainingSeconds().ToString()} Seconds";
        }

        //
        // Button Click Event
        //
        private void Button_TOTP_Click(object sender, RoutedEventArgs e)
        {
            // Copy OTP code to clipboard
            this.StatusBar_TextBlock.Text = "Copied Code";
            if (totp != null){
                Clipboard.SetText(totp.ComputeTotp(), TextDataFormat.Text);
            }           
        }

        private void MenuItem_ClickSettings(object sender, RoutedEventArgs e)
        {
            WindowSettings settings = new WindowSettings();
            settings.Left = this.Left;
            settings.Top = this.Top;
            settings.Show();
            this.Close();
        }

        private void MenuItem_ClickOpen(object sender, RoutedEventArgs e)
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
                }
                catch (Exception ex) { }

                // Read file
                _Key = File.ReadAllText(configFile);

                // Stop Timer
                timer1.Stop();

                // Clear StatusBar Text
                this.StatusBar_TextBlock.Text = "";

                // Generate new code
                Start_TOTP(_Key);

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


    }
}
