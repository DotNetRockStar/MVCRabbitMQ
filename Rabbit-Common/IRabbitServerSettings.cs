using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// Settings required to connect to a RabbitMQ server.
    /// </summary>
    public interface IRabbitServerSettings
    {
        string Host { get; }
        int Port { get; }
        string Username { get; }
        string Password { get; }
    }
}
