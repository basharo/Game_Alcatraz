using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface
{
    public class MessagingActor : TypedActor, IHandle<string>
    {
        public MessagingActor()
        {

        }

        public void Handle(string message)
        {
            Console.WriteLine(message);
            if(message != "received")
                Sender.Tell("received");
            // Sender.Tell("received", Self);

        }
    }
}
