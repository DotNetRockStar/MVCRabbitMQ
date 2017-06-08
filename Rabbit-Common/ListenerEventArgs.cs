using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// Listener even arguments 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListenerEventArgs<T> : EventArgs
    {
        public Listener<T> Listener { get; set; }
        public T Item { get; set; }
    }
}
