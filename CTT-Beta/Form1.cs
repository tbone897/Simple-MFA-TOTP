using System;
using System.IO;
using System.Windows.Forms;

using OtpNet;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace CTT_Beta
{

    public partial class Form1 : Form
    {
        private Timer timer1;
        private Totp totp;
        private String code;

        public String config = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");

        public Form1()
        {
            InitializeComponent();

            // Check if config file exists 
            if (File.Exists(config)){

                // Config File Found, Start TOTP
                code = File.ReadAllText(config);
                Start_TOTP(code);

            }else
            {

                // Config File Not Found, Prompt for Code
                GetCode();

            }
        }

        //
        // Show Form2, prompting for code.
        //
        private void GetCode()
        {
            // Prompt for code
            Form2 form2 = new Form2();
            form2.StartPosition = FormStartPosition.CenterParent;

            var result = form2.ShowDialog();
            if (result == DialogResult.OK)
            {
                code = form2.ReturnValue1;
                Start_TOTP(code);
            }
            else
            {
                Application.Exit();
            }
        }

        //
        // Generate TOTP Code with Timer
        //
        private void Start_TOTP(String code)
        {
            // Convert code to byte[]
            byte[] secretKey = Base32Encoding.ToBytes(code);

            // Initilize Totp
            totp = new Totp(secretKey, mode: OtpHashMode.Sha1, step: 30, totpSize: 6);

            // Generate Code
            var totpCode = totp.ComputeTotp();

            // Set Code in TextBox
            textBox2.Text = totpCode;

            // Start Timer to show how long code is valid for
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 500;
            timer1.Start();
            label1.Text = $"Valid for ${totp.RemainingSeconds().ToString()} Seconds";
        }

        //
        // Generate new code when old code expires
        //
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (totp.RemainingSeconds() >= 1)
            {
                timer1.Stop();
                Start_TOTP(code);
            }

            label1.Text = $"Valid for {totp.RemainingSeconds().ToString()} Seconds";
        }

        //
        // Copy code to clipboard
        //
        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(totp.ComputeTotp(), TextDataFormat.Text);
        }

        //
        // Delete Config File, Reset App
        //
        private void resetConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String message = "Delete Saved configuration?";
            String caption = "Are you Sure";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;

            var result = MessageBox.Show(message, caption, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(config);
                    timer1.Stop();
                    textBox2.Text = "";
                    label1.Text = "";

                }
                catch(Exception ex) { }

                GetCode();
            }
        }
    }
}
