using System;
using Akka.Actor;
using Akka.Configuration;
using Alcatraz;
using System.Configuration;
using Newtonsoft.Json;
using Interface;

namespace Server
{
    
    class Program
    {
        public static ActorSystem mainActorSystem { get; set; }

        static void Main(string[] args)
        {
            string actorName = "server";
            Console.Title = actorName;
            

            try
            {

                startActorSystem("alcatraz");
                var localChatActor = mainActorSystem.ActorOf(Props.Create<RegisterActor>(), "RegisterActor");

                
                string line = string.Empty;
                while (line != null)
                {
                    if(line == "gamestart")
                    {
                                
                        return;
                    }
                    line = Console.ReadLine();
                       
                }
            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void startActorSystem(string actorSystemName)
        {
            mainActorSystem = ActorSystem.Create(actorSystemName);

        }
    }
}
