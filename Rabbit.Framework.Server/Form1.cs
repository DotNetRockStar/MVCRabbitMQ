using Jerrod.RabbitCommon;
using Jerrod.RabbitCommon.Framework;
using Jerrod.RabbitCommon.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rabbit.Framework.Server
{
    public partial class Form1 : Form
    {
        public static RichTextBox Textbox { get; private set; }

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegistrationUtility.DisposeAndExit();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                RegistrationUtility.ResolveControllerDependency = NinjectConfig.RegisterContainer;
                RegistrationUtility.RegisterAllControllers();
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString());
                richTextBox1.AppendText("\n\n");
            }

            Textbox = this.richTextBox1;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://" + ConfigSectionUtility.Host + ":15672"));
        }
    }

}
