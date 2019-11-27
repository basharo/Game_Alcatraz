using System;
using Akka.Actor;
using Akka.Event;

namespace Alcatraz
{
    /// <summary>
    /// Actor that just replies the message that it received earlier
    /// </summary>
    public class GameActor : ReceiveActor
    {
        private readonly ILoggingAdapter log = Context.GetLogger();
        private Client[] clientArr = new Client[1];
        private int iterator = 0;

        public GameActor()
        {
            Receive<Move>(player => {
                // echo message back to sender
                Sender.Tell("ss");
            });
            Receive<string>(player => {
                // echo message back to sender
                Sender.Tell("ss");
            });
            Receive<Client>(client => {
                Console.WriteLine("received message from" + client.getPlayerID());
                Context.Watch(Sender);
                //clientArr[iterator] = client;
                //iterator++;
                //Sender.Tell("ss");

                //if (iterator == clientArr.Length-1)
                //{
                //    Test.receiveClients(clientArr);                  
                //}
                //else
                //    Console.WriteLine("Waiting for others");

               
            });


            Receive<Terminated>(t =>
            {

            });

        }
        public sealed class Terminated : IAutoReceivedMessage, IPossiblyHarmful
        {
            public Terminated(IActorRef actorRef, bool existenceConfirmed, bool addressTerminated)
            {
                ActorRef = actorRef;
                ExistenceConfirmed = existenceConfirmed;
                AddressTerminated = addressTerminated;
            }

            public IActorRef ActorRef { get; private set; }


            public bool AddressTerminated { get; private set; }

            public bool ExistenceConfirmed { get; private set; }

            public override string ToString()
            {
                return "<Terminated>: " + ActorRef + " - ExistenceConfirmed=" + ExistenceConfirmed;
            }
        }
    }


    public class Msg1
    {
        public string Content { get; set; }
    }
}
