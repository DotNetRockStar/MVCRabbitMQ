using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jerrod.RabbitCommon;
using Newtonsoft.Json;

namespace Rabbit_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is the console server.  This console app should be recieving messages from the client.");
            Console.WriteLine();

            Exchange exchange = new Exchange("myExchange", Jerrod.RabbitCommon.ExchangeType.Direct);

            //IListenerSettings listenerSettings = new ListenerSettings() { QueueName = "myQueue" };
            //Listener<Order> orderListener = new Listener<Order>(listenerSettings, exchange);
            //Listener<Product> productListener = new Listener<Product>(listenerSettings, exchange);
            Listener<OrderMessage> orderListener = new Listener<OrderMessage>(exchange);
            Listener<ProductMessage> productListener = new Listener<ProductMessage>(exchange);

            orderListener.Listen += orderListener_ListenFor;    
            productListener.Listen += productListener_ListenFor;
            
            Console.ReadLine();

            orderListener.Dispose();
            productListener.Dispose();
            exchange.Dispose();
        }

        static void productListener_ListenFor(ListenerEventArgs<ProductMessage> args)
        {
            Console.WriteLine("I heard that a product got created...");
            Console.WriteLine(JsonConvert.SerializeObject(args.Item, Formatting.Indented));
        }

        static void orderListener_ListenFor(ListenerEventArgs<OrderMessage> args)
        {
            // do whatever you want here if you hear a message.
            Console.WriteLine("I heard that an order got created...");
            Console.WriteLine(JsonConvert.SerializeObject(args.Item, Formatting.Indented));
        }
    }
}
