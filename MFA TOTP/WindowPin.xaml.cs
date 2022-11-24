using System;
using System.Windows;
using System.Windows.Input;

namespace MFA_TOTP
{
    /// <summary>
    /// Interaction logic for WindowPin.xaml
    /// </summary>
    public partial class WindowPin : Window
    {
        private static String _Path { get; set; }
        public String _Key { get; set; }
        public String _Pin { get; set; }

        public WindowPin(String path)
        {
            InitializeComponent();
            _Path = path;
        }

        private void TextBox_Pin_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Button_Click(sender, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String pin = this.TextBox_Pin.Text;
            bool isPinValid = new Tools().CheckPin(_Path, pin);
            if (isPinValid)
            {
                _Key = new Tools().GetKey(_Path, pin);
                _Pin = pin;
                Window.GetWindow(this).DialogResult = true;
                Window.GetWindow(this).Close();
            }else
            {
                this.TextBlock_Status.Text = "Invalid Pin";
                this.TextBox_Pin.Text = String.Empty;
            }
        }
    }
}
