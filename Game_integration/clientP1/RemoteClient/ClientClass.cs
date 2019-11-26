using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Alcatraz
{
    class ClientClass
    {

        private IActorRef localChatActor;
        private IActorRef child;
        private ActorSystem actorSystem;
        Alcatraz clientAlcatraz = new Alcatraz();
        private string actorSystemName;
        private static ActorSelection[] remoteChatActorClient;
        private int playerNumber;
        private int iterator;       

        public ClientClass(ClientData[] data, string uniqueName)
        {
            actorSystemName = uniqueName;
            actorSystem = ActorSystem.Create(actorSystemName);
            remoteChatActorClient = new ActorSelection[data.Length];

            child = actorSystem.ActorOf(Props.Create<GameActor>(), uniqueName);

            iterator = 0;
            //remoteChatActorClient[0] = actorSystem.ActorSelection("s");
            for (int i = 0; i < data.Length; i++)
            {
                string remoteActorAddressClient1 = data[i].getAddress() + data[i].getPort() + data[i].getUrlAddition();
                remoteChatActorClient[iterator] = actorSystem.ActorSelection(remoteActorAddressClient1);
                iterator++;
            }

            this.localChatActor = actorSystem.ActorOf(Props.Create<GameActor>(), "GameActor");
            
        }


        public Client initializeClient(int playerID, int numberOfPlayer, ClientData[] data, string uniqueName)
        {
            //Game logic
            clientAlcatraz.init(numberOfPlayer, playerID);

            for (int j = 1; j < numberOfPlayer + 1; j++)
            {
                int help = j - 1;
                clientAlcatraz.getPlayer(help).Name = "Player " + j;
            }
            Client client = new Client(clientAlcatraz, playerID);

            for (int jj = 0; jj < numberOfPlayer - 1; jj++)
            {
                Console.WriteLine("Tell Client" + jj);
                //System.Threading.Thread.Sleep(5000);
                Console.WriteLine("send to: " + remoteChatActorClient[jj].Path);
                Console.WriteLine("send to: " + remoteChatActorClient[jj].PathString);
                //remoteActor.Tell(client, sender);
                //string line = "";
                //while (line != null && line != "received")
                //{
                //    line = Console.ReadLine();
                //    remoteActor.Tell(client, sender);
                //}
                //            remoteChatActorClient[jj].Tell(client, child);

            }
            playerNumber = numberOfPlayer;
            return client;
        }

        public IActorRef getChild()
        {
            return this.child;
        }

        public static ActorSelection[] getRemoteChatActorClient()
        {
            return remoteChatActorClient;
        }    

        public void undoMove()
        {
            Console.WriteLine("Undoing move");
        }

        public void gameWon(Player player)
        {
            Console.WriteLine("Player " + player.Id + " wins.");
        }
    }
}
