using System;
using Akka.Actor;
using Akka.Configuration;
using Alcatraz;
using System.Configuration;
using Newtonsoft.Json;
using Interface;

namespace Server
{
    
    public class Program
    {

        static void Main(string[] args)
        {
            string actorName = "server";
            Globals.groupSize = 2;
            Console.Title = actorName;
            


            try
            {

                startActorSystem("alcatraz");
                var localChatActor = Globals.mainActorSystem.ActorOf(Props.Create<RegisterActor>(), "RegisterActor");

                
                string line = string.Empty;
                while (line != null)
                {
                    
                       
                }
            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void startActorSystem(string actorSystemName)
        {
            Globals.mainActorSystem = ActorSystem.Create(actorSystemName);

        }
    }
}
