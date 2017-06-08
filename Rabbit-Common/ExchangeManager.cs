using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    public static class ExchangeManager
    {
        internal static List<Exchange> CreatedExchanges { get; set; }

        static ExchangeManager()
        {
            CreatedExchanges = new List<Exchange>();
        }

        internal static void AddCreatedExchange(Exchange exchange)
        {
            var item = CreatedExchanges.FirstOrDefault(d => d.Name.ToLower() == exchange.Name.ToLower());
            if (item == null)
                CreatedExchanges.Add(exchange);
        }

        public static void CloseAllCreatedExchanges()
        {
            foreach (var exchange in CreatedExchanges)
            {
                exchange.Channel.ExchangeDelete(exchange.Name);
            }
        }

        public static void CloseAllCreatedExchangesIfInactive()
        {
            foreach (var exchange in CreatedExchanges)
            {
                exchange.Channel.ExchangeDelete(exchange.Name, true);
            }
        }
    }
}
