﻿using System;
using Akka.Actor;
using Akka.Event;

namespace final_client_logic_akka
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
            Receive<Client[]>(client => {
                //Console.WriteLine("received message from" + client[0].playerID);

                clientArr[iterator] = client[0];
                iterator++;
                Sender.Tell("ss");

                if (iterator == clientArr.Length-1)
                {
                    Test.receiveClients(clientArr);                  
                }
                else
                    Console.WriteLine("Waiting for others");

               
            });

        }
    }


    public class Msg1
    {
        public string Content { get; set; }
    }
}
