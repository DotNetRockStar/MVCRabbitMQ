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

namespace Rabbit.RPC.Server
{
    public partial class Form1 : Form
    {
        private Server<AddNumbersMessage, int> listener;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (listener != null)
                listener.Dispose();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            listener = new Server<AddNumbersMessage, int>(new AddTwoNumbersTogether(richTextBox1));
        }
    }

    public class AddTwoNumbersTogether : IListenerInvoker<AddNumbersMessage, int>
    {
        private readonly RichTextBox _textBox;
        public AddTwoNumbersTogether(RichTextBox textbox)
        {
            _textBox = textbox;
        }

        public int Execute(AddNumbersMessage message)
        {
            _textBox.AppendText("Some dude just asked me to add " + message.Number1.ToString() + " and " + message.Number2.ToString() + " haha...  Let me do that real fast.\n");
            var result = message.Number1 + message.Number2;
            _textBox.AppendText("Got it...  " + message.Number1.ToString() + " + " + message.Number2.ToString() + " = " + result.ToString() + "\n");
            _textBox.AppendText("Sending him the result.\n\n");
            return result;
        }
    }
}
