using System;
using Akka.Actor;
using Akka.Event;

namespace GameInterface
{
    /// <summary>
    /// Actor that just replies the message that it received earlier
    /// </summary>
    public class EchoActor : ReceiveActor
    {
        private readonly ILoggingAdapter log = Context.GetLogger();
        public EchoActor()
        {           

        Receive<Hello>(hello =>
            {
                Console.WriteLine("[{0}]: {1}", Sender, hello.Message);
                Sender.Tell(hello);
            });

            Receive<Client>(client =>
                {
                    Console.WriteLine("[{0}]: {1}", Sender, client.unique_name + "--" + client.ipAddress + "--" + client.port);

                });
                Receive<Msg>(msg => {
                // echo message back to sender
                Sender.Tell(msg);
            });

        }
    }

    public class Msg
    {
        public string Content { get; set; }
    }
}
