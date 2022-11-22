using System;
using System.Windows;


namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for WindowPin.xaml
    /// </summary>
    public partial class WindowPin : Window
    {
        private static String _Path { get; set; }

        public WindowPin(String path)
        {
            InitializeComponent();
            _Path = path;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetKey(this.TextBox_Pin.Text);
            this.Close();
        }

        internal string GetKey(String pin)
        {

            bool isPinValid = new Tools().CheckPin(_Path, pin);
            if (isPinValid == false)
            {
                this.TextBlock_Status.Text = "Invalid Pin";
                this.TextBox_Pin.Text = String.Empty;
                return null;
            }
            else
            {
                String key = new Tools().GetKey(_Path, pin);
                return key;
            }            
        }
    }
}
