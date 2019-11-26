using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka;
using Akka.Actor;
using Alcatraz;

namespace final_client_logic_akka

{
    public class Test : MoveListener
    {
        private static Client clientItem;
        private static Client[] clientItems;
        private static ClientClass clientClass;
        private static Alcatraz.Alcatraz clientAlcatraz = new Alcatraz.Alcatraz();
        private static Alcatraz.Alcatraz[] other;
        private static ClientData[] data;
        private int numPlayer;
        private static Boolean boolVar = false;
        private static Test t1;
        private static string line;
        public static ActorSystem mainActorSystem { get; set; }

        public Test()
        {

        }

        //  [STAThread]
        public static void Main(String[] args)
        {

            

            Console.WriteLine("Please choose a player name:");
            string playerName = Console.ReadLine();

            while (playerName == "")
            {
                Console.WriteLine("Your name cannot be empty. Please choose a player name:");
                playerName = Console.ReadLine();
            }


            try
            {

                startActorSystem("alcatraz");
                var localChatActor = mainActorSystem.ActorOf(Props.Create<GameActor>(), "GameActor");

                //Players players = new Players(new string[10, 10]);
                //players.players[1, 1] = actorSystemName;
                string remoteActorAddressClient1 = "akka.tcp://alcatraz@localhost:5555/user/RegisterActor";
                //string remoteActorAddressClient2 = "akka.tcp://client2@localhost:2222/user/EchoActor";
                var remoteChatActorClient1 = mainActorSystem.ActorSelection(remoteActorAddressClient1);
                //var remoteChatActorClient2 = actorSystem.ActorSelection(remoteActorAddressClient2);

                if (remoteChatActorClient1 != null)
                {
                        

                    remoteChatActorClient1.Tell(playerName, localChatActor);
                    //string ip = GetLocalIPAddress();
                    //string actorAdress = localChatActor.Path.;
                    //ClientData clientData = new ClientData(temp.Protocol, temp.Host, temp.Port, remoteChatActorClient1.PathString, 0, playerName);
                    Console.ReadLine();
                    //remoteChatActorClient1.Tell(new ClientData(), localChatActor);
                    //remoteChatActorClient2.Tell(players, child);

                } else {
                    Console.WriteLine("Could not get remote actor ref");
                    Console.ReadLine();
                }  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            data = new ClientData[1];
            //data[0] = new ClientData("akka.tcp://" + playerName + "@localhost:", 1111, "/user/GameActor", 1, playerName);
            other = new Alcatraz.Alcatraz[data.Length+1];

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
        

        public void setOther(int i, Alcatraz.Alcatraz t)
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
            Console.WriteLine("moving " + prisoner + " to " + (rowOrCol == Alcatraz.Alcatraz.ROW ? "row" : "col") + " " + (rowOrCol == Alcatraz.Alcatraz.ROW ? row : col));
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

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static void startActorSystem(string actorSystemName)
        {
            mainActorSystem = ActorSystem.Create(actorSystemName);
      
        }
    }    
}
