using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    /// <summary>
    /// Class that can be used to return an object along with metadata from a RabbitController method.
    /// </summary>
    /// <typeparam name="TResponse">The response type being returned.</typeparam>
    public class RabbitResponse<TResponse> : RabbitResponse
    {
        /// <summary>
        /// Create instance of RabbitResponse.
        /// </summary>
        /// <param name="response">The response object to be returned from the RabbitController method.</param>
        /// <param name="onlyRespondIfRPC">Defaults to false.  When set to true, this response will only respond back to publisher if the publisher indicated they were making an RPC request.</param>
        public RabbitResponse(TResponse response, bool onlyRespondIfRPC)
            : base(onlyRespondIfRPC)
        {
            this.Response = response;
        }
        /// <summary>
        /// Create instance of RabbitResponse.
        /// </summary>
        /// <param name="response">The response object to be returned from the RabbitController method.</param>
        public RabbitResponse(TResponse response)
        {
            this.Response = response;
        }
        internal TResponse Response { get; private set; }

        internal override object GetResponse()
        {
            return this.Response;
        }
    }

    /// <summary>
    /// Class that can be used to return an object along with metadata from a RabbitController method.
    /// </summary>
    public class RabbitResponse
    {
        internal readonly bool _onlyRespondIfRPC;

        /// <summary>
        /// Create instance of RabbitResponse.
        /// </summary>
        public RabbitResponse() { }
        /// <summary>
        /// Create instance of RabbitResponse.
        /// </summary>
        /// <param name="onlyRespondIfRPC">Defaults to false.  When set to true, this response will only respond back to publisher if the publisher indicated they were making an RPC request.</param>
        public RabbitResponse(bool onlyRespondIfRPC)
        {
            _onlyRespondIfRPC = onlyRespondIfRPC;
        }

        /// <summary>
        /// Optional.  Specify the routing key to be used when responding to the publisher.
        /// </summary>
        public string RoutingKey { get; set; }

        internal virtual object GetResponse()
        {
            return null;
        }
    }
}
