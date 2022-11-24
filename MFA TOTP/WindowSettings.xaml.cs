using OtpNet;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

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
            // Will fail if invalid key, Run in Try/Catch
            try
            {
                // Convert code to byte[]
                byte[] secretKey = Base32Encoding.ToBytes(key);

                // Initilize Totp
                totp = new Totp(secretKey, mode: OtpHashMode.Sha1, step: 30, totpSize: 6);

                // Start Timer to show how long code is valid for
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
                timer1.Start();
                this.Label1.Content = $"Valid for ${totp.RemainingSeconds().ToString()} Seconds";
            }
            catch (Exception ex)
            {

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
            this.TextBox_TOTPCode.Text = totpCode;

            // Update Valid Time
            this.Label1.Content = $"Valid for {totp.RemainingSeconds().ToString()} Seconds";
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            // Copy OTP code to clipboard
            this.StatusBar_TextBlock.Text = "Copied Code";
            Clipboard.SetText(totp.ComputeTotp(), TextDataFormat.Text);
        }

        private void TextBox_Pin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Save();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }


        private void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TOTP file (*.totp)|*.totp";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Write Config
                new Tools().Write(saveFileDialog.FileName, this.TextBox_Pin.Text, _Key);

                // Open TOTP Window
                WindowTOTP windowTOTP = new WindowTOTP(_Key);
                windowTOTP.Left = this.Left;
                windowTOTP.Top = this.Top;
                windowTOTP.Show();

                // Close This Window
                Window.GetWindow(this).DialogResult = true;
                Window.GetWindow(this).Close();
            }
        }

    }
}
