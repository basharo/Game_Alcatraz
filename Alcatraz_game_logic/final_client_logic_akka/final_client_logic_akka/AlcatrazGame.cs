using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;
using Alcatraz;
using Newtonsoft.Json;

namespace Alcatraz

{
    public class Test : MoveListener
    {
        private static Client clientItem;
        private static Client[] clientItems;
        private static ClientClass clientClass;
        private static Alcatraz clientAlcatraz = new Alcatraz();
        private static Alcatraz[] other;
        private static ClientData[] data;
        private int numPlayer;
        private static Boolean boolVar = false;
        private static Test t1;
        private static string line;

        public Test()
        {

        }

        //  [STAThread]
        public static void Main(String[] args)
        {

            Console.WriteLine("Please choose a player name:");
            string playerName = Console.ReadLine();

            try
            {
                using (var actorSystem = ActorSystem.Create(playerName))
                {

                    clientAlcatraz = new Alcatraz();
                    clientAlcatraz.init(2, 1);
                    clientItem = new Client(clientAlcatraz, new ClientData("", 1111, "", 0, ""));


                    var localChatActor = actorSystem.ActorOf(Props.Create<GameActor>(), "GameActor");

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

                            //remoteChatActorClient1.Tell("test", localChatActor);
                            remoteChatActorClient1.Tell(new Client(), localChatActor);
                            //remoteChatActorClient2.Tell(players, child);
                            line = Console.ReadLine();
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

            data = new ClientData[1];
            data[0] = new ClientData("akka.tcp://" + playerName + "@localhost:", 1111, "/user/GameActor", 1, playerName);
            other = new Alcatraz[data.Length+1];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            clientClass = new ClientClass(data,playerName);
            clientItem = clientClass.initializeClient(1, data.Length+1, data);

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
                    clientAlcatraz = clientItem.alcatraz;
                    clientItem.alcatraz.showWindow();
                    clientAlcatraz.addMoveListener(t1);
                    clientItem.alcatraz.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                    clientItem.alcatraz.start();
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
                t1.setOther(i,clients[i].alcatraz);
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
