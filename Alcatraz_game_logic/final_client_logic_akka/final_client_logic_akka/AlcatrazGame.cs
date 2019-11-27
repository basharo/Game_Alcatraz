using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
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
        private static string playerName;
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

            
            Console.WriteLine("To cancel the registration enter 'delete'");
            Console.WriteLine("Please choose a player name:");
            playerName = Console.ReadLine();

            while (playerName == "")
            {
                Console.WriteLine("Your name cannot be empty. Please choose a player name:");
                playerName = Console.ReadLine();
            }


            try
            {

                startActorSystem("alcatraz");

                // Setup an actor that will handle deadletter type messages
                var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
                var deadletterWatchActorRef = mainActorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");

                // subscribe to the event stream for messages of type "DeadLetter"
                mainActorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

                var localChatActor = mainActorSystem.ActorOf(Props.Create<GameActor>(), "GameActor");

                
                string remoteActorAddressClient1 = "akka.tcp://alcatraz@localhost:5555/user/RegisterActor";
                var remoteChatActorClient1 = mainActorSystem.ActorSelection(remoteActorAddressClient1);

                if (remoteChatActorClient1 != null)
                {
                        
                    remoteChatActorClient1.Tell(playerName, localChatActor);
                    
                    string line = string.Empty;
                        while (line != null) {
                            line = Console.ReadLine();
                            
                            if(line == "delete")
                        {
                            remoteChatActorClient1.Tell("delete|" + playerName, localChatActor);
                        }


                    }

                    } else {
                    Console.WriteLine("Could not get remote actor ref");
                    Console.ReadLine();
                }  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            

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
