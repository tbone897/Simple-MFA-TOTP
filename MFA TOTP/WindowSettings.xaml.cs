using OtpNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for WindowSettings.xaml
    /// </summary>
    public partial class WindowSettings : Window
    {
        private DispatcherTimer timer1 = new DispatcherTimer();
        private Totp totp;
        private String _Key;

        public WindowSettings()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void TextBox_SecretKey_KeyUp(object sender, KeyEventArgs e)
        {
            timer1.Stop();            
            this.StatusBar_TextBlock.Text = "";
            _Key = this.TextBox_SecretKey.Text;
            Start_TOTP(_Key);
        }

        private void TextBox_SecretKey_MouseUp(object sender, MouseButtonEventArgs e)
        {
            timer1.Stop();
            this.StatusBar_TextBlock.Text = "";
            _Key = this.TextBox_SecretKey.Text;
            Start_TOTP(_Key);
        }

        //
        // Generate TOTP Code with Timer
        //
        private void Start_TOTP(String key)
        {
            // Convert code to byte[]
            byte[] secretKey = Base32Encoding.ToBytes(key);

            // Will fail if invalid key, Run in Try/Catch
            try
            {
                // Initilize Totp
                totp = new Totp(secretKey, mode: OtpHashMode.Sha1, step: 30, totpSize: 6);

                // Generate Code
                var totpCode = totp.ComputeTotp();

                // Set Code in TextBox
                this.TextBox_TOTPCode.Text = totpCode;

                // Start Timer to show how long code is valid for
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
                timer1.Start();
                this.Label1.Content = $"Valid for ${totp.RemainingSeconds().ToString()} Seconds";
            }
            catch (Exception ex) { 

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

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            // Copy OTP code to clipboard
            this.StatusBar_TextBlock.Text = "Copied Code";
            Clipboard.SetText(totp.ComputeTotp(), TextDataFormat.Text);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // 1st try writing to current directory
            bool writeFail = true;
            try
            {
                String config = Path.Combine(Directory.GetCurrentDirectory(), "config.totp");
                File.WriteAllText(config, _Key);
                writeFail = false;
            }catch(Exception ex) { }


            // 2nd try writing to appdata
            if (writeFail)
            {
                try
                {
                    String config = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "config.totp");
                    File.WriteAllText(config, _Key);
                }
                catch (Exception ex) { }
            }

            WindowTOTP windowTOTP = new WindowTOTP(_Key);
            windowTOTP.Left = this.Left;
            windowTOTP.Top = this.Top;
            windowTOTP.Show();
            this.Close();
        }
    }
}
