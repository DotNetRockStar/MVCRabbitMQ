using Jerrod.RabbitCommon;
using Jerrod.RabbitCommon.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rabbit.Framework.Client
{
    public partial class Form1 : Form
    {
        #region Properties

        private Client<FrameworkTestMessage> frameworkTestClient = null;
        private Client<VoidFrameworkTestMessage> voidFrameworkTestClient = null;
        private Listener<FrameworkTestResponse> frameworkTestResponseListener = null;
        private readonly Exchange _exchange = null;// new Exchange("test", ExchangeType.Direct);

        #endregion

        #region UI Methods

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (frameworkTestClient != null)
                    frameworkTestClient.Dispose();
                if (frameworkTestResponseListener != null)
                    frameworkTestResponseListener.Dispose();
                if (voidFrameworkTestClient != null)
                    voidFrameworkTestClient.Dispose();
                if (_exchange != null)
                    _exchange.Dispose();
            }
            catch { }

            Environment.Exit(1);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                frameworkTestClient = new Client<FrameworkTestMessage>();
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.Message + "\n\n");
            }

            try
            {
                voidFrameworkTestClient = new Client<VoidFrameworkTestMessage>();
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.Message + "\n\n");
            }

            try
            {
                frameworkTestResponseListener = new Listener<FrameworkTestResponse>();
                frameworkTestResponseListener.Listen += frameworkTestResponseListener_Listen;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.Message + "\n\n");
            }

            comboBox1.SelectedIndex = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = this.checkBox1.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://" + ConfigSectionUtility.Host + ":15672"));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "FrameworkTestMessage":
                    SendFrameworkTestMessage();
                    break;
                case "VoidFrameworkTestMessage":
                    SendVoidFrameworkTestMessage();
                    break;
                default:
                    MessageBox.Show("Unable to determine what message to send.");
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Listen for a message of type FrameworkTestResponse.
        /// </summary>
        /// <param name="args"></param>
        void frameworkTestResponseListener_Listen(ListenerEventArgs<FrameworkTestResponse> args)
        {
            string text = "A little birdy told me that a FrameworkTestResponse was detected...\nFrameworkTestResponse.Id=" + args.Item.Id + "\n\n";
            richTextBox1.BeginInvoke((MethodInvoker)delegate () { this.richTextBox1.AppendText(text); });
        }

        /// <summary>
        /// Send FrameworkTestMessage and update UI.
        /// </summary>
        private void SendFrameworkTestMessage()
        {
            richTextBox1.AppendText("Publishing FrameworkTestMessage...\n");

            if (checkBox1.Checked)
            {
                try
                {
                    // Set timeout for client that sends messages
                    frameworkTestClient.TimeoutInSeconds = int.Parse(textBox1.Text);
                    // Make an RPC call and wait for response.
                    var response = frameworkTestClient.Call<FrameworkTestResponse>(new FrameworkTestMessage() { CreatedBy = "Jerrod Test App" });

                    if (response == null)
                        richTextBox1.AppendText("Response received but client was unable to deserialize the response into type " + typeof(FrameworkTestResponse).FullName);
                    else
                        richTextBox1.AppendText("Response received.  Response returned Id = " + response.Id + ".\n\n");
                }
                catch (TimeoutException tEx)
                {
                    richTextBox1.AppendText("client request has timed out.\n\n");
                }
                catch (Exception ex)
                {
                    richTextBox1.AppendText("An unknown exception has occured.\n" + ex.ToString() + "\n\n");
                }
            }
            else
            {
                // Publish FrameworkTestMessage and be done.
                frameworkTestClient.Publish(new FrameworkTestMessage() { CreatedBy = "Jerrod Test App" });

                richTextBox1.AppendText("Finished Publishing FrameworkTestMessage...\n\n");
            }
        }

        /// <summary>
        /// Send VoidFrameworkTestMessage and update UI.
        /// </summary>
        private void SendVoidFrameworkTestMessage()
        {
            richTextBox1.AppendText("Publishing VoidFrameworkTestMessage...\n");

            if (checkBox1.Checked)
            {
                try
                {
                    // Send RPC VoidFrameworkTestMessage request and wait for response.
                    voidFrameworkTestClient.Call(new VoidFrameworkTestMessage() { SecretMessage = "Dodge Vipers are fast." });
                }
                catch (Exception ex)
                {
                    richTextBox1.AppendText("An unknown exception has occured.\n" + ex.ToString() + "\n\n");
                }
            }
            else
            {
                // Publish message and be done.
                voidFrameworkTestClient.Publish(new VoidFrameworkTestMessage() { SecretMessage = "Vipers are fast." });
                richTextBox1.AppendText("Finished Publishing VoidFrameworkTestMessage...\n\n");
            }
        }
    }
}
