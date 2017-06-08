using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Jerrod.RabbitCommon;
using Jerrod.RabbitCommon.Messages;
using Microsoft.AspNet.SignalR.Hubs;

namespace Jerrod.Prototype.Rabbit.WebUI.Chat
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            MvcApplication.Publisher.Publish(new ChatMessage()
            {
                CreatedBy = name,
                DateCreated = DateTime.UtcNow,
                Message = message
            });
        }
    }
}