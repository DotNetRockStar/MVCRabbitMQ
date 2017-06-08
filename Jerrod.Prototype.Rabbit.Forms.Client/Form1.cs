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

namespace Jerrod.Prototype.Rabbit.Forms.Client
{
    public partial class Form1 : Form
    {
        Exchange exchange;

        Publisher<OrderMessage> orderPublisher;
        Publisher<ProductMessage> productPublisher;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            orderPublisher.Dispose();
            productPublisher.Dispose();
            exchange.Dispose();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            exchange = new Exchange("myExchange", RabbitCommon.ExchangeType.Direct);

            orderPublisher = new Publisher<OrderMessage>(exchange);
            productPublisher = new Publisher<ProductMessage>(exchange);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // create order
            orderPublisher.Publish(new OrderMessage()
            {
                Id = Guid.NewGuid().ToString(),
                CreatedBy = textBox1.Text,
                DateCreated = DateTime.UtcNow,
                Total = decimal.Parse(textBox2.Text)
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // create product
            productPublisher.Publish(new ProductMessage()
            {
                Id = Guid.NewGuid().ToString(),
                Name = textBox4.Text,
                Total = decimal.Parse(textBox3.Text)
            });
        }
    }
}
