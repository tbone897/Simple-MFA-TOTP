using System;
using System.IO;
using System.Windows.Forms;


namespace CTT_Beta
{
    public partial class Form2 : Form
    {
        public string ReturnValue1 { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String config = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");

            File.WriteAllText(config, textBox1.Text);
            ReturnValue1 = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
