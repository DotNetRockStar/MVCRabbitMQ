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

namespace Jerrod.Prototype.Rabbit.UI.Chat
{
    public partial class Form1 : Form
    {
        #region Properties 

        private Publisher<ChatMessage> chatPublisher = null;
        private Listener<ChatMessage> chatListener = null;
        private Server<ChatMessage, ChatMessage> server = null;
        private Exchange exchange = null;
        private Settings uniqueQueueSettingsForThisAppInstance = null;
        public static string Nickname { get; set; }

        #endregion

        private void SendMessage(string text)
        {
            var now = DateTime.UtcNow;
            chatPublisher.Publish(new ChatMessage() { CreatedBy = Nickname, Message = text, DateCreated = now }, null);

            textBox1.Text = "";
            textBox1.Focus();
        }

        #region UI Methods

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Nickname))
                    SendMessage("left the chatroom.");
            }
            catch { }

            ExchangeManager.CloseAllCreatedExchanges();
            QueueManager.ForceCloseAllQueues();

            chatPublisher.Dispose();
            chatListener.Dispose();

            Environment.Exit(1);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            EnterNicknameForm form = new EnterNicknameForm();
            form.ShowDialog();

            if (string.IsNullOrWhiteSpace(Nickname))
                Environment.Exit(1);

            this.Text = "Hello " + Nickname;

            try
            {
                uniqueQueueSettingsForThisAppInstance = new Settings()
                {
                    QueueName = "Jerrod.RabbitCommon.Messages." + Guid.NewGuid().ToString("N")
                };

                exchange = new Exchange("Jerrod.Prototype.Rabbit.UI.Chat", ExchangeType.Fanout);

                chatPublisher = new Publisher<ChatMessage>(uniqueQueueSettingsForThisAppInstance, exchange);
                chatListener = new Listener<ChatMessage>(uniqueQueueSettingsForThisAppInstance, exchange);
                chatListener.Listen += chatListener_Listen;
                //server = new Server<ChatMessage, ChatMessage>(new ChatListenerInvoker());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Environment.Exit(1);
            }

            SendMessage("joined the chatroom.");
        }

        void chatListener_Listen(ListenerEventArgs<ChatMessage> args)
        {
            string str = "\n(" + args.Item.DateCreated.ToShortTimeString() + ") " + args.Item.CreatedBy + ": " + args.Item.Message;
            AppendTextBox(str);
        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            this.richTextBox1.AppendText(value);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                return;

            if (e.KeyCode == Keys.Enter)
            {
                SendMessage(textBox1.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                return;

            SendMessage(textBox1.Text);
        }

        #endregion
    }
}
