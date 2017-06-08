using Jerrod.RabbitCommon;
using Jerrod.RabbitCommon.Messages;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jerrod.Prototype.Rabbit.WebUI.Chat
{
    public class SignalRListener : Listener<ChatMessage>
    {
        private readonly IHubConnectionContext<dynamic> _clients = null;

        public SignalRListener(IQueueSettings settings, Exchange exchange, IHubConnectionContext<dynamic> clients)
            : base(settings, exchange)
        {
            _clients = clients;
            this.Listen += SignalRListener_Listen;
        }

        void SignalRListener_Listen(ListenerEventArgs<ChatMessage> args)
        {
            _clients.All.broadcastMessage(args.Item.CreatedBy, args.Item.Message);
        }
    }
}