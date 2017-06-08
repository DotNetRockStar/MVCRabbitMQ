using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jerrod.RabbitCommon;

namespace Rabbit_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is the console client.  This console app should be sending messages to the server.");
            Console.WriteLine();

            Exchange exchange = new Exchange("myExchange", Jerrod.RabbitCommon.ExchangeType.Direct);

            //IQueueSettings settings = new QueueSettings() { QueueName = "myQueue" };
            //var publisher = new Publisher<Order>(settings, exchange); // listening to orders
            //var productPublisher = new Publisher<Product>(settings, exchange);
            var publisher = new Publisher<OrderMessage>(exchange); // listening to orders
            var productPublisher = new Publisher<ProductMessage>(exchange);

            string createdBy = "";
            decimal total = 0m;

            // **** Read User Input ****
            Console.WriteLine("What is your name?");
            createdBy = Console.ReadLine(); // user input name

            Console.WriteLine("What is the order total?");
            decimal.TryParse(Console.ReadLine(), out total); // user input total
            // ************************

            // publish the message that an order has been created.
            publisher.Publish(new OrderMessage()
            {
                CreatedBy = createdBy,
                DateCreated = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                Total = total
            });

            Console.WriteLine("Submitted Order.");
            Console.WriteLine();

            // **** Read User Input ****
            Console.WriteLine("What is the product name?");
            string productName = Console.ReadLine();
            Console.WriteLine("What is the product total?");
            decimal productTotal = 0m;
            decimal.TryParse(Console.ReadLine(), out productTotal);
            // ************************

            // publish the message that a product has been created.
            productPublisher.Publish(new ProductMessage()
            {
                Id = Guid.NewGuid().ToString(),
                Total = productTotal,
                Name = productName
            });

            Console.WriteLine("Submitted Product");
            Console.WriteLine();

            Console.WriteLine("Attempting to dispose publishers and exchange");
            publisher.Dispose();
            productPublisher.Dispose();
            exchange.Dispose();

            Environment.Exit(1);
        }
    }
}
