using Jerrod.RabbitCommon;
using Jerrod.RabbitCommon.Framework;
using Jerrod.RabbitCommon.Messages;
using Rabbit.Framework.Server.ActionFilters;
using Rabbit.Framework.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rabbit.Framework.Server
{
    [ExceptionActionFilter]
    public class TestRabbitController : RabbitController
    {
        private readonly IDataRepository _dataRepository;
        private readonly ILoggingRepository _loggingRepository;

        public TestRabbitController(IDataRepository dataRepository, ILoggingRepository loggingRepository)
            : base()
        {
            if (dataRepository == null)
                throw new ArgumentNullException("dataRepository");
            if (loggingRepository == null)
                throw new ArgumentNullException("loggingRepository");

            this._loggingRepository = loggingRepository;
            this._dataRepository = dataRepository;
        }

        [TestActionFilter]
        [RabbitMethod] // Tells the framework this is a RabbitMQ method and to listen for message specified in input parameter.
        public RabbitResponse<FrameworkTestResponse> ListenThenSleepThenRespond(FrameworkTestMessage message)
        {
            // Let the UI know that a message was received
            WriteToTextbox(@"== FrameworkTestMessage Received ==
- FrameworkTestMessage.CreatedBy = " + message.CreatedBy + @"
-> Going to thread sleep for 3 seconds then return...
");

            // simulate a 3 second hang
            System.Threading.Thread.Sleep(3000); 

            var response = new FrameworkTestResponse() { Id = Guid.NewGuid().ToString() };

            // Let the UI know that a response is being returned.
            WriteToTextbox(@"-> Finished sleeping.
-> Returning FrameworkTestResponse back to caller.
-> FrameworkTestResponse.Id = " + response.Id + @"

");
            
            // Return the response.
            return new RabbitResponse<FrameworkTestResponse>(response);
        }

        [RabbitMethod]
        public void ListenButDoNotRespond(VoidFrameworkTestMessage message)
        {
            var DoSomethingHere = 999;

            Rabbit.Framework.Server.Form1.Textbox.AppendText(@"== VoidFrameworkTestMessageReceived ==
- VoidFrameworkTestMessage.SecretMessage = " + message.SecretMessage + @"
-> Going to thread sleep for 2 seconds then return...
");
            System.Threading.Thread.Sleep(2000);

            Rabbit.Framework.Server.Form1.Textbox.AppendText(@"-> Finished sleeping.
-> Returning from RabbitController method now.

");

            var DoStuff = 123;
        }

        // Demonstrating how this method is not invoked or detected by the framework.
        public void ThisIsNotARabbitMethod(FrameworkTestMessage message)
        {
            // This method will never fire.
        }

        private void WriteToTextbox(string str)
        {
            Rabbit.Framework.Server.Form1.Textbox.AppendText(str);
        }
    }
}
