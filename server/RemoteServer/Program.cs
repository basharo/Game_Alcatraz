using System;
using Akka.Actor;
using Akka.Configuration;
using Alcatraz;
using System.Configuration;
using Newtonsoft.Json;

namespace Server
{
    
    class Program
    {
            
        static void Main(string[] args)
        {
            string actorSystemName = "server";
            Console.Title = actorSystemName;

            try
            {
                using (var actorSystem = ActorSystem.Create(actorSystemName))
                {

                    var localChatActor = actorSystem.ActorOf(Props.Create<RegisterActor>(), "RegisterActor");

                    //string remoteActorAddress = "akka.tcp://server@localhost:6666/user/RegisterActor";
                    //var remoteChatActor = actorSystem.ActorSelection(remoteActorAddress);

                    //if (remoteChatActor != null)
                    //{
                        string line = string.Empty;
                        while (line != null)
                        {
                            line = Console.ReadLine();
                            //remoteChatActor.Tell(line, localChatActor);
                        }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Could not get remote actor ref");
                    //    Console.ReadLine();
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
