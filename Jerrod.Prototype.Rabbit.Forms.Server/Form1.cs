using Newtonsoft.Json;
using Jerrod.RabbitCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jerrod.Prototype.Rabbit.Forms.Server
{
    public partial class Form1 : Form
    {
        private Listener<OrderMessage> orderListener;
        private Listener<ProductMessage> productListener;
        private Exchange exchange;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            orderListener.Dispose();
            productListener.Dispose();
            exchange.Dispose();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            exchange = new Exchange("myExchange", RabbitCommon.ExchangeType.Direct);

            orderListener = new Listener<OrderMessage>(exchange);
            productListener = new Listener<ProductMessage>(exchange);

            orderListener.Listen += orderListener_Listen;
            productListener.Listen += productListener_Listen;
        }

        void productListener_Listen(ListenerEventArgs<ProductMessage> args)
        {
            richTextBox1.AppendText("Product Created.\n");
            richTextBox1.AppendText(JsonConvert.SerializeObject(args.Item, Formatting.Indented) + "\n\n");
        }

        void orderListener_Listen(ListenerEventArgs<OrderMessage> args)
        {
            richTextBox1.AppendText("Order Created.\n");
            richTextBox1.AppendText(JsonConvert.SerializeObject(args.Item, Formatting.Indented) + "\n\n");
        }
    }
}
