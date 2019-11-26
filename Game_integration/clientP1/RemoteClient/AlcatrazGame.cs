using System;
using Akka.Actor;
using System.Windows.Forms;

namespace Alcatraz

{
    public class Test : MoveListener
    {
        private static Client clientItem;
        private static Client[] clientItems;
        private static ClientClass clientClass;
        private static Alcatraz clientAlcatraz;
        private static Alcatraz[] other;
        private static ClientData[] data;
        private int numPlayer;
        private static Boolean boolVar = false;
        private static Test t1;
        private static string line;

        protected static ActorSelection remoteChatActorClient1;
        protected static IActorRef child;
        public Test()
        {

        }

        //  [STAThread]
        public static void Main(String[] args)
        {
            string actorSystemName = "client1";
            Console.Title = actorSystemName;

                var actorSystem = ActorSystem.Create(actorSystemName);
                
                    var localChatActor = actorSystem.ActorOf(Props.Create<GameActor>(), "GameActor");
                    child = actorSystem.ActorOf(Props.Create<GameActor>(), "GameActorClient1Child");
                    string remoteActorAddressClient1 = "akka.tcp://client2@localhost:2222/user/GameActor";
                    //string remoteActorAddressClient2 = "akka.tcp://client3@localhost:3333/user/EchoActor";

                    remoteChatActorClient1 = actorSystem.ActorSelection(remoteActorAddressClient1);
                    //var remoteChatActorClient2 = actorSystem.ActorSelection(remoteActorAddressClient2);
                    //if (remoteChatActorClient1 != null)//&& remoteChatActorClient2 != null)
                    //{
                    //    string line = string.Empty;
                    //    while (line != null && line != "received")
                    //    {
                    //        line = Console.ReadLine();
                    //        remoteChatActorClient1.Tell(clientItem, child);
                    //    }
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Could not get remote actor ref");
                    //    Console.ReadLine();
                    //}
                
            
         

            data = new ClientData[1];
            data[0] = new ClientData("akka.tcp://client1@localhost:", 1111, "/user/GameActor", 1, "client1");
            other = new Alcatraz[data.Length+1];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            clientClass = new ClientClass(data,"client2");
            clientItem = clientClass.initializeClient(1, data.Length+1, data, "client1");
            // loop here and sent to other
            if(clientItem != null)
            {
                string line = string.Empty;
                while (line != null && line != "received")
                {
                    line = Console.ReadLine();
                    remoteChatActorClient1.Tell(clientItem, child);
                }
            }
            t1 = new Test();
            t1.setNumPlayer(data.Length+1);


            // line = Console.ReadLine();
            line = "";
            //System.Threading.Thread.Sleep(5000);

            while (line != null)
            {
                line = Console.ReadLine();
                if(line == "start")
                {
                    clientAlcatraz = clientItem.getAlcatraz();
                    clientItem.getAlcatraz().showWindow();
                    clientAlcatraz.addMoveListener(t1);
                    clientItem.getAlcatraz().getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                    clientItem.getAlcatraz().start();
                    Application.Run();
                }
               
            }

        }
        

        public void setOther(int i, Alcatraz t)
        {
            other[i] = t;
        }

        public static void receiveClients(Client[] clients)
        {
            clientItems = clients;
            Console.WriteLine(clients.Length);
            for(int i = 0; i < clients.Length; i++)
            {
                t1.setOther(i,clients[i].getAlcatraz());
            }
            line = "start";
        }

        public static void Test_FormClosed(object sender, FormClosedEventArgs args)
        {
            Environment.Exit(0);
        }

        public int getNumPlayer()
        {
            return numPlayer;
        }

        public void setNumPlayer(int numPlayer)
        {
            this.numPlayer = numPlayer;
        }

        public void doMove(Player player, Prisoner prisoner, int rowOrCol, int row, int col)
        {
            Console.WriteLine("moving " + prisoner + " to " + (rowOrCol == Alcatraz.ROW ? "row" : "col") + " " + (rowOrCol == Alcatraz.ROW ? row : col));
            Console.WriteLine("ID" + player.Id);
            ActorSelection[] remoteActors = ClientClass.getRemoteChatActorClient();

            for(int i = 0; i < remoteActors.Length; i++)
            {
                remoteActors[i].Tell(this.convertMove(player, prisoner, rowOrCol, row, col), clientClass.getChild());
            }
        }

        public Move convertMove(Player player, Prisoner prisoner, int rowOrCol, int row, int col)
        {
            return new Move(player, prisoner, rowOrCol, row, col);
        }

        public void undoMove()
        {
            Console.WriteLine("Undoing move");
        }

        public void gameWon(Player player)
        {
            Console.WriteLine("Player " + player.Id + " wins.");
        }

    /*    public static void main(String[] args)
        {
            Console.WriteLine("main main main");
        }*/
    }
}
