using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jerrod.Prototype.Rabbit.UI.Chat
{
    public partial class EnterNicknameForm : Form
    {
        public EnterNicknameForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Enter a nickname...");
                return;
            }

            Form1.Nickname = textBox1.Text;
            this.Close();
        }
    }
}
