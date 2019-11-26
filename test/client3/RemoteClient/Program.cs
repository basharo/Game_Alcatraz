using System;
using Akka.Actor;
using Akka.Configuration;
using Interface;

namespace ClientPlayer
{
    class Program
    {

        static void Main(string[] args)
        {
            string actorSystemName = "client3";
            Console.Title = actorSystemName;
            //Alcatraz.Alcatraz player = new Alcatraz.Alcatraz();

            try
            {
                using (var actorSystem = ActorSystem.Create(actorSystemName))
                {
                    var localChatActor = actorSystem.ActorOf(Props.Create<EchoActor>(), "EchoActor");
                    
                    //Players players = new Players(new string[10, 10]);
                    //players.players[1, 1] = actorSystemName;
                    string remoteActorAddressClient1 = "akka.tcp://server@localhost:5555/user/RegisterActor";
                    //string remoteActorAddressClient2 = "akka.tcp://client2@localhost:2222/user/EchoActor";
                    var remoteChatActorClient1 = actorSystem.ActorSelection(remoteActorAddressClient1);
                    //var remoteChatActorClient2 = actorSystem.ActorSelection(remoteActorAddressClient2);

                    //string serverActor = "akka.tcp://server@localhost:1111/user/EchoActor";

                    if (remoteChatActorClient1 != null)
                    {
                        string line = string.Empty;
                        while (line != null)
                        {
                            line = Console.ReadLine();
                            remoteChatActorClient1.Tell(new Client(1, "Franz", 3, localChatActor.ToString()), localChatActor);
                            //remoteChatActorClient2.Tell(players, child);

                            //remoteChatActorClient1.Tell(line, child);
                            //remoteChatActorClient2.Tell(line, child);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Could not get remote actor ref");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

}

