using System;
using Akka.Actor;
using Akka.Event;

namespace Alcatraz
{
    /// <summary>
    /// Actor that just replies the message that it received earlier
    /// </summary>
    public class GameActor : UntypedActor
    {
       
        public GameActor()
        {
        }

        protected override void OnReceive(object message)
        {
            var messageString = message.ToString();

            if (messageString.Contains("exists"))
            {
                Console.WriteLine(message.ToString());
                string playerName = Console.ReadLine();

                Sender.Tell(playerName, Self);
                
            }

            if(messageString.Contains("successfully registered"))
            {
                Console.WriteLine(messageString);
            }


            if (messageString == "start")
            {
                return;
            }
        }
    }


}
