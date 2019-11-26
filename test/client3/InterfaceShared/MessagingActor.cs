using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public class MessagingActor : TypedActor, IHandle<string>
    {
        string[,] players = new string[10, 10];
        public MessagingActor()
        {

        }
        public MessagingActor(string[,] players)
        {
            this.players = players;
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
