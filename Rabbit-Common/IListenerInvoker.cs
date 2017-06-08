using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// Interface used with the Server object.  This interface represents a method that should take a message input and
    /// return a result.  In the Server object, this interface is invoked when the server detects a message is sent of type TMessage
    /// and the Server expects TResult.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IListenerInvoker<TMessage, TResult>
    {
        TResult Execute(TMessage message);
    }
}
