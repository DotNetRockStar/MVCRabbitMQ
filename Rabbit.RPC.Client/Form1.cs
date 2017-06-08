using Jerrod.RabbitCommon;
using Jerrod.RabbitCommon.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rabbit.RPC.Client
{
    public partial class Form1 : Form
    {
        Client<AddNumbersMessage> client;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
                client.Dispose();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new Client<AddNumbersMessage>();
            }
            catch (Exception ex)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int first = int.Parse(textBox1.Text);
            int second = int.Parse(textBox2.Text);
            richTextBox1.AppendText("Submitting AddNumbersMessage(" + first.ToString() + ", " + second.ToString() + "\n");

            int response = client.Call<int>(new AddNumbersMessage() { Number1 = first, Number2 = second });

            richTextBox1.AppendText("Received response back.\n");
            if (response != null)
                richTextBox1.AppendText("Result = " + response.ToString() + "\n\n");
            else
                richTextBox1.AppendText("Result = null\n\n");
        }
    }
}
